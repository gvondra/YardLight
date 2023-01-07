UPDATE [yl].[ProjectUser]
SET [EmailAddress] = (SELECT TOP 1 [EmailAddress]
	FROM [yl].[ProjectUser] [pu]
	WHERE [yl].[ProjectUser].[UserId] = [pu].[UserId]
	AND [pu].[EmailAddress] <> ''
	),
[UpdateTimestamp] = SYSUTCDATETIME()
WHERE [EmailAddress] = ''
AND EXISTS (SELECT TOP 1 1 
	FROM [yl].[ProjectUser] [pu]
	WHERE [yl].[ProjectUser].[UserId] = [pu].[UserId]
	AND [pu].[EmailAddress] <> ''
	)
;