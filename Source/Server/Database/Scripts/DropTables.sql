DROP TABLE IF EXISTS [yl].[ProjectUser_v2];


IF COL_LENGTH('[yl].[ProjectUser]', 'UserId') IS NOT NULL
BEGIN
ALTER TABLE [yl].[ProjectUser]
DROP COLUMN [UserId]
END