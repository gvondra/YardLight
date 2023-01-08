CREATE TABLE [yl].[WorkItem_Comment]
(
	[WorkItemId] UNIQUEIDENTIFIER NOT NULL,
	[CommentId] UNIQUEIDENTIFIER NOT NULL,
	[Type] SMALLINT NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_WorkItem_Comment_CreateTimestamp] DEFAULT (SYSUTCDATETIME()) NOT NULL,
	[CreateUserId] UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT [PK_WorkItem_Comment] PRIMARY KEY CLUSTERED ([WorkItemId], [CommentId]), 
    CONSTRAINT [FK_WorkItem_Comment_To_WorkItem] FOREIGN KEY ([WorkItemId]) REFERENCES [yl].[WorkItem]([WorkItemId]), 
    CONSTRAINT [FK_WorkItem_Comment_To_Comment] FOREIGN KEY ([CommentId]) REFERENCES [yl].[Comment]([CommentId])
)

GO

CREATE INDEX [IX_WorkItem_Comment_WorkItemId] ON [yl].[WorkItem_Comment] ([WorkItemId])

GO

CREATE INDEX [IX_WorkItem_Comment_CommentId] ON [yl].[WorkItem_Comment] ([CommentId])
