CREATE TABLE [yl].[Itteration]
(
	[ItterationId] UNIQUEIDENTIFIER NOT NULL,
	[ProjectId] UNIQUEIDENTIFIER NOT NULL,
	[Name] NVARCHAR(512) NOT NULL,
	[Start] DATE NULL,
	[End] DATE NULL,
	[Hidden] BIT CONSTRAINT [DF_Itteration_Hidden] DEFAULT (0) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_Itteration_CreateTimestamp] DEFAULT (SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_Itteration_UpdateTimestamp] DEFAULT (SYSUTCDATETIME()) NOT NULL,
	[CreateUserId] UNIQUEIDENTIFIER NOT NULL,
	[UpdateUserId] UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT [PK_Itteration] PRIMARY KEY NONCLUSTERED ([ItterationId]), 
    CONSTRAINT [FK_Itteration_To_Project] FOREIGN KEY ([ProjectId]) REFERENCES [yl].[Project]([ProjectId])
)

GO

CREATE UNIQUE CLUSTERED INDEX [IX_Itteration_ProjectId] ON [yl].[Itteration] ([ProjectId], [Name] DESC)
