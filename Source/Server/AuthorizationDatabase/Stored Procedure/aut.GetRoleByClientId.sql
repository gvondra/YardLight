CREATE PROCEDURE [aut].[GetRoleByClientId]
	@clientId UNIQUEIDENTIFIER
AS
SELECT [rl].[RoleId], [rl].[Name], [rl].[PolicyName], 
		[rl].[CreateTimestamp], [rl].[UpdateTimestamp]
FROM [aut].[Role] [rl]
INNER JOIN [aut].[ClientRole] [cr] on [rl].[RoleId] = [cr].[RoleId]
WHERE [cr].[ClientId] = @clientId
AND [cr].[IsActive] = 1
;
