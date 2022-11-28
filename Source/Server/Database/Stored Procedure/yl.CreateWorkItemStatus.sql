CREATE PROCEDURE [yl].[CreateWorkItemStatus]
	@id UNIQUEIDENTIFIER OUT,
	@projectId UNIQUEIDENTIFIER,
	@title NVARCHAR(256),
	@colorCode NVARCHAR(128),
	@order SMALLINT,
	@isActive BIT,
	@userId UNIQUEIDENTIFIER,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @id = NEWID();
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO  [yl].[WorkItemStatus] ([WorkItemStatusId],[ProjectId],[Title],[ColorCode],[Order],[IsActive],
		[CreateTimestamp],[UpdateTimestamp],[CreateUserId],[UpdateUserId]) 
	VALUES (@id, @projectId, @title, @colorCode, @order, @isActive,
		@timestamp, @timestamp, @userId, @userId) 
	;
END