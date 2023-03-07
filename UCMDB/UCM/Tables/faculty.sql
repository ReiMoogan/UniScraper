CREATE TABLE [UCM].[faculty] (
    [class_id]        INT           NOT NULL,
    [professor_email] VARCHAR (256) NOT NULL,
    CONSTRAINT [FK_faculty_class] FOREIGN KEY ([class_id]) REFERENCES [UCM].[class] ([id]),
    CONSTRAINT [FK_faculty_professor] FOREIGN KEY ([professor_email]) REFERENCES [UCM].[professor] ([email])
);




GO
CREATE UNIQUE CLUSTERED INDEX [class_professor_unique]
    ON [UCM].[faculty]([class_id] ASC, [professor_email] ASC);



