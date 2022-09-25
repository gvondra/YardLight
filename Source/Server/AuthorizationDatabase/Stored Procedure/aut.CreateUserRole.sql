CREATE PROCEDURE [aut].[CreateUserRole]
	@userId UNIQUEIDENTIFIER,
	@roleId INT
AS
BEGIN
	DECLARE @timestamp DATETIME2(4) = SYSUTCDATETIME();
	UPDATE [aut].[UserRole]
	SET [IsActive] = 1,
		[UpdateTimestamp] = SYSUTCDATETIME()
	WHERE [UserId] = @userId
		AND [RoleId] = @roleId
	;
	IF @@ROWCOUNT = 0
	BEGIN
	INSERT INTO [aut].[UserRole] ([UserId], [RoleId], [IsActive], [CreateTimestamp], [UpdateTimestamp])
	VALUES (@userId, @roleId, 1, @timestamp, @timestamp)
	;
	END
END