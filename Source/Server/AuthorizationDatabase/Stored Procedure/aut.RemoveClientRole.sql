CREATE PROCEDURE [aut].[RemoveClientRole]
	@clientId UNIQUEIDENTIFIER,
	@roleId INT
AS
UPDATE [aut].[ClientRole]
SET
	[IsActive] = 0,
	[UpdateTimestamp] = SYSUTCDATETIME()
WHERE [ClientId] = @clientId
	AND [RoleId] = @roleId
;