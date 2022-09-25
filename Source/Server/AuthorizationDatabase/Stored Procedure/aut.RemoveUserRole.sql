CREATE PROCEDURE [aut].[RemoveUserRole]
	@userId UNIQUEIDENTIFIER,
	@roleId INT
AS
UPDATE [aut].[UserRole]
SET
	[IsActive] = 0,
	[UpdateTimestamp] = SYSUTCDATETIME()
WHERE [UserId] = @userId
	AND [RoleId] = @roleId
;