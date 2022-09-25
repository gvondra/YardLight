CREATE PROCEDURE [aut].[CreateRole]
	@id INT OUT,
	@name VARCHAR(1024),
	@policyName VARCHAR(256),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO [aut].[Role] ([Name], [PolicyName], 
		[CreateTimestamp], [UpdateTimestamp])
	VALUES (@name, @policyName, 
		@timestamp, @timestamp)
	;
	SET @id = SCOPE_IDENTITY();
END