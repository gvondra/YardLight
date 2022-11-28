CREATE PROCEDURE [yl].[GetWorkItemType_by_ProjectId]
	@projectId UNIQUEIDENTIFIER,
	@isActive BIT = null
AS
SELECT [WorkItemTypeId],[ProjectId],[Title],[ColorCode],[IsActive],
	[CreateTimestamp],[UpdateTimestamp],[CreateUserId],[UpdateUserId]
FROM [yl].[WorkItemType]
WHERE [ProjectId] = @projectId
AND (@isActive IS NULL OR [IsActive] = @isActive)
ORDER BY [Title]
;