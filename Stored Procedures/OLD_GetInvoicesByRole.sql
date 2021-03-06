SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	процедура возвращает invoices-кандидаты, отобранные для конкретного пользователя
--основываясь на его роли и значении селектора IsOnlyOwn
-- =============================================
CREATE PROCEDURE GetInvoicesByRole
	@userId bigint,
	@companyId bigint,
	@Role nvarchar(50),
	@IsOnlyOwn bit = 0
AS
BEGIN
	SET NOCOUNT ON;
	
	IF (@Role = 'Technician' OR @Role = 'RITechnician' OR @IsOnlyOwn = 1)
		BEGIN
			SELECT  * 
			FROM Invoices 
			WHERE RepairOrderFk IN (SELECT RepairOrderFk 
									FROM TeamEmployeePercents 
									WHERE TeamEmployeeFk = @userId) 
								AND Invoices.IsDiscard = 0
								AND CompanyFk = @companyId
		RETURN
		END
	
	ELSE IF (@Role = 'Admin' AND @IsOnlyOwn = 0)
		BEGIN
			SELECT * 
			FROM Invoices 
			WHERE IsDiscard = 0
				  AND CompanyFk = @companyId
		RETURN
		END
	
	ELSE IF (@Role = 'Manager' AND @IsOnlyOwn = 0  AND (SELECT IsShowAllTeams FROM Users WHERE Id = @userId) = 0)
		BEGIN
			WITH 
			empTeams (TeamFk)
			AS (SELECT TeamFk FROM TeamEmployees_Teams WHERE TeamEmployeeFk = @userId),

			employees (TeamEmployeeFk)
			AS (SELECT TeamEmployeeFk FROM TeamEmployees_Teams WHERE TeamFk IN (SELECT TeamFk FROM empTeams)),

			ro (RepairOrderFk)
			AS (SELECT RepairOrderFk FROM TeamEmployeePercents WHERE TeamEmployeeFk IN (SELECT TeamEmployeeFk FROM employees))	
	
		
			SELECT * 
			FROM Invoices inv 
			WHERE inv.IsDiscard = 0  
				  AND CompanyFk = @companyId
				  AND inv.RepairOrderFk IN (SELECT RepairOrderFk FROM ro) 	  
				  AND (inv.CustomerFk IN (SELECT Customers_Teams.CustomerFk 
										  FROM Customers_Teams 									 
										  INNER JOIN (SELECT TeamFk FROM empTeams) TableB 									 
										  ON Customers_Teams.CustomerFk = inv.CustomerFk 
										  AND Customers_Teams.TeamFk = TableB.TeamFk) 							 
									  OR (SELECT COUNT (*) 
										  FROM Customers_Teams 
										  WHERE Customers_Teams.CustomerFk = inv.CustomerFk) = 0 )
		RETURN
		END
	ELSE 
		BEGIN
			SELECT * 
			FROM Invoices 
			WHERE IsDiscard = 0
				  AND CompanyFk = @companyId
		RETURN
		END
END
