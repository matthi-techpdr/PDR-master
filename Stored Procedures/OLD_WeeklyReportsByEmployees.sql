/****** Object:  StoredProcedure [dbo].[WeeklyReportsByEmployees]    Script Date: 2014-10-01 16:52:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	Процедура возвращает еженедельный отчет по сотрудникам
-- =============================================
CREATE PROCEDURE [dbo].[WeeklyReportsByEmployees]
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

--------------------------------------------------------------------------

	INSERT INTO #ReportsByEmployees (Id, EmployeeName, EmployeeRole)
	SELECT Users.Id, Name, Class
	FROM Users 
	WHERE Class IN ('Technician', 'Manager', 'Admin', 'RITechnician') 
	AND Users.CompanyFk = @companyId  
	AND Users.Status != 2
	AND Users.Id NOT IN(SELECT TeamEmployees_Teams.TeamEmployeeFk 
						FROM TeamEmployees_Teams, Teams
						WHERE TeamEmployeeFk = Users.Id
						AND Teams.Id = TeamEmployees_Teams.TeamFk
						AND (Teams.Title LIKE '%test%'OR Teams.Title LIKE '%training%')
				       );

	--Total count documents in which participates current employee 
	UPDATE #ReportsByEmployees
	SET TotalInvoices = (SELECT COUNT(inv.Id) 
						 FROM Invoices inv, TeamEmployeePercents tep
						 WHERE inv.CreationDate BETWEEN @DateFrom AND @DateTo
						 AND inv.RepairOrderFk = tep.RepairOrderFk
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
						FROM Invoices inv, TeamEmployeePercents tep
						WHERE inv.CreationDate BETWEEN @DateFrom AND @DateTo
						AND inv.RepairOrderFk = tep.RepairOrderFk
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
