CREATE PROCEDURE [aut].[GetClientCredentialByClientId]
	@clientId UNIQUEIDENTIFIER
AS
SELECT [ClientCredentialId], [ClientId], [Secret], [IsActive], [CreateTimestamp], [UpdateTimestamp]
FROM [aut].[ClientCredential]
WHERE [ClientId] = @clientId
;