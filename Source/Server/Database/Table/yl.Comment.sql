﻿CREATE TABLE [yl].[Comment]
(
	[CommentId] UNIQUEIDENTIFIER NOT NULL,
	[Text] NVARCHAR(MAX) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_Comment_CreateTimestamp] DEFAULT (SYSUTCDATETIME()) NOT NULL,
	[CreateUserId] UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT [PK_Comment] PRIMARY KEY NONCLUSTERED ([CommentId])
)
WITH (DATA_COMPRESSION = PAGE)
