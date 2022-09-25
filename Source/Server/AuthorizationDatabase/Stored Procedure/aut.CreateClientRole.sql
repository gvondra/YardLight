CREATE PROCEDURE [aut].[CreateClientRole]
	@clientId UNIQUEIDENTIFIER,
	@roleId INT
AS
BEGIN
	DECLARE @timestamp DATETIME2(4) = SYSUTCDATETIME();
	UPDATE [aut].[ClientRole]
	SET [IsActive] = 1,
		[UpdateTimestamp] = SYSUTCDATETIME()
	WHERE [ClientId] = @clientId
		AND [RoleId] = @roleId
	;
	IF @@ROWCOUNT = 0
	BEGIN
	INSERT INTO [aut].[ClientRole] ([ClientId], [RoleId], [IsActive], [CreateTimestamp], [UpdateTimestamp])
	VALUES (@clientId, @roleId, 1, @timestamp, @timestamp)
	;
	END
END