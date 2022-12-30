CREATE PROCEDURE [yl].[UpdateWorkItemStatus]
	@id UNIQUEIDENTIFIER,
	@title NVARCHAR(256),
	@colorCode NVARCHAR(128),
	@order SMALLINT,
	@isActive BIT,
	@isDefaultHidden BIT = 0,
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
		[IsDefaultHidden] = @isDefaultHidden,
		[UpdateUserId] = @userId,
		[UpdateTimestamp] = @timestamp
	WHERE [WorkItemStatusId] = @id
	;
END