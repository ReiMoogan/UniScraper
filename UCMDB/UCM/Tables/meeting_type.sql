CREATE TABLE [UCM].[meeting_type] (
    [id]   TINYINT      NOT NULL,
    [name] VARCHAR (16) NOT NULL,
    [type] VARCHAR (8)  NOT NULL,
    CONSTRAINT [meeting_type_pk] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [meeting_type_id_uindex]
    ON [UCM].[meeting_type]([id] ASC);

