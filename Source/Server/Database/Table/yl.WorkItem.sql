CREATE TABLE [yl].[WorkItem]
(
	[WorkItemId] UNIQUEIDENTIFIER NOT NULL,
	[ParentWorkItemId] UNIQUEIDENTIFIER NULL,
	[ProjectId] UNIQUEIDENTIFIER NOT NULL,
	[Title] NVARCHAR(512) NOT NULL,
	[TypeId] UNIQUEIDENTIFIER NOT NULL,
	[StatusId] UNIQUEIDENTIFIER NOT NULL,
	[Team] NVARCHAR(1024) CONSTRAINT [DF_WorkItem_Team] DEFAULT ('') NOT NULL,
	[Itteration] NVARCHAR(1024) CONSTRAINT [DF_WorkItem_Itteration] DEFAULT ('') NOT NULL,
	[StartDate] DATE NULL,
	[TargetDate] DATE NULL,
	[CloseDate] DATE NULL,
	[Priority] NVARCHAR(128) CONSTRAINT [DF_WorkItem_Priority] DEFAULT ('') NOT NULL,
	[Effort] NVARCHAR(128) CONSTRAINT [DF_WorkItem_Effort] DEFAULT ('') NOT NULL,
	[Value] NVARCHAR(128) CONSTRAINT [DF_WorkItem_Value] DEFAULT ('') NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_WorkItem_CreateTimestamp] DEFAULT (SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_WorkItem_UpdateTimestamp] DEFAULT (SYSUTCDATETIME()) NOT NULL,
	[CreateUserId] UNIQUEIDENTIFIER NOT NULL,
	[UpdateUserId] UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT [PK_WorkItem] PRIMARY KEY NONCLUSTERED ([WorkItemId]), 
    CONSTRAINT [FK_WorkItem_To_Project] FOREIGN KEY ([ProjectId]) REFERENCES [yl].[Project]([ProjectId]), 
    CONSTRAINT [FK_WorkItem_To_Status] FOREIGN KEY ([StatusId]) REFERENCES [yl].[WorkItemStatus]([WorkItemStatusId]), 
    CONSTRAINT [FK_WorkItem_To_Type] FOREIGN KEY ([TypeId]) REFERENCES [yl].[WorkItemType]([WorkItemTypeId]), 
    CONSTRAINT [FK_WorkItem_To_ParentWorkItem] FOREIGN KEY ([ParentWorkItemId]) REFERENCES [yl].[WorkItem]([WorkItemId]) 
)

GO

CREATE CLUSTERED INDEX [IX_WorkItem_ProjectId] ON [yl].[WorkItem] ([ProjectId])

GO

CREATE INDEX [IX_WorkItem_StatusId] ON [yl].[WorkItem] ([StatusId])

GO

CREATE INDEX [IX_WorkItem_TypeId] ON [yl].[WorkItem] ([TypeId])

GO

CREATE INDEX [IX_WorkItem_Team] ON [yl].[WorkItem] ([Team])

GO

CREATE INDEX [IX_WorkItem_Itteration] ON [yl].[WorkItem] ([Itteration])

GO

CREATE INDEX [IX_WorkItem_ParentWorkItemId] ON [yl].[WorkItem] ([ParentWorkItemId])
