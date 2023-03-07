CREATE TABLE [UCM].[professor] (
    [email]                    VARCHAR (256) NOT NULL,
    [rmp_id]                   VARCHAR (32)  NULL,
    [last_name]                NVARCHAR (64) NOT NULL,
    [first_name]               NVARCHAR (64) NOT NULL,
    [department]               VARCHAR (64)  NULL,
    [num_ratings]              INT           CONSTRAINT [DF__professor__num_r__239E4DCF] DEFAULT ((0)) NOT NULL,
    [rating]                   REAL          CONSTRAINT [DF__professor__ratin__24927208] DEFAULT ((0.0)) NOT NULL,
    [difficulty]               REAL          CONSTRAINT [DF__professor__diffi__4D4E4A42] DEFAULT ((0.0)) NOT NULL,
    [would_take_again_percent] REAL          CONSTRAINT [DF__professor__would__4E426E7B] DEFAULT ((0.0)) NOT NULL,
    [full_name]                AS            (([first_name]+' ')+[last_name]) PERSISTED NOT NULL,
    CONSTRAINT [PK_professor] PRIMARY KEY NONCLUSTERED ([email] ASC)
);






GO



GO
CREATE NONCLUSTERED INDEX [IX_professor_1]
    ON [UCM].[professor]([full_name] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_professor]
    ON [UCM].[professor]([first_name] ASC, [last_name] ASC);

