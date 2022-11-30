CREATE PROCEDURE [yl].[UpdateWorkItem]
	@id UNIQUEIDENTIFIER,
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
SET @timestamp = SYSUTCDATETIME();
UPDATE [yl].[WorkItem]
SET 
	[Title] = @title,
	[TypeId] = @typeId,
	[StatusId] = @statusId,
	[Team] = @team,
	[Itteration] = @itteration,
	[StartDate] = @startDate,
	[TargetDate] = @targetDate,
	[CloseDate] = @closeDate,
	[Priority] = @priority,
	[Effort] = @effort,
	[Value] = @value,
	[UpdateTimestamp] = @timestamp,
	[UpdateUserId] = @userId
WHERE [WorkItemId] = @id
;
END