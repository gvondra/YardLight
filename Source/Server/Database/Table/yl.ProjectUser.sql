﻿CREATE TABLE [yl].[ProjectUser]
(
	[ProjectId] UNIQUEIDENTIFIER NOT NULL,
	[EmailAddress] VARCHAR(1024) NOT NULL,
	[IsActive] BIT CONSTRAINT [DF_ProjectUser_IsActive] DEFAULT (1) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_ProjectUser_CreateTimestamp] DEFAULT (SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_ProjectUser_UpdateTimestamp] DEFAULT (SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_ProjectUser] PRIMARY KEY CLUSTERED ([ProjectId], [EmailAddress]), 
    CONSTRAINT [FK_ProjectUser_To_Project] FOREIGN KEY ([ProjectId]) REFERENCES [yl].[Project]([ProjectId])
)

GO

CREATE INDEX [IX_ProjectUser_ProjectId] ON [yl].[ProjectUser] ([ProjectId])

GO

CREATE INDEX [IX_ProjectUser_EmailAddress] ON [yl].[ProjectUser] ([EmailAddress])
