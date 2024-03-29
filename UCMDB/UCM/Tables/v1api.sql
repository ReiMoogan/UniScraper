﻿CREATE TABLE [UCM].[v1api] (
    [crn]            INT             NOT NULL,
    [subject]        VARCHAR (32)    NULL,
    [course_id]      VARCHAR (16)    NULL,
    [course_name]    VARCHAR (128)   NULL,
    [units]          TINYINT         NULL,
    [type]           VARCHAR (8)     NULL,
    [days]           VARCHAR (8)     NULL,
    [hours]          VARCHAR (13)    NULL,
    [room]           VARCHAR (33)    NULL,
    [dates]          VARCHAR (13)    NULL,
    [instructor]     NVARCHAR (4000) NULL,
    [lecture_crn]    VARCHAR (8)     NULL,
    [attached_crn]   VARCHAR (8)     NULL,
    [term]           INT             NOT NULL,
    [capacity]       SMALLINT        NULL,
    [enrolled]       SMALLINT        NULL,
    [available]      SMALLINT        NULL,
    [final_type]     VARCHAR (8)     NULL,
    [final_days]     VARCHAR (8)     NULL,
    [final_hours]    VARCHAR (13)    NULL,
    [final_room]     VARCHAR (33)    NULL,
    [final_dates]    VARCHAR (13)    NULL,
    [simple_name]    VARCHAR (10)    NULL,
    [linked_courses] NVARCHAR (MAX)  NULL
);




GO
CREATE NONCLUSTERED INDEX [IX_term]
    ON [UCM].[v1api]([term] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_crn]
    ON [UCM].[v1api]([crn] ASC, [term] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_class]
    ON [UCM].[v1api]([crn] ASC, [term] ASC, [type] ASC);

