CREATE PROCEDURE [aut].[CreateClientCredential]
	@id UNIQUEIDENTIFIER OUT,
	@clientId UNIQUEIDENTIFIER,
	@secret BINARY(64),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	SET @id = NEWID();
	UPDATE [aut].[ClientCredential]
	SET [IsActive] = 0,
	[UpdateTimestamp] = @timestamp
	WHERE [ClientId] = @clientId
	;
	INSERT INTO [aut].[ClientCredential] ([ClientCredentialId], [ClientId], [Secret], [CreateTimestamp], [UpdateTimestamp])
	VALUES (@id, @clientId, @secret, @timestamp, @timestamp)
	;
END