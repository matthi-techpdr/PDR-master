SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	процедура возвращает Estimates-кандидаты, отобранные для конкретного пользователя
-- основываясь на его роли и значении селектора IsOnlyOwn
-- =============================================
CREATE PROCEDURE GetEstimatesByRole
	@userId bigint,
	@companyId bigint,
	@Role nvarchar(50),
	@IsOnlyOwn bit = 0,
	@IsForReport bit

AS
BEGIN
	SET NOCOUNT ON;
	
	IF ((@Role = 'Technician') OR (@Role = 'Estimator') OR (@IsOnlyOwn = 1))
		BEGIN
			IF(@IsForReport = 1)
				BEGIN
					SELECT * --Id, CreationDate, EstimateStatus, Archived, CustomerFk, New, TotalAmount, CarImageFk, WorkByThemselve, AffiliateFk
					FROM Estimates
					WHERE (EmployeeFk = @userId
						  OR Id IN (SELECT EstimateFk FROM PreEmployees_PreEstimates WHERE EmployeeFk = @userId))
						  AND CompanyFk = @companyId
						  AND EstimateStatus != 4
				RETURN
				END
			ELSE
				BEGIN
					SELECT * --Id, CreationDate, EstimateStatus, Archived, CustomerFk, New, TotalAmount, CarImageFk, WorkByThemselve, AffiliateFk 
					FROM Estimates 
					WHERE EmployeeFk = @userId 	
						  AND CompanyFk = @companyId
						  AND EstimateStatus != 4
				RETURN
				END
		END
	ELSE IF (@Role = 'Manager' AND @IsOnlyOwn = 0 AND (SELECT IsShowAllTeams FROM Users WHERE Id = @userId) = 0)
			BEGIN
				WITH 
				empTeams (TeamFk)
				AS (SELECT TeamFk FROM TeamEmployees_Teams WHERE TeamEmployeeFk = @userId),

				employees (TeamEmployeeFk)
				AS (SELECT TeamEmployeeFk FROM TeamEmployees_Teams WHERE TeamFk IN (SELECT TeamFk FROM empTeams)),

				ests (Id)
				AS (SELECT Id FROM Estimates WHERE EmployeeFk IN (SELECT TeamEmployeeFk FROM employees))	
			
				SELECT * --Id, CreationDate, EstimateStatus, Archived, CustomerFk, New, TotalAmount, CarImageFk, WorkByThemselve, AffiliateFk 
				FROM Estimates estimates 
				WHERE CompanyFk = @companyId
					  AND EstimateStatus != 4
					  AND estimates.Id IN (SELECT Id FROM ests) 	  
					  AND (estimates.CustomerFk IN (SELECT Customers_Teams.CustomerFk 
											  FROM Customers_Teams 									 
											  INNER JOIN (SELECT TeamFk FROM empTeams) TableB 									 
											  ON Customers_Teams.CustomerFk = estimates.CustomerFk 
											  AND Customers_Teams.TeamFk = TableB.TeamFk) 							 
										  OR (SELECT COUNT (*) 
											  FROM Customers_Teams 
											  WHERE Customers_Teams.CustomerFk = estimates.CustomerFk) = 0 )
			RETURN
			END
		ELSE 
			BEGIN
				SELECT *--Id, CreationDate, EstimateStatus, Archived, CustomerFk, New, TotalAmount, CarImageFk, WorkByThemselve, AffiliateFk 
				FROM Estimates 
				WHERE CompanyFk = @companyId
					  AND EstimateStatus != 4
			RETURN
			END
END
