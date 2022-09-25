CREATE PROCEDURE [aut].[GetRoleAll]
AS
SELECT [RoleId], [Name], [PolicyName], 
		[CreateTimestamp], [UpdateTimestamp]
FROM [aut].[Role]
ORDER BY [PolicyName]
;
