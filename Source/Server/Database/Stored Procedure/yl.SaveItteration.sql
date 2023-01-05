CREATE PROCEDURE [yl].[SaveItteration]
	@id UNIQUEIDENTIFIER,
	@projectId UNIQUEIDENTIFIER,
	@name NVARCHAR(MAX),
	@start DATE,
	@end DATE,
	@hidden BIT,
	@userId UNIQUEIDENTIFIER,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();

	UPDATE [yl].[Itteration]
	SET 
		[Start] = @start,
		[End] = @end,
		[Hidden] = @hidden,
		[UpdateTimestamp] = @timestamp,
		[UpdateUserId] = @userId
	WHERE [ItterationId] = @id
		AND [ProjectId] = @projectId
		AND [Name] = @name
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