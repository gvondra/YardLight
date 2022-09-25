CREATE PROCEDURE [aut].[UpdateUser]
	@id UNIQUEIDENTIFIER,
	@name VARCHAR(1024),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [aut].[User] 
	SET 
		[Name] = @name, 
		[UpdateTimestamp] = @timestamp
	WHERE [UserId] = @id
	;
END