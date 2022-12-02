CREATE PROCEDURE [yl].[GetItterationByProjectId]
	@projectId UNIQUEIDENTIFIER
AS
SELECT [wi].[Itteration]
FROM [yl].[WorkItem] [wi]
WHERE [wi].[ProjectId] = @projectId
AND [wi].[Itteration] <> ''
GROUP BY [wi].[Itteration]
ORDER BY [wi].[Itteration]
;