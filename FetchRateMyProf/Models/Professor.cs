using Newtonsoft.Json;

namespace FetchRateMyProf.Models
{
    public class Professor
    {
        [JsonConstructor]
        internal Professor(string department, string firstName, string middleName, string lastName, int id, int numRatings, string classRating, string overallRating)
        {
            Department = department;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            Id = id;
            NumRatings = numRatings;
            ClassRating = classRating;
            OverallRatingRaw = overallRating;
        }

        [JsonProperty("tDept")] public string Department { get; private set; }
        [JsonProperty("tFname")] public string FirstName { get; private set; }
        [JsonProperty("tMiddlename")] public string MiddleName { get; private set; }
        [JsonProperty("tLname")] public string LastName { get; private set; }
        [JsonProperty("tid")] public int Id { get; private set; }
        [JsonProperty("tNumRatings")] public int NumRatings { get; private set; }
        [JsonProperty("rating_class")] public string ClassRating { get; private set; }
        [JsonProperty("overall_rating")] public string OverallRatingRaw { get; private set; }
        public float OverallRating
        {
            get
            {
                var ratingSuccess = float.TryParse(OverallRatingRaw, out var rating);
                return ratingSuccess ? rating : 0.0f;
            }
        }

        public override string ToString()
        {
            return $"{FirstName} {MiddleName} {LastName}";
        }
    }
}