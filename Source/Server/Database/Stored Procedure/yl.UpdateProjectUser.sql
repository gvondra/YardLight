CREATE PROCEDURE [yl].[UpdateProjectUser]
	@projectId UNIQUEIDENTIFIER,
	@userId UNIQUEIDENTIFIER,
	@emailAddress VARCHAR(1024) = '',
	@isActive BIT = 0,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = sysutcdatetime();
	UPDATE [yl].[ProjectUser]
	SET
		[EmailAddress] = @emailAddress,
		[IsActive] = @isActive,
		[UpdateTimestamp] = @timestamp
	WHERE [ProjectId] = @projectId
	AND [UserId] = @userId
	;
END