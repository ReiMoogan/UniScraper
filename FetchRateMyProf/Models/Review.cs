using System;
using Newtonsoft.Json;

namespace FetchRateMyProf.Models;

public record Review (
    [JsonProperty("adminReviewedAt"), JsonConverter(typeof(RMPDateTimeOffsetConverter))] DateTimeOffset? AdminReviewedAt,
    [JsonProperty("attendanceMandatory")] string AttendanceMandatory,
    [JsonProperty("clarityRating")] int ClarityRating,
    [JsonProperty("class")] string Class,
    [JsonProperty("comment")] string Comment,
    [JsonProperty("createdByUser")] bool CreatedByUser,
    [JsonProperty("date"), JsonConverter(typeof(RMPDateTimeOffsetConverter))] DateTimeOffset? Date,
    [JsonProperty("difficultyRating")] int DifficultyRating,
    [JsonProperty("flagStatus")] string FlagStatus,
    [JsonProperty("grade")] string Grade,
    [JsonProperty("helpfulRating")] int HelpfulRating,
    [JsonProperty("id")] string ID,
    [JsonProperty("isForCredit")] bool IsForCredit,
    [JsonProperty("isForOnlineClass")] bool IsForOnlineClass,
    [JsonProperty("legacyId")] int LegacyId,
    [JsonProperty("ratingTags")] string RatingTags,
    [JsonProperty("teacherNote")] object TeacherNote,
    [JsonProperty("textbookUse")] int TextbookUse,
    [JsonProperty("thumbs")] object[] Thumbs,
    [JsonProperty("thumbsDownTotal")] int ThumbsDownTotal,
    [JsonProperty("thumbsUpTotal")] int ThumbsUpTotal,
    [JsonProperty("wouldTakeAgain")] object WouldTakeAgain
);