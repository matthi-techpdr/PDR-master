SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	Процедура будет возвращать все необходимые данные
-- для работы пользователя со списками инвойсов.
-- =============================================
CREATE PROCEDURE GetInvoicesByUser 
	@userId bigint,
	@FilterByTeam bigint = NULL,
	@FilterByCustomers bigint = NULL,
	@IsOnlyOwn bit = 0,
	@RowsPerPage int = 10,
	@PageNumber int = 1, --нумерация страниц начинается с 1
	@SortByColumn nvarchar(50) = NULL,
	@SortType  nvarchar(10) = 'DESC', --Может принимать два значения DESC  или ASC
	@IsArchive bit = NULL,
	@VinCode nvarchar(50) = NULL,
	@Stock nvarchar(50) = NULL,
	@CustRo nvarchar(50) = NULL,
	@InvoicesStatus int = NULL,
	@DateFrom datetime = NULL,
	@DateTo datetime = NULL,
	@GetPaidUnpaidInvoicesSum bit = NULL,
	@AffiliateId bigint = NULL,
	@IsGetAllInvoices bit = NULL,
	@IsDiscarded bit = NULL,

	@totalCountRows int OUTPUT,
	@customersForFilter nvarchar(max) OUTPUT,
	@newActivityAmount int OUTPUT,
	@invoiceIds nvarchar(max) OUTPUT,
	@paidInvSum float OUTPUT,
	@unpaidInvSum float OUTPUT

AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @companyId bigint;
	SET @companyId = (SELECT CompanyFk FROM Users WHERE Id = @userId);

	DECLARE @Role nvarchar(255);
	SET @Role = (SELECT Class FROM Users WHERE Id = @userId);

	--Таблица для хранения промежуточных значений инвойсов
	CREATE TABLE #invoicesResultTable(
		Id bigint NOT NULL,
		CreationDate datetime NULL,
		PaidSum float NULL,
		[Status] bigint NULL,
		RepairOrderFk bigint NULL,
		TeamEmployeeFk bigint NULL,
		New bit NULL,
		Archived bit NULL,
		CompanyFk bigint NULL,
		Commission nvarchar(255) NULL,
		InvoiceSum float NULL,
		CustomerFk bigint NULL,
		IsImported bit NULL,
		PaidDate datetime NULL,
		IsDiscard bit NOT NULL,
		);
	
	IF(@IsDiscarded IS NOT NULL AND @IsDiscarded = 1)
		BEGIN
			--выборка всех инвойсов в зависимости от компании и без исключения Discard
			INSERT #invoicesResultTable 
			SELECT *
			FROM Invoices 
			WHERE IsDiscard = 1 AND CompanyFk = @companyId
 
		END
	ELSE
		BEGIN
			--выборка инвойсов в зависимости от компании и пользовательской роли и с исключением Discard инвойсов
			INSERT #invoicesResultTable EXEC [dbo].[GetInvoicesByRole] @userId = @userId, @companyId = @companyId,
											@Role =@Role, @IsOnlyOwn = @IsOnlyOwn;
		END

    ALTER TABLE #invoicesResultTable ADD CarsMakeModelYear nvarchar(255) NULL DEFAULT NULL;

	IF(@FilterByTeam IS NOT NULL) 
	DELETE FROM #invoicesResultTable
	WHERE (@FilterByTeam NOT IN (SELECT TeamFk FROM Customers_Teams WHERE CustomerFk = #invoicesResultTable.CustomerFk))
			AND (#invoicesResultTable.CustomerFk NOT IN (SELECT CustomerFk FROM Customers_Teams WHERE TeamFk = @FilterByTeam)); 

	--Колличество новых документов
	SET @newActivityAmount = (SELECT COUNT(*) FROM #invoicesResultTable WHERE New = 1);

	--выборка инвойсов по различным критериям
	IF(@InvoicesStatus IS NOT NULL) DELETE FROM #invoicesResultTable WHERE [Status] != @InvoicesStatus;

	--TeamEmployeeFk NOT IN (SELECT TeamEmployeeFk  FROM TeamEmployees_Teams WHERE TeamFk = @FilterByTeam);

	IF(@FilterByCustomers IS NOT NULL) DELETE FROM #invoicesResultTable WHERE CustomerFk != @FilterByCustomers;

	IF(@IsArchive IS NOT NULL) DELETE FROM #invoicesResultTable WHERE Archived != @IsArchive;

	IF(@VinCode IS NOT NULL) 
	BEGIN
		DELETE FROM #invoicesResultTable 
		WHERE #invoicesResultTable.Id NOT IN(
						SELECT #invoicesResultTable.Id FROM #invoicesResultTable, RepairOrders, Estimates, Cars
						WHERE #invoicesResultTable.RepairOrderFk = RepairOrders.Id 
							  AND RepairOrders.EstimateFk = Estimates.Id
							  AND Estimates.CarFk = Cars.Id 
							  AND upper(Cars.VIN) LIKE ('%' + upper(@VinCode))											
			  );
	END

	IF(@Stock IS NOT NULL) 
	BEGIN 
		DELETE FROM #invoicesResultTable 
		WHERE #invoicesResultTable.Id NOT IN(
						SELECT #invoicesResultTable.Id FROM #invoicesResultTable, RepairOrders, Estimates, Cars
						WHERE #invoicesResultTable.RepairOrderFk = RepairOrders.Id 
							  AND RepairOrders.EstimateFk = Estimates.Id
							  AND Estimates.CarFk = Cars.Id 
							  AND upper(Cars.Stock) = upper(@Stock)										
			  );
	END

	IF(@CustRo IS NOT NULL)
	BEGIN 
		DELETE FROM #invoicesResultTable 
		WHERE #invoicesResultTable.Id NOT IN(
						SELECT #invoicesResultTable.Id FROM #invoicesResultTable, RepairOrders, Estimates, Cars
						WHERE #invoicesResultTable.RepairOrderFk = RepairOrders.Id 
								AND RepairOrders.EstimateFk = Estimates.Id
								AND Estimates.CarFk = Cars.Id 
								AND upper(Cars.CustRO) = upper(@CustRo)										
				);
	END

	IF(@DateFrom IS NOT NULL) DELETE FROM #invoicesResultTable WHERE CreationDate < @DateFrom;

	IF(@DateTo IS NOT NULL) DELETE FROM #invoicesResultTable WHERE CreationDate > @DateTo;

	--для страницы Customers -> Locations делаем выборку по Affiliate
	IF(@AffiliateId IS NOT NULL)
	BEGIN
		DELETE FROM #invoicesResultTable
		WHERE #invoicesResultTable.Id NOT IN(
						SELECT #invoicesResultTable.Id FROM #invoicesResultTable, RepairOrders, Estimates
						WHERE #invoicesResultTable.RepairOrderFk = RepairOrders.Id 
								AND RepairOrders.EstimateFk = Estimates.Id
								AND Estimates.AffiliateFk = @AffiliateId
				);
	END

	--Кастомеры для фильтра.
	SET @customersForFilter = (
								SELECT CAST(customers.Id AS varchar) + ',' AS [text()] 
								FROM (SELECT Id 
								      FROM Customers 
									  WHERE Id IN( SELECT DISTINCT CustomerFk FROM #invoicesResultTable)) customers
								FOR XML PATH ('')
							  );
	
	--Общее колличество записей
	SET @totalCountRows = (SELECT COUNT(*) FROM #invoicesResultTable);

	--Если нужно посчитать сумму оплаченных и неоплаченных инвойсов - считаем здесь
	IF((@GetPaidUnpaidInvoicesSum IS NOT NULL) AND @GetPaidUnpaidInvoicesSum = 1)
	BEGIN
		SET @paidInvSum = (SELECT SUM(PaidSum) FROM #invoicesResultTable WHERE [Status] = 0 OR [Status] = 2);
		DECLARE @partialyPaidSum float;
		SET @partialyPaidSum = (SELECT SUM(InvoiceSum - PaidSum) FROM #invoicesResultTable WHERE [Status] = 0);
		IF (@partialyPaidSum IS NULL) SET @partialyPaidSum = 0;
		SET @unpaidInvSum = ((SELECT SUM(InvoiceSum) FROM #invoicesResultTable WHERE [Status] = 1) + @partialyPaidSum);
	END

	--Сортировка инвойсов и пагинация.
	--Для сортировки инвойсов доступны следующие значения: CreationDate, InvoiceSum, CarsMakeModelYear, Status
	--Для типа сортировки два значения: ASC и DESC
	DECLARE @sqlQuery nvarchar(max);
	IF(@SortByColumn IS NOT NULL) 
	BEGIN
		--Если сортировка по CarsMakeModelYear - заполняем данными соответсвующий столбец
		IF (@SortByColumn = 'CarsMakeModelYear') 
			BEGIN
				UPDATE #invoicesResultTable
				SET CarsMakeModelYear = (SELECT CAST(Cars.Year AS varchar) + '/' +  Cars.Make + '/' + Cars.Model 
															  FROM RepairOrders, Estimates, Cars
															  WHERE #invoicesResultTable.RepairOrderFk = RepairOrders.Id 
															  AND RepairOrders.EstimateFk = Estimates.Id
															  AND Estimates.CarFk = Cars.Id);
			END 
		IF(((@SortByColumn = 'CreationDate') OR (@SortByColumn = 'InvoiceSum') OR (@SortByColumn = 'CarsMakeModelYear') 
			OR (@SortByColumn = 'Status')) AND ((@SortType = 'ASC') OR (@SortType = 'DESC')))
			BEGIN
				SET @sqlQuery = 'SELECT Id
								 FROM (
											SELECT Id,
											ROW_NUMBER() OVER (ORDER BY ' + @SortByColumn + ' ' + @SortType + ') AS RowNum
											FROM #invoicesResultTable ) AS SOD
								WHERE SOD.RowNum BETWEEN ((' + CAST(@PageNumber AS varchar) + '-1)*' + CAST(@RowsPerPage AS varchar) + ')+1
								AND ' + CAST(@RowsPerPage AS varchar) + '*(' + CAST(@PageNumber AS varchar) + ')';

			END
	END
	DECLARE @sortedList AS TABLE (Id bigint NOT NULL);
	INSERT INTO @sortedList EXECUTE(@sqlQuery);


	--Выборка всех инвойсов:
	IF((@IsGetAllInvoices IS NOT NULL) AND (@IsGetAllInvoices = 1))
	BEGIN
		SET @invoiceIds = (
								SELECT CAST(docs.Id AS varchar) + ',' AS [text()] 
								FROM (SELECT Id FROM #invoicesResultTable) docs
								FOR XML PATH ('')
					   );
		RETURN

	END

	--Получение списка айдишников отсортировынных инвойсов
	SET @invoiceIds = (
								SELECT CAST(docs.Id AS varchar) + ',' AS [text()] 
								FROM (SELECT Id FROM @sortedList) docs
								FOR XML PATH ('')
					   );
	--SELECT DISTINCT Invoices.* FROM Invoices, @sortedList
	--WHERE Invoices.Id IN (SELECT Id FROM @sortedList) 

	RETURN
END
GO
