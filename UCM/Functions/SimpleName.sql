CREATE FUNCTION [UCM].[SimpleName] (@course_number VARCHAR(16))
RETURNS VARCHAR(10)
WITH EXECUTE AS CALLER
AS
BEGIN
    DECLARE @first_dash AS int = CHARINDEX('-', @course_number);
    DECLARE @subject AS VARCHAR(8) = SUBSTRING(@course_number, 1, @first_dash - 1);
    -- Funky substring to get the middle part, dumdum one-indexing
    DECLARE @section AS VARCHAR(8) = SUBSTRING(@course_number, @first_dash + 1, CHARINDEX('-', @course_number, @first_dash + 1) - @first_dash - 1);
    -- Remove leading zeros
    DECLARE @section_short AS VARCHAR(8) = SUBSTRING(@section, PATINDEX('%[^0]%', @section + '.'), LEN(@section));
    
    RETURN(@subject + '-' + @section_short);
END