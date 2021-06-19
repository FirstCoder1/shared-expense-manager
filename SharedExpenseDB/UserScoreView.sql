CREATE PROCEDURE [dbo].[UserScoreView]
	@UserId int = 0
AS

DECLARE @Cursor int = 1
DECLARE @Score decimal = 0.00
DECLARE @PayerId int
DECLARE @Expense decimal = 0.00
DECLARE @GroupCount int = 0

DECLARE @ScoreView TABLE(
	UserName nvarchar(256),
	Score decimal
)

DECLARE @PayerGroupMapping TABLE(
	ID INT IDENTITY(1,1) PRIMARY KEY,
	GroupId int,
	UserId int,
	ExpenseAmount decimal,
	IsPayer bit
)

DECLARE @GroupList TABLE(
	ID INT IDENTITY(1,1) PRIMARY KEY,
	GroupId int
)

DECLARE @SelfGroupExpenseAmount TABLE(
	ID INT IDENTITY(1,1) PRIMARY KEY,
	GroupId int,
	UserId int,
	PayerId int,
	ExpenseAmount decimal
)

BEGIN
	INSERT INTO @PayerGroupMapping
	SELECT GroupId, UserId, TransactionAmount, IsPayer FROM (SELECT * FROM [dbo].[UserGroup] WHERE GroupId in 
	(SELECT GroupId FROM [dbo].[UserGroup] WHERE UserId = @UserId)) A ORDER BY GroupId ASC

	--Pick groupId
	INSERT INTO @GroupList
	SELECT DISTINCT(GroupId) FROM @PayerGroupMapping
	SELECT @GroupCount = COUNT(*) FROM @GroupList

	WHILE(@GroupCount > 0)
		BEGIN
			--If Current user is a payer for the group
			IF @UserId = (SELECT UserId  FROM @PayerGroupMapping WHERE GroupId = (SELECT GroupId FROM @GroupList WHERE Id = @Cursor) AND IsPayer =1 )
				BEGIN
					INSERT INTO @SelfGroupExpenseAmount
					SELECT UG.GroupId, UG.UserId, PG.UserId, (PG.ExpenseAmount*-1) FROM [dbo].[UserGroup] UG
					INNER JOIN @PayerGroupMapping PG ON UG.GroupId = PG.GroupId
					WHERE UG.GroupId IN ((SELECT GroupId FROM @GroupList WHERE Id = @Cursor)) AND UG.UserId = @UserId AND PG.IsPayer = 0
				END
			ELSE
				BEGIN
					INSERT INTO @SelfGroupExpenseAmount
					SELECT UG.GroupId, UG.UserId, PG.UserId, UG.TransactionAmount FROM [dbo].[UserGroup] UG 
					INNER JOIN @PayerGroupMapping PG ON UG.GroupId = PG.GroupId
					WHERE UG.GroupId IN ((SELECT GroupId FROM @GroupList WHERE Id = @Cursor)) AND UG.UserId = @UserId AND PG.IsPayer = 1
				END
			SET @Cursor = @Cursor + 1
			SET @GroupCount = @GroupCount - 1
		END
	SET @Cursor = 1

	INSERT INTO @ScoreView
	SELECT UI.UserName, A.TotalExpense FROM (SELECT PayerId, SUM(ExpenseAmount) AS TotalExpense FROM @SelfGroupExpenseAmount SGE 
	GROUP BY SGE.PayerId) A 	
	INNER JOIN [dbo].[ApplicationUserInformation] AI ON A .PayerId = AI.UserId
	INNER JOIN [dbo].[UserLogIn] UI ON AI.LogInId = UI.LogInId

	SELECT * FROM @ScoreView
END
GO
