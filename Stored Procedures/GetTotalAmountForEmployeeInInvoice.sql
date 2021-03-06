USE [PDR_test]
GO
/****** Object:  UserDefinedFunction [dbo].[GetTotalAmountForEmployeeInInvoice]    Script Date: 6/4/2015 4:13:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER Function [dbo].[GetTotalAmountForEmployeeInInvoice](@InvoiceId bigint, @EmployeeId bigint)
Returns Float
AS
Begin
--------------------------------------------------------------------------
	DECLARE @Result Float;
	SET @Result = 0;

	DECLARE @Class nvarchar(255);
	SET @Class = (SELECT Class FROM Users WHERE Id = @EmployeeId);

	DECLARE @InvoiceSum Float;
	SET @InvoiceSum = (SELECT InvoiceSum FROM Invoices WHERE Id = @InvoiceId);

	DECLARE @RepairOrderId bigint;
	SET @RepairOrderId = (SELECT RepairOrderFk FROM Invoices WHERE Id = @InvoiceId);
	
	DECLARE @EstimateId bigint;
	SET @EstimateId = (SELECT EstimateFk FROM RepairOrders WHERE Id = @RepairOrderId);

	DECLARE @IsFlatFee bit;
	SET @IsFlatFee = (  SELECT ro.IsFlatFee 
						FROM RepairOrders ro, Invoices inv 
						WHERE inv.Id = @InvoiceId
						AND inv.RepairOrderFk = ro.Id);

	DECLARE @totalHours Float;
	SET @totalHours = (SELECT SUM(eff.Hours)
					 FROM RepairOrders ro, Estimates est, CarInspections insp, ChosenEffortItems eff
					 WHERE ro.Id = @RepairOrderId
					 AND ro.EstimateFk = est.Id
					 AND insp.EstimateFk = est.Id
					 AND eff.CarInspectionFK = insp.Id);
	IF(@TotalHours IS NULL) SET @TotalHours = 0;

	DECLARE @ClassCustomer nvarchar(255);
	SET @ClassCustomer =  ( SELECT Class 
							FROM Customers 
							WHERE Id = (SELECT CustomerFk
										FROM Estimates
										WHERE Id = @EstimateId));
	DECLARE @currrentCustomerId bigint;
	SET @currrentCustomerId = (SELECT CustomerFk FROM Estimates WHERE Id = @EstimateId);

	DECLARE @AffiliateId bigint;
	SET @AffiliateId = (SELECT AffiliateFk FROM Estimates WHERE Id = @EstimateId);

	DECLARE @CurrentHourlyRate float;
	IF(@ClassCustomer = 'WholesaleCustomer') SET @CurrentHourlyRate = (SELECT EstHourlyRate FROM Estimates WHERE Id = @EstimateId)
	--IF(@ClassCustomer = 'WholesaleCustomer') SET @CurrentHourlyRate = (SELECT HourlyRate FROM Customers WHERE Id = @currrentCustomerId)
	ELSE IF(@AffiliateId IS NOT NULL) SET @CurrentHourlyRate = (SELECT HourlyRate FROM Customers WHERE Id = @AffiliateId)
	ELSE SET @CurrentHourlyRate = 0;
	IF(@CurrentHourlyRate IS NULL) SET @CurrentHourlyRate = 0;

	DECLARE @LaborSum Float;
	SET @LaborSum = ROUND((@totalHours * @CurrentHourlyRate), 2)

	DECLARE @EstimateDiscount Decimal(32,16);
	IF(@ClassCustomer = 'WholesaleCustomer') SET @EstimateDiscount = @LaborSum * ((SELECT Discount FROM Customers WHERE Id = @currrentCustomerId) * 0.01)
	ELSE SET @EstimateDiscount = 0;
	IF(@EstimateDiscount IS NULL) SET @EstimateDiscount = 0;

	/*logic for calculate part former RI tecticians*/
	DECLARE @RepairOrderCreationDate datetime;
	SET @RepairOrderCreationDate = (SELECT CreationDate FROM RepairOrders WHERE Id = @RepairOrderId);

	DECLARE @WasHeRITechnician bit;
	DECLARE @RoleChangeDate datetime;

	SET @RoleChangeDate = (SELECT RoleChangeDate FROM FormerRIs WHERE EmployeeFk = @EmployeeId);
	IF (@RoleChangeDate IS NOT NULL AND @RepairOrderCreationDate IS NOT NULL  AND @RepairOrderCreationDate < @RoleChangeDate) 
		SET  @WasHeRITechnician = 1
	ELSE SET  @WasHeRITechnician = 0;

--------------------------------------------------------------------------
	
	IF (@IsFlatFee IS NULL) 
	BEGIN
		DECLARE @EmployeePart Decimal(32,16);
		SET @EmployeePart = (SELECT EmployeePart 
							 FROM TeamEmployeePercents 
							 WHERE TeamEmployeeFk = @EmployeeId 
							 AND RepairOrderFk = @RepairOrderId);

		SET @Result = ROUND (@InvoiceSum / 100 * @EmployeePart, 2);
	END

	IF (@IsFlatFee = 1)
	BEGIN 
		IF(@Class = 'RITechnician' OR @WasHeRITechnician = 1) 
			SET @Result = (ROUND((SELECT Payment FROM RepairOrders WHERE Id = @RepairOrderId) / 
			(SELECT COUNT(Id)
				FROM Users
				WHERE Id IN (SELECT TeamEmployeeFk 
							FROM TeamEmployeePercents 
							WHERE RepairOrderFk = @RepairOrderId)
				AND (Class = 'RITechnician' OR Id IN(SELECT EmployeeFk FROM FormerRIs WHERE RoleChangeDate > @RepairOrderCreationDate))), 2))
		ELSE
		BEGIN
			SET @EmployeePart = (SELECT EmployeePart 
								 FROM TeamEmployeePercents 
								 WHERE TeamEmployeeFk = @EmployeeId 
								 AND RepairOrderFk = @RepairOrderId);

			SET @Result = (ROUND((@InvoiceSum - ( SELECT Payment 
												FROM RepairOrders 
												WHERE Id = @RepairOrderId)) / 100 * @EmployeePart, 2));
		END
	END

	IF (@IsFlatFee = 0)
	BEGIN 
		DECLARE @CommissionRi Float;
		SET @CommissionRi = @LaborSum - @EstimateDiscount;

		IF(@Class = 'RITechnician' OR @WasHeRITechnician = 1) 
			SET @Result = (ROUND (@CommissionRi / (SELECT COUNT(Id)
			FROM Users
			WHERE Id IN (SELECT TeamEmployeeFk 
						FROM TeamEmployeePercents 
						WHERE RepairOrderFk = @RepairOrderId)
			AND (Class = 'RITechnician' OR Id IN(SELECT EmployeeFk FROM FormerRIs WHERE RoleChangeDate > @RepairOrderCreationDate))), 2));
		ELSE 
		BEGIN
			SET @EmployeePart = (SELECT EmployeePart 
								 FROM TeamEmployeePercents 
								 WHERE TeamEmployeeFk = @EmployeeId 
								 AND RepairOrderFk = @RepairOrderId);
			SET @Result = (ROUND(((@InvoiceSum - @CommissionRi) / 100 * @EmployeePart) ,2));
		END
	END
	RETURN @Result
End
