CREATE PROCEDURE [yl].[GetWorkItemType]
	@id UNIQUEIDENTIFIER
AS
SELECT [WorkItemTypeId],[ProjectId],[Title],[ColorCode],[IsActive],
	[CreateTimestamp],[UpdateTimestamp],[CreateUserId],[UpdateUserId]
FROM [yl].[WorkItemType]
WHERE [WorkItemTypeId] = @id
;