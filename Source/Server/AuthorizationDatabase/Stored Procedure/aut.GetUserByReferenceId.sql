CREATE PROCEDURE [aut].[GetUserByReferenceId]
	@referenceId VARCHAR(1024)
AS
SELECT [UserId], [ReferenceId], [EmailAddressId], [Name], 
	[CreateTimestamp], [UpdateTimestamp]
FROM [aut].[User] 
WHERE [ReferenceId] = @referenceId
;