/****** Object:  StoredProcedure [dbo].[WeeklyReportsByEmployees]    Script Date: 2014-10-01 16:52:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	Weekly Operations - Customer Report
-- =============================================
CREATE PROCEDURE [dbo].[WeeklyReportsByCustomers]
	@companyId bigint,
	@DateFrom datetime = NULL,
	@DateTo datetime = NULL
 
AS
BEGIN
	SET NOCOUNT ON;

-----------------------------------------------------------------------------------------------------
    DECLARE @SqlQuery nvarchar(max);
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

-----------------------------------------------------------------------------------------------------

	INSERT INTO #ReportsByCustomers (Id, Class, CustomerName, [State], City)
	SELECT Id, Class, Name, [State], City
	FROM Customers 
	WHERE Customers.CompanyFk = @companyId
	AND Class IN ('WholesaleCustomer', 'Affiliate')  
	AND Customers.Id NOT IN(SELECT Id FROM Customers WHERE Name LIKE '%test%'OR Name LIKE '%training%' 
														OR FirstName LIKE '%test%'OR FirstName LIKE '%training%'
														OR LastName LIKE '%test%'OR LastName LIKE '%training%');

-----------------------------------------------------------------------------------------------------
	--Total count documents in which participates current employee for WholesaleCustomers 
	UPDATE #ReportsByCustomers
	SET TotalInvoices = (SELECT COUNT(inv.Id) 
						 FROM Invoices inv
						 WHERE inv.CreationDate BETWEEN @DateFrom AND @DateTo
						 AND inv.CustomerFk = #ReportsByCustomers.Id
						 AND inv.TeamEmployeeFk NOT IN ((SELECT TeamEmployees_Teams.TeamEmployeeFk 
															FROM TeamEmployees_Teams, Teams
															WHERE Teams.Id = TeamEmployees_Teams.TeamFk
															AND (Teams.Title LIKE '%test%'OR Teams.Title LIKE '%training%'))))
	WHERE Class = 'WholesaleCustomer';

	UPDATE #ReportsByCustomers
	SET TotalAmount = (SELECT SUM(inv.InvoiceSum) 
						 FROM Invoices inv
						 WHERE inv.CreationDate BETWEEN @DateFrom AND @DateTo
						 AND inv.CustomerFk = #ReportsByCustomers.Id
						 AND inv.TeamEmployeeFk NOT IN ((SELECT TeamEmployees_Teams.TeamEmployeeFk 
															FROM TeamEmployees_Teams, Teams
															WHERE Teams.Id = TeamEmployees_Teams.TeamFk
															AND (Teams.Title LIKE '%test%'OR Teams.Title LIKE '%training%'))))
	WHERE Class = 'WholesaleCustomer';

-----------------------------------------------------------------------------------------------------
	--Total count documents in which participates current employee for Affiliate 
	UPDATE #ReportsByCustomers
	SET TotalInvoices = (SELECT COUNT(inv.Id) 
						 FROM Invoices inv, RepairOrders ro, Estimates est
						 WHERE inv.CreationDate BETWEEN @DateFrom AND @DateTo
						 AND inv.RepairOrderFk = ro.Id
						 AND ro.EstimateFk = est.Id
						 AND est.AffiliateFk = #ReportsByCustomers.Id
						 AND inv.TeamEmployeeFk NOT IN ((SELECT TeamEmployees_Teams.TeamEmployeeFk 
															FROM TeamEmployees_Teams, Teams
															WHERE Teams.Id = TeamEmployees_Teams.TeamFk
															AND (Teams.Title LIKE '%test%'OR Teams.Title LIKE '%training%')))
						AND ((SELECT Class FROM Customers WHERE Id = inv.CustomerFk) = 'RetailCustomer')
						)
	WHERE Class = 'Affiliate';

	UPDATE #ReportsByCustomers
	SET TotalAmount = (SELECT SUM(inv.InvoiceSum) 
						 FROM Invoices inv, RepairOrders ro, Estimates est
						 WHERE inv.CreationDate BETWEEN @DateFrom AND @DateTo
						 AND inv.RepairOrderFk = ro.Id
						 AND ro.EstimateFk = est.Id
						 AND est.AffiliateFk = #ReportsByCustomers.Id
						 AND inv.TeamEmployeeFk NOT IN ((SELECT TeamEmployees_Teams.TeamEmployeeFk 
															FROM TeamEmployees_Teams, Teams
															WHERE Teams.Id = TeamEmployees_Teams.TeamFk
															AND (Teams.Title LIKE '%test%'OR Teams.Title LIKE '%training%')))
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
