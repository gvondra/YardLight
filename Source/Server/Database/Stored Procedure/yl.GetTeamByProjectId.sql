CREATE PROCEDURE [yl].[GetTeamByProjectId]
	@projectId UNIQUEIDENTIFIER
AS
SELECT [wi].[Team]
FROM [yl].[WorkItem] [wi]
WHERE [wi].[ProjectId] = @projectId
AND [wi].[Team] <> ''
GROUP BY [wi].[Team]
ORDER BY [wi].[Team]
;