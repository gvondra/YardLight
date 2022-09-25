CREATE PROCEDURE [aut].[GetEmailAddressByAddress]
	@address NVARCHAR(MAX)
AS
SELECT [EmailAddressId], [Address], [CreateTimestamp]
FROM [aut].[EmailAddress]
WHERE [Address] = @address
;
