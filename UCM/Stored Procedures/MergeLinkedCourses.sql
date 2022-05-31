
CREATE PROCEDURE [UCM].[MergeLinkedCourses]
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

    BEGIN TRAN

	MERGE INTO UniScraper.UCM.linked_section WITH (HOLDLOCK) AS Target
    USING (SELECT parent, child FROM #linked_section) AS SOURCE(parent, child)
    ON Target.parent = SOURCE.parent AND Target.child = SOURCE.child WHEN NOT MATCHED THEN
    INSERT (parent, child) VALUES (SOURCE.parent, SOURCE.child);

	COMMIT
END
