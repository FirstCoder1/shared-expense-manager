--SELECT UserId, GroupId, COUNT(*) AS uTotal FROM (SELECT * FROM (SELECT * FROM [dbo].[UserGroup] WHERE GroupId in 
--(SELECT GroupId FROM [dbo].[UserGroup] WHERE UserId = 10) ) A WHERE A.UserId <> 10) B GROUP BY UserId, GroupId

--Load all the distinct userID to dispaly on the page
--SELECT DISTINCT(UserId) FROM (SELECT * FROM [dbo].[UserGroup] WHERE GroupId in 
--(SELECT GroupId FROM [dbo].[UserGroup] WHERE UserId = 10) ) A WHERE A.UserId <> 10 ORDER BY UserId ASC

DECLARE @UserCount int = 0
DECLARE @UserId int = 10
DECLARE @Parser int = 1;
DECLARE @ScoreView TABLE(
	UserName varchar(max),
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

--Create table with unique userIds

INSERT INTO @UserGroupMapping
SELECT GroupId, UserId, TransactionAmount, IsPayer FROM (SELECT * FROM [dbo].[UserGroup] WHERE GroupId in 
(SELECT GroupId FROM [dbo].[UserGroup] WHERE UserId = @UserId) ) A WHERE A.UserId <> 10 ORDER BY GroupId ASC
SELECT * FROM @UserGroupMapping

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
	SELECT * FROM @UserGroupMapping WHERE UserId = (SELECT UserId FROM @UserList WHERE Id = @Parser) AND IsPayer = 1
END