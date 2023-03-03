
CREATE PROCEDURE [UCM].[TriggerReminders]
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @Triggered TABLE (
		user_id DECIMAL(18, 0) NOT NULL,
		course_reference_number INT NOT NULL,
		min_trigger INT
	);

	UPDATE reminder SET reminder.triggered = 1
	OUTPUT
		INSERTED.user_id,
		INSERTED.course_reference_number,
		INSERTED.min_trigger
	INTO @Triggered
	FROM  [UniScraper].[UCM].[reminder]
	JOIN [UniScraper].[UCM].[class]
	ON reminder.course_reference_number = class.course_reference_number
	WHERE reminder.triggered = 0 AND ((reminder.for_waitlist = 1 AND reminder.min_trigger <= class.wait_available)
	OR (reminder.for_waitlist = 0 AND reminder.min_trigger <= class.seats_available));

	UPDATE reminder SET reminder.triggered = 0
	FROM [UniScraper].[UCM].[reminder]
	INNER JOIN [UniScraper].[UCM].[class]
	ON reminder.course_reference_number = class.course_reference_number
	WHERE reminder.triggered = 1 AND ((reminder.for_waitlist = 1 AND reminder.min_trigger > class.wait_available)
	OR (reminder.for_waitlist = 0 AND reminder.min_trigger > class.seats_available));

	SELECT * FROM @Triggered;
END