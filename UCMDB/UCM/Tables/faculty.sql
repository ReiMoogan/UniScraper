CREATE TABLE [UCM].[faculty] (
    [class_id]     INT NOT NULL,
    [professor_id] INT NOT NULL,
    CONSTRAINT [FK_faculty_class] FOREIGN KEY ([class_id]) REFERENCES [UCM].[class] ([id]) ON DELETE CASCADE,
    CONSTRAINT [FK_faculty_professor] FOREIGN KEY ([professor_id]) REFERENCES [UCM].[professor] ([id]) ON DELETE CASCADE
);


GO
CREATE UNIQUE CLUSTERED INDEX [class_professor_unique]
    ON [UCM].[faculty]([class_id] ASC, [professor_id] ASC);

