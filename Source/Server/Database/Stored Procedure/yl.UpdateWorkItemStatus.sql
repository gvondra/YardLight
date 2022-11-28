CREATE PROCEDURE [yl].[UpdateWorkItemStatus]
	@id UNIQUEIDENTIFIER,
	@title NVARCHAR(256),
	@colorCode NVARCHAR(128),
	@order SMALLINT,
	@isActive BIT,
	@userId UNIQUEIDENTIFIER,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [yl].[WorkItemStatus]
	SET 
		[Title] = @title,
		[ColorCode] = @colorCode,
		[Order] = @order,
		[IsActive] = @isActive,
		[UpdateUserId] = @userId,
		[UpdateTimestamp] = @timestamp
	WHERE [WorkItemStatusId] = @id
	;
END