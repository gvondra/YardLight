CREATE PROCEDURE [aut].[UpdateClientCredentialDeactivate]
	@clientId UNIQUEIDENTIFIER	
AS
BEGIN
	DECLARE @timestamp DATETIME2(4) = SYSUTCDATETIME();
	UPDATE [aut].[ClientCredential]
	SET 
		[IsActive] = 0, 
		[UpdateTimestamp] = @timestamp
	WHERE [ClientId] = @clientId
	;
END