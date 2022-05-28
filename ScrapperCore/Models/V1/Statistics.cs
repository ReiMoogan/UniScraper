using System;

namespace ScrapperCore.Models.V1;

public class Statistics
{
    // For backwards compatibility with https://cse120-course-planner.herokuapp.com/api/statistics/
    public int TotalSchedulesGenerated { get; init; }
    public int TotalUsers { get; init; }
    public int TotalSavedSchedules { get; init; }
    public int TotalWaitlists { get; init; }
    
    // Actual data
    public int TotalClasses { get; init; }
    public int TotalProfessors { get; init; }
    public int TotalMeetings { get; init; }
    public DateTime LastUpdate { get; init; }
}