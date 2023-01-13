CREATE PROCEDURE [yl].[UpdateProjectUser]
	@projectId UNIQUEIDENTIFIER,
	@emailAddress VARCHAR(1024),
	@isActive BIT = 0,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = sysutcdatetime();
	UPDATE [yl].[ProjectUser]
	SET
		[IsActive] = @isActive,
		[UpdateTimestamp] = @timestamp
	WHERE [ProjectId] = @projectId
	AND [EmailAddress] = @emailAddress
	;
	if @@ROWCOUNT = 0 AND @isActive = 1
	BEGIN
	-- we want this to be an active user, and the update didn't change any rows
	-- so we'll insert this a new user
	EXEC [yl].[CreateProjectUser] @projectId, @emailAddress, @isActive, @timestamp OUT;
	END
END