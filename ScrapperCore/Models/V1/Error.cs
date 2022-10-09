using Newtonsoft.Json;

namespace ScrapperCore.Models.V1;

public class Error
{
    public static Error GetError(int code)
    {
        return code switch
        {
            2 => new Error("term_not_numeric", "Term must be convertible to a numeric value", code),
            102 => new Error("no_course_or_term", "Course or term not provided", code),
            _ => new Error("unknown_error", "An unknown error has occurred", code)
        };
    }

    public Error(string errorTitle = null, string errorDescription = null, int errorCode = default)
    {
        ErrorTitle = errorTitle;
        ErrorDescription = errorDescription;
        ErrorCode = errorCode;
    }

    [JsonProperty("error_title")] public string ErrorTitle { get; init; }
    [JsonProperty("error_description")] public string ErrorDescription { get; init; }
    [JsonProperty("error_code")] public int ErrorCode { get; init; }
}