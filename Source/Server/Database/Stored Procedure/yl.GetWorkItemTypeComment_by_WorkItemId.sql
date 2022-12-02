CREATE PROCEDURE [yl].[GetWorkItemTypeComment_by_WorkItemId]
	@workItemId UNIQUEIDENTIFIER
AS
SELECT [wic].[WorkItemId], [wic].[Type],
[cmt].[CommentId], [cmt].[Text], [cmt].[CreateTimestamp], [cmt].[CreateUserId]
FROM [yl].[WorkItem_Comment] [wic]
INNER JOIN [yl].[Comment] [cmt] on [wic].[CommentId] = [cmt].[CommentId]
WHERE [wic].[WorkItemId] = @workItemId
ORDER BY [cmt].[CreateTimestamp] DESC
;