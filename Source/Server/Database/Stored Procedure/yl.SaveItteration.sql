CREATE PROCEDURE [yl].[SaveItteration]
	@id UNIQUEIDENTIFIER,
	@projectId UNIQUEIDENTIFIER,
	@name NVARCHAR(512),
	@start DATE,
	@end DATE,
	@hidden BIT,
	@userId UNIQUEIDENTIFIER,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	DECLARE @currentName NVARCHAR(512);

	SELECT TOP 1 @currentName = [Name] FROM [yl].[Itteration] WHERE [ItterationId] = @id AND [ProjectId] = @projectId;
	IF @currentName IS NOT NULL AND @currentName <> @name
	BEGIN
		UPDATE [yl].[WorkItem]
		SET [Itteration] = @name
		WHERE [ProjectId] = @projectId
		AND [Itteration] = @currentName
		;
	END

	UPDATE [yl].[Itteration]
	SET 
		[Name] = @name,
		[Start] = @start,
		[End] = @end,
		[Hidden] = @hidden,
		[UpdateTimestamp] = @timestamp,
		[UpdateUserId] = @userId
	WHERE [ItterationId] = @id
		AND [ProjectId] = @projectId
	;
	IF @@ROWCOUNT = 0
	BEGIN
		INSERT INTO [yl].[Itteration] ([ItterationId], [ProjectId], [Name], [Start], [End], [Hidden],	
			[CreateTimestamp], [UpdateTimestamp], [CreateUserId], [UpdateUserId])
		VALUES (@id, @projectId, @name, @start, @end, @hidden,
			@timestamp, @timestamp, @userId, @userId)
		;
	END
END