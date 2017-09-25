SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	Процедура будет возвращать все необходимые данные
-- для работы пользователя со списками RO.
-- =============================================
CREATE PROCEDURE GetRepairOrdersByUser 
	@userId bigint,
	@FilterByTeam bigint = NULL,
	@FilterByCustomers bigint = NULL,
	@IsOnlyOwn bit = 0,
	@RowsPerPage int = 10,
	@PageNumber int = 1, --нумерация страниц начинается с 1
	@SortByColumn nvarchar(50) = NULL,
	@SortType  nvarchar(10) = 'DESC', --Может принимать два значения DESC  или ASC
	@IsInvoice bit = NULL,
	@VinCode nvarchar(50) = NULL,
	@Stock nvarchar(50) = NULL,
	@CustRo nvarchar(50) = NULL,
	@RepairOrdersStatus smallint = NULL,
	@DateFrom datetime = NULL,
	@DateTo datetime = NULL,
	@IsForReport bit = 0,
	@IsFinalised bit = 0,
	@AffiliateId bigint = NULL,
	@IsGetAllRo bit = NULL,
	@GetTotalAmount bit = NULL,

	@totalCountRows int OUTPUT,
	@customersForFilter nvarchar(max) OUTPUT,
	@newActivityAmount int OUTPUT,
	@repairOrdersIds nvarchar(max) OUTPUT,
	@totalAmount Decimal(32,2) OUTPUT

AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @companyId bigint;
	SET @companyId = (SELECT CompanyFk FROM Users WHERE Id = @userId);

	DECLARE @Role nvarchar(255);
	SET @Role = (SELECT Class FROM Users WHERE Id = @userId);

	--Таблица для хранения промежуточных значений RO
	CREATE TABLE #RoResultTable(
		Id bigint NOT NULL,
		CreationDate datetime NULL,
		EstimateFk bigint NULL,
		RepairOrderStatus smallint NULL,
		TotalSum float NULL,
		IsConfirmed bit NULL,
		SupplementsApproved bit NULL,
		TeamEmployeeFk bigint NULL,
		New bit NULL,
		CompanyFk bigint NULL,
		IsInvoice bit NULL,
		CustomerFk bigint NULL,
		Finalised bit NULL,
		RoCustomerSignatureFk bigint NULL,
		RetailDiscount int NULL,
		AdditionalDiscount float NULL,
		WorkByThemselve bit NULL,
		IsFlatFee bit NULL,
		Payment float NULL,
		EditedStatus smallint NOT NULL		
		);
	
	--выборка RO в зависимости от компании и пользовательской роли
	INSERT #RoResultTable EXEC [dbo].[GetRepairOrdersByRole] @userId = @userId, @companyId = @companyId,
									@Role =@Role, @IsOnlyOwn = @IsOnlyOwn;

    ALTER TABLE #RoResultTable ADD CarsMakeModelYear nvarchar(255) NULL DEFAULT NULL;

    ALTER TABLE #RoResultTable ADD TotalAmount float NULL DEFAULT NULL;
	
	IF(@FilterByTeam IS NOT NULL) 
	DELETE FROM #RoResultTable 
	WHERE  (@FilterByTeam NOT IN (SELECT TeamFk FROM Customers_Teams WHERE CustomerFk = #RoResultTable.CustomerFk))
			AND (#RoResultTable.CustomerFk NOT IN (SELECT CustomerFk FROM Customers_Teams WHERE TeamFk = @FilterByTeam)); 
	
	--Колличество новых документов
	SET @newActivityAmount = (SELECT COUNT(*) FROM #RoResultTable WHERE New = 1);

	--выборка инвойсов по различным критериям
	IF(@RepairOrdersStatus IS NOT NULL) DELETE FROM #RoResultTable WHERE RepairOrderStatus != @RepairOrdersStatus;

	--TeamEmployeeFk NOT IN (SELECT TeamEmployeeFk  FROM TeamEmployees_Teams WHERE TeamFk = @FilterByTeam);

	IF(@FilterByCustomers IS NOT NULL) DELETE FROM #RoResultTable WHERE CustomerFk != @FilterByCustomers;

	IF(@IsForReport = 0)
	BEGIN
		IF(@IsFinalised = 1) DELETE FROM #RoResultTable WHERE RepairOrderStatus != 3;
		IF(@IsFinalised = 0) DELETE FROM #RoResultTable WHERE ((RepairOrderStatus = 3) OR IsInvoice = 1);
	END

	IF(@IsInvoice IS NOT NULL) DELETE FROM #RoResultTable WHERE RepairOrderStatus != @IsInvoice;

	IF(@VinCode IS NOT NULL) 
	BEGIN
		DELETE FROM #RoResultTable 
		WHERE #RoResultTable.Id NOT IN(
						SELECT #RoResultTable.Id FROM #RoResultTable, Estimates, Cars
						WHERE #RoResultTable.EstimateFk = Estimates.Id
							  AND Estimates.CarFk = Cars.Id 
							  AND upper(Cars.VIN) LIKE ('%' + upper(@VinCode))											
			  );
	END

	IF(@Stock IS NOT NULL) 
	BEGIN 
		DELETE FROM #RoResultTable 
		WHERE #RoResultTable.Id NOT IN(
						SELECT #RoResultTable.Id FROM #RoResultTable, Estimates, Cars
						WHERE #RoResultTable.EstimateFk = Estimates.Id
							  AND Estimates.CarFk = Cars.Id 
							  AND upper(Cars.Stock) = upper(@Stock)										
			  );
	END

	IF(@CustRo IS NOT NULL)
	BEGIN 
		DELETE FROM #RoResultTable 
		WHERE #RoResultTable.Id NOT IN(
						SELECT #RoResultTable.Id FROM #RoResultTable, Estimates, Cars
						WHERE  #RoResultTable.EstimateFk = Estimates.Id
								AND Estimates.CarFk = Cars.Id 
								AND upper(Cars.CustRO) = upper(@CustRo)										
				);
	END

	IF(@DateFrom IS NOT NULL) DELETE FROM #RoResultTable WHERE CreationDate < @DateFrom;

	IF(@DateTo IS NOT NULL) DELETE FROM #RoResultTable WHERE CreationDate > @DateTo;

	--для страницы Customers -> Locations делаем выборку по Affiliate
	IF(@AffiliateId IS NOT NULL)
	BEGIN
		DELETE FROM #RoResultTable
		WHERE #RoResultTable.Id NOT IN(
						SELECT #RoResultTable.Id FROM #RoResultTable, Estimates
						WHERE #RoResultTable.EstimateFk = Estimates.Id
								AND Estimates.AffiliateFk = @AffiliateId
				);
	END

	IF(@GetTotalAmount = 1) SET @totalAmount = (SELECT SUM([dbo].GetTotalAmountForRO(#RoResultTable.Id)) FROM #RoResultTable);

	--Кастомеры для фильтра.
	SET @customersForFilter = (
								SELECT CAST(customers.Id AS varchar) + ',' AS [text()] 
								FROM (SELECT Id 
								      FROM Customers 
									  WHERE Id IN( SELECT DISTINCT CustomerFk FROM #RoResultTable)) customers
								FOR XML PATH ('')
							  );
	
	--Общее колличество записей
	SET @totalCountRows = (SELECT COUNT(*) FROM #RoResultTable);

	--Сортировка RO и пагинация.
	--Для сортировки RO доступны следующие значения: CreationDate, TotalAmount, CarsMakeModelYear
	--Для типа сортировки два значения: ASC и DESC
	DECLARE @sqlQuery nvarchar(max);
	IF(@SortByColumn IS NOT NULL) 
	BEGIN
		--Если сортировка по CarsMakeModelYear - заполняем данными соответсвующий столбец
		IF (@SortByColumn = 'CarsMakeModelYear') 
			BEGIN
				UPDATE #RoResultTable
				SET CarsMakeModelYear = (SELECT CAST(Cars.Year AS varchar) + '/' +  Cars.Make + '/' + Cars.Model 
															  FROM Estimates, Cars
															  WHERE #RoResultTable.EstimateFk = Estimates.Id
															  AND Estimates.CarFk = Cars.Id);
			END 

		--Если сортировка по TotalAmount - заполняем данными соответсвующий столбец
		--IF (@SortByColumn = 'TotalAmount') 
		--	BEGIN
		--		UPDATE #RoResultTable
		--		SET TotalAmount = ();
		--	END 

		IF(((@SortByColumn = 'CreationDate') OR (@SortByColumn = 'TotalAmount') OR (@SortByColumn = 'CarsMakeModelYear')) 
					AND ((@SortType = 'ASC') OR (@SortType = 'DESC')))
			BEGIN
				SET @sqlQuery = 'SELECT Id
								 FROM (
											SELECT Id,
											ROW_NUMBER() OVER (ORDER BY ' + @SortByColumn + ' ' + @SortType + ') AS RowNum
											FROM #RoResultTable ) AS SOD
								WHERE SOD.RowNum BETWEEN ((' + CAST(@PageNumber AS varchar) + '-1)*' + CAST(@RowsPerPage AS varchar) + ')+1
								AND ' + CAST(@RowsPerPage AS varchar) + '*(' + CAST(@PageNumber AS varchar) + ')';

			END
	END
	DECLARE @sortedList AS TABLE (Id bigint NOT NULL);
	INSERT INTO @sortedList EXECUTE(@sqlQuery);


	--Выборка всех RO:
	IF((@IsGetAllRo IS NOT NULL) AND (@IsGetAllRo = 1))
	BEGIN
		SET @repairOrdersIds = (
								SELECT CAST(docs.Id AS varchar) + ',' AS [text()] 
								FROM (SELECT Id FROM #RoResultTable) docs
								FOR XML PATH ('')
					   );
		RETURN
	END

	--Получение списка айдишников отсортировынных инвойсов
	SET @repairOrdersIds = (
								SELECT CAST(docs.Id AS varchar) + ',' AS [text()] 
								FROM (SELECT Id FROM @sortedList) docs
								FOR XML PATH ('')
					   );
	--SELECT DISTINCT RepairOrders.* FROM RepairOrders, @sortedList
	--WHERE RepairOrders.Id IN (SELECT Id FROM @sortedList)
	RETURN
	END
GO
