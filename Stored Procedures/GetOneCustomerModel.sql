GO
/****** Object:  StoredProcedure [dbo].[GetCustomerModels]    Script Date: 2014-09-16 12:25:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[GetOneCustomerModel] 
	@userId bigint,
	@customerId bigint,
	@FilterByTeam bigint = NULL,
	@IsOnlyOwn bit = 0,
	@CustomerType nvarchar(50),

	@AmountOfOpenEstimates int OUTPUT,
	@SumOfOpenEstimates Decimal(32,2) OUTPUT,
	@SumOfOpenWorkOrders Decimal(32,2) OUTPUT,
	@SumOfPaidInvoices Decimal(32,2) OUTPUT,
	@SumOfUnpaidInvoices Decimal(32,2) OUTPUT

AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @companyId bigint;
	SET @companyId = (SELECT CompanyFk FROM Users WHERE Id = @userId);
	DECLARE @Role nvarchar(255);
	SET @Role = (SELECT Class FROM Users WHERE Id = @userId);


	--to do check logic..
	IF(@CustomerType != 'affiliate' AND NOT(@Role = 'Manager'AND @IsOnlyOwn = 0 AND (SELECT IsShowAllTeams FROM Users WHERE Id = @userId) = 0))
	BEGIN
		IF(@IsOnlyOwn = 0 AND @FilterByTeam IS NULL)
		BEGIN 
				SET @AmountOfOpenEstimates = (SELECT COUNT(*) FROM Estimates 
				WHERE CompanyFk = @companyId
				AND EstimateStatus != 4
				AND Archived = 0
				AND CustomerFk = @customerId
				);

				SET @SumOfOpenEstimates = (SELECT SUM(TotalAmount) FROM Estimates 
				WHERE CompanyFk = @companyId
				AND EstimateStatus != 4
				AND Archived = 0
				AND CustomerFk = @customerId
				);
		END
		IF(@IsOnlyOwn = 1 AND @FilterByTeam IS NULL)
		BEGIN
				SET @AmountOfOpenEstimates = (SELECT COUNT(*) FROM Estimates 
				WHERE CompanyFk = @companyId
				AND EstimateStatus != 4
				AND Archived = 0
				AND CustomerFk = @customerId
				AND EmployeeFk = @userId
				);

				SET @SumOfOpenEstimates = (SELECT SUM(TotalAmount) FROM Estimates 
				WHERE CompanyFk = @companyId
				AND EstimateStatus != 4
				AND Archived = 0
				AND CustomerFk = @customerId
				AND EmployeeFk = @userId
				);
		END
		IF(@IsOnlyOwn = 0 AND @FilterByTeam IS NOT NULL)
		BEGIN
				SET @AmountOfOpenEstimates = (SELECT COUNT(*) FROM Estimates 
				WHERE CompanyFk = @companyId
				AND EstimateStatus != 4
				AND Archived = 0
				AND CustomerFk = @customerId
				AND Estimates.CustomerFk IN (SELECT CustomerFk FROM Customers_Teams WHERE TeamFk = @FilterByTeam));

				SET @SumOfOpenEstimates = (SELECT SUM(TotalAmount) FROM Estimates 
				WHERE CompanyFk = @companyId
				AND EstimateStatus != 4
				AND Archived = 0
				AND CustomerFk = @customerId
				AND Estimates.CustomerFk IN (SELECT CustomerFk FROM Customers_Teams WHERE TeamFk = @FilterByTeam));
		END
	END
	IF(@CustomerType = 'affiliate')
	BEGIN
		IF(@IsOnlyOwn = 0 AND @FilterByTeam IS NULL)
		BEGIN 
				SET @AmountOfOpenEstimates = (SELECT COUNT(*) FROM Estimates 
				WHERE CompanyFk = @companyId
				AND EstimateStatus != 4
				AND Archived = 0
				AND AffiliateFk = @customerId
				);

				SET @SumOfOpenEstimates = (SELECT SUM(TotalAmount) FROM Estimates 
				WHERE CompanyFk = @companyId
				AND EstimateStatus != 4
				AND Archived = 0
				AND AffiliateFk = @customerId
				);
		END
		IF(@IsOnlyOwn = 1 AND @FilterByTeam IS NULL)
		BEGIN
				SET @AmountOfOpenEstimates = (SELECT COUNT(*) FROM Estimates 
				WHERE CompanyFk = @companyId
				AND EstimateStatus != 4
				AND Archived = 0
				AND AffiliateFk = @customerId
				AND EmployeeFk = @userId
				);

				SET @SumOfOpenEstimates = (SELECT SUM(TotalAmount) FROM Estimates 
				WHERE CompanyFk = @companyId
				AND EstimateStatus != 4
				AND Archived = 0
				AND AffiliateFk = @customerId
				AND EmployeeFk = @userId
				);
		END
		IF(@IsOnlyOwn = 0 AND @FilterByTeam IS NOT NULL)
		BEGIN
				SET @AmountOfOpenEstimates = (SELECT COUNT(*) FROM Estimates 
				WHERE CompanyFk = @companyId
				AND EstimateStatus != 4
				AND Archived = 0
				AND AffiliateFk = @customerId
				AND Estimates.CustomerFk IN (SELECT CustomerFk FROM Customers_Teams WHERE TeamFk = @FilterByTeam));

				SET @SumOfOpenEstimates = (SELECT SUM(TotalAmount) FROM Estimates 
				WHERE CompanyFk = @companyId
				AND EstimateStatus != 4
				AND Archived = 0
				AND AffiliateFk = @customerId
				AND Estimates.CustomerFk IN (SELECT CustomerFk FROM Customers_Teams WHERE TeamFk = @FilterByTeam));
		END
	END

		DECLARE	@AffId bigint,	@Customer bigint;

		IF(@CustomerType = 'affiliate') 
		BEGIN
			SET @AffId = @customerId; 
			SET @Customer = NULL;
		END 
		ELSE
		BEGIN
			SET @AffId = NULL;
			SET @Customer = @customerId;
		END 

		DECLARE
			@totalCountRows int,
			@customersForFilter nvarchar(max),
			@newActivityAmount int,
			@invoiceIds nvarchar(max),
			@paidInvSum float,
			@unpaidInvSum float,
			@EstimatesIds nvarchar(max),
			@EstimatesTotalAmountSum float,
			@repairOrdersIds nvarchar(max),
			@totalAmount decimal(32, 2)

		EXEC [dbo].[GetInvoicesByUser]
			@userId = @userId,
			@FilterByCustomers = @Customer,
			@IsOnlyOwn = @IsOnlyOwn,
			@GetPaidUnpaidInvoicesSum = 1,
			@AffiliateId = @AffId,
			@totalCountRows = @totalCountRows OUTPUT,
			@customersForFilter = @customersForFilter OUTPUT,
			@newActivityAmount = @newActivityAmount OUTPUT,
			@invoiceIds = @invoiceIds OUTPUT,
			@paidInvSum = @paidInvSum OUTPUT,
			@unpaidInvSum = @unpaidInvSum OUTPUT;

		EXEC [dbo].[GetRepairOrdersByUser]
			@userId = @userId,
			@FilterByCustomers = @Customer,
			@IsOnlyOwn = @IsOnlyOwn,
			@IsGetAllRo = 1,
			@GetTotalAmount = 1,
			@AffiliateId = @AffId,
			@totalCountRows = @totalCountRows OUTPUT,
			@customersForFilter = @customersForFilter OUTPUT,
			@newActivityAmount = @newActivityAmount OUTPUT,
			@repairOrdersIds = @repairOrdersIds OUTPUT,
			@totalAmount = @totalAmount OUTPUT

		IF(@Role = 'Manager'AND @IsOnlyOwn = 0 AND (SELECT IsShowAllTeams FROM Users WHERE Id = @userId) = 0)
		BEGIN
			EXEC [dbo].[GetEstimatesByUser]
			@userId = @userId,
			@FilterByCustomers = @Customer,
			@IsOnlyOwn = @IsOnlyOwn,
			@IsGetAllEstimates = 1,
			@GetTotalAmountSum = 1,
			@totalCountRows = @totalCountRows OUTPUT,
			@customersForFilter = @customersForFilter OUTPUT,
			@newActivityAmount = @newActivityAmount OUTPUT,
			@EstimatesIds = @EstimatesIds OUTPUT,
			@EstimatesTotalAmountSum = @EstimatesTotalAmountSum OUTPUT

			SET @SumOfOpenEstimates = @EstimatesTotalAmountSum;
			SET @AmountOfOpenEstimates = @totalCountRows;
		END

		IF(@paidInvSum IS NULL) SET @paidInvSum = 0;
		IF(@unpaidInvSum IS NULL) SET @unpaidInvSum = 0;
		IF(@totalAmount IS NULL) SET @totalAmount = 0;

		SET @SumOfPaidInvoices = @paidInvSum;
		SET @SumOfUnpaidInvoices = @unpaidInvSum;
		SET @SumOfOpenWorkOrders = @totalAmount;
END	
