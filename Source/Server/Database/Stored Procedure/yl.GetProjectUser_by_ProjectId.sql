CREATE PROCEDURE [yl].[GetProjectUser_by_ProjectId]
	@projectId UNIQUEIDENTIFIER
AS
SELECT [pu].[ProjectId], [pu].[EmailAddress], [pu].[IsActive], [pu].[CreateTimestamp], [pu].[UpdateTimestamp]
FROM [yl].[ProjectUser] [pu]
WHERE [pu].[ProjectId] = @projectId
AND [pu].[IsActive] = 1
;