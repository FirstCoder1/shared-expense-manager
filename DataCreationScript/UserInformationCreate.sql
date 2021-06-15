DECLARE @UserCount int = 0, @LogInId int = 1
DECLARE @UserName varchar(256)
SELECT @UserCount = COUNT(LogInId) FROM [dbo].UserLogIn

WHILE(@UserCount > 0)
BEGIN
	SELECT @UserName = UserName FROM [dbo].UserLogIn WHERE LogInId = @LogInId
	INSERT INTO [dbo].[ApplicationUserInformation]
			   ([UserFirstName]
			   ,[UserLastName]
			   ,[UserEmailId]
			   ,[UserPhoneNumber]
			   ,[CreateDate]
			   ,[LastUpdate]
			   ,[LogInId])
		 VALUES
			   (CONCAT(@UserName, 'firstName'),
			    CONCAT(@UserName,'lastName')
			   ,CONCAT(@UserName,'@abc.com')
			   ,8798885871
			   ,GETDATE()
			   ,GETDATE()
			   ,@LogInId)
	SET @LogInId = @LogInId + 1
	SET @UserCount = @UserCount - 1
END