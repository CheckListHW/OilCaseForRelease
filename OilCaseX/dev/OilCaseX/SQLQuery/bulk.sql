USE [OILCASEDB]
GO

BULK INSERT [dbo].WellLogs FROM 'C:\Users\KosachevIV\Desktop\exportLite.csv'
WITH (FIRSTROW = 2,FIELDTERMINATOR = ';' , ROWTERMINATOR = '\n');

GO

