CREATE PROCEDURE [yl].[GetWorkItem_by_ParentIds]
	@parentIds VARCHAR(MAX)
AS
SELECT [wi].[WorkItemId],[wi].[ParentWorkItemId],[wi].[ProjectId],[wi].[Title],[wi].[TypeId],[wi].[StatusId],[wi].[Team],[wi].[Itteration],
	[wi].[StartDate],[wi].[TargetDate],[wi].[CloseDate],[wi].[Priority],[wi].[Effort],[wi].[Value],
	[wi].[CreateTimestamp],[wi].[UpdateTimestamp],[wi].[CreateUserId],[wi].[UpdateUserId]
FROM [yl].[WorkItem] [wi]
INNER JOIN (SELECT [value] idValue FROM string_split(@parentIds, ',')) [ids] ON [ids].[idValue] = [wi].[ParentWorkItemId]
;
