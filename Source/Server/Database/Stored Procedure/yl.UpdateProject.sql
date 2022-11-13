CREATE PROCEDURE [yl].[UpdateProject]
	@projectId UNIQUEIDENTIFIER,
	@title NVARCHAR(MAX),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = sysutcdatetime();
	UPDATE [yl].[Project]
	SET 
		[Title] = @title,
		[UpdateTimestamp] = @timestamp
	WHERE [ProjectId] = @projectId
	;
END