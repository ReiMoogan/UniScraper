CREATE TABLE [UCM].[class] (
    [id]                      INT           NOT NULL,
    [term]                    INT           NOT NULL,
    [course_reference_number] INT           NOT NULL,
    [course_number]           VARCHAR (16)  NOT NULL,
    [campus_description]      VARCHAR (32)  NULL,
    [course_title]            VARCHAR (128) NULL,
    [credit_hours]            TINYINT       CONSTRAINT [DF__class__credit_ho__3B75D760] DEFAULT ((0)) NOT NULL,
    [maximum_enrollment]      SMALLINT      CONSTRAINT [DF__class__maximum_e__3C69FB99] DEFAULT ((0)) NOT NULL,
    [enrollment]              SMALLINT      CONSTRAINT [DF__class__enrollmen__3D5E1FD2] DEFAULT ((0)) NOT NULL,
    [seats_available]         SMALLINT      CONSTRAINT [DF__class__seats_ava__3E52440B] DEFAULT ((0)) NOT NULL,
    [wait_capacity]           SMALLINT      CONSTRAINT [DF__class__wait_capa__3F466844] DEFAULT ((0)) NOT NULL,
    [wait_available]          SMALLINT      CONSTRAINT [DF__class__wait_avai__403A8C7D] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [class_pk] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [class_id_uindex]
    ON [UCM].[class]([id] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [class_course_reference_number_uindex]
    ON [UCM].[class]([course_reference_number] ASC);


GO
CREATE NONCLUSTERED INDEX [ix_crn]
    ON [UCM].[class]([course_number] ASC)
    INCLUDE([course_reference_number]);


GO
CREATE NONCLUSTERED INDEX [_dta_index_class_5_18099105__K4_K1_K2_3]
    ON [UCM].[class]([course_number] ASC, [id] ASC, [term] ASC)
    INCLUDE([course_reference_number]);


GO
CREATE NONCLUSTERED INDEX [IX_term_course_reference_number]
    ON [UCM].[class]([term] ASC)
    INCLUDE([course_reference_number]);


GO
CREATE STATISTICS [_dta_stat_18099105_1_2]
    ON [UCM].[class]([id], [term]);


GO
CREATE STATISTICS [_dta_stat_18099105_1_4_2]
    ON [UCM].[class]([id], [course_number], [term]);


GO
CREATE STATISTICS [_dta_stat_18099105_2_4]
    ON [UCM].[class]([term], [course_number]);

