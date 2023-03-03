using FetchRateMyProf.Models;

namespace FetchRateMyProf;

public static class RateMyProfessor
{
    /// <summary>
    /// Get the <see cref="School"/> object associated with the RateMyProfessors ID.
    /// </summary>
    /// <param name="schoolId">The ID used on RateMyProfessors</param>
    /// <returns>A <see cref="School"/> object.</returns>
    public static School GetSchool(string schoolId) => new(schoolId);
}