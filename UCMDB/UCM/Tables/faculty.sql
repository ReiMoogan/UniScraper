CREATE TABLE [UCM].[faculty] (
    [class_id]        INT           NOT NULL,
    [professor_email] VARCHAR (256) NOT NULL,
    [id]              INT           IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_id] PRIMARY KEY NONCLUSTERED ([id] ASC),
    CONSTRAINT [FK_faculty_class] FOREIGN KEY ([class_id]) REFERENCES [UCM].[class] ([id]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT [FK_faculty_professor] FOREIGN KEY ([professor_email]) REFERENCES [UCM].[professor] ([email]) ON DELETE CASCADE ON UPDATE CASCADE
);






GO
CREATE UNIQUE CLUSTERED INDEX [class_professor_unique]
    ON [UCM].[faculty]([class_id] ASC, [professor_email] ASC);



