MERGE INTO [yl].[ProjectUser_v2] [target]
USING (
	SELECT DISTINCT [ProjectId], [EmailAddress], [IsActive], [CreateTimestamp], [UpdateTimestamp]
	FROM [yl].[ProjectUser]
	WHERE [EmailAddress] <> ''
) [source]
ON [target].[ProjectId] = [source].[ProjectId] AND [target].[EmailAddress] = [source].[EmailAddress]
WHEN MATCHED THEN
	UPDATE SET 
	[IsActive] = [source].[IsActive],
	[CreateTimestamp] = [source].[CreateTimestamp],
	[UpdateTimestamp] = [source].[UpdateTimestamp]
WHEN NOT MATCHED THEN
	INSERT ([ProjectId], [EmailAddress], [IsActive], [CreateTimestamp], [UpdateTimestamp])
	VALUES ([source].[ProjectId], [source].[EmailAddress], [source].[IsActive], [source].[CreateTimestamp], [source].[UpdateTimestamp])
WHEN NOT MATCHED BY SOURCE THEN
	DELETE
;