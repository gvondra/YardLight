CREATE TABLE [yl].[WorkItemStatus]
(
	[WorkItemStatusId] UNIQUEIDENTIFIER NOT NULL,
	[WorkItemTypeId] UNIQUEIDENTIFIER NOT NULL,
	[ProjectId] UNIQUEIDENTIFIER NOT NULL,
	[Title] NVARCHAR(256) NOT NULL,
	[ColorCode] NVARCHAR(128) CONSTRAINT [DF_WorkItemStatus_ColorCode] DEFAULT 'black' NOT NULL,
	[Order] SMALLINT CONSTRAINT [DF_WorkItemStatus_Order] DEFAULT 0 NOT NULL,
	[IsActive] BIT CONSTRAINT [DF_WorkItemStatus_IsActive] DEFAULT 1 NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_WorkItemStatus_CreateTimestamp] DEFAULT (SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_WorkItemStatus_UpdateTimestamp] DEFAULT (SYSUTCDATETIME()) NOT NULL,
	[CreateUserId] UNIQUEIDENTIFIER NOT NULL,
	[UpdateUserId] UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT [PK_WorkItemStatus] PRIMARY KEY CLUSTERED ([WorkItemStatusId]), 
    CONSTRAINT [FK_WorkItemStatus_To_Project] FOREIGN KEY ([ProjectId]) REFERENCES [yl].[Project]([ProjectId]), 
    CONSTRAINT [FK_WorkItemStatus_To_WorkItemType] FOREIGN KEY ([WorkItemTypeId]) REFERENCES [yl].[WorkItemType]([WorkItemTypeId])
)

GO

CREATE INDEX [IX_WorkItemStatus_ProjectId] ON [yl].[WorkItemStatus] ([ProjectId])

GO

CREATE INDEX [IX_WorkItemStatus_WorkItemTypeId] ON [yl].[WorkItemStatus] ([WorkItemTypeId])
