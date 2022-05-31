CREATE TABLE [UCM].[subject] (
    [subject] VARCHAR (8)  NOT NULL,
    [name]    VARCHAR (32) NOT NULL,
    CONSTRAINT [subject_pk] PRIMARY KEY CLUSTERED ([subject] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [subject_subject_uindex]
    ON [UCM].[subject]([subject] ASC);

