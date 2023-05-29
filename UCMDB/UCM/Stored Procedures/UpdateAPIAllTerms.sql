CREATE PROCEDURE [UCM].[UpdateAPIAllTerms]
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRAN
        DROP TABLE IF EXISTS [UCM].[v1api];
        CREATE TABLE [UCM].[v1api](
              [crn] [int] NOT NULL,
              [subject] [varchar](32) NULL,
              [course_id] [varchar](16) NULL,
              [course_name] [varchar](128) NULL,
              [units] [tinyint] NULL,
              [type] [varchar](8) NULL,
              [days] [varchar](8) NULL,
              [hours] [varchar](13) NULL,
              [room] [varchar](33) NULL,
              [dates] [varchar](13) NULL,
              [instructor] [nvarchar](4000) NULL,
              [lecture_crn] [varchar](8) NULL,
              [attached_crn] [varchar](8) NULL,
              [term] [int] NOT NULL,
              [capacity] [smallint] NULL,
              [enrolled] [smallint] NULL,
              [available] [smallint] NULL,
              [final_type] [varchar](8) NULL,
              [final_days] [varchar](8) NULL,
              [final_hours] [varchar](13) NULL,
              [final_room] [varchar](33) NULL,
              [final_dates] [varchar](13) NULL,
              [simple_name] [varchar](10) NULL,
              [linked_courses] [nvarchar](max) NULL,

              INDEX IX_class NONCLUSTERED (crn, term, type),
              INDEX IX_crn NONCLUSTERED (crn, term),
              INDEX IX_term NONCLUSTERED (term),
        )

        SELECT DISTINCT term, processed = 0 INTO #terms FROM [UniScraper].[UCM].[class];

        DECLARE @recent_term INT;

        WHILE (SELECT COUNT(*) FROM #terms WHERE processed = 0) > 0
            BEGIN
                SELECT TOP 1 @recent_term = term FROM #terms WHERE processed = 0;

                -- Start processing for term

                EXEC [UCM].[UpdateAPIForTerm] @term = @recent_term;

                -- End processing for term

                UPDATE #terms SET processed = 1 WHERE term = @recent_term;
            END

        DROP TABLE IF EXISTS #terms;
    COMMIT
END