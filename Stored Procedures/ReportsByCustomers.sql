USE [PDR_test]
GO
/****** Object:  StoredProcedure [dbo].[ReportsByCustomers]    Script Date: 5/14/2015 7:57:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	Weekly Operations - Customer Report
-- =============================================

ALTER PROCEDURE [dbo].[ReportsByCustomers]
	@companyId bigint,
	@DateFrom datetime = NULL,
	@DateTo datetime = NULL,
	@States AS VARCHAR(MAX) = NULL
 
AS
BEGIN
	SET NOCOUNT ON;

-----------------------------------------------------------------------------------------------------
    DECLARE @SqlQuery nvarchar(max);
    DECLARE @PartSqlQuery nvarchar(max);
-----------------------------------------------------------------------------------------------------

	CREATE TABLE #ReportsByCustomers(
		Id bigint,
		Class nvarchar(255),
		CustomerName nvarchar(255),
		[State] nvarchar(255),
		City nvarchar(255),
		TotalInvoices int,
		TotalAmount float	
	);

	CREATE TABLE #InvoicesFromCurrentWeek(
		Id bigint,
		CreationDate datetime,
		RepairOrderFk bigint,
		CustomerFk bigint,
		InvoiceSum float
	);

	CREATE TABLE #InvoicesFromCurrentWeekWithFilteredCustomers(
		Id bigint,
		CreationDate datetime,
		RepairOrderFk bigint,
		CustomerFk bigint,
		InvoiceSum float
	);

--------------------------------------------------------------------------

	SET @PartSqlQuery = 'SELECT inv.Id, inv.CreationDate, inv.RepairOrderFk, inv.CustomerFk, inv.InvoiceSum
	FROM Invoices inv
	WHERE inv.CreationDate BETWEEN ''' + CAST(@DateFrom AS varchar) + ''' AND ''' + CAST(@DateTo AS varchar) + '''
	AND inv.IsDiscard = 0
	AND inv.CustomerFk NOT IN (SELECT Customers_Teams.CustomerFk 
							   FROM Customers_Teams, Teams 
							   WHERE Customers_Teams.TeamFk = Teams.Id
							   AND (Teams.Title LIKE ''%test%''OR Teams.Title LIKE ''%training%'') 
							   )
	AND inv.CustomerFk NOT IN (SELECT Id 
							   FROM Customers 
							   WHERE Class = ''RetailCustomer''
							   AND (FirstName LIKE ''%test%'' OR LastName LIKE ''%test%'')
							  )

	AND inv.CustomerFk NOT IN (175505408)
	AND inv.CompanyFk = ' + CAST(@companyId AS varchar);

	SET @SqlQuery = 'INSERT INTO #InvoicesFromCurrentWeekWithFilteredCustomers (Id, CreationDate, RepairOrderFk, CustomerFk, InvoiceSum)'+@PartSqlQuery;

	IF(@States IS NOT NULL)
	BEGIN
		SET @SqlQuery+='AND inv.CustomerFk IN (SELECT Id 
							   FROM Customers 
							   WHERE ''' + CAST(@States AS varchar) + ''' like ''%;''+cast([State] as varchar(20))+'';%'')';
	END
	
	EXECUTE(@SqlQuery);

	SET @SqlQuery = 'INSERT INTO #InvoicesFromCurrentWeek (Id, CreationDate, RepairOrderFk, CustomerFk, InvoiceSum)'+@PartSqlQuery;

    EXECUTE(@SqlQuery);



-----------------------------------------------------------------------------------------------------

	INSERT INTO #ReportsByCustomers (Id, Class, CustomerName, [State], City)
	SELECT DISTINCT cust.Id, cust.Class, cust.Name, cust.[State], cust.City
	FROM Customers cust, #InvoicesFromCurrentWeekWithFilteredCustomers inv
	WHERE cust.Class IN ('WholesaleCustomer')
	AND inv.CustomerFk = cust.Id;

	SET @SqlQuery = 'INSERT INTO #ReportsByCustomers (Id, Class, CustomerName, [State], City)
	SELECT DISTINCT cust.Id, cust.Class, cust.Name, cust.[State], cust.City
	FROM Customers cust, #InvoicesFromCurrentWeek inv, RepairOrders ro, Estimates est
	WHERE cust.Class = ''Affiliate''
	AND inv.RepairOrderFk = ro.Id
	AND ro.EstimateFk = est.Id
	AND est.CustomerFk = inv.CustomerFk
	AND est.AffiliateFk = cust.Id';

	IF(@States IS NOT NULL)
	BEGIN
		SET @SqlQuery+=' AND ''' + CAST(@States AS varchar) + ''' like ''%;''+cast(cust.[State] as varchar(20))+'';%''';
	END
   EXECUTE(@SqlQuery);
						
-----------------------------------------------------------------------------------------------------
	--Total count documents in which participates current employee for WholesaleCustomers 
	UPDATE #ReportsByCustomers
	SET TotalInvoices = (SELECT COUNT(inv.Id) 
						 FROM #InvoicesFromCurrentWeekWithFilteredCustomers inv
						 WHERE inv.CustomerFk = #ReportsByCustomers.Id
						)
	WHERE Class = 'WholesaleCustomer';

	UPDATE #ReportsByCustomers
	SET TotalAmount = (SELECT SUM(inv.InvoiceSum) 
						 FROM #InvoicesFromCurrentWeekWithFilteredCustomers inv
						 WHERE inv.CustomerFk = #ReportsByCustomers.Id
					   )
	WHERE Class = 'WholesaleCustomer';

-----------------------------------------------------------------------------------------------------
	--Total count documents in which participates current employee for Affiliate 
	UPDATE #ReportsByCustomers
	SET TotalInvoices = (SELECT COUNT(inv.Id) 
						 FROM #InvoicesFromCurrentWeek inv, RepairOrders ro, Estimates est
						 WHERE inv.RepairOrderFk = ro.Id
						 AND ro.EstimateFk = est.Id
						 AND est.AffiliateFk = #ReportsByCustomers.Id
  						 AND ((SELECT Class FROM Customers WHERE Id = inv.CustomerFk) = 'RetailCustomer')
						)
	WHERE Class = 'Affiliate';

	UPDATE #ReportsByCustomers
	SET TotalAmount = (SELECT SUM(inv.InvoiceSum) 
						 FROM #InvoicesFromCurrentWeek inv, RepairOrders ro, Estimates est
						 WHERE inv.RepairOrderFk = ro.Id
						 AND ro.EstimateFk = est.Id
						 AND est.AffiliateFk = #ReportsByCustomers.Id
						 AND ((SELECT Class FROM Customers WHERE Id = inv.CustomerFk) = 'RetailCustomer')
						)
	WHERE Class = 'Affiliate';


-----------------------------------------------------------------------------------------------------
	DELETE FROM #ReportsByCustomers
	WHERE TotalInvoices IS NULL OR TotalInvoices = 0 OR TotalAmount IS NULL OR TotalAmount = 0;

	SELECT CustomerName, [State], City, TotalInvoices, TotalAmount 
	FROM #ReportsByCustomers
	ORDER BY TotalAmount DESC;
END
