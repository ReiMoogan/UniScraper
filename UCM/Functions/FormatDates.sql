CREATE FUNCTION [UCM].[FormatDates] (@start VARCHAR(10), @end VARCHAR(10))
RETURNS VARCHAR(13)
WITH EXECUTE AS CALLER
AS
BEGIN
    RETURN([UCM].[FormatDate](@start) + ' ' + [UCM].[FormatDate](@end))
END