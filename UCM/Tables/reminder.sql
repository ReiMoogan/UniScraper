CREATE TABLE [UCM].[reminder] (
    [user_id]                 DECIMAL (18) NOT NULL,
    [course_reference_number] INT          NULL,
    [min_trigger]             INT          CONSTRAINT [DF_reminder_min_trigger] DEFAULT ((1)) NOT NULL,
    [for_waitlist]            BIT          CONSTRAINT [DF_reminder_for_waitlist] DEFAULT ((0)) NULL,
    [triggered]               BIT          CONSTRAINT [DF_reminder_triggered] DEFAULT ((0)) NULL,
    CONSTRAINT [CK_reminder] CHECK ([min_trigger]>=(1)),
    CONSTRAINT [FK_reminder_class] FOREIGN KEY ([course_reference_number]) REFERENCES [UCM].[class] ([course_reference_number])
);


GO
ALTER TABLE [UCM].[reminder] NOCHECK CONSTRAINT [FK_reminder_class];


GO
CREATE UNIQUE CLUSTERED INDEX [user_crn_unique]
    ON [UCM].[reminder]([user_id] ASC, [course_reference_number] ASC);

