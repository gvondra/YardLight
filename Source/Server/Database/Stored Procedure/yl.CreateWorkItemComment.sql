CREATE PROCEDURE [yl].[CreateWorkItemComment]
	@workItemId UNIQUEIDENTIFIER,
	@commentId UNIQUEIDENTIFIER OUT,
	@text NVARCHAR(MAX),
	@type SMALLINT,
	@userId UNIQUEIDENTIFIER,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
EXEC [yl].[CreateComment] @commentId OUT, @text, @userId, @timestamp OUT;
INSERT INTO [yl].[WorkItem_Comment] ([WorkItemId], [CommentId], [Type], [CreateTimestamp], [CreateUserId])
VALUES (@workItemId, @commentId, @type, @timestamp, @userId) 
;
END