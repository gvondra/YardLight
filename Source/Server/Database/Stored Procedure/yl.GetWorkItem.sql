CREATE PROCEDURE [yl].[GetWorkItem]
	@id UNIQUEIDENTIFIER
AS
SELECT [WorkItemId],[ProjectId],[Title],[TypeId],[StatusId],[Team],[Itteration],[StartDate],[TargetDate],[CloseDate],[Priority],[Effort],[Value],
	[CreateTimestamp],[UpdateTimestamp],[CreateUserId],[UpdateUserId]
FROM [yl].[WorkItem]
WHERE [WorkItemId] = @id
;