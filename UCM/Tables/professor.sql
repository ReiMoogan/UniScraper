CREATE TABLE [UCM].[professor] (
    [id]          INT           IDENTITY (1, 1) NOT NULL,
    [rmp_id]      INT           NULL,
    [last_name]   NVARCHAR (64) NOT NULL,
    [first_name]  NVARCHAR (64) NOT NULL,
    [middle_name] NVARCHAR (64) NULL,
    [full_name]   AS            ((([first_name]+' ')+isnull(case when [middle_name]='' then NULL else [middle_name] end+' ',''))+[last_name]),
    [email]       VARCHAR (256) NULL,
    [department]  VARCHAR (64)  NULL,
    [num_ratings] INT           CONSTRAINT [DF__professor__num_r__239E4DCF] DEFAULT ((0)) NOT NULL,
    [rating]      REAL          CONSTRAINT [DF__professor__ratin__24927208] DEFAULT ((0.0)) NOT NULL,
    CONSTRAINT [professor_pk] PRIMARY KEY NONCLUSTERED ([id] ASC)
);


GO
CREATE UNIQUE CLUSTERED INDEX [email_unique]
    ON [UCM].[professor]([email] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [professor_id_uindex]
    ON [UCM].[professor]([id] ASC);

