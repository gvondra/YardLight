CREATE PROCEDURE [aut].[GetClientAll]
AS
SELECT [ClientId], [Name], [CreateTimestamp], [UpdateTimestamp]
FROM [aut].[Client]
ORDER BY [Name]
;
