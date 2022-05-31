namespace ScrapperCore.Models.V1;

public class Class
{
    public string Crn { get; init; }
    public string Subject { get; init; }
    public string CourseId { get; init; }
    public string CourseName { get; init; }
    public int Units { get; init; }
    public string Type { get; init; }
    public string Days { get; init; }
    public string Hours { get; init; }
    public string Room { get; init; }
    public string Dates { get; init; }
    public string Instructor { get; init; }
    public string LectureCrn { get; init; }
    public string AttachedCrn { get; init; }
    public string Term { get; init; }
    public int Capacity { get; init; }
    public int Enrolled { get; init; }
    public int Available { get; init; }
    public string FinalType { get; init; }
    public string FinalDays { get; init; }
    public string FinalHours { get; init; }
    public string FinalRoom { get; init; }
    public string FinalDates { get; init; }
    public string SimpleName { get; init; }
    public string LinkedCourses { get; init; }
    // Apparently this is equivalent to LectureCrn... WHY????
    public string Lecture => LectureCrn;
    // Honestly have no idea what this is supposed to represent, they're all NULL as of 202230 on the old courses API.
    public string Discussion { get; init; }
}