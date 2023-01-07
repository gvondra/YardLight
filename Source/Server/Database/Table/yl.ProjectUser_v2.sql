CREATE TABLE [yl].[ProjectUser_v2]
(
	[ProjectId] UNIQUEIDENTIFIER NOT NULL,
	[EmailAddress] VARCHAR(1024) NOT NULL,
	[IsActive] BIT CONSTRAINT [DF_ProjectUser_v2_IsActive] DEFAULT (1) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_ProjectUser_v2_CreateTimestamp] DEFAULT (SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_ProjectUser_v2_UpdateTimestamp] DEFAULT (SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_ProjectUser_v2] PRIMARY KEY CLUSTERED ([ProjectId], [EmailAddress]), 
    CONSTRAINT [FK_ProjectUser_v2_To_Project] FOREIGN KEY ([ProjectId]) REFERENCES [yl].[Project]([ProjectId])
)

GO

CREATE INDEX [IX_ProjectUser_v2_ProjectId] ON [yl].[ProjectUser_v2] ([ProjectId])

GO

CREATE INDEX [IX_ProjectUser_v2_UserId] ON [yl].[ProjectUser_v2] ([EmailAddress])
