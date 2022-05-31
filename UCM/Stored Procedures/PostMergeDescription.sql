
CREATE PROCEDURE [UCM].[PostMergeDescription]
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

    BEGIN TRAN

	MERGE INTO UniScraper.UCM.[description] WITH (HOLDLOCK) AS Target
    USING (SELECT course_number, course_description FROM #description) AS SOURCE(course_number, course_description)
    ON Target.course_number = SOURCE.course_number WHEN MATCHED THEN
    UPDATE SET course_description = SOURCE.course_description;

	COMMIT
END
