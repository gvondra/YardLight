CREATE PROCEDURE [yl].[GetWorkItemStatus_by_WorkItemTypeId]
	@workItemTypeId UNIQUEIDENTIFIER,
	@isActive BIT = null
AS
SELECT [WorkItemStatusId],[WorkItemTypeId],[ProjectId],[Title],[ColorCode],[Order],[IsActive],
	[CreateTimestamp],[UpdateTimestamp],[CreateUserId],[UpdateUserId]
FROM [yl].[WorkItemStatus]
WHERE [WorkItemTypeId] = @workItemTypeId
AND (@isActive IS NULL OR [IsActive] = @isActive)
ORDER BY [Order], [Title]
;