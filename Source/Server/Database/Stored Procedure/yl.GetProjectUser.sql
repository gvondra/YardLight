CREATE PROCEDURE [yl].[GetProjectUser]
	@projectId UNIQUEIDENTIFIER,
	@emailAddress VARCHAR(1024)
AS
SELECT [pu].[ProjectId], [pu].[EmailAddress], [pu].[IsActive], [pu].[CreateTimestamp], [pu].[UpdateTimestamp]
FROM [yl].[ProjectUser] [pu]
WHERE [pu].[ProjectId] = @projectId
AND [pu].[EmailAddress] = @emailAddress
;