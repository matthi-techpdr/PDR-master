USE [PDR_test]
GO
/****** Object:  StoredProcedure [dbo].[GetInvoicesByUser]    Script Date: 4/27/2015 12:50:47 PM ******/
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
ALTER PROCEDURE [dbo].[GetInvoicesByUser] 
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
	@IsForCustomerFilter bit = NULL,

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


------------------------------------------------------------------------------------------
    DECLARE @SqlQuery nvarchar(max);
	DECLARE @SqlSubQuery nvarchar(max);
------------------------------------------------------------------------------------------


	--Таблицы для хранения промежуточных значений инвойсов
	
	CREATE TABLE #invoicesTable(
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
		CarsMakeModelYear nvarchar(255) NULL
		);
	
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
		CarsMakeModelYear nvarchar(255) NULL
		);
		
-----------------------------------------------------------------------------------------
--invoices-кандидаты, отобранные для конкретного пользователя, основываясь на его роли и значении селектора IsOnlyOwn
	IF (@Role = 'Technician' OR @Role = 'RITechnician' OR @IsOnlyOwn = 1)
		SET @SqlSubQuery = '
			SELECT  * 
			FROM Invoices 
			WHERE RepairOrderFk IN (SELECT RepairOrderFk 
									FROM TeamEmployeePercents 
									WHERE TeamEmployeeFk = ' + CAST(@userId AS varchar) + ') 
								AND Invoices.IsDiscard = 0
								AND CompanyFk = ' + CAST(@companyId AS varchar);
	ELSE IF (@Role = 'Admin' AND @IsOnlyOwn = 0)
			SET @SqlSubQuery = '
			SELECT * 
			FROM Invoices 
			WHERE IsDiscard = 0
				  AND CompanyFk = ' + CAST(@companyId AS varchar);
	ELSE IF (@Role = 'Manager' AND @IsOnlyOwn = 0  AND (SELECT IsShowAllTeams FROM Users WHERE Id = @userId) = 0)
			SET @SqlSubQuery = '
			SELECT * 
			FROM Invoices inv 
			WHERE inv.IsDiscard = 0  
				  AND CompanyFk = ' + CAST(@companyId AS varchar) + '
				  AND inv.RepairOrderFk IN (SELECT RepairOrderFk FROM (SELECT RepairOrderFk FROM TeamEmployeePercents WHERE TeamEmployeeFk IN (SELECT TeamEmployeeFk FROM (SELECT TeamEmployeeFk FROM TeamEmployees_Teams WHERE TeamFk IN (SELECT TeamFk FROM (SELECT TeamFk FROM TeamEmployees_Teams WHERE TeamEmployeeFk = ' + CAST(@userId AS varchar) + ') t12)) t13)) t14) 	  
				  AND (inv.CustomerFk IN (SELECT Customers_Teams.CustomerFk 
										  FROM Customers_Teams 									 
										  INNER JOIN (SELECT TeamFk FROM (SELECT TeamFk FROM TeamEmployees_Teams WHERE TeamEmployeeFk = ' + CAST(@userId AS varchar) + ') t15) TableB 									 
										  ON Customers_Teams.CustomerFk = inv.CustomerFk 
										  AND Customers_Teams.TeamFk = TableB.TeamFk) 							 
									  OR (SELECT COUNT (*) 
										  FROM Customers_Teams 
										  WHERE Customers_Teams.CustomerFk = inv.CustomerFk) = 0 )';
	ELSE IF (@Role = 'Wholesaler')
				SET @SqlSubQuery = '
			SELECT * 
			FROM Invoices 
			WHERE CustomerFk = (SELECT cust.Id
										FROM Customers cust, Users us
										WHERE us.Id = ' + CAST(@userId AS varchar) + '
										AND us.Login = cust.Email) 	 
			AND IsDiscard = 0
			AND CompanyFk = ' + CAST(@companyId AS varchar);
	ELSE 
			SET @SqlSubQuery = '
			SELECT * 
			FROM Invoices 
			WHERE IsDiscard = 0
			AND CompanyFk = ' + CAST(@companyId AS varchar);

