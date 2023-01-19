CREATE TABLE [dbo].[Logs]
(
	    Id          int             NOT NULL    PRIMARY KEY IDENTITY(1,1),
        CreatedOn   datetime        NOT NULL,
        LogLevel    nvarchar(10)    NULL,
        LogMessage  nvarchar(max)   NULL,
        StackTrace  nvarchar(max)   NULL,
        Exception   nvarchar(max)   NULL,
        Logger      nvarchar(255)   NULL,
        LogUrl      nvarchar(255)   NULL
)
