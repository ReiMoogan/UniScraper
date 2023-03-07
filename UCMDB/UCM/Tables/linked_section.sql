CREATE TABLE [UCM].[linked_section] (
    [parent] INT NOT NULL,
    [child]  INT NOT NULL,
    CONSTRAINT [FK_linked_section_class] FOREIGN KEY ([parent]) REFERENCES [UCM].[class] ([id]),
    CONSTRAINT [FK_linked_section_class1] FOREIGN KEY ([child]) REFERENCES [UCM].[class] ([id])
);



