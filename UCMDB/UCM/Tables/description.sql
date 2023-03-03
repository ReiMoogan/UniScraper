CREATE TABLE [UCM].[description] (
    [id]                 INT            IDENTITY (1, 1) NOT NULL,
    [course_number]      VARCHAR (16)   NOT NULL,
    [term_start]         INT            NOT NULL,
    [term_end]           INT            NOT NULL,
    [department]         VARCHAR (32)   NOT NULL,
    [department_code]    VARCHAR (8)    NOT NULL,
    [course_description] VARCHAR (1024) NULL,
    CONSTRAINT [PK_description] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [IX_description] UNIQUE NONCLUSTERED ([course_number] ASC)
);

