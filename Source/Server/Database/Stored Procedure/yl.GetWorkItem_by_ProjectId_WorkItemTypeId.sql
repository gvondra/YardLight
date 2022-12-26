CREATE PROCEDURE [yl].[GetWorkItem_by_ProjectId_WorkItemTypeId]
	@projectId UNIQUEIDENTIFIER,
	@workItemTypeId UNIQUEIDENTIFIER
AS
SELECT [WorkItemId],[ParentWorkItemId],[ProjectId],[Title],[TypeId],[StatusId],[Team],[Itteration],[StartDate],[TargetDate],[CloseDate],[Priority],[Effort],[Value],
	[CreateTimestamp],[UpdateTimestamp],[CreateUserId],[UpdateUserId]
FROM [yl].[WorkItem]
WHERE [ProjectId] = @projectId
AND [TypeId] = @workItemTypeId
ORDER BY [StartDate], [Title], [UpdateTimestamp] DESC
;