------------------------------------------------------------------------------------------
	DECLARE @SqlQueryForFilter nvarchar(max);

	IF(@IsDiscarded = 1)
	BEGIN
		SET @SqlQuery = 'SELECT Id, CreationDate, PaidSum, [Status], RepairOrderFk, TeamEmployeeFk, New, Archived,	CompanyFk,
					            Commission, InvoiceSum, CustomerFk, IsImported, PaidDate, IsDiscard
				         FROM   Invoices table11
					     WHERE  IsDiscard = 1
					     AND    CompanyFk = ' + CAST(@companyId AS varchar);

		SET @SqlQueryForFilter = 'SELECT CustomerFk
				         FROM   Invoices table11
					     WHERE  IsDiscard = 1
					     AND    CompanyFk = ' + CAST(@companyId AS varchar);
	END
	ELSE
	BEGIN
		SET @SqlQuery = 'SELECT Id, CreationDate, PaidSum, [Status], RepairOrderFk, TeamEmployeeFk, New, Archived,	CompanyFk,
								Commission, InvoiceSum, CustomerFk, IsImported, PaidDate, IsDiscard
						 FROM  (' + @SqlSubQuery +') table11
						 WHERE  0=0';
		SET @SqlQueryForFilter = 'SELECT CustomerFk
						 FROM  (' + @SqlSubQuery +') table11
						 WHERE  0=0';
	END
		
	IF(@IsForCustomerFilter = 1)
		BEGIN
			IF(@IsArchive IS NOT NULL) SET @SqlQueryForFilter = @SqlQueryForFilter + ' AND Archived = ' + CAST(@IsArchive AS varchar);

			CREATE TABLE #Customers(CustomerFk bigint NOT NULL);
			INSERT INTO #Customers	( CustomerFk) EXECUTE(@SqlQueryForFilter);

			--Кастомеры для фильтра.
			SET @customersForFilter = (
										SELECT CAST(CustomerFk AS varchar) + ',' AS [text()] 
										FROM (SELECT DISTINCT CustomerFk FROM #Customers) customers
										FOR XML PATH ('')
									  );
		END
						 
	IF(@FilterByTeam IS NOT NULL)
		SET @SqlQuery = @SqlQuery + ' AND (CustomerFk IN (SELECT CustomerFk
														 FROM   Customers_Teams
														 WHERE  TeamFk = ' + CAST(@FilterByTeam AS varchar) + ')
											OR ' + CAST(@FilterByTeam AS varchar) +' IN (SELECT Customers_Teams.TeamFk 
																						 FROM Customers_Teams, Estimates, RepairOrders
																						 WHERE RepairOrders.Id = RepairOrderFk
																						 AND RepairOrders.EstimateFk = Estimates.Id 
																						 AND Estimates.AffiliateFk IS NOT NULL
																						 AND Customers_Teams.CustomerFk = Estimates.AffiliateFk)
										  )
									  AND ((SELECT COUNT(TeamEmployeeFk)
											FROM TeamEmployeePercents, Users 
											WHERE TeamEmployeePercents.RepairOrderFk = table11.RepairOrderFk
											AND TeamEmployeePercents.TeamEmployeeFk = Users.Id
											AND Users.Class != ''Admin'') 
											=
											(SELECT COUNT(*) 
											FROM TeamEmployees_Teams
											WHERE TeamEmployees_Teams.TeamEmployeeFk IN(
												SELECT TeamEmployeeFk 
												FROM TeamEmployeePercents 
												WHERE RepairOrderFk = table11.RepairOrderFk)
											AND TeamEmployees_Teams.TeamFk = '  + CAST(@FilterByTeam AS varchar) + '))';

------------------------------------------------------------------------------------------
	INSERT INTO #invoicesTable
		(Id, CreationDate, PaidSum, [Status], RepairOrderFk, TeamEmployeeFk, New, Archived,	CompanyFk,
		 Commission, InvoiceSum, CustomerFk, IsImported, PaidDate, IsDiscard)
	EXECUTE(@SqlQuery);
