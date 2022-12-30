CREATE PROCEDURE [yl].[GetWorkItemStatus_by_ProjectId]
	@projectId UNIQUEIDENTIFIER,
	@isActive BIT = null
AS
SELECT [WorkItemStatusId],[WorkItemTypeId],[ProjectId],[Title],[ColorCode],[Order],[IsActive],[IsDefaultHidden],
	[CreateTimestamp],[UpdateTimestamp],[CreateUserId],[UpdateUserId]
FROM [yl].[WorkItemStatus]
WHERE [ProjectId] = @projectId
AND (@isActive IS NULL OR [IsActive] = @isActive)
ORDER BY [Order], [Title]
;