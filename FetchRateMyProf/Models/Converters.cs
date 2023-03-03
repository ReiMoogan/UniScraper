using System;
using Newtonsoft.Json;

namespace FetchRateMyProf.Models;

internal class RMPDateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
{
    public override void WriteJson(JsonWriter writer, DateTimeOffset? value, JsonSerializer serializer)
    {
        var utc = value?.ToUniversalTime();
        
        if (utc == null)
        {
            writer.WriteNull();
            return;
        }
        
        writer.WriteValue(utc.Value.ToString("yyyy-MM-dd hh-mm-ss") + " +0000 UTC");
    }

    public override DateTimeOffset? ReadJson(JsonReader reader, Type objectType, DateTimeOffset? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var time = (string?) reader.Value;
        if (time == null)
            return null;
        
        var year = int.Parse(time[..4]);
        var month = int.Parse(time[5..7]);
        var day = int.Parse(time[8..10]);
        var hour = int.Parse(time[11..13]);
        var minutes = int.Parse(time[14..16]);
        var seconds = int.Parse(time[17..19]);
        var offsetHours = int.Parse(time[21..23]);
        var offsetMinutes = int.Parse(time[23..25]);
        
        var offset = new TimeSpan(offsetHours, offsetMinutes, 0);

        return new DateTimeOffset(year, month, day, hour, minutes, seconds, offset);
    }
}