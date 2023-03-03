CREATE FUNCTION [UCM].[FormatDate] (@date VARCHAR(10))
RETURNS VARCHAR(6)
WITH EXECUTE AS CALLER
AS
BEGIN
    DECLARE @month AS TINYINT = CAST(SUBSTRING(@date, 1, 2) AS tinyint);
    DECLARE @output VARCHAR(6)= SUBSTRING(@date, 4, 2) + '-';

    RETURN(@output +
        CASE
            WHEN @month = 1 THEN 'JAN'
            WHEN @month = 2 THEN 'FEB'
            WHEN @month = 3 THEN 'MAR'
            WHEN @month = 4 THEN 'APR'
            WHEN @month = 5 THEN 'MAY'
            WHEN @month = 6 THEN 'JUN'
            WHEN @month = 7 THEN 'JUL'
            WHEN @month = 8 THEN 'AUG'
            WHEN @month = 9 THEN 'SEP'
            WHEN @month = 10 THEN 'OCT'
            WHEN @month = 11 THEN 'NOV'
            WHEN @month = 12 THEN 'DEC'
        END);
END