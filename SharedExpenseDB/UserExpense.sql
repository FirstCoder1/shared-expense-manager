CREATE TABLE [dbo].[UserExpense]
(
	[ExpenseId] INT NOT NULL PRIMARY KEY, 
    [Title] NVARCHAR(256) NOT NULL, 
    [Description] NVARCHAR(MAX) NULL, 
    [ExpenseDate] DATETIME2 NOT NULL, 
    [ExpenseTypeId] INT NOT NULL, 
    [ExpenseAmount] MONEY NOT NULL, 
    [ExpenseStatusId] INT NOT NULL, 
    [UserId] INT NOT NULL DEFAULT 0.00,
    [CreateDate] DATETIME2 NOT NULL DEFAULT GETDATE(), 
    [LastUpdate] DATETIME2 NOT NULL DEFAULT GETDATE()
    CONSTRAINT [FK_UserExpense_ExpenseType] FOREIGN KEY ([ExpenseTypeId]) REFERENCES [ExpenseType]([ExpenseTypeId]), 
    CONSTRAINT [FK_UserExpense_ApplicationUserInformation] FOREIGN KEY ([UserId]) REFERENCES [ApplicationUserInformation]([UserId]), 
    CONSTRAINT [FK_UserExpense_Status] FOREIGN KEY ([ExpenseStatusId]) REFERENCES [Status]([StatusId])
)
