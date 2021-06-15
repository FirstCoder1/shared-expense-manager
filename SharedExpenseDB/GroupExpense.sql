CREATE TABLE [dbo].[GroupExpense]
(
	[ExpenseId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [Title] NVARCHAR(256) NOT NULL, 
    [Description] NVARCHAR(MAX) NULL, 
    [ExpenseDate] DATETIME2 NOT NULL, 
    [ExpenseTypeId] INT NOT NULL, 
    [ExpenseAmount] MONEY NOT NULL, 
    [ExpenseStatusId] INT NOT NULL, 
    [GroupId] INT NOT NULL,
    [CreateDate] DATETIME2 NOT NULL DEFAULT GETDATE(), 
    [LastUpdate] DATETIME2 NOT NULL DEFAULT GETDATE()
    CONSTRAINT [FK_UserExpense_ExpenseType] FOREIGN KEY ([ExpenseTypeId]) REFERENCES [ExpenseType]([ExpenseTypeId]), 
    CONSTRAINT [FK_UserExpense_Status] FOREIGN KEY ([ExpenseStatusId]) REFERENCES [Status]([StatusId]), 
    CONSTRAINT [FK_UserExpense_Group] FOREIGN KEY ([GroupId]) REFERENCES [Groups]([GroupId])
)
