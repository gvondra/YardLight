CREATE PROCEDURE [aut].[UpdateRole]
	@id INT,
	@name VARCHAR(1024),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [aut].[Role] 
	SET [Name] = @name, 
	[UpdateTimestamp] = @timestamp
	WHERE [RoleId] = @id
	;
END