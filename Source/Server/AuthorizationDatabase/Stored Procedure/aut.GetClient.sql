CREATE PROCEDURE [aut].[GetClient]
	@id UNIQUEIDENTIFIER
AS
SELECT [ClientId], [Name], [CreateTimestamp], [UpdateTimestamp]
FROM [aut].[Client]
WHERE [ClientId] = @id
;
