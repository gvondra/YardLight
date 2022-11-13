CREATE PROCEDURE [yl].[GetProjectUser]
	@projectId UNIQUEIDENTIFIER,
	@userId UNIQUEIDENTIFIER
AS
SELECT [pu].[ProjectId], [pu].[UserId], [pu].[IsActive], [pu].[CreateTimestamp], [pu].[UpdateTimestamp]
FROM [yl].[ProjectUser] [pu]
WHERE [pu].[ProjectId] = @projectId
AND [pu].[UserId] = @userId
;