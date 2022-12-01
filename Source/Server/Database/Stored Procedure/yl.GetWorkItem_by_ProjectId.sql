CREATE PROCEDURE [yl].[GetWorkItem_by_ProjectId]
	@projectId UNIQUEIDENTIFIER
AS
SELECT [WorkItemId],[ParentWorkItemId],[ProjectId],[Title],[TypeId],[StatusId],[Team],[Itteration],[StartDate],[TargetDate],[CloseDate],[Priority],[Effort],[Value],
	[CreateTimestamp],[UpdateTimestamp],[CreateUserId],[UpdateUserId]
FROM [yl].[WorkItem]
WHERE [ProjectId] = @projectId
ORDER BY [StartDate], [Title], [UpdateTimestamp] DESC
;