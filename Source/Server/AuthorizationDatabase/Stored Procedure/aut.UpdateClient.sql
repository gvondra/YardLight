CREATE PROCEDURE [aut].[UpdateClient]
	@id UNIQUEIDENTIFIER,
	@name nvarchar(1024),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();	
	UPDATE [aut].[Client]
	SET 
		[Name] = @name, 
		[UpdateTimestamp] = @timestamp
	WHERE [ClientId] = @id
	;
END