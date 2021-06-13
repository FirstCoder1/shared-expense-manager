CREATE TABLE [dbo].[ExpenseType]
(
	[ExpenseTypeId] INT NOT NULL PRIMARY KEY, 
    [ExpenseTypeName] NCHAR(10) NOT NULL, 
    [ExpenseTypeStatus] INT NOT NULL,
    [CreateDate] DATETIME2 NOT NULL DEFAULT GETDATE(), 
    [LastUpdate] DATETIME2 NOT NULL DEFAULT GETDATE()
)
