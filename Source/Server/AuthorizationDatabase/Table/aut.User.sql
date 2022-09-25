CREATE TABLE [aut].[User]
(
	[UserId] UNIQUEIDENTIFIER CONSTRAINT [DF_User_Id] DEFAULT NEWID() NOT NULL,
	[ReferenceId] VARCHAR(1024) NOT NULL,
	[EmailAddressId] UNIQUEIDENTIFIER NOT NULL,
	[Name] VARCHAR(1024) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_User_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_User_UpdateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([UserId]), 
    CONSTRAINT [FK_User_To_EmailAddress] FOREIGN KEY ([EmailAddressId]) REFERENCES [aut].[EmailAddress]([EmailAddressId])
)

GO

CREATE UNIQUE INDEX [IX_User_ReferenceId] ON [aut].[User] ([ReferenceId])

GO

CREATE UNIQUE INDEX [IX_User_EmailAddressId] ON [aut].[User] ([EmailAddressId])
