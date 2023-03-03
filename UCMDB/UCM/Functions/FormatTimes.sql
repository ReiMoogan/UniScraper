CREATE FUNCTION [UCM].[FormatTimes] (@start VARCHAR(4), @end VARCHAR(4))
RETURNS VARCHAR(13)
WITH EXECUTE AS CALLER
AS
BEGIN
    RETURN([UCM].[FormatTime](@start, 0) + '-' + [UCM].[FormatTime](@end, 1))
END