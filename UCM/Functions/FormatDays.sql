CREATE FUNCTION [UCM].[FormatDays] (@days tinyint)
RETURNS VARCHAR(8)
WITH EXECUTE AS CALLER
AS
BEGIN
    DECLARE @day_string AS VARCHAR(8) = '';
	-- M T W R F S U -> 2 4 8 16 32 64 1, respectively

	IF @days & 2 = 2
    BEGIN
	    SET @day_string = @day_string + 'M';
    END

    IF @days & 4 = 4
    BEGIN
	    SET @day_string = @day_string + 'T';
    END

    IF @days & 8 = 8
    BEGIN
	    SET @day_string = @day_string + 'W';
    END

    IF @days & 16 = 16
    BEGIN
	    SET @day_string = @day_string + 'R';
    END

    IF @days & 32 = 32
    BEGIN
	    SET @day_string = @day_string + 'F';
    END

    IF @days & 64 = 64
    BEGIN
	    SET @day_string = @day_string + 'S';
    END

	IF @days & 1 = 1
    BEGIN
	    SET @day_string = @day_string + 'U';
    END

    RETURN(@day_string);
END