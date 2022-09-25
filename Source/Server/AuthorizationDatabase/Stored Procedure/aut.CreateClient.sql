CREATE PROCEDURE [aut].[CreateClient]
	@id UNIQUEIDENTIFIER OUT,
	@name nvarchar(1024),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	SET @id = NEWID();
	INSERT INTO [aut].[Client] ([ClientId], [Name], [CreateTimestamp], [UpdateTimestamp])
	VALUES (@id, @name, @timestamp, @timestamp)
	;
END