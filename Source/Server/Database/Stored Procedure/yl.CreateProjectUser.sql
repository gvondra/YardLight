CREATE PROCEDURE [yl].[CreateProjectUser]
	@projectId UNIQUEIDENTIFIER,
	@userId UNIQUEIDENTIFIER,
	@isActive BIT = 1,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = sysutcdatetime();
	INSERT INTO [yl].[ProjectUser] ([ProjectId], [UserId], [IsActive], [CreateTimestamp], [UpdateTimestamp])
	VALUES (@projectId, @userId, @isActive, @timestamp, @timestamp)
	;
END
