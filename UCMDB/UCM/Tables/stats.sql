CREATE TABLE [UCM].[stats] (
    [table_name]  VARCHAR (32)  NOT NULL,
    [last_update] DATETIME2 (7) CONSTRAINT [DF__stats__last_upda__29572725] DEFAULT (sysdatetime()) NOT NULL
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [stats_table_name_uindex]
    ON [UCM].[stats]([table_name] ASC);

