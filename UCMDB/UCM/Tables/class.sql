CREATE TABLE [UCM].[class] (
    [id]                      AS            ([term]*(10000)+[course_reference_number]) PERSISTED NOT NULL,
    [term]                    INT           NOT NULL,
    [course_reference_number] INT           NOT NULL,
    [course_number]           VARCHAR (16)  NOT NULL,
    [campus_description]      VARCHAR (32)  NULL,
    [course_title]            VARCHAR (128) NULL,
    [credit_hours]            TINYINT       CONSTRAINT [DF__class__credit_ho__3B75D760] DEFAULT ((0)) NOT NULL,
    [maximum_enrollment]      SMALLINT      CONSTRAINT [DF__class__maximum_e__3C69FB99] DEFAULT ((0)) NOT NULL,
    [enrollment]              SMALLINT      CONSTRAINT [DF__class__enrollmen__3D5E1FD2] DEFAULT ((0)) NOT NULL,
    [seats_available]         SMALLINT      CONSTRAINT [DF__class__seats_ava__3E52440B] DEFAULT ((0)) NOT NULL,
    [wait_capacity]           SMALLINT      CONSTRAINT [DF__class__wait_capa__3F466844] DEFAULT ((0)) NULL,
    [wait_available]          SMALLINT      CONSTRAINT [DF__class__wait_avai__403A8C7D] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_class] PRIMARY KEY CLUSTERED ([id] ASC)
);








GO



GO



GO



GO



GO



GO



GO



GO


