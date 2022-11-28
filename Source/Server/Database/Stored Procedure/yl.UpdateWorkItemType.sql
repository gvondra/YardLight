CREATE PROCEDURE [yl].[UpdateWorkItemType]
	@id UNIQUEIDENTIFIER,
	@title NVARCHAR(256),
	@colorCode NVARCHAR(128),
	@isActive BIT,
	@userId UNIQUEIDENTIFIER,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [yl].[WorkItemType]
	SET 
		[Title] = @title,
		[ColorCode] = @colorCode,
		[IsActive] = @isActive,
		[UpdateUserId] = @userId,
		[UpdateTimestamp] = @timestamp
	WHERE [WorkItemTypeId] = @id
	;
END