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
CREATE PROCEDURE [dbo].[GetCustomerModels] 
	@userId bigint,
	@FilterByTeam bigint = NULL,
	@FilterByStates bigint = NULL,
	@IsOnlyOwn bit = 0,
	@RowsPerPage int = 10,
	@PageNumber int = 1,
	@SortByColumn nvarchar(50) = 'Name',
	@SortType  nvarchar(10) = 'ASC',
	@CustomerType nvarchar(50),
	@IsAllRow bit = 0

AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @companyId bigint;
	SET @companyId = (SELECT CompanyFk FROM Users WHERE Id = @userId);
	DECLARE @Role nvarchar(255);
	SET @Role = (SELECT Class FROM Users WHERE Id = @userId);

	CREATE TABLE #CustomerTable(
		Id bigint NOT NULL,
		Name nvarchar(255) NULL,
		State smallint NULL,
		Phone nvarchar(255) NULL,
		City nvarchar(255) NULL,
		Email nvarchar(255) NULL,
		Email2 nvarchar(255) NULL,
		Email3 nvarchar(255) NULL,
		Email4 nvarchar(255) NULL,
		TotalCountRows int NULL
		);

	INSERT #CustomerTable (Id, Name, [State], Phone)
	SELECT Id, Name, [State] , Phone
	FROM Customers WHERE [Class] = @CustomerType AND CompanyFk = @companyId;
	
	IF(@FilterByTeam IS NOT NULL) 
	DELETE FROM #CustomerTable 
	WHERE @FilterByTeam NOT IN (SELECT TeamFk FROM Customers_Teams WHERE CustomerFk = #CustomerTable.Id); 

	IF(@FilterByStates IS NOT NULL) 
	DELETE FROM #CustomerTable 
	WHERE #CustomerTable.State != @FilterByStates;
	
	IF((@Role = 'Technician' OR (@Role = 'Manager' AND (SELECT IsShowAllTeams FROM Users WHERE Id = @userId) = 0) 
			OR (@Role = 'Manager' AND (SELECT IsShowAllTeams FROM Users WHERE Id = @userId) = 1) AND @IsOnlyOwn = 0) 
				AND @CustomerType = 'wholesalecustomer')
	BEGIN
		DELETE FROM #CustomerTable 
		WHERE #CustomerTable.Id NOT IN (SELECT CustomerFk FROM Customers_Teams WHERE TeamFk IN
													 (SELECT TeamFk FROM TeamEmployees_Teams WHERE TeamEmployeeFk = @userId)); 
			
	END

	DECLARE @sqlQuery nvarchar(max);
	IF((@SortByColumn IS NOT NULL) AND (@SortByColumn = 'Name') AND (@IsAllRow = 0))
			BEGIN
				SET @sqlQuery = 'SELECT Id
								 FROM (
											SELECT Id,
											ROW_NUMBER() OVER (ORDER BY ' + @SortByColumn + ' ' + @SortType + ') AS RowNum
											FROM #CustomerTable ) AS SOD
								WHERE SOD.RowNum BETWEEN ((' + CAST(@PageNumber AS varchar) + '-1)*' + CAST(@RowsPerPage AS varchar) + ')+1
								AND ' + CAST(@RowsPerPage AS varchar) + '*(' + CAST(@PageNumber AS varchar) + ')';

			END
	IF(@IsAllRow = 1) SET @sqlQuery = 'SELECT Id FROM #CustomerTable' 


	DECLARE @sortedList AS TABLE (Id bigint NOT NULL);
	INSERT INTO @sortedList EXECUTE(@sqlQuery);
	DECLARE @Total int;
	SET @Total = (SELECT COUNT(*) FROM #CustomerTable);
	DELETE FROM #CustomerTable;
	
	IF (@SortType = 'DESC')
		INSERT #CustomerTable (Id, Name, State, Phone, City, Email, Email2, Email3, Email4)
		SELECT Id, Name, State, Phone, City, Email, Email2, Email3, Email4
		FROM Customers WHERE Id IN (SELECT Id FROM @sortedList)
		ORDER BY Name DESC;
	ELSE 
		INSERT #CustomerTable (Id, Name, State, Phone, City, Email, Email2, Email3, Email4)
		SELECT Id, Name, State, Phone, City, Email, Email2, Email3, Email4 
		FROM Customers WHERE Id IN (SELECT Id FROM @sortedList)
		ORDER BY Name ASC;

	--to do check logic..

		UPDATE #CustomerTable
		SET TotalCountRows = @Total

	SELECT * FROM #CustomerTable;
END