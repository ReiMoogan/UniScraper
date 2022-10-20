using System.Collections.Generic;
using ScrapperCore.Models.V1;

namespace ScrapperCore.Controllers.V1.API.CoursesAPI;

// Implemented from https://github.com/dragonbone81/bobcat-courses-backend/blob/master/course_api/data_managers/uc_merced/course_scheduler.py
// Honestly, I am not sure I like this design, but screw it.
public class CourseScheduler
{
    private int _term;
    private string? _earliestTime;
    private string? _latestTime;
    private IEnumerable<string> _badCRNs;
    private SortOrder _gaps;
    private SortOrder _days;
    private bool _searchFull;

    public CourseScheduler(int term, string? earliestTime, string? latestTime, IEnumerable<string> badCRNs, SortOrder gaps = SortOrder.Ascending,
        SortOrder days = SortOrder.Ascending, bool searchFull = false)
    {
        _term = term;
        _earliestTime = earliestTime;
        _latestTime = latestTime;
        _badCRNs = badCRNs;
        _gaps = gaps;
        _days = days;
        _searchFull = searchFull;
    }
}