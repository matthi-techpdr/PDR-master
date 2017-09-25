SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	��������� ����� ���������� ��� ����������� ������
-- ��� ������ ������������ �� �������� Estimates.
-- =============================================
CREATE PROCEDURE GetEstimatesByUser 
	@userId bigint,
	@FilterByTeam bigint = NULL,
	@FilterByCustomers bigint = NULL,
	@IsOnlyOwn bit = 0,
	@RowsPerPage int = 10,
	@PageNumber int = 1, --��������� ������� ���������� � 1
	@SortByColumn nvarchar(50) = NULL,
	@SortType  nvarchar(10) = 'DESC', --����� ��������� ��� �������� DESC  ��� ASC
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

	--������� ��� �������� ������������� �������� Estimates
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
		NewLaborRate float NULL
		);
	
	--������� Estimates � ����������� �� �������� � ���������������� ����
	INSERT #EstimatesResultTable EXEC [dbo].[GetEstimatesByRole] @userId = @userId, @companyId = @companyId,
									@Role =@Role, @IsOnlyOwn = @IsOnlyOwn, @IsForReport = @IsForReport;

    ALTER TABLE #EstimatesResultTable ADD CarsMakeModelYear nvarchar(255) NULL DEFAULT NULL;

	IF(@FilterByTeam IS NOT NULL) 
	DELETE FROM #EstimatesResultTable 
	WHERE  (@FilterByTeam NOT IN (SELECT TeamFk FROM Customers_Teams WHERE CustomerFk = #EstimatesResultTable.CustomerFk))
			AND (#EstimatesResultTable.CustomerFk NOT IN (SELECT CustomerFk FROM Customers_Teams WHERE TeamFk = @FilterByTeam)); 
	
	--����������� ����� ����������
	SET @newActivityAmount = (SELECT COUNT(*) FROM #EstimatesResultTable WHERE New = 1);

	--������� Estimates �� ��������� ���������
	IF(@EstimatesStatus IS NOT NULL) DELETE FROM #EstimatesResultTable WHERE EstimateStatus != @EstimatesStatus;

	IF(@FilterByCustomers IS NOT NULL) DELETE FROM #EstimatesResultTable WHERE CustomerFk != @FilterByCustomers;

	IF(@IsForReport = 0) DELETE FROM #EstimatesResultTable WHERE Archived != @IsArchived;

	IF(@VinCode IS NOT NULL) 
	BEGIN
		DELETE FROM #EstimatesResultTable 
		WHERE #EstimatesResultTable.Id NOT IN(
						SELECT #EstimatesResultTable.Id FROM #EstimatesResultTable, Cars
						WHERE #EstimatesResultTable.CarFk = Cars.Id 
							  AND upper(Cars.VIN) LIKE ('%' + upper(@VinCode))											
			  );
	END

	IF(@Stock IS NOT NULL) 
	BEGIN 
		DELETE FROM #EstimatesResultTable 
		WHERE #EstimatesResultTable.Id NOT IN(
						SELECT #EstimatesResultTable.Id FROM #EstimatesResultTable, Cars
						WHERE  #EstimatesResultTable.CarFk = Cars.Id 
							  AND upper(Cars.Stock) = upper(@Stock)										
			  );
	END

	IF(@CustRo IS NOT NULL)
	BEGIN 
		DELETE FROM #EstimatesResultTable 
		WHERE #EstimatesResultTable.Id NOT IN(
						SELECT #EstimatesResultTable.Id FROM #EstimatesResultTable, Cars
						WHERE   #EstimatesResultTable.CarFk = Cars.Id 
								AND upper(Cars.CustRO) = upper(@CustRo)										
				);
	END

	IF(@DateFrom IS NOT NULL) DELETE FROM #EstimatesResultTable WHERE CreationDate < @DateFrom;

	IF(@DateTo IS NOT NULL) DELETE FROM #EstimatesResultTable WHERE CreationDate > @DateTo;

	--��� �������� Customers -> Locations ������ ������� �� Affiliate
	IF(@AffiliateId IS NOT NULL) DELETE FROM #EstimatesResultTable
								 WHERE #EstimatesResultTable.AffiliateFk IS NULL
								 OR #EstimatesResultTable.AffiliateFk != @AffiliateId

	--��������� ��� �������.
	SET @customersForFilter = (
								SELECT CAST(customers.Id AS varchar) + ',' AS [text()] 
								FROM (SELECT Id 
								      FROM Customers 
									  WHERE Id IN( SELECT DISTINCT CustomerFk FROM #EstimatesResultTable)) customers
								FOR XML PATH ('')
							  );
	
	--����� ����������� �������
	SET @totalCountRows = (SELECT COUNT(*) FROM #EstimatesResultTable);

	IF(@GetTotalAmountSum = 1) SET @EstimatesTotalAmountSum = (SELECT SUM(#EstimatesResultTable.TotalAmount) FROM #EstimatesResultTable);

	--���������� Estimates � ���������.
	--��� ���������� Estimates �������� ��������� ��������: CreationDate, TotalAmount, CarsMakeModelYear
	--��� ���� ���������� ��� ��������: ASC � DESC
	DECLARE @sqlQuery nvarchar(max);
	IF(@SortByColumn IS NOT NULL) 
	BEGIN
		--���� ���������� �� CarsMakeModelYear - ��������� ������� �������������� �������
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

	--������� ���� Estimates:
	IF((@IsGetAllEstimates IS NOT NULL) AND (@IsGetAllEstimates = 1))
	BEGIN
		SET @EstimatesIds = (
								SELECT CAST(docs.Id AS varchar) + ',' AS [text()] 
								FROM (SELECT Id FROM #EstimatesResultTable) docs
								FOR XML PATH ('')
					   );
		RETURN
	END

	--��������� ������ ���������� ��������������� Estimates
	SET @EstimatesIds = (
								SELECT CAST(docs.Id AS varchar) + ',' AS [text()] 
								FROM (SELECT Id FROM @sortedList) docs
								FOR XML PATH ('')
					   );
	RETURN
	END
GO
