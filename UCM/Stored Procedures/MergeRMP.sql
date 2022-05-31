
CREATE PROCEDURE [UCM].[MergeRMP]
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

    BEGIN TRAN

	-- Stupid merge limitations, must iterate twice.
	-- Also CONTAINS can't accept fields? Bruh.

	MERGE INTO UniScraper.UCM.professor WITH (HOLDLOCK) AS Target
    USING (SELECT rmp_id, first_name, middle_name, last_name, department, num_ratings, rating FROM #professor_rmp) AS SOURCE(rmp_id, first_name, middle_name, last_name, department, num_ratings, rating)
    ON (Target.rmp_id IS NULL AND REPLACE(REPLACE(Target.first_name, ' ', ''), '-', '') LIKE REPLACE(REPLACE(SOURCE.first_name, ' ', ''), '-', '') AND REPLACE(REPLACE(Target.last_name, ' ', ''), '-', '') LIKE REPLACE(REPLACE(SOURCE.last_name, ' ', ''), '-', ''))
	WHEN MATCHED THEN
	UPDATE SET rmp_id = SOURCE.rmp_id, middle_name = SOURCE.middle_name, department = SOURCE.department, num_ratings = SOURCE.num_ratings, rating = SOURCE.rating;

	MERGE INTO UniScraper.UCM.professor WITH (HOLDLOCK) AS Target
    USING (SELECT rmp_id, first_name, middle_name, last_name, department, num_ratings, rating FROM #professor_rmp) AS SOURCE(rmp_id, first_name, middle_name, last_name, department, num_ratings, rating)
    ON (Target.rmp_id = SOURCE.rmp_id AND REPLACE(REPLACE(Target.first_name, ' ', ''), '-', '') LIKE REPLACE(REPLACE(SOURCE.first_name, ' ', ''), '-', '') AND REPLACE(REPLACE(Target.last_name, ' ', ''), '-', '') LIKE REPLACE(REPLACE(SOURCE.last_name, ' ', ''), '-', ''))
	WHEN MATCHED THEN
	UPDATE SET middle_name = SOURCE.middle_name, department = SOURCE.department, num_ratings = SOURCE.num_ratings, rating = SOURCE.rating;

	COMMIT
END
