CREATE PROCEDURE [aut].[CreateEmailAddress]
	@id UNIQUEIDENTIFIER OUT,
	@address NVARCHAR(3072),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	SET @id = NEWID();
	INSERT INTO [aut].[EmailAddress] ([EmailAddressId], [Address], [CreateTimestamp])
	VALUES (@id, @address, @timestamp)
	;
END