CREATE PROCEDURE [yl].[CreateProjectUser]
	@projectId UNIQUEIDENTIFIER,
	@userId UNIQUEIDENTIFIER,
	@emailAddress VARCHAR(1024) = '',
	@isActive BIT = 1,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = sysutcdatetime();
	INSERT INTO [yl].[ProjectUser] ([ProjectId], [UserId], [EmailAddress], [IsActive], [CreateTimestamp], [UpdateTimestamp])
	VALUES (@projectId, @userId, @emailAddress, @isActive, @timestamp, @timestamp)
	;
END
