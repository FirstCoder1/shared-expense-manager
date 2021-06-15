CREATE TABLE [dbo].[ExpenseType]
(
	[ExpenseTypeId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [ExpenseTypeName] NCHAR(50) NOT NULL, 
    [StatusId] INT NOT NULL,
    [CreateDate] DATETIME2 NOT NULL DEFAULT GETDATE(), 
    [LastUpdate] DATETIME2 NOT NULL DEFAULT GETDATE(), 
    CONSTRAINT [FK_ExpenseType_Status] FOREIGN KEY ([StatusId]) REFERENCES [Status]([StatusId])
)
