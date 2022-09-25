CREATE PROCEDURE [aut].[GetRoleByUserId]
	@userId UNIQUEIDENTIFIER
AS
SELECT [rl].[RoleId], [rl].[Name], [rl].[PolicyName], 
		[rl].[CreateTimestamp], [rl].[UpdateTimestamp]
FROM [aut].[Role] [rl]
INNER JOIN [aut].[UserRole] [ur] on [rl].[RoleId] = [ur].[RoleId]
WHERE [ur].[UserId] = @userId
AND [ur].[IsActive] = 1
;
