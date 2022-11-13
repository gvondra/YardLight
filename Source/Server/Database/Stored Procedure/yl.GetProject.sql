CREATE PROCEDURE [yl].[GetProject]
	@projectId UNIQUEIDENTIFIER
AS
SELECT [p].[ProjectId], [p].[Title], [p].[CreateTimestamp], [p].[UpdateTimestamp]
FROM [yl].[Project] [p]
WHERE [p].[ProjectId] = @projectId
;