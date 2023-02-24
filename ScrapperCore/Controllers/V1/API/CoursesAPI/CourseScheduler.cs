using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ScrapperCore.Models.V1;

// This entire class is a W.I.P., and probably will not be finished.
// ReSharper disable NotAccessedField.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedVariable
// ReSharper disable CollectionNeverQueried.Local

namespace ScrapperCore.Controllers.V1.API.CoursesAPI;

// Implemented from https://github.com/dragonbone81/bobcat-courses-backend/blob/master/course_api/data_managers/uc_merced/course_scheduler.py
// Honestly, I am not sure I like this design, but screw it.
// I'd implement this using a stored procedure instead.
public class CourseScheduler
{
    private IDbConnection _connection;
    private int _term;
    private string? _earliestTime;
    private string? _latestTime;
    private IEnumerable<string> _badCRNs;
    private SortOrder _gaps;
    private SortOrder _days;
    private bool _searchFull;

    public CourseScheduler(IDbConnection connection, int term, string? earliestTime, string? latestTime, IEnumerable<string> badCRNs, SortOrder gaps = SortOrder.Ascending,
        SortOrder days = SortOrder.Ascending, bool searchFull = false)
    {
        _connection = connection;
        _term = term;
        _earliestTime = earliestTime;
        _latestTime = latestTime;
        _badCRNs = badCRNs;
        _gaps = gaps;
        _days = days;
        _searchFull = searchFull;
    }

    private static TimeInterval ConvertTime(string time)
    {
        var timeSplit = time.Split('-');
        var startTime = timeSplit[0].Split(':');
        var endTime = timeSplit[1].Split(':');

        var startTimeHour = int.Parse(startTime[0]);
        var endTimeHour = int.Parse(endTime[0]);
        var startTimeMinute = int.Parse(startTime[1]);
        var endTimeMinute = int.Parse(endTime[1][..^2]); // Remove the am/pm thing

        if (endTime[1].EndsWith("pm")) // Convert our times to 24-hour format.
        {
            if (endTimeHour != 12) // 12:00 PM is valid. Otherwise, advance by 12 hours.
            {
                endTimeHour += 12;
            }

            if (startTimeHour + 12 <= endTimeHour) // 9:00 - 13:00 implies the former is AM. 1:00 - 15:00, former PM.
            {
                startTimeHour += 12;
            }
        }

        var start = new TimeOnly(startTimeHour, startTimeMinute);
        var end = new TimeOnly(endTimeHour, endTimeMinute);

        return new TimeInterval(start, end);
    }

    private static Class GetCourse(string crn, IEnumerable<Class> courses)
    {
        return courses.First(o => o.Crn == crn);
        // return await _connection.QuerySingleOrDefaultAsync<Class>("SELECT * FROM [UCM].[v1api] WHERE crn = @Crn", new { Crn = crn });
    }

    private static Dictionary<string, Dictionary<string, Class>> GetSections(List<Class> courses)
    {
        // Honestly, I can't read their code, so I'll be writing it mostly verbatim.
        var sections = new Dictionary<string, Dictionary<string, Class>>();

        foreach (var course in courses)
        {
            if (course.Type != null && course.Type != "LECT")
            {
                var classId = course.CourseId?.Split("-")[2][..2];
                if (classId == null)
                    continue;

                if (!sections.ContainsKey(classId))
                {
                    sections[classId] = new Dictionary<string, Class>();
                }

                sections[classId][course.Type] = course;
            }
        }

        foreach (var section in sections.Values)
        {
            void CopyLectureCourse(string courseType)
            {
                if (section.ContainsKey(courseType))
                {
                    var discussion = section[courseType];
                    if (discussion.LectureCrn != null)
                    {
                        section["LECT"] = GetCourse(discussion.LectureCrn, courses);
                    }
                }
            }

            CopyLectureCourse("DISC");
            CopyLectureCourse("LAB");
        }

        if (sections.Count == 0)
        {
            foreach (var course in courses)
            {
                var classId = course.CourseId?.Split("-")[2][..2];
                if (classId == null)
                    continue;

                sections[classId] = new Dictionary<string, Class>
                {
                    { "LECT", course }
                };
            }
        }

        return sections;
    }

    private static Dictionary<string, Class> GetNthPermutation(Dictionary<string, Dictionary<string, Class>> classes, int i)
    {
        var permutation = new Dictionary<string, Class>();
        var n = 1;

        foreach (var pair in classes)
        {
            pair.Deconstruct(out var classId, out var data);
            var sections = data.ToList(); // horribly bad practice since dictionaries aren't ordered
            var section = sections[i / (n % sections.Count)];
            permutation[classId] = data[section.Key];
            n *= sections.Count;
        }

        return permutation;
    }

    private record struct TimeInterval(TimeOnly Start, TimeOnly End);

    private static bool DayConflicts(TimeInterval time, IEnumerable<TimeInterval> day, bool isEvent = false)
    {
        foreach (var t in day)
        {
            if (isEvent)
            {
                if (time.Start >= t.Start && time.Start < t.End)
                    return true;
                if (time.End > t.Start && time.End < t.End)
                    return true;
                if (t.Start >= time.Start && t.Start < time.End)
                    return true;
                if (t.End > time.Start && t.End < time.End)
                    return true;
            }
            else
            {
                if (time.Start >= t.Start && time.Start <= t.End)
                    return true;
                if (time.End >= t.Start && time.End <= t.End)
                    return true;
                if (t.Start >= time.Start && t.Start <= time.End)
                    return true;
                if (t.End >= time.Start && t.End <= time.End)
                    return true;
            }
        }

        return false;
    }

    private static bool HasConflict(Dictionary<string, Dictionary<string, Class>> schedule)
    {
        var times = new Dictionary<string, List<TimeInterval>>
        {
            { "M", new List<TimeInterval>() },
            { "T", new List<TimeInterval>() },
            { "W", new List<TimeInterval>() },
            { "R", new List<TimeInterval>() },
            { "F", new List<TimeInterval>() },
            { "S", new List<TimeInterval>() },
            { "U", new List<TimeInterval>() }
        };

        var finals = times.ToDictionary(o => o.Key, _ => new List<TimeInterval>());

        var allCourses = schedule.Values.SelectMany(o => o.Values);

        foreach (var course in allCourses)
        {
            // if (course.Hours == null && course.)
        }

        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Schedule>> GetValidSchedules(IEnumerable<string> coursesToSearch,
        IEnumerable<CustomEvent> customEvents)
    {
        var schedules = new List<Schedule>();

        var classes = new Dictionary<string, IEnumerable<Class>>();

        var sqlPredicates = new List<string>();
        var queryParams = new DynamicParameters();
        queryParams.Add("Term", _term, DbType.Int32);

        var i = 1;
        foreach (var course in coursesToSearch)
        {
            var name = $"P{i}";
            sqlPredicates.Add($"simple_name LIKE @{name}");
            queryParams.Add(name, course, DbType.String);
            ++i;
        }

        var courseData = await _connection.QueryAsync<Class>(
            "SELECT * FROM [UCM].[v1api] WHERE term = @Term"
        );

        throw new NotImplementedException();
    }
}