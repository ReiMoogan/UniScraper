CREATE PROCEDURE [UCM].[MergeUpload]
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRAN

    MERGE INTO UniScraper.UCM.class WITH (HOLDLOCK) AS Target
    USING (SELECT id, term, course_reference_number, course_number, campus_description, course_title, credit_hours, maximum_enrollment, enrollment, seats_available, wait_capacity, wait_available FROM #class) AS SOURCE(id, term, course_reference_number, course_number, campus_description, course_title, credit_hours, maximum_enrollment, enrollment, seats_available, wait_capacity, wait_available)
    ON Target.id = SOURCE.id WHEN MATCHED THEN
    UPDATE SET term = SOURCE.term, course_reference_number = SOURCE.course_reference_Number, course_number = SOURCE.course_number, campus_description = SOURCE.campus_description, course_title = SOURCE.course_title, credit_hours = SOURCE.credit_hours, maximum_enrollment = SOURCE.maximum_enrollment, enrollment = SOURCE.enrollment, seats_available = SOURCE.seats_available, wait_capacity = SOURCE.wait_capacity, wait_available = SOURCE.wait_available
    WHEN NOT MATCHED THEN
    INSERT (id, term, course_reference_number, course_number, campus_description, course_title, credit_hours, maximum_enrollment, enrollment, seats_available, wait_capacity, wait_available) VALUES (SOURCE.id, SOURCE.term, SOURCE.course_reference_number, SOURCE.course_number, SOURCE.campus_description, SOURCE.course_title, SOURCE.credit_hours, SOURCE.maximum_enrollment, SOURCE.enrollment, SOURCE.seats_available, SOURCE.wait_capacity, SOURCE.wait_available);
            
    MERGE INTO UniScraper.UCM.professor WITH (HOLDLOCK) AS Target
    USING (SELECT last_name, first_name, email FROM #professor) AS SOURCE(last_name, first_name, email)
    ON Target.email = SOURCE.email WHEN MATCHED THEN
    UPDATE SET last_name = SOURCE.last_name, first_name = SOURCE.first_name
    WHEN NOT MATCHED THEN
    INSERT (last_name, first_name, email) VALUES (SOURCE.last_name, SOURCE.first_name, SOURCE.email);

    MERGE INTO UniScraper.UCM.faculty WITH (HOLDLOCK) AS Target
    USING (SELECT #faculty.class_id, #faculty.professor_email, professor.id FROM #faculty INNER JOIN UniScraper.UCM.professor ON professor.email = #faculty.professor_email) AS SOURCE(class_id, professor_email, professor_id)
    ON Target.class_id = SOURCE.class_id AND Target.professor_id = SOURCE.professor_id
    WHEN NOT MATCHED BY TARGET THEN INSERT (class_id, professor_id) VALUES (SOURCE.class_id, SOURCE.professor_id);

    MERGE INTO UniScraper.UCM.meeting WITH (HOLDLOCK) AS Target
    USING (SELECT class.id, begin_time, end_time, begin_date, end_date, building, building_description, campus, #meeting.campus_description, room, credit_hour_session, hours_per_week, in_session, meeting_type FROM #meeting INNER JOIN UniScraper.UCM.class ON class.course_reference_number = #meeting.class_id) AS SOURCE(class_id, begin_time, end_time, begin_date, end_date, building, building_description, campus, campus_description, room, credit_hour_session, hours_per_week, in_session, meeting_type)
    ON Target.class_id = SOURCE.class_id AND Target.meeting_type = SOURCE.meeting_type WHEN MATCHED THEN
    UPDATE SET begin_time = SOURCE.begin_time, end_time = SOURCE.end_time, begin_date = SOURCE.begin_date, end_date = SOURCE.end_date, building = SOURCE.building, building_description = SOURCE.building_description, campus = SOURCE.campus, campus_description = SOURCE.campus_description, room = SOURCE.room, credit_hour_session = SOURCE.credit_hour_session, hours_per_week = SOURCE.hours_per_week, in_session = SOURCE.in_session, meeting_type = SOURCE.meeting_type
    WHEN NOT MATCHED THEN
    INSERT (class_id, begin_time, end_time, begin_date, end_date, building, building_description, campus, campus_description, room, credit_hour_session, hours_per_week, in_session, meeting_type) VALUES (SOURCE.class_id, SOURCE.begin_time, SOURCE.end_time, SOURCE.begin_date, SOURCE.end_date, SOURCE.building, SOURCE.building_description, SOURCE.campus, SOURCE.campus_description, SOURCE.room, SOURCE.credit_hour_session, SOURCE.hours_per_week, SOURCE.in_session, SOURCE.meeting_type);

	COMMIT
END