CREATE TABLE [UCM].[reminder] (
    [user_id]      DECIMAL (18) NOT NULL,
    [class_id]     INT          NOT NULL,
    [min_trigger]  INT          CONSTRAINT [DF_reminder_min_trigger] DEFAULT ((1)) NOT NULL,
    [for_waitlist] BIT          CONSTRAINT [DF_reminder_for_waitlist] DEFAULT ((0)) NULL,
    [triggered]    BIT          CONSTRAINT [DF_reminder_triggered] DEFAULT ((0)) NULL,
    CONSTRAINT [CK_reminder] CHECK ([min_trigger]>=(1)),
    CONSTRAINT [FK_reminder_class] FOREIGN KEY ([class_id]) REFERENCES [UCM].[class] ([id]) ON DELETE CASCADE ON UPDATE CASCADE
);






GO



GO
CREATE UNIQUE CLUSTERED INDEX [user_crn_unique]
    ON [UCM].[reminder]([user_id] ASC, [class_id] ASC);



