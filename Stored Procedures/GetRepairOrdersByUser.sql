USE [PDR_test]
GO
/****** Object:  StoredProcedure [dbo].[GetRepairOrdersByUser]    Script Date: 4/27/2015 12:51:14 PM ******/
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
ALTER PROCEDURE [dbo].[GetRepairOrdersByUser]
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
	@IsForCustomerFilter bit = NULL,

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

------------------------------------------------------------------------------------------
    DECLARE @SqlQuery nvarchar(max);
	DECLARE @SqlSubQuery nvarchar(max);
------------------------------------------------------------------------------------------

	--Таблицы для хранения промежуточных значений RO
	CREATE TABLE #RoTable(
		Id bigint NOT NULL,
		CreationDate datetime NULL,
		EstimateFk bigint NULL,
		RepairOrderStatus smallint NULL,
		New bit NULL,
		IsInvoice bit NULL,
		CustomerFk bigint NULL,
		CarsMakeModelYear nvarchar(255) NULL,
		TotalAmount float NULL DEFAULT NULL		
		);

	CREATE TABLE #RoResultTable(
		Id bigint NOT NULL,
		CreationDate datetime NULL,
		EstimateFk bigint NULL,
		RepairOrderStatus smallint NULL,
		New bit NULL,
		IsInvoice bit NULL,
		CustomerFk bigint NULL,
		CarsMakeModelYear nvarchar(255) NULL,
		TotalAmount float NULL DEFAULT NULL		
		);

------------------------------------------------------------------------------------------
--выборка RO в зависимости от компании и пользовательской роли
	IF (@Role = 'Technician' OR @Role = 'RITechnician' OR @IsOnlyOwn = 1)
		SET @SqlSubQuery = '
			SELECT  * 
			FROM RepairOrders orders
			WHERE Id IN (SELECT  RepairOrderFk 
						 FROM TeamEmployeePercents 
						 WHERE TeamEmployeeFk = ' + CAST(@userId AS varchar) + ') 
					 AND CompanyFk = ' + CAST(@companyId AS varchar) + '
					 AND RepairOrderStatus != 4';

	ELSE IF (@Role = 'Manager' AND @IsOnlyOwn = 0 AND (SELECT IsShowAllTeams FROM Users WHERE Id = @userId) = 0)
		SET @SqlSubQuery = '
			SELECT * 
			FROM RepairOrders orders 
			WHERE CompanyFk = ' + CAST(@companyId AS varchar) + '
				  AND orders.Id IN (SELECT RepairOrderFk FROM (SELECT RepairOrderFk FROM TeamEmployeePercents WHERE TeamEmployeeFk IN (SELECT TeamEmployeeFk FROM (SELECT TeamEmployeeFk FROM TeamEmployees_Teams WHERE TeamFk IN (SELECT TeamFk FROM (SELECT TeamFk FROM TeamEmployees_Teams WHERE TeamEmployeeFk = ' + CAST(@userId AS varchar) + ') t12)) t14)) t15) 	  
				  AND (orders.CustomerFk IN (SELECT Customers_Teams.CustomerFk 
										  FROM Customers_Teams 									 
										  INNER JOIN (SELECT TeamFk FROM (SELECT TeamFk FROM TeamEmployees_Teams WHERE TeamEmployeeFk = ' + CAST(@userId AS varchar) + ') t13) TableB 									 
										  ON Customers_Teams.CustomerFk = orders.CustomerFk 
										  AND Customers_Teams.TeamFk = TableB.TeamFk) 							 
									  OR (SELECT COUNT (*) 
										  FROM Customers_Teams 
										  WHERE Customers_Teams.CustomerFk = orders.CustomerFk) = 0 )
				  AND RepairOrderStatus != 4';
	ELSE IF (@Role = 'Wholesaler')
			SET @SqlSubQuery = '
			SELECT * 
			FROM RepairOrders orders
			WHERE CustomerFk = (SELECT cust.Id
										FROM Customers cust, Users us
										WHERE us.Id = ' + CAST(@userId AS varchar) + '
										AND us.Login = cust.Email) 	 
			AND CompanyFk = ' + CAST(@companyId AS varchar) + '
			AND RepairOrderStatus != 4 AND RepairOrderStatus != 3';
	ELSE 
		SET @SqlSubQuery = '
			SELECT * 
			FROM RepairOrders orders
			WHERE CompanyFk = ' + CAST(@companyId AS varchar) + '
			AND RepairOrderStatus != 4';

