CREATE FUNCTION [UCM].[FormatTime] (@time VARCHAR(4), @meridiem BIT = 1)
RETURNS VARCHAR(7)
WITH EXECUTE AS CALLER
AS
BEGIN
    DECLARE @minute_str AS VARCHAR(2) = SUBSTRING(@time, 3, 2);
    DECLARE @hour AS TINYINT = CAST(SUBSTRING(@time, 1, 2) AS tinyint);
    DECLARE @output VARCHAR(7);

    IF @hour = 0 BEGIN
        SET @output = '12:' + @minute_str;
        IF @meridiem = 1 BEGIN
            SET @output = @output + 'am';
        END
        RETURN(@output);
    END

    IF @hour = 12 BEGIN
        SET @output = '12:' + @minute_str;
        IF @meridiem = 1 BEGIN
            SET @output = @output + 'pm';
        END
        RETURN(@output);
    END

    IF @hour < 12 BEGIN
        SET @output = CAST(@hour AS VARCHAR(2)) + ':' + @minute_str;
        IF @meridiem = 1 BEGIN
            SET @output = @output + 'am';
        END
        RETURN(@output);
    END

    SET @output = CAST((@hour - 12) AS VARCHAR(2)) + ':' + @minute_str;
        IF @meridiem = 1 BEGIN
            SET @output = @output + 'pm';
        END
    RETURN(@output);
END