------------------------------------------------------------------------------------------

	--Колличество новых документов
	SET @newActivityAmount = (SELECT COUNT(*) FROM #invoicesTable WHERE New = 1);
	
------------------------------------------------------------------------------------------
	SET @SqlQuery = 'SELECT Id, CreationDate, PaidSum, [Status], RepairOrderFk, TeamEmployeeFk, New, Archived,	CompanyFk,
				            Commission, InvoiceSum, CustomerFk, IsImported, PaidDate, IsDiscard
				     FROM   #invoicesTable
				     WHERE  0=0';

	--выборка инвойсов по различным критериям
	IF(@InvoicesStatus IS NOT NULL)
		SET @SqlQuery = @SqlQuery + ' AND [Status] = ' + CAST(@InvoicesStatus AS varchar);

	IF(@FilterByCustomers IS NOT NULL)
		SET @SqlQuery = @SqlQuery + ' AND CustomerFk = ' + CAST(@FilterByCustomers AS varchar);

	IF(@IsArchive IS NOT NULL)
		SET @SqlQuery = @SqlQuery + ' AND Archived = ' + CAST(@IsArchive AS varchar);

	IF(@VinCode IS NOT NULL)
		SET @SqlQuery = @SqlQuery + ' AND RepairOrderFk IN (SELECT ro.Id
		                                                    FROM   RepairOrders ro, Estimates es, Cars cs
		                                                    WHERE  ro.EstimateFk = es.Id
		                                                    AND    es.CarFk = cs.Id
		                                                    AND    upper(cs.VIN) LIKE (''%'' + upper(''' + CAST(@VinCode AS varchar) + ''')))';
		
	IF(@Stock IS NOT NULL)
		SET @SqlQuery = @SqlQuery + ' AND RepairOrderFk IN (SELECT ro.Id
		                                                    FROM   RepairOrders ro, Estimates es, Cars cs
		                                                    WHERE  ro.EstimateFk = es.Id
		                                                    AND    es.CarFk = cs.Id
		                                                    AND    upper(cs.Stock) = upper(''' + CAST(@Stock AS varchar) + '''))';
		
	IF(@CustRo IS NOT NULL)
		SET @SqlQuery = @SqlQuery + ' AND RepairOrderFk IN (SELECT ro.Id
		                                                    FROM   RepairOrders ro, Estimates es, Cars cs
		                                                    WHERE  ro.EstimateFk = es.Id
		                                                    AND    es.CarFk = cs.Id
		                                                    AND    upper(cs.CustRo) = upper(''' + CAST(@CustRo AS varchar) + '''))';
		
	IF(@DateFrom IS NOT NULL)
		SET @SqlQuery = @SqlQuery + ' AND CreationDate >= ' + '''' + CAST(@DateFrom AS varchar) + '''';

	IF(@DateTo IS NOT NULL)
		SET @SqlQuery = @SqlQuery + ' AND CreationDate <= ' + '''' + CAST(@DateTo AS varchar) + '''';
		
	--для страницы Customers -> Locations делаем выборку по Affiliate
	IF(@AffiliateId IS NOT NULL)
		SET @SqlQuery = @SqlQuery + ' AND RepairOrderFk IN (SELECT ro.Id
		                                                    FROM   RepairOrders ro, Estimates es
		                                                    WHERE  ro.EstimateFk = es.Id
		                                                    AND    es.AffiliateFk = ' + CAST(@AffiliateId AS varchar) + ')';

------------------------------------------------------------------------------------------
	INSERT INTO #invoicesResultTable
		(Id, CreationDate, PaidSum, [Status], RepairOrderFk, TeamEmployeeFk, New, Archived,	CompanyFk,
		 Commission, InvoiceSum, CustomerFk, IsImported, PaidDate, IsDiscard)
	EXECUTE(@SqlQuery);
------------------------------------------------------------------------------------------

	----Кастомеры для фильтра.
	--SET @customersForFilter = (
	--							SELECT CAST(customers.Id AS varchar) + ',' AS [text()] 
	--							FROM (SELECT Id 
	--							      FROM Customers 
	--								  WHERE Id IN( SELECT DISTINCT CustomerFk FROM #invoicesResultTable)) customers
	--							FOR XML PATH ('')
	--						  );
	
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

	SET @SqlQuery = NULL;
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
				SET @SqlQuery = 'SELECT Id
								 FROM (
											SELECT Id,
											ROW_NUMBER() OVER (ORDER BY ' + @SortByColumn + ' ' + @SortType + ') AS RowNum
											FROM #invoicesResultTable ) AS SOD
								WHERE SOD.RowNum BETWEEN ((' + CAST(@PageNumber AS varchar) + '-1)*' + CAST(@RowsPerPage AS varchar) + ')+1
								AND ' + CAST(@RowsPerPage AS varchar) + '*(' + CAST(@PageNumber AS varchar) + ')';

			END
	END
	DECLARE @sortedList AS TABLE (Id bigint NOT NULL);
	INSERT INTO @sortedList EXECUTE(@SqlQuery);


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
	RETURN
END