------------------------------------------------------------------------------------------
	DECLARE @SqlQueryForFilter nvarchar(max);

	SET @SqlQuery = 'SELECT	Id, CreationDate, EstimateFk, RepairOrderStatus, New, IsInvoice, CustomerFk
						FROM  (' + @SqlSubQuery +') table11
						WHERE  0=0';
		
	SET @SqlQueryForFilter = 'SELECT	CustomerFk
						FROM  (' + @SqlSubQuery +') table11
						WHERE  0=0';

	IF(@IsForCustomerFilter = 1)
		BEGIN
			IF(@IsForReport = 0) 
			BEGIN
				IF(@IsFinalised = 1)
					SET  @SqlQueryForFilter = @SqlQueryForFilter + ' AND RepairOrderStatus = 3';
				IF(@IsFinalised = 0) 
					SET  @SqlQueryForFilter = @SqlQueryForFilter + ' AND (RepairOrderStatus != 3 OR IsInvoice = 0)';
			END

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
																						 FROM Customers_Teams, Estimates
																						 WHERE Estimates.AffiliateFk IS NOT NULL
																						 AND Estimates.Id = EstimateFk
																						 AND Customers_Teams.CustomerFk = Estimates.AffiliateFk)
										   )
									  AND ((SELECT COUNT(TeamEmployeeFk)
											FROM TeamEmployeePercents, Users 
											WHERE TeamEmployeePercents.RepairOrderFk = table11.Id
											AND TeamEmployeePercents.TeamEmployeeFk = Users.Id
											AND Users.Class != ''Admin'') 
											=
											(SELECT COUNT(*) 
											FROM TeamEmployees_Teams
											WHERE TeamEmployees_Teams.TeamEmployeeFk IN(
												SELECT TeamEmployeeFk 
												FROM TeamEmployeePercents 
												WHERE RepairOrderFk = table11.Id)
											AND TeamEmployees_Teams.TeamFk = '  + CAST(@FilterByTeam AS varchar) + '))';

------------------------------------------------------------------------------------------

	INSERT INTO #RoTable (Id, CreationDate, EstimateFk, RepairOrderStatus, New, IsInvoice, CustomerFk)
	EXECUTE(@SqlQuery); 

