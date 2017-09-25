SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	процедура возвращает RepairOrder-кандидаты, отобранные для конкретного пользователя
--основываясь на его роли и значении селектора IsOnlyOwn
-- =============================================
CREATE PROCEDURE GetRepairOrdersByRole
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
			FROM RepairOrders 
			WHERE Id IN (SELECT  RepairOrderFk 
						 FROM TeamEmployeePercents 
						 WHERE TeamEmployeeFk = @userId) 
					 AND CompanyFk = @companyId
					 AND RepairOrderStatus != 4
		RETURN
		END

	ELSE IF (@Role = 'Manager' AND @IsOnlyOwn = 0 AND (SELECT IsShowAllTeams FROM Users WHERE Id = @userId) = 0)
		BEGIN
			WITH 
			empTeams (TeamFk)
			AS (SELECT TeamFk FROM TeamEmployees_Teams WHERE TeamEmployeeFk = @userId),

			employees (TeamEmployeeFk)
			AS (SELECT TeamEmployeeFk FROM TeamEmployees_Teams WHERE TeamFk IN (SELECT TeamFk FROM empTeams)),

			ro (RepairOrderFk)
			AS (SELECT RepairOrderFk FROM TeamEmployeePercents WHERE TeamEmployeeFk IN (SELECT TeamEmployeeFk FROM employees))	
	
		
			SELECT * 
			FROM RepairOrders orders 
			WHERE CompanyFk = @companyId
				  AND orders.Id IN (SELECT RepairOrderFk FROM ro) 	  
				  AND (orders.CustomerFk IN (SELECT Customers_Teams.CustomerFk 
										  FROM Customers_Teams 									 
										  INNER JOIN (SELECT TeamFk FROM empTeams) TableB 									 
										  ON Customers_Teams.CustomerFk = orders.CustomerFk 
										  AND Customers_Teams.TeamFk = TableB.TeamFk) 							 
									  OR (SELECT COUNT (*) 
										  FROM Customers_Teams 
										  WHERE Customers_Teams.CustomerFk = orders.CustomerFk) = 0 )
				  AND RepairOrderStatus != 4
		RETURN
		END
	ELSE 
		BEGIN
			SELECT * 
			FROM RepairOrders 
			WHERE CompanyFk = @companyId
			AND RepairOrderStatus != 4
		RETURN
		END
END
