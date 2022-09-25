CREATE TABLE [aut].[ClientRole]
(
	[ClientId] UNIQUEIDENTIFIER NOT NULL,
	[RoleId] INT NOT NULL,
	[IsActive] BIT CONSTRAINT [DF_ClientRole_IsActive] DEFAULT 1 NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_ClientRole_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_ClientRole_UpdateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_ClientRole] PRIMARY KEY CLUSTERED ([ClientId], [RoleId]), 
    CONSTRAINT [FK_ClientRole_To_Client] FOREIGN KEY ([ClientId]) REFERENCES [aut].[Client]([ClientId]), 
    CONSTRAINT [FK_ClientRole_To_Role] FOREIGN KEY ([RoleId]) REFERENCES [aut].[Role]([RoleId])
)

GO

CREATE INDEX [IX_ClientRole_ClientId] ON [aut].[ClientRole] ([ClientId])

GO

CREATE INDEX [IX_ClientRole_RoleId] ON [aut].[ClientRole] ([RoleId])
