CREATE PROCEDURE [aut].[GetRole]
	@id INT
AS
SELECT [RoleId], [Name], [PolicyName], 
		[CreateTimestamp], [UpdateTimestamp]
FROM [aut].[Role]
WHERE [RoleId] = @id
;
