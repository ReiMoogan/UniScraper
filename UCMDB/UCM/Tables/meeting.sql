CREATE TABLE [UCM].[meeting] (
    [class_id]             INT          NOT NULL,
    [begin_time]           VARCHAR (4)  DEFAULT ('0000') NULL,
    [end_time]             VARCHAR (4)  DEFAULT ('0000') NULL,
    [begin_date]           VARCHAR (10) DEFAULT ('00/00/0000') NULL,
    [end_date]             VARCHAR (10) DEFAULT ('00/00/0000') NULL,
    [building]             VARCHAR (16) NULL,
    [building_description] VARCHAR (64) NULL,
    [campus]               VARCHAR (16) NULL,
    [campus_description]   VARCHAR (64) NULL,
    [room]                 VARCHAR (16) NULL,
    [credit_hour_session]  REAL         DEFAULT ((0.0)) NOT NULL,
    [hours_per_week]       REAL         DEFAULT ((0.0)) NOT NULL,
    [in_session]           TINYINT      DEFAULT ((0)) NOT NULL,
    [meeting_type]         TINYINT      DEFAULT ((1)) NOT NULL,
    CONSTRAINT [FK_meeting_class] FOREIGN KEY ([class_id]) REFERENCES [UCM].[class] ([id])
);






GO
CREATE NONCLUSTERED INDEX [meeting_class_id_index]
    ON [UCM].[meeting]([class_id] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_class]
    ON [UCM].[meeting]([class_id] ASC, [meeting_type] ASC);

