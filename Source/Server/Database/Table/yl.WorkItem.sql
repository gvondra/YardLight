﻿CREATE TABLE [yl].[WorkItem]
(
	[WorkItemId] UNIQUEIDENTIFIER NOT NULL,
	[Title] NVARCHAR(512) NOT NULL,
	[Type] SMALLINT NOT NULL,
	[Status] SMALLINT CONSTRAINT [DF_WorkItem_Status] DEFAULT (0) NOT NULL,
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
	CONSTRAINT [PK_WorkItem] PRIMARY KEY CLUSTERED ([WorkItemId])
)
