USE [PDR_test]
GO
/****** Object:  StoredProcedure [dbo].[ReportsByEmployees]    Script Date: 2015-01-19 10:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	Процедура возвращает еженедельный отчет по сотрудникам
-- =============================================
ALTER PROCEDURE [dbo].[ReportsByEmployees]
	@companyId bigint,
	@DateFrom datetime = NULL,
	@DateTo datetime = NULL
 
AS
BEGIN
	SET NOCOUNT ON;

--------------------------------------------------------------------------
    DECLARE @SqlQuery nvarchar(max);
--------------------------------------------------------------------------

	CREATE TABLE #ReportsByEmployees(
		Id bigint,
		EmployeeName nvarchar(255),
		EmployeeRole nvarchar(255),
		TotalInvoices int,
		TotalAmount float	
	);

		CREATE TABLE #InvoicesFromCurrentWeek(
		Id bigint,
		CreationDate datetime,
		RepairOrderFk bigint
	);

--------------------------------------------------------------------------
	
	INSERT INTO #InvoicesFromCurrentWeek (Id, CreationDate, RepairOrderFk)
	SELECT inv.Id, inv.CreationDate, inv.RepairOrderFk
	FROM Invoices inv
	WHERE inv.CreationDate BETWEEN @DateFrom AND @DateTo
	AND inv.IsDiscard = 0
	AND inv.CustomerFk NOT IN (SELECT Customers_Teams.CustomerFk 
							   FROM Customers_Teams, Teams 
							   WHERE Customers_Teams.TeamFk = Teams.Id
							   AND (Teams.Title LIKE '%test%'OR Teams.Title LIKE '%training%') 
							   )
	AND inv.CustomerFk NOT IN (SELECT Id 
							   FROM Customers 
							   WHERE Class = 'RetailCustomer'
							   AND (FirstName LIKE '%test%' OR LastName LIKE '%test%')
							  )
	--It is hard code for find	and exclude employees evgenia  and antonina  
	AND inv.TeamEmployeeFk NOT IN (229081088, 498958337)
	AND inv.CompanyFk = @companyId;

--------------------------------------------------------------------------
	INSERT INTO #ReportsByEmployees (Id, EmployeeName, EmployeeRole)
	SELECT DISTINCT Users.Id, Name, Class
	FROM Users, #InvoicesFromCurrentWeek inv, TeamEmployeePercents perc
	WHERE Class IN ('Technician', 'Manager', 'Admin', 'RITechnician') 
	AND Users.CompanyFk = @companyId  
	--AND Users.Status != 2
	AND inv.RepairOrderFk = perc.RepairOrderFk
	AND perc.TeamEmployeeFk = Users.Id;

	--Total count documents in which participates current employee 
	UPDATE #ReportsByEmployees
	SET TotalInvoices = (SELECT COUNT(inv.Id) 
						 FROM #InvoicesFromCurrentWeek inv, TeamEmployeePercents tep
						 WHERE inv.RepairOrderFk = tep.RepairOrderFk
						 AND tep.TeamEmployeeFk = #ReportsByEmployees.Id);

	DELETE FROM #ReportsByEmployees
	WHERE TotalInvoices IS NULL OR TotalInvoices = 0;

	DECLARE @EmployeeId bigint;
	SET @EmployeeId = (SELECT MIN(Id) FROM #ReportsByEmployees);
	WHILE @EmployeeId IS NOT NULL
	BEGIN

		DECLARE @total float;
		SET @total = 0;


		SET @total = (  SELECT SUM([dbo].GetTotalAmountForEmployeeInInvoice(inv.Id, @EmployeeId)) 
						FROM #InvoicesFromCurrentWeek inv, TeamEmployeePercents tep
						WHERE inv.RepairOrderFk = tep.RepairOrderFk
						AND tep.TeamEmployeeFk = @EmployeeId);

		UPDATE #ReportsByEmployees
		SET TotalAmount = @total
		WHERE Id = @EmployeeId;

		SET @EmployeeId = (SELECT MIN(Id) FROM #ReportsByEmployees WHERE Id > @EmployeeId);
	END

	DELETE FROM #ReportsByEmployees
	WHERE TotalAmount IS NULL OR TotalAmount = 0;

	SELECT EmployeeName, EmployeeRole, TotalInvoices, TotalAmount 
	FROM #ReportsByEmployees
	ORDER BY TotalAmount DESC;

END
