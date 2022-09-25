CREATE PROCEDURE [aut].[GetUser]
	@id UNIQUEIDENTIFIER
AS
SELECT [UserId], [ReferenceId], [EmailAddressId], [Name], 
	[CreateTimestamp], [UpdateTimestamp]
FROM [aut].[User] 
WHERE [UserId] = @id
;