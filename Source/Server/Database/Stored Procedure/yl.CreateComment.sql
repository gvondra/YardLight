CREATE PROCEDURE [yl].[CreateComment]
	@id UNIQUEIDENTIFIER OUT,
	@text NVARCHAR(MAX),
	@userId UNIQUEIDENTIFIER,
	@timestamp DATETIME2(4) OUT
AS
BEGIN 
SET @id = NEWID();
SET @timestamp = SYSUTCDATETIME();
INSERT INTO [yl].[Comment] ([CommentId], [Text], [CreateTimestamp], [CreateUserId])
VALUES (@id, @text, @timestamp, @userId)
;
END