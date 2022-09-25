CREATE PROCEDURE [aut].[GetUserByEmailAddress]
	@address NVARCHAR(3072)
AS
SELECT [usr].[UserId], [usr].[ReferenceId], [usr].[EmailAddressId], [usr].[Name],
	[usr].[CreateTimestamp], [usr].[UpdateTimestamp]
FROM [aut].[User] [usr]
INNER JOIN [aut].[EmailAddress] [ea] on [usr].[EmailAddressId] = [ea].[EmailAddressId]
WHERE [ea].[Address] = @address
;