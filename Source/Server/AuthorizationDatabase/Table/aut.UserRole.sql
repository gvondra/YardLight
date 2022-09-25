CREATE TABLE [aut].[UserRole]
(
	[UserId] UNIQUEIDENTIFIER NOT NULL,
	[RoleId] INT NOT NULL,
	[IsActive] BIT CONSTRAINT [DF_UserRole_IsActive] DEFAULT 1 NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_UserRole_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_UserRole_UpdateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED ([UserId], [RoleId]), 
    CONSTRAINT [FK_UserRole_To_User] FOREIGN KEY ([UserId]) REFERENCES [aut].[User]([UserId]), 
    CONSTRAINT [FK_UserRole_To_Role] FOREIGN KEY ([RoleId]) REFERENCES [aut].[Role]([RoleId])
)

GO

CREATE INDEX [IX_UserRole_UserId] ON [aut].[UserRole] ([UserId])

GO

CREATE INDEX [IX_UserRole_RoleId] ON [aut].[UserRole] ([RoleId])
