CREATE PROCEDURE [yl].[GetProject_by_UserId]
	@userId UNIQUEIDENTIFIER
AS
SELECT [p].[ProjectId], [p].[Title], [p].[CreateTimestamp], [p].[UpdateTimestamp]
FROM [yl].[Project] [p]
INNER JOIN [yl].[ProjectUser] [pu] ON [pu].[ProjectId] = [p].[ProjectId] AND [pu].[UserId] = @userId
WHERE [pu].[IsActive] = 1
ORDER BY [p].[Title]
;