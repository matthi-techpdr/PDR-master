USE [PDR_test]
GO
/****** Object:  StoredProcedure [dbo].[GetEstimatesByUser]    Script Date: 6/22/2015 2:48:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	Процедура будет возвращать все необходимые данные
-- для работы пользователя со списками Estimates.
-- =============================================
ALTER PROCEDURE [dbo].[GetEstimatesByUser]
	@userId bigint,
	@FilterByTeam bigint = NULL,
	@FilterByCustomers bigint = NULL,
	@IsOnlyOwn bit = 0,
	@RowsPerPage int = 10,
	@PageNumber int = 1, --нумерация страниц начинается с 1
	@SortByColumn nvarchar(50) = NULL,
	@SortType  nvarchar(10) = 'DESC', --Может принимать два значения DESC  или ASC
	@VinCode nvarchar(50) = NULL,
	@Stock nvarchar(50) = NULL,
	@CustRo nvarchar(50) = NULL,
	@EstimatesStatus smallint = NULL,
	@DateFrom datetime = NULL,
	@DateTo datetime = NULL,
	@IsForReport bit = 0,
	@IsArchived bit = 0,
	@AffiliateId bigint = NULL,
	@IsGetAllEstimates bit = NULL,
	@GetTotalAmountSum bit = 0,
	@IsForCustomerFilter bit = NULL,

	@totalCountRows int OUTPUT,
	@customersForFilter nvarchar(max) OUTPUT,
	@newActivityAmount int OUTPUT,
	@EstimatesIds nvarchar(max) OUTPUT,
	@EstimatesTotalAmountSum float OUTPUT

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

--Таблицы для хранения промежуточных значений Estimate

	CREATE TABLE #EstimatesTable(
		Id bigint NOT NULL,
		CreationDate datetime NULL,
		PriorDamages nvarchar(max) NULL,
		EstimateStatus smallint NULL,
		Signature bit NULL,
		Archived bit NULL,
		CompanyFk bigint NULL,
		EmployeeFk bigint NULL,
		InsuranceFk bigint NULL,
		CarFk bigint NULL,
		CustomerFk bigint NULL,
		MatrixFk bigint NULL,
		New bit NULL,
		UpdatedSum float NULL,
		CalculatedSum float NULL,
		SignatureImage varbinary(max) NULL,
		VINStatus nvarchar(max) NULL,
		LaborSum float NULL,
		Subtotal float NULL,
		DiscountSum float NULL,
		TaxSum float NULL,
		TotalAmount float NULL,
		EstHourlyRate float NULL,
		EstDiscount float NULL,
		EstLaborTax float NULL,
		EstMaxCorProtect float NULL,
		EstAluminiumPer float NULL,
		EstOversizedRoofPer float NULL,
		EstDoubleMetalPer float NULL,
		EstLimitForBodyPart float NULL,
		EstOversizedDents float NULL,
		EstMaxPercent float NULL,
		EstCorProtectPart float NULL,
		CustomerSignatureFk bigint NULL,
		ExtraQuickCost float NULL,
		Type smallint NULL,
		CarImageFk bigint NULL,
		WorkByThemselve bit NULL,
		AffiliateFk bigint NULL,
		NewLaborRate float NULL,
		CarsMakeModelYear nvarchar(255) NULL
		);

	CREATE TABLE #EstimatesResultTable(
			Id bigint NOT NULL,
		CreationDate datetime NULL,
		PriorDamages nvarchar(max) NULL,
		EstimateStatus smallint NULL,
		Signature bit NULL,
		Archived bit NULL,
		CompanyFk bigint NULL,
		EmployeeFk bigint NULL,
		InsuranceFk bigint NULL,
		CarFk bigint NULL,
		CustomerFk bigint NULL,
		MatrixFk bigint NULL,
		New bit NULL,
		UpdatedSum float NULL,
		CalculatedSum float NULL,
		SignatureImage varbinary(max) NULL,
		VINStatus nvarchar(max) NULL,
		LaborSum float NULL,
		Subtotal float NULL,
		DiscountSum float NULL,
		TaxSum float NULL,
		TotalAmount float NULL,
		EstHourlyRate float NULL,
		EstDiscount float NULL,
		EstLaborTax float NULL,
		EstMaxCorProtect float NULL,
		EstAluminiumPer float NULL,
		EstOversizedRoofPer float NULL,
		EstDoubleMetalPer float NULL,
		EstLimitForBodyPart float NULL,
		EstOversizedDents float NULL,
		EstMaxPercent float NULL,
		EstCorProtectPart float NULL,
		CustomerSignatureFk bigint NULL,
		ExtraQuickCost float NULL,
		Type smallint NULL,
		CarImageFk bigint NULL,
		WorkByThemselve bit NULL,
		AffiliateFk bigint NULL,
		NewLaborRate float NULL,
		CarsMakeModelYear nvarchar(255) NULL
		);

------------------------------------------------------------------------------------------
	--выборка Estimates в зависимости от компании и пользовательской роли
	IF ((@Role = 'Technician') OR (@Role = 'Estimator') OR (@IsOnlyOwn = 1))
			IF(@IsForReport = 1)
					SET @SqlSubQuery = '
					SELECT *
					FROM Estimates
					WHERE (EmployeeFk = ' + CAST(@userId AS varchar) + '
						  OR Id IN (SELECT EstimateFk FROM PreEmployees_PreEstimates WHERE EmployeeFk = ' + CAST(@userId AS varchar) + '))
						  AND CompanyFk = ' + CAST(@companyId AS varchar) + '
						  AND EstimateStatus != 4';
			ELSE
					SET @SqlSubQuery = '
					SELECT * 
					FROM Estimates 
					WHERE EmployeeFk = ' + CAST(@userId AS varchar) + ' 	
						  AND CompanyFk = ' + CAST(@companyId AS varchar) + '
						  AND EstimateStatus != 4';
	ELSE IF (@Role = 'Manager' AND @IsOnlyOwn = 0 AND (SELECT IsShowAllTeams FROM Users WHERE Id = @userId) = 0)
				SET @SqlSubQuery = '
				SELECT *
				FROM Estimates estimates 
				WHERE CompanyFk = ' + CAST(@companyId AS varchar) + '
					  AND EstimateStatus != 4
					  AND estimates.Id IN (SELECT Id FROM (SELECT Id FROM Estimates WHERE EmployeeFk IN (SELECT TeamEmployeeFk FROM (SELECT TeamEmployeeFk FROM TeamEmployees_Teams WHERE TeamFk IN (SELECT TeamFk FROM (SELECT TeamFk FROM TeamEmployees_Teams WHERE TeamEmployeeFk = ' + CAST(@userId AS varchar) + ') t12)) t15)) t13) 	  
					  AND (estimates.CustomerFk IN (SELECT Customers_Teams.CustomerFk 
											  FROM Customers_Teams 									 
											  INNER JOIN (SELECT TeamFk FROM (SELECT TeamFk FROM TeamEmployees_Teams WHERE TeamEmployeeFk = ' + CAST(@userId AS varchar) + ') t14) TableB 									 
											  ON Customers_Teams.CustomerFk = estimates.CustomerFk 
											  AND Customers_Teams.TeamFk = TableB.TeamFk) 							 
										  OR (SELECT COUNT (*) 
											  FROM Customers_Teams 
											  WHERE Customers_Teams.CustomerFk = estimates.CustomerFk) = 0 )';
		ELSE IF (@Role = 'Wholesaler')
					SET @SqlSubQuery = '
					SELECT * 
					FROM Estimates 
					WHERE CustomerFk = (SELECT cust.Id
										FROM Customers cust, Users us
										WHERE us.Id = ' + CAST(@userId AS varchar) + '
										AND us.Login = cust.Email) 	
							AND CompanyFk = ' + CAST(@companyId AS varchar) + '
							AND EstimateStatus != 4 AND EstimateStatus != 3';
		ELSE 
				SET @SqlSubQuery = '
				SELECT * 
				FROM Estimates 
				WHERE CompanyFk = ' + CAST(@companyId AS varchar) + '
					  AND EstimateStatus != 4';

------------------------------------------------------------------------------------------
	DECLARE @SqlQueryForFilter nvarchar(max);

	SET @SqlQuery = 'SELECT Id,	CreationDate, PriorDamages, EstimateStatus, [Signature],	Archived, CompanyFk, EmployeeFk, InsuranceFk, CarFk, CustomerFk, 
							MatrixFk, New, UpdatedSum, CalculatedSum, SignatureImage, VINStatus, LaborSum, Subtotal, DiscountSum, TaxSum, TotalAmount, EstHourlyRate, 
							EstDiscount, EstLaborTax, EstMaxCorProtect, EstAluminiumPer, EstOversizedRoofPer, EstDoubleMetalPer,	EstLimitForBodyPart, EstOversizedDents, 
							EstMaxPercent, EstCorProtectPart, CustomerSignatureFk, ExtraQuickCost, [Type], CarImageFk, WorkByThemselve, AffiliateFk, NewLaborRate
					 FROM  (' + @SqlSubQuery +') table21
					 WHERE  0=0';

	SET @SqlQueryForFilter = 'SELECT CustomerFk
					 FROM  (' + @SqlSubQuery +') table21
					 WHERE  0=0';

	IF(@IsForCustomerFilter = 1)
		BEGIN
			IF(@IsForReport = 0) SET @SqlQueryForFilter = @SqlQueryForFilter + ' AND Archived = ' + CAST(@IsArchived AS varchar);

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
											OR (AffiliateFk IS NOT NULL
												AND ' + CAST(@FilterByTeam AS varchar) +' IN (SELECT Customers_Teams.TeamFk 
																							  FROM Customers_Teams
																							  WHERE Customers_Teams.CustomerFk = AffiliateFk)
												)
										   )
									  AND EmployeeFk IN (SELECT TeamEmployeeFk
														 FROM TeamEmployees_Teams
														 WHERE TeamFk = ' + CAST(@FilterByTeam AS varchar) + ')';

------------------------------------------------------------------------------------------
	INSERT INTO #EstimatesTable
	(Id, CreationDate, PriorDamages, EstimateStatus, [Signature],	Archived, CompanyFk, EmployeeFk, InsuranceFk, CarFk, CustomerFk, 
							MatrixFk, New, UpdatedSum, CalculatedSum, SignatureImage, VINStatus, LaborSum, Subtotal, DiscountSum, TaxSum, TotalAmount, EstHourlyRate, 
							EstDiscount, EstLaborTax, EstMaxCorProtect, EstAluminiumPer, EstOversizedRoofPer, EstDoubleMetalPer,	EstLimitForBodyPart, EstOversizedDents, 
							EstMaxPercent, EstCorProtectPart, CustomerSignatureFk, ExtraQuickCost, [Type], CarImageFk, WorkByThemselve, AffiliateFk, NewLaborRate)
	EXECUTE(@SqlQuery);
------------------------------------------------------------------------------------------
	--Колличество новых документов
	SET @newActivityAmount = (SELECT COUNT(*) FROM #EstimatesTable WHERE New = 1);
------------------------------------------------------------------------------------------

	SET @SqlQuery = 'SELECT Id,	CreationDate, PriorDamages, EstimateStatus, [Signature],	Archived, CompanyFk, EmployeeFk, InsuranceFk, CarFk, CustomerFk, 
							MatrixFk, New, UpdatedSum, CalculatedSum, SignatureImage, VINStatus, LaborSum, Subtotal, DiscountSum, TaxSum, TotalAmount, EstHourlyRate, 
							EstDiscount, EstLaborTax, EstMaxCorProtect, EstAluminiumPer, EstOversizedRoofPer, EstDoubleMetalPer,	EstLimitForBodyPart, EstOversizedDents, 
							EstMaxPercent, EstCorProtectPart, CustomerSignatureFk, ExtraQuickCost, [Type], CarImageFk, WorkByThemselve, AffiliateFk, NewLaborRate
				     FROM   #EstimatesTable
				     WHERE  0=0';

	--выборка Estimates по различным критериям
	IF(@EstimatesStatus IS NOT NULL) SET @SqlQuery = @SqlQuery + ' AND EstimateStatus = ' + CAST(@EstimatesStatus AS varchar);

	IF(@FilterByCustomers IS NOT NULL) SET @SqlQuery = @SqlQuery+ ' AND CustomerFk = ' + CAST(@FilterByCustomers AS varchar);

	IF(@IsForReport = 0) SET @SqlQuery = @SqlQuery + ' AND Archived = ' + CAST(@IsArchived AS varchar);

	IF(@VinCode IS NOT NULL) 
		SET @SqlQuery = @SqlQuery + ' AND CarFk IN (SELECT cs.Id
		                                                FROM  Cars cs
		                                                WHERE upper(cs.VIN) LIKE (''%'' + upper(''' + CAST(@VinCode AS varchar) + ''')))';
	IF(@Stock IS NOT NULL) 
		SET @SqlQuery = @SqlQuery + ' AND CarFk IN (SELECT cs.Id
		                                                FROM  Cars cs
		                                                WHERE upper(cs.Stock) = upper(''' + CAST(@Stock AS varchar) + '''))';
	IF(@CustRo IS NOT NULL)
		SET @SqlQuery = @SqlQuery + ' AND CarFk IN (SELECT cs.Id
		                                                FROM   Cars cs
		                                                WHERE  upper(cs.CustRo) = upper(''' + CAST(@CustRo AS varchar) + '''))';
	IF(@DateFrom IS NOT NULL)
		SET @SqlQuery = @SqlQuery + ' AND CreationDate >= ' + '''' + CAST(@DateFrom AS varchar) + '''';

	IF(@DateTo IS NOT NULL)
		SET @SqlQuery = @SqlQuery + ' AND CreationDate <= ' + '''' + CAST(@DateTo AS varchar) + '''';

	--для страницы Customers -> Locations делаем выборку по Affiliate
	IF(@AffiliateId IS NOT NULL)
		SET @SqlQuery = @SqlQuery + ' AND AffiliateFk = ' + CAST(@AffiliateId AS varchar);

------------------------------------------------------------------------------------------
	INSERT INTO #EstimatesResultTable
	(Id, CreationDate, PriorDamages, EstimateStatus, [Signature],	Archived, CompanyFk, EmployeeFk, InsuranceFk, CarFk, CustomerFk, 
							MatrixFk, New, UpdatedSum, CalculatedSum, SignatureImage, VINStatus, LaborSum, Subtotal, DiscountSum, TaxSum, TotalAmount, EstHourlyRate, 
							EstDiscount, EstLaborTax, EstMaxCorProtect, EstAluminiumPer, EstOversizedRoofPer, EstDoubleMetalPer,	EstLimitForBodyPart, EstOversizedDents, 
							EstMaxPercent, EstCorProtectPart, CustomerSignatureFk, ExtraQuickCost, [Type], CarImageFk, WorkByThemselve, AffiliateFk, NewLaborRate)
	EXECUTE(@SqlQuery);
------------------------------------------------------------------------------------------

	--Кастомеры для фильтра.
	--SET @customersForFilter = (
	--							SELECT CAST(customers.Id AS varchar) + ',' AS [text()] 
	--							FROM (SELECT Id 
	--							      FROM Customers 
	--								  WHERE Id IN( SELECT DISTINCT CustomerFk FROM #EstimatesResultTable)) customers
	--							FOR XML PATH ('')
	--						  );
	
	--Общее колличество записей
	SET @totalCountRows = (SELECT COUNT(*) FROM #EstimatesResultTable);

	IF(@GetTotalAmountSum = 1) SET @EstimatesTotalAmountSum = (SELECT SUM(#EstimatesResultTable.TotalAmount) FROM #EstimatesResultTable);

	--Сортировка Estimates и пагинация.
	--Для сортировки Estimates доступны следующие значения: CreationDate, TotalAmount, CarsMakeModelYear
	--Для типа сортировки два значения: ASC и DESC
	SET @SqlQuery = NULL;
	IF(@SortByColumn IS NOT NULL) 
	BEGIN
		--Если сортировка по CarsMakeModelYear - заполняем данными соответсвующий столбец
		IF (@SortByColumn = 'CarsMakeModelYear') 
			BEGIN
				UPDATE #EstimatesResultTable
				SET CarsMakeModelYear = (SELECT CAST(Cars.Year AS varchar) + '/' +  Cars.Make + '/' + Cars.Model 
															  FROM Cars
															  WHERE #EstimatesResultTable.CarFk = Cars.Id);
			END 

		IF(((@SortByColumn = 'CreationDate') OR (@SortByColumn = 'TotalAmount') OR (@SortByColumn = 'CarsMakeModelYear')) 
					AND ((@SortType = 'ASC') OR (@SortType = 'DESC')))
			BEGIN
				SET @sqlQuery = 'SELECT Id
								 FROM (
											SELECT Id,
											ROW_NUMBER() OVER (ORDER BY ' + @SortByColumn + ' ' + @SortType + ') AS RowNum
											FROM #EstimatesResultTable ) AS SOD
								WHERE SOD.RowNum BETWEEN ((' + CAST(@PageNumber AS varchar) + '-1)*' + CAST(@RowsPerPage AS varchar) + ')+1
								AND ' + CAST(@RowsPerPage AS varchar) + '*(' + CAST(@PageNumber AS varchar) + ')';

			END
	END
	DECLARE @sortedList AS TABLE (Id bigint NOT NULL);
	INSERT INTO @sortedList EXECUTE(@sqlQuery);

	--Выборка всех Estimates:
	IF((@IsGetAllEstimates IS NOT NULL) AND (@IsGetAllEstimates = 1))
	BEGIN
		SET @EstimatesIds = (
								SELECT CAST(docs.Id AS varchar) + ',' AS [text()] 
								FROM (SELECT Id FROM #EstimatesResultTable) docs
								FOR XML PATH ('')
					   );
		RETURN
	END

	--Получение списка айдишников отсортировынных Estimates
	SET @EstimatesIds = (
								SELECT CAST(docs.Id AS varchar) + ',' AS [text()] 
								FROM (SELECT Id FROM @sortedList) docs
								FOR XML PATH ('')
					   );
	RETURN
	END
