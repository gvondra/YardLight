﻿IF OBJECT_ID('[yl].[ProjectUser]') IS NOT NULL 
BEGIN
ALTER TABLE [yl].[ProjectUser]
DROP COLUMN IF EXISTS [UserId]
;
END