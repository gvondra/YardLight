CREATE PROCEDURE [aut].[GetEmailAddress]
	@id UNIQUEIDENTIFIER
AS
SELECT [EmailAddressId], [Address], [CreateTimestamp]
FROM [aut].[EmailAddress]
WHERE [EmailAddressId] = @id
;
