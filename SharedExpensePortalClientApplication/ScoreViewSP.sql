DECLARE @UserCount int = 0
DECLARE @Cursor int = 1
DECLARE @InnerUserCount int = 0
DECLARE @InnerCursor int = 1
DECLARE @UserName nvarchar(256)
DECLARE @Score decimal = 0.00
DECLARE @PayerId int
DECLARE @Expense decimal = 0.00
SET @PayerId = 10

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

DECLARE @UsersToDisplay TABLE(
	ID INT IDENTITY(1,1) PRIMARY KEY,
	PayerId int,
	UserCount int
)

DECLARE @UserScore TABLE(
	ID INT IDENTITY(1,1) PRIMARY KEY,
	PayerId int,
	Score decimal
)

--
INSERT INTO @PayerGroupMapping
SELECT GroupId, UserId, TransactionAmount, IsPayer FROM (SELECT * FROM [dbo].[UserGroup] WHERE GroupId in 
(SELECT GroupId FROM [dbo].[UserGroup] WHERE UserId = @PayerId)) A WHERE A.IsPayer = 1 ORDER BY GroupId ASC
SELECT * FROM @PayerGroupMapping

--Pick groupId
INSERT INTO @GroupList
SELECT GroupId FROM @PayerGroupMapping
SELECT * FROM @GroupList

INSERT INTO @SelfGroupExpenseAmount
SELECT UG.GroupId, UG.UserId, PG.UserId, UG.TransactionAmount FROM [dbo].[UserGroup] UG 
INNER JOIN @PayerGroupMapping PG ON UG.GroupId = PG.GroupId
WHERE UG.GroupId IN (SELECT GroupId FROM @GroupList) AND UG.UserId = @PayerId
SELECT * FROM @SelfGroupExpenseAmount

INSERT INTO @UsersToDisplay
SELECT PayerId, COUNT(*) Count FROM @SelfGroupExpenseAmount GROUP BY PayerId
SELECT * FROM @UsersToDisplay

SELECT @UserCount = COUNT(*) FROM @UsersToDisplay
WHILE(@UserCount > 0)
BEGIN
	INSERT INTO @UserScore
	SELECT PayerId, ExpenseAmount FROM @SelfGroupExpenseAmount WHERE PayerId = (SELECT PayerId FROM @UsersToDisplay WHERE ID = @Cursor)
	SELECT @InnerUserCount = COUNT(*) FROM @UserScore
	SELECT * FROM @UserScore

	SELECT @UserName = A.UserName FROM(SELECT DISTINCT(UI.UserName) AS UserName FROM @UserScore GA
	INNER JOIN [dbo].[ApplicationUserInformation] AI ON GA.PayerId = AI.UserId
	INNER JOIN [dbo].[UserLogIn] UI ON AI.LogInId = UI.LogInId) A

	WHILE (@InnerUserCount > 0)
		BEGIN
			SELECT @Score = @Score + Score FROM @UserScore WHERE ID = @InnerCursor
			SET @InnerCursor = @InnerCursor + 1
			SET @InnerUserCount = @InnerUserCount - 1
		END
	INSERT INTO @ScoreView (UserName, Score) VALUES (@UserName, @Score)
	SET @Cursor = @Cursor + 1
	SET @UserCount = @UserCount - 1
	DELETE FROM @UserScore
END
SELECT * FROM @ScoreView
GO
