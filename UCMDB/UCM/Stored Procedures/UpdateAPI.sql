
CREATE PROCEDURE [UCM].[UpdateAPI]
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRAN
		SELECT course_reference_number AS crn INTO #crn FROM [UCM].[class];

		-- Fetch all relevant linked sections
		DROP TABLE IF EXISTS #linked_section;
		SELECT
			parent AS crn,
			CAST(child AS VARCHAR(8)) AS child
		INTO #linked_section
		FROM #crn
		INNER JOIN [UCM].[linked_section]
		ON #crn.crn = [linked_section].parent;

		-- Convert those linked sections into an "array" for each class CRN
		DROP TABLE IF EXISTS #linked_section_array;
		SELECT
			crn,
			ISNULL((REPLACE(REPLACE((SELECT child FROM #linked_section WHERE #linked_section.crn = #crn.crn FOR JSON AUTO), '{"child":', ''), '}', '')), '[]') AS array
		INTO #linked_section_array
		FROM #crn;

		-- Get all relevant professors for the classes searched
		DROP TABLE IF EXISTS #professor;
		SELECT
			class.course_reference_number AS crn,
			last_name + ' ' + first_name AS full_name
		INTO #professor
		FROM #crn
		INNER JOIN [UCM].[class] ON class.course_reference_number = #crn.crn
		LEFT JOIN [UCM].[faculty] ON class.id = faculty.class_id
		LEFT JOIN [UCM].[professor] ON faculty.professor_email = professor.email;

		-- Create a list of professors for each class
		DROP TABLE IF EXISTS #professor_list;
		SELECT
			crn,
			ISNULL(STRING_AGG(full_name, ' / '), 'Staff') AS instructor
		INTO #professor_list
		FROM #professor
		GROUP BY #professor.crn;

		-- Get all relevant meetings for each class
		DROP TABLE IF EXISTS #meeting;
		SELECT
			class.course_reference_number AS crn,
			meeting_type.type AS type,
			([UCM].[FormatDays](meeting.in_session)) AS days,
			([UCM].[FormatTimes](meeting.begin_time, meeting.end_time)) AS hours,
			(meeting.building + ' ' + meeting.room) AS room,
			([UCM].[FormatDates](meeting.begin_date, meeting.end_date)) AS dates
		INTO #meeting
		FROM #crn
		INNER JOIN [UCM].[class] ON class.course_reference_number = #crn.crn
		LEFT JOIN [UCM].[meeting] ON class.id = meeting.class_id
		LEFT JOIN [UCM].[meeting_type] ON meeting_type.id = meeting.meeting_type;

		-- Merge the main meeting and exam dates into one row (i.e., all meetings for each class)
		DROP TABLE IF EXISTS #meeting_times;
		SELECT
			#crn.crn,
			main.type,
			main.days,
			main.hours,
			main.room,
			main.dates,
			exam.type AS final_type,
			exam.days AS final_days,
			exam.hours AS final_hours,
			exam.room AS final_room,
			exam.dates AS final_dates,
			lecture.child AS lecture_crn,
			attached.child AS attached_crn
		INTO #meeting_times
		FROM #crn
		LEFT JOIN #meeting AS main
		ON #crn.crn = main.crn AND main.type != 'EXAM'
		LEFT JOIN #meeting AS exam
		ON #crn.crn = exam.crn AND exam.type = 'EXAM'
		LEFT JOIN #linked_section lecture
		ON main.type != 'LECT' AND #crn.crn = lecture.crn AND lecture.child = (SELECT MIN(child) FROM #linked_section WHERE crn = lecture.crn)
		LEFT JOIN #linked_section attached
		ON main.type != 'LECT' AND (SELECT COUNT(*) FROM #linked_section WHERE crn = attached.crn) > 1 AND #crn.crn = attached.crn AND attached.child = (SELECT MAX(child) FROM #linked_section WHERE crn = attached.crn)

		-- Get the main class info
		DROP TABLE IF EXISTS #class;
		SELECT
			course_reference_number AS crn,
			subject.name AS subject,
			course_number AS course_id,
			course_title AS course_name,
			credit_hours AS units,
			maximum_enrollment AS capacity,
			enrollment AS enrolled,
			seats_available AS available,
			term
		INTO #class
		FROM #crn
		LEFT JOIN [UCM].[class] ON class.course_reference_number = #crn.crn
		LEFT JOIN [UCM].[subject] ON subject.subject = SUBSTRING(class.course_number, 0, CHARINDEX('-', class.course_number));
	
		-- reimu
		DROP TABLE IF EXISTS [UCM].[v1api];
		SELECT
			#class.crn,
			subject,
			course_id,
			course_name,
			units,
			type,
			days,
			hours,
			room,
			dates,
			instructor,
			lecture_crn,
			attached_crn,
			term,
			capacity,
			enrolled,
			available,
			final_type,
			final_days,
			final_hours,
			final_room,
			final_dates,
			([UCM].[SimpleName](course_id)) AS simple_name,
			#linked_section_array.array AS linked_courses
		INTO [UCM].[v1api]
		FROM #class
		LEFT JOIN #meeting_times
		ON #class.crn = #meeting_times.crn
		LEFT JOIN #linked_section_array
		ON #class.crn = #linked_section_array.crn
		LEFT JOIN #professor_list on #class.crn = #professor_list.crn;
	
		-- Clean up.
		DROP TABLE IF EXISTS #crn;
		DROP TABLE IF EXISTS #linked_section;
		DROP TABLE IF EXISTS #linked_section_array;
		DROP TABLE IF EXISTS #professor;
		DROP TABLE IF EXISTS #professor_list;
		DROP TABLE IF EXISTS #meeting;
		DROP TABLE IF EXISTS #meeting_times;
		DROP TABLE IF EXISTS #class;
	COMMIT
END