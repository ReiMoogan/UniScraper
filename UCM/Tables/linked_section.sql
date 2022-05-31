CREATE TABLE [UCM].[linked_section] (
    [parent] INT NOT NULL,
    [child]  INT NOT NULL,
    CONSTRAINT [PK_linked_section] PRIMARY KEY CLUSTERED ([parent] ASC, [child] ASC),
    CONSTRAINT [FK_child_class] FOREIGN KEY ([child]) REFERENCES [UCM].[class] ([course_reference_number]),
    CONSTRAINT [FK_parent_class] FOREIGN KEY ([parent]) REFERENCES [UCM].[class] ([course_reference_number])
);

