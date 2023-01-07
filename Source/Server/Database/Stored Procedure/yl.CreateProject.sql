CREATE PROCEDURE [yl].[CreateProject]
	@projectId UNIQUEIDENTIFIER OUT,
	@userId UNIQUEIDENTIFIER,
	@userEmailAddress VARCHAR(1024) = '',
	@title NVARCHAR(MAX),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = sysutcdatetime();
	SET @projectId = newid();
	INSERT INTO [yl].[Project] ([ProjectId], [Title], [CreateTimestamp], [UpdateTimestamp])
	VALUES (@projectId, @title, @timestamp, @timestamp)
	;
	DECLARE @projectUserTimestamp DATETIME2(4);
	EXEC [yl].[CreateProjectUser] @projectId=@projectId, @userId=@userId, @emailAddress=@userEmailAddress, @isActive=1, @timestamp=@projectUserTimestamp OUT
END