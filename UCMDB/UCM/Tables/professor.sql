CREATE TABLE [UCM].[professor] (
    [id]                    INT           IDENTITY (1, 1) NOT NULL,
    [rmp_id]                VARCHAR (32)  NULL,
    [last_name]             NVARCHAR (64) NOT NULL,
    [first_name]            NVARCHAR (64) NOT NULL,
    [email]                 VARCHAR (256) NULL,
    [department]            VARCHAR (64)  NULL,
    [num_ratings]           INT           CONSTRAINT [DF__professor__num_r__239E4DCF] DEFAULT ((0)) NOT NULL,
    [rating]                REAL          CONSTRAINT [DF__professor__ratin__24927208] DEFAULT ((0.0)) NOT NULL,
    [difficulty]            REAL          CONSTRAINT [DF__professor__diffi__4D4E4A42] DEFAULT ((0.0)) NOT NULL,
    [wouldTakeAgainPercent] REAL          CONSTRAINT [DF__professor__would__4E426E7B] DEFAULT ((0.0)) NOT NULL,
    [full_name]             AS            (([first_name]+' ')+[last_name]) PERSISTED NOT NULL,
    CONSTRAINT [professor_pk] PRIMARY KEY NONCLUSTERED ([id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [professor_id_uindex]
    ON [UCM].[professor]([id] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [email_unique]
    ON [UCM].[professor]([email] ASC);