------------------------------------------------------------------------------------------

	--Колличество новых документов
	SET @newActivityAmount = (SELECT COUNT(*) FROM #RoTable WHERE New = 1);

------------------------------------------------------------------------------------------

	SET @SqlQuery = 'SELECT Id, CreationDate, EstimateFk, RepairOrderStatus, New, IsInvoice, CustomerFk
				     FROM   #RoTable
				     WHERE  0=0';

	--выборка инвойсов по различным критериям
	IF(@RepairOrdersStatus IS NOT NULL)	
		SET @SqlQuery = @SqlQuery + ' AND RepairOrderStatus = ' + CAST(@RepairOrdersStatus AS varchar);

	IF(@FilterByCustomers IS NOT NULL)
		SET @SqlQuery = @SqlQuery + ' AND CustomerFk = ' + CAST(@FilterByCustomers AS varchar);

	IF(@IsForReport = 0)
	BEGIN
		IF(@IsFinalised = 1)
			SET  @SqlQuery = @SqlQuery + ' AND RepairOrderStatus = 3';
		IF(@IsFinalised = 0) 
			SET  @SqlQuery = @SqlQuery + ' AND (RepairOrderStatus != 3 OR IsInvoice = 0)';
	END

	IF(@IsInvoice IS NOT NULL)
		SET  @SqlQuery = @SqlQuery + 'IsInvoice = ' + CAST(@IsInvoice AS varchar);	
		
	IF(@VinCode IS NOT NULL)
		SET @SqlQuery = @SqlQuery + ' AND EstimateFk IN (SELECT es.Id
		                                                 FROM   Estimates es, Cars cs
		                                                 WHERE  es.CarFk = cs.Id
		                                                 AND    upper(cs.VIN) LIKE (''%'' + upper(''' + CAST(@VinCode AS varchar) + ''')))';
		
	IF(@Stock IS NOT NULL)
		SET @SqlQuery = @SqlQuery + ' AND EstimateFk IN (SELECT es.Id
		                                                    FROM   Estimates es, Cars cs
		                                                    WHERE  es.CarFk = cs.Id
		                                                    AND    upper(cs.Stock) = upper(''' + CAST(@Stock AS varchar) + '''))';
		
	IF(@CustRo IS NOT NULL)
		SET @SqlQuery = @SqlQuery + ' AND EstimateFk IN (SELECT es.Id
		                                                    FROM   Estimates es, Cars cs
		                                                    WHERE  es.CarFk = cs.Id
		                                                    AND    upper(cs.CustRo) = upper(''' + CAST(@CustRo AS varchar) + '''))';

	IF(@DateFrom IS NOT NULL)
		SET @SqlQuery = @SqlQuery + ' AND CreationDate >= ' + '''' + CAST(@DateFrom AS varchar) + '''';

	IF(@DateTo IS NOT NULL)
		SET @SqlQuery = @SqlQuery + ' AND CreationDate <= ' + '''' + CAST(@DateTo AS varchar) + '''';

	--для страницы Customers -> Locations делаем выборку по Affiliate
	IF(@AffiliateId IS NOT NULL)
		SET @SqlQuery = @SqlQuery + ' AND EstimateFk IN (SELECT es.Id
		                                                 FROM   Estimates es
		                                                 WHERE  es.AffiliateFk = ' + CAST(@AffiliateId AS varchar) + ')';

------------------------------------------------------------------------------------------

	INSERT INTO #RoResultTable (Id, CreationDate, EstimateFk, RepairOrderStatus, New, IsInvoice, CustomerFk)
	EXECUTE(@SqlQuery); 

------------------------------------------------------------------------------------------

	IF(@GetTotalAmount = 1) 
		SET @totalAmount = (SELECT SUM([dbo].GetTotalAmountForRO(#RoResultTable.Id)) FROM #RoResultTable);

	--Кастомеры для фильтра.
	--SET @customersForFilter = (
	--							SELECT CAST(customers.Id AS varchar) + ',' AS [text()] 
	--							FROM (SELECT Id 
	--							      FROM Customers 
	--								  WHERE Id IN( SELECT DISTINCT CustomerFk FROM #RoResultTable)) customers
	--							FOR XML PATH ('')
	--						  );
	
	--Общее колличество записей
	SET @totalCountRows = (SELECT COUNT(*) FROM #RoResultTable);

	--Сортировка RO и пагинация.
	--Для сортировки RO доступны следующие значения: CreationDate, TotalAmount, CarsMakeModelYear
	--Для типа сортировки два значения: ASC и DESC

	SET @SqlQuery = NULL;
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

		IF(((@SortByColumn = 'CreationDate') OR (@SortByColumn = 'TotalAmount') OR (@SortByColumn = 'CarsMakeModelYear')) 
					AND ((@SortType = 'ASC') OR (@SortType = 'DESC')))
			BEGIN
				SET @SqlQuery = 'SELECT Id
								 FROM (
											SELECT Id,
											ROW_NUMBER() OVER (ORDER BY ' + @SortByColumn + ' ' + @SortType + ') AS RowNum
											FROM #RoResultTable ) AS SOD
								WHERE SOD.RowNum BETWEEN ((' + CAST(@PageNumber AS varchar) + '-1)*' + CAST(@RowsPerPage AS varchar) + ')+1
								AND ' + CAST(@RowsPerPage AS varchar) + '*(' + CAST(@PageNumber AS varchar) + ')';

			END
	END
	DECLARE @sortedList AS TABLE (Id bigint NOT NULL);
	INSERT INTO @sortedList EXECUTE(@SqlQuery);


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
	RETURN
	END
