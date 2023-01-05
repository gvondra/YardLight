CREATE TABLE [yl].[WorkItemType]
(
	[WorkItemTypeId] UNIQUEIDENTIFIER NOT NULL,
	[ProjectId] UNIQUEIDENTIFIER NOT NULL,
	[Title] NVARCHAR(256) NOT NULL,
	[ColorCode] NVARCHAR(128) CONSTRAINT [DF_WorkItemType_ColorCode] DEFAULT 'black' NOT NULL,
	[IsActive] BIT CONSTRAINT [DF_WorkItemType_IsActive] DEFAULT 1 NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_WorkItemType_CreateTimestamp] DEFAULT (SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_WorkItemType_UpdateTimestamp] DEFAULT (SYSUTCDATETIME()) NOT NULL,
	[CreateUserId] UNIQUEIDENTIFIER NOT NULL,
	[UpdateUserId] UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT [PK_WorkItemType] PRIMARY KEY NONCLUSTERED ([WorkItemTypeId]), 
    CONSTRAINT [FK_WorkItemType_To_Project] FOREIGN KEY ([ProjectId]) REFERENCES [yl].[Project]([ProjectId])
)

GO

CREATE CLUSTERED INDEX [IX_WorkItemType_ProjectId] ON [yl].[WorkItemType] ([ProjectId])
