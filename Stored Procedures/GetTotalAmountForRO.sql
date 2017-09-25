-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION GetTotalAmountForRO
(
	@RepairOrderId bigint
)
RETURNS Decimal(32,2)
AS
BEGIN
	DECLARE @estId bigint;
	SET @estId= (SELECT EstimateFk 
				 FROM RepairOrders 
				 WHERE RepairOrders.Id = @RepairOrderId);

	DECLARE @TotalAmount Decimal(32,16);
	SET  @TotalAmount = (SELECT TotalAmount FROM Estimates WHERE Id = @estId);
	IF (@TotalAmount IS NULL) SET  @TotalAmount = 0;

	DECLARE @EstimatesTaxSum Decimal(32,16);
	SET @EstimatesTaxSum = (SELECT TaxSum FROM Estimates WHERE Id = @estId);
	IF(@EstimatesTaxSum IS NULL) SET @EstimatesTaxSum = 0;

	DECLARE @CleanTotalAmount Decimal(32,16);
	SET @CleanTotalAmount = @TotalAmount - @EstimatesTaxSum;
	IF(@CleanTotalAmount IS NULL) SET @CleanTotalAmount = 0;

	DECLARE @SupplementsSum Decimal(32,16);
	SET @SupplementsSum = (SELECT SUM(Supplements.Sum) FROM Supplements WHERE RepairOrderFk = @RepairOrderId);
	IF(@SupplementsSum IS NULL) SET @SupplementsSum = 0;

	DECLARE @currrentCustomerId bigint;
	SET @currrentCustomerId = (SELECT CustomerFk FROM Estimates WHERE Id = @estId);
	DECLARE @CurrentHourlyRate Decimal(32,16);
	DECLARE @ClassCustomer nvarchar(255);
	DECLARE @AffiliateId bigint;
	SET @AffiliateId = (SELECT AffiliateFk FROM Estimates WHERE Id = @estId);
	SET  @ClassCustomer = (SELECT Class FROM Customers WHERE Id = @currrentCustomerId);

	IF(@ClassCustomer = 'WholesaleCustomer') SET @CurrentHourlyRate = (SELECT HourlyRate FROM Customers WHERE Id = @currrentCustomerId)
	ELSE IF(@AffiliateId IS NOT NULL) SET @CurrentHourlyRate = (SELECT HourlyRate FROM Customers WHERE Id = @AffiliateId)
	ELSE SET @CurrentHourlyRate = 0;
	IF(@CurrentHourlyRate IS NULL) SET @CurrentHourlyRate = 0;

	DECLARE @TotalHours Decimal(32,16);
	SET @TotalHours =  (SELECT SUM(Hours) 
						FROM ChosenEffortItems
						WHERE CarInspectionFK IN (SELECT Id FROM CarInspections WHERE EstimateFk = @estId));
	IF(@TotalHours IS NULL) SET @TotalHours = 0;
	
	DECLARE @TotalLaborHours Decimal(32,16);
	SET @TotalLaborHours = (SELECT [dbo].RoundUp(@TotalHours, 2));
	IF(@TotalLaborHours IS NULL) SET @TotalLaborHours =0;

	DECLARE @LaborSum Decimal(32,16);
	DECLARE @NewLaborRate Decimal(32,16);
	SET @NewLaborRate = (SELECT est.NewLaborRate
						 FROM Estimates est 
						 WHERE est.Id = @estId);
	IF (@NewLaborRate IS NULL) SET @LaborSum = (SELECT [dbo].RoundUp(@TotalLaborHours * @CurrentHourlyRate, 2));
	ELSE SET @LaborSum = (SELECT [dbo].RoundUp(@TotalLaborHours * @NewLaborRate, 2));
	
	IF(@LaborSum IS NULL) SET @LaborSum =0;

	DECLARE @RepairOrderSumWithoutDiscountAndTax Decimal(32,16);
	SET @RepairOrderSumWithoutDiscountAndTax = @CleanTotalAmount + @SupplementsSum;
	IF(@RepairOrderSumWithoutDiscountAndTax IS NULL) SET @RepairOrderSumWithoutDiscountAndTax = 0;

	DECLARE @RepairOrderSumWithoutLaborSum Decimal(32,16);
	SET @RepairOrderSumWithoutLaborSum = @RepairOrderSumWithoutDiscountAndTax - @LaborSum;
	IF(@RepairOrderSumWithoutLaborSum IS NULL) SET @RepairOrderSumWithoutLaborSum = 0;

	DECLARE @WorkByThemselve bit;
	SET @WorkByThemselve = (SELECT WorkByThemselve FROM RepairOrders WHERE Id = @RepairOrderId);  
	DECLARE @AdditionalDiscount Decimal(32,16);
	SET @AdditionalDiscount = (SELECT AdditionalDiscount FROM RepairOrders WHERE Id = @RepairOrderId);
	IF(@AdditionalDiscount IS NULL) SET @AdditionalDiscount = 0;

	DECLARE @RepairOrderSum Decimal(32,16);
	IF(@WorkByThemselve = 1) SET @RepairOrderSum = (SELECT [dbo].RoundUp(@RepairOrderSumWithoutLaborSum - @AdditionalDiscount, 2))
	ELSE SET @RepairOrderSum = (SELECT [dbo].RoundUp(@RepairOrderSumWithoutDiscountAndTax - @AdditionalDiscount, 2));
	IF(@RepairOrderSum IS NULL) SET @RepairOrderSum = 0;

	DECLARE @EstimateDiscount Decimal(32,16);
	IF(@ClassCustomer = 'WholesaleCustomer') SET @EstimateDiscount = ((SELECT Discount FROM Customers WHERE Id = @currrentCustomerId) * 0.01)
	ELSE SET @EstimateDiscount = 0;
	IF(@EstimateDiscount IS NULL) SET @EstimateDiscount = 0;

	DECLARE @Discount Decimal(32,16);
	IF(@EstimateDiscount > 0) SET @Discount = @EstimateDiscount
	ELSE SET @Discount = ((SELECT RetailDiscount FROM RepairOrders WHERE Id = @RepairOrderId) * 0.01);
	IF(@Discount IS NULL) SET @Discount = 0;

	DECLARE @DiscountSum Decimal(32,16);
	SET @DiscountSum = (SELECT [dbo].RoundUp((@RepairOrderSum * @Discount), 2));
	IF(@DiscountSum IS NULL) SET @DiscountSum = 0; 

	DECLARE @EstimateCurrentLaborTax Decimal(32,16);
	IF(@ClassCustomer = 'WholesaleCustomer') SET @EstimateCurrentLaborTax = ((SELECT LaborRate FROM Customers WHERE Id = @currrentCustomerId) * 0.01)
	ELSE SET @EstimateCurrentLaborTax = 0;
	IF(@EstimateCurrentLaborTax IS NULL) SET @EstimateCurrentLaborTax = 0;

	DECLARE @TaxSum Decimal(32,16);
	SET @TaxSum = (SELECT [dbo].RoundUp(((@RepairOrderSum - @DiscountSum) * @EstimateCurrentLaborTax), 2));
	IF(@TaxSum IS NULL) SET @TaxSum = 0;

	DECLARE @Total Decimal(32,16);
	SET @Total = @RepairOrderSum - @DiscountSum + @TaxSum;
	IF(@Total IS NULL) SET @Total = 0;

	DECLARE @TotalAmountRepairOrder Decimal(32,2);
	SET @TotalAmountRepairOrder = (SELECT [dbo].RoundUp(@Total, 2));
	IF(@TotalAmountRepairOrder IS NULL) SET @TotalAmountRepairOrder = 0;

	RETURN @TotalAmountRepairOrder
END
GO

