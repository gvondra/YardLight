CREATE PROCEDURE [yl].[CreateWorkItemType]
	@id UNIQUEIDENTIFIER OUT,
	@projectId UNIQUEIDENTIFIER,
	@title NVARCHAR(256),
	@colorCode NVARCHAR(128),
	@isActive BIT,
	@userId UNIQUEIDENTIFIER,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @id = NEWID();
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO [yl].[WorkItemType] ([WorkItemTypeId],[ProjectId],[Title],[ColorCode],[IsActive],
		[CreateTimestamp],[UpdateTimestamp],[CreateUserId],[UpdateUserId]) 
	VALUES (@id, @projectId, @title, @colorCode, @isActive,
		@timestamp, @timestamp, @userId, @userId)
	;
END