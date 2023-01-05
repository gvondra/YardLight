CREATE PROCEDURE [yl].[GetItterationByProjectId]
	@projectId UNIQUEIDENTIFIER
AS
BEGIN
SELECT [ItterationId], [ProjectId], [Name], [Start], [End], [Hidden],	
	[CreateTimestamp], [UpdateTimestamp], [CreateUserId], [UpdateUserId]
FROM [yl].[Itteration]
WHERE [ProjectId] = @projectId

UNION

SELECT NEWID() [ItterationId], @projectId [ProjectId], [wi].[Itteration] [Name],
	NULL [Start], NULL [End], 0 [Hidden],
	SYSUTCDATETIME() [CreateTimestamp], SYSUTCDATETIME() [UpdateTimestamp], 
	NULL [CreateUserId], NULL [UpdateUserId]
FROM [yl].[WorkItem] [wi]
WHERE [wi].[ProjectId] = @projectId
AND [wi].[Itteration] <> ''
AND NOT EXISTS (
	SELECT TOP 1 1 
	FROM [yl].[Itteration] [itt]
	WHERE [itt].[ProjectId] = @projectId
	AND [itt].[Name] = [wi].[Itteration]
)
GROUP BY [wi].[Itteration]
;
END