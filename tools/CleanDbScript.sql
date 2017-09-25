DELETE FROM Invoices
DELETE FROM Supplements
DELETE FROM TeamEmployeePercents
DELETE FROM RepairOrders
DELETE FROM ChosenEffortItems
DELETE FROM CustomLines
UPDATE Insurances SET EstimateFk=NULL
UPDATE Estimates SET InsuranceFk=NULL
DELETE FROM Insurances
DELETE FROM Photos WHERE Class <> 'CompanyLogo'
DELETE FROM CarInspections
DELETE FROM Estimates
DELETE FROM Locations
DELETE FROM Licenses
DELETE FROM Prices
DELETE FROM Matrices
DELETE FROM PriceMatrices_WholesaleCustomers
DELETE FROM PreEmployees_PreEstimates