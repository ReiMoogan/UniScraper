CREATE TABLE [UCM].[linked_section] (
    [parent] INT NOT NULL,
    [child]  INT NOT NULL,
    [id]     INT IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_linked_section_id] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_linked_section_class] FOREIGN KEY ([parent]) REFERENCES [UCM].[class] ([id]),
    CONSTRAINT [FK_linked_section_class1] FOREIGN KEY ([child]) REFERENCES [UCM].[class] ([id])
);





