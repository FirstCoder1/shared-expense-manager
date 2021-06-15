/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [UserGroupId]
      ,[UserId]
      ,[GroupId]
      ,[CreateDate]
      ,[LastUpdate]
      ,[TransactionAmount]
  FROM [SharedExpenseDB].[dbo].[UserGroup] 


SELECT UserId, GroupId, COUNT(*) AS uTotal FROM (SELECT * FROM (SELECT * FROM [dbo].[UserGroup] WHERE GroupId in 
(SELECT GroupId FROM [dbo].[UserGroup] WHERE UserId = 10) ) A WHERE A.UserId <> 10) B GROUP BY UserId, GroupId

--Load all the distinct userID to dispaly on the page
SELECT DISTINCT(UserId) FROM (SELECT * FROM [dbo].[UserGroup] WHERE GroupId in 
(SELECT GroupId FROM [dbo].[UserGroup] WHERE UserId = 10) ) A WHERE A.UserId <> 10 ORDER BY UserId ASC




SELECT * FROM dbo.ApplicationUserInformation WHERE UserId in (11,12,13,14)
