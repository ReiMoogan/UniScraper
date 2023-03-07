CREATE PROCEDURE [UCM].[MergeRMP]
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

    BEGIN TRAN

	-- Stupid merge limitations, must iterate twice.
	-- Also CONTAINS can't accept fields? Bruh.

	MERGE INTO UniScraper.UCM.professor WITH (HOLDLOCK) AS Target
    USING (SELECT rmp_id, first_name, last_name, department, num_ratings, rating, difficulty, would_take_again_percent FROM #professor_rmp) AS SOURCE(rmp_id, first_name, last_name, department, num_ratings, rating, difficulty, would_take_again_percent)
    ON (Target.rmp_id IS NULL AND REPLACE(REPLACE(Target.first_name, ' ', ''), '-', '') LIKE REPLACE(REPLACE(SOURCE.first_name, ' ', ''), '-', '') AND REPLACE(REPLACE(Target.last_name, ' ', ''), '-', '') LIKE REPLACE(REPLACE(SOURCE.last_name, ' ', ''), '-', ''))
	WHEN MATCHED THEN
	UPDATE SET rmp_id = SOURCE.rmp_id, department = SOURCE.department, num_ratings = SOURCE.num_ratings, rating = SOURCE.rating, difficulty = SOURCE.difficulty, would_take_again_percent = SOURCE.would_take_again_percent;

	MERGE INTO UniScraper.UCM.professor WITH (HOLDLOCK) AS Target
    USING (SELECT rmp_id, first_name, last_name, department, num_ratings, rating, difficulty, would_take_again_percent FROM #professor_rmp) AS SOURCE(rmp_id, first_name, last_name, department, num_ratings, rating, difficulty, would_take_again_percent)
    ON (Target.rmp_id = SOURCE.rmp_id AND REPLACE(REPLACE(Target.first_name, ' ', ''), '-', '') LIKE REPLACE(REPLACE(SOURCE.first_name, ' ', ''), '-', '') AND REPLACE(REPLACE(Target.last_name, ' ', ''), '-', '') LIKE REPLACE(REPLACE(SOURCE.last_name, ' ', ''), '-', ''))
	WHEN MATCHED THEN
	UPDATE SET department = SOURCE.department, num_ratings = SOURCE.num_ratings, rating = SOURCE.rating, difficulty = SOURCE.difficulty, would_take_again_percent = SOURCE.would_take_again_percent;

	COMMIT
    
    UPDATE UniScraper.UCM.stats SET last_update = SYSDATETIME() WHERE table_name = 'professor';
END