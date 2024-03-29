﻿CREATE TABLE [dbo].[Grids]
(
	[Id]            INT             NOT NULL PRIMARY KEY IDENTITY, 
    [Order]         INT             NOT NULL DEFAULT 0,
    [Image]         VARCHAR(255)    NULL, 
    [Title]         VARCHAR(255)    NULL, 
    [CreateDate]    DATETIMEOFFSET  NOT NULL DEFAULT GETDATE(), 
    [Used]          BIT             NOT NULL DEFAULT 0
)
