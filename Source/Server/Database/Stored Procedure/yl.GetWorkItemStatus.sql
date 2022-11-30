CREATE PROCEDURE [yl].[GetWorkItemStatus]
	@id UNIQUEIDENTIFIER
AS
SELECT [WorkItemStatusId],[WorkItemTypeId],[ProjectId],[Title],[ColorCode],[Order],[IsActive],
	[CreateTimestamp],[UpdateTimestamp],[CreateUserId],[UpdateUserId]
FROM [yl].[WorkItemStatus]
WHERE [WorkItemStatusId] = @id
;