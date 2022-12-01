CREATE PROCEDURE [yl].[CreateWorkItem]
	@id UNIQUEIDENTIFIER OUT,
	@parentWorkItemId UNIQUEIDENTIFIER,
	@projectId UNIQUEIDENTIFIER,
	@title NVARCHAR(512),
	@typeId UNIQUEIDENTIFIER,
	@statusId UNIQUEIDENTIFIER,
	@team NVARCHAR(1024),
	@itteration NVARCHAR(1024),
	@startDate DATE,
	@targetDate DATE,
	@closeDate DATE,
	@priority NVARCHAR(128),
	@effort NVARCHAR(128),
	@value NVARCHAR(128),
	@userId UNIQUEIDENTIFIER,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
SET @id = NEWID();
SET @timestamp = SYSUTCDATETIME();
INSERT INTO [yl].[WorkItem] ([WorkItemId],[ParentWorkItemId],[ProjectId],[Title],[TypeId],[StatusId],[Team],[Itteration],[StartDate],[TargetDate],[CloseDate],[Priority],[Effort],[Value],
	[CreateTimestamp],[UpdateTimestamp],[CreateUserId],[UpdateUserId])
VALUES (@id, @parentWorkItemId, @projectId, @title, @typeId, @statusId, @team, @itteration, @startDate, @targetDate, @closeDate, @priority, @effort, @value,
	@timestamp, @timestamp, @userId, @userId);
END