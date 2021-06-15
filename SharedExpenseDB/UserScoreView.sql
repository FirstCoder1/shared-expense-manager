CREATE PROCEDURE [dbo].[UserScoreView]
	@UserId int = 0
AS

DECLARE @UserCount int = 0
DECLARE @Parser int = 1
DECLARE @GroupCount int = 1
DECLARE @Score decimal = 0.00
DECLARE @GroupId int = 0
DECLARE @GroupParser int = 1
declare @ScoreUser nvarchar(256)
DECLARE @RowCount int = 0

DECLARE @ScoreView TABLE(
	UserName nvarchar(256),
	Score decimal
)
DECLARE @UserList TABLE(
	ID INT IDENTITY(1,1) PRIMARY KEY,
	UserId int
)
DECLARE @GroupList TABLE(
	ID INT IDENTITY(1,1) PRIMARY KEY,
	GroupId int
)

DECLARE @UserGroupMapping TABLE(
	ID INT IDENTITY(1,1) PRIMARY KEY,
	GroupId int,
	UserId int,
	ExpenseAmount decimal,
	IsPayer bit
)

DECLARE @UserGroupList TABLE(
	ID INT IDENTITY(1,1) PRIMARY KEY,
	GroupId int
)

--Create table with unique userIds

INSERT INTO @UserGroupMapping
SELECT GroupId, UserId, TransactionAmount, IsPayer FROM (SELECT * FROM [dbo].[UserGroup] WHERE GroupId in 
(SELECT GroupId FROM [dbo].[UserGroup] WHERE UserId = @UserId) ) A WHERE A.UserId <> 10 ORDER BY GroupId ASC

INSERT INTO @UserList
SELECT DISTINCT(UserId) FROM (SELECT * FROM [dbo].[UserGroup] WHERE GroupId in 
(SELECT GroupId FROM [dbo].[UserGroup] WHERE UserId = @UserId) ) A WHERE A.UserId <> 10 ORDER BY UserId ASC

INSERT INTO @GroupList
SELECT DISTINCT(GroupId) FROM (SELECT * FROM [dbo].[UserGroup] WHERE GroupId in 
(SELECT GroupId FROM [dbo].[UserGroup] WHERE UserId = @UserId) ) A WHERE A.UserId <> 10 ORDER BY GroupId ASC

SELECT @UserCount = COUNT(*) FROM @UserList

--Loop through all the users and I will owe money to whoever paid
WHILE(@UserCount > 0)
BEGIN
	SELECT @RowCount = COUNT(*) FROM @UserGroupMapping WHERE UserId = (SELECT UserId FROM @UserList WHERE Id = @Parser) AND IsPayer = 1
	--Capture GROUP information for each group user is part of and someone else paid
	IF @RowCount > 0 
		--For account we need to capture score for, we will capture all the groups where user was present and someone else was payer
		INSERT INTO @UserGroupList
		SELECT GroupId FROM @UserGroupMapping WHERE UserId = (SELECT UserId FROM @UserList WHERE Id = @Parser) AND IsPayer = 1
		SELECT @GroupCount = COUNT(*) FROM @UserGroupList

		SELECT @ScoreUser = UI.UserName FROM [dbo].[ApplicationUserInformation] AI INNER JOIN
		[dbo].[UserLogIn] UI ON AI.LogInId = UI.LogInId
		WHERE AI.UserId in (SELECT DISTINCT UserId FROM @UserList WHERE Id = @Parser)

		WHILE(@GroupCount > 0)
			BEGIN
				SELECT @GroupId = GroupId FROM @UserGroupList WHERE Id = @GroupParser
				SELECT  @Score = @Score + TransactionAmount FROM [dbo].[UserGroup] WHERE GroupId = @GroupId AND UserId = @UserId
				SET @GroupCount = @GroupCount - 1
				SET @GroupParser = @GroupParser + 1
			END
			INSERT @ScoreView (UserName, Score) VALUES (@ScoreUser, @Score)

	SET @UserCount = @UserCount - 1
	SET @Parser = @Parser + 1
	SET @Score = 0
END
SELECT * FROM @ScoreView

GO
