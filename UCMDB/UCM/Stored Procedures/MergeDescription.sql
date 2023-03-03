
CREATE PROCEDURE [UCM].[MergeDescription]
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

    BEGIN TRAN

	MERGE INTO UniScraper.UCM.[description] WITH (HOLDLOCK) AS Target
    USING (SELECT term_start, term_end, course_number, department, department_code, course_description FROM #description) AS SOURCE(term_start, term_end, course_number, department, department_code, course_description)
    ON Target.course_number = SOURCE.course_number WHEN MATCHED THEN
    UPDATE SET term_start = SOURCE.term_start, term_end = SOURCE.term_end, department = SOURCE.department, department_code = SOURCE.department_code, course_description = SOURCE.course_description
    WHEN NOT MATCHED THEN
    INSERT (term_start, term_end, course_number, department, department_code, course_description) VALUES (SOURCE.term_start, SOURCE.term_end, SOURCE.course_number, SOURCE.department, SOURCE.department_code, SOURCE.course_description);

	COMMIT

	;WITH cte AS
	(
		SELECT [description].[course_number], [class].[course_reference_number], [class].[term], ROW_NUMBER() OVER 
			(PARTITION BY [description].[course_number] ORDER BY [class].[term] DESC, [class].[course_number] ASC) AS row_num
		FROM [UCM].[description]
		INNER JOIN [UCM].[class]
		ON class.course_number LIKE '%' + description.course_number + '%'
	)

	SELECT course_number, course_reference_number, term FROM cte WHERE row_num = 1;
END