using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using FetchRateMyProf;
using FetchUCM;
using ScrapperCore.Utilities;

namespace StandaloneTester
{
    internal static class Program
    {
        private static async Task Main()
        {
            Console.WriteLine("Please put a cookie with session data.");
            var jsessionid = Console.ReadLine();
            var config = ScrapperConfig.Load().Verify();
            await using var connection = new SqlConnection(config.SqlConnection);
            Console.WriteLine("Opening database connection...");
            await connection.OpenAsync();
            await UpdateClasses(connection, jsessionid);
            await UpdateProfessors(connection);
            Console.WriteLine("Closing database connection...");
            await connection.CloseAsync();
            Console.WriteLine("Finished!");
        }

        private static async Task UpdateProfessors(IDbConnection connection)
        {
            Console.WriteLine("Updating professors...");
            const int ucmsid = 4767;
            var uni = RateMyProfessor.GetSchool(ucmsid);
            var count = 0;
            await foreach (var professor in uni.GetAllProfessors())
            {
                var result = await connection.ExecuteAsync(
                   "UPDATE UniScraper.UCM.professor SET rmp_id = @RMPID, middle_name = @MiddleName, department = @Department, num_ratings = @NumRatings, rating = @OverallRating WHERE REPLACE(REPLACE(first_name, ' ', ''), '-', '') LIKE REPLACE(REPLACE(@FirstName, ' ', ''), '-', '') AND REPLACE(REPLACE(last_name, ' ', ''), '-', '') LIKE REPLACE(REPLACE(@LastName, ' ', ''), '-', '');",
                    new { RMPID = professor.Id, FirstName = $"%{professor.FirstName}%", LastName = $"%{professor.LastName}%", professor.MiddleName, professor.Department, professor.NumRatings, professor.OverallRating });
                if (result == 0)
                    Console.WriteLine($"Could not find {professor.FirstName}, {professor.MiddleName}, {professor.LastName} in the database...");
                else
                    ++count;
            }

            await connection.ExecuteAsync(
                "UPDATE UniScraper.UCM.stats SET last_update = SYSDATETIME() WHERE table_name = 'professor';");
            Console.WriteLine($"Updated {count} professors!");
        }
        
        private static async Task UpdateClasses(IDbConnection connection, string cookie)
        {
            Console.WriteLine("Updating classes...");
            var catalog = new UCMCatalog();
            var term = (await catalog.GetAllTerms())[0].Code;
            Console.WriteLine($"Reading from term {term}");
            var countClasses = 0;
            var countProfessors = 0;
            var countMeetings = 0;

            await foreach (var item in catalog.GetAllClasses(term, cookie))
            {
                await connection.ExecuteAsync(
                    "MERGE INTO UniScraper.UCM.class WITH (HOLDLOCK) AS Target " +
                    "USING (Values(@Term, @CourseReferenceNumber, @CourseNumber, @CampusDescription, @CourseTitle, @CreditHours, @MaximumEnrollment, @Enrollment, @SeatsAvailable, @WaitCapacity, @WaitAvailable)) AS SOURCE(term, course_reference_number, course_number, campus_description, course_title, credit_hours, maximum_enrollment, enrollment, seats_available, wait_capacity, wait_available) " +
                    "ON Target.id = @Id WHEN MATCHED THEN " +
                    "UPDATE SET term = SOURCE.term, course_reference_number = SOURCE.course_reference_Number, course_number = SOURCE.course_number, campus_description = SOURCE.campus_description, course_title = SOURCE.course_title, credit_hours = SOURCE.credit_hours, maximum_enrollment = SOURCE.maximum_enrollment, enrollment = SOURCE.enrollment, seats_available = SOURCE.seats_available, wait_capacity = SOURCE.wait_capacity, wait_available = SOURCE.wait_available " +
                    "WHEN NOT MATCHED THEN " +
                    "INSERT (id, term, course_reference_number, course_number, campus_description, course_title, credit_hours, maximum_enrollment, enrollment, seats_available, wait_capacity, wait_available) VALUES (@Id, @Term, @CourseReferenceNumber, @CourseNumber, @CampusDescription, @CourseTitle, @CreditHours, @MaximumEnrollment, @Enrollment, @SeatsAvailable, @WaitCapacity, @WaitAvailable);",
                    new { item.Id, item.Term, item.CourseReferenceNumber, item.CourseNumber, item.CampusDescription, item.CourseTitle, item.CreditHours, item.MaximumEnrollment, item.Enrollment, item.SeatsAvailable, item.WaitCapacity, item.WaitAvailable });
                
                foreach (var professor in item.Faculty)
                {
                    var nameSplit = professor.DisplayName.Split(',');
                    var firstName = nameSplit[^1].Trim();
                    var lastName = nameSplit[0].Trim();
                    await connection.ExecuteAsync(
                        "MERGE INTO UniScraper.UCM.professor WITH (HOLDLOCK) AS Target " +
                        "USING (Values(@LastName, @FirstName, @Email)) AS SOURCE(last_name, first_name, email) " +
                        "ON Target.id = @Id WHEN MATCHED THEN " +
                        "UPDATE SET last_name = SOURCE.last_name, first_name = SOURCE.first_name, email = SOURCE.email " +
                        "WHEN NOT MATCHED THEN " +
                        "INSERT (id, last_name, first_name, email) VALUES (@Id, @LastName, @FirstName, @Email);",
                        new { Id = professor.BannerId, FirstName = firstName, LastName = lastName, professor.Email });

                    await connection.ExecuteAsync(
                        "MERGE INTO UniScraper.UCM.faculty WITH (HOLDLOCK) AS Target " +
                        "USING (VALUES(@ClassId, @ProfessorId)) AS SOURCE(class_id, professor_id) " +
                        "ON Target.class_id = @ClassId AND Target.professor_id = @ProfessorId " +
                        "WHEN NOT MATCHED THEN INSERT (class_id, professor_id) VALUES (@ClassId, @ProfessorId);",
                        new { ClassId = item.Id, ProfessorId = professor.BannerId });
                    
                    ++countProfessors;
                }

                foreach (var meeting in item.MeetingsFaculty)
                {
                    var time = meeting.Time;
                    await connection.ExecuteAsync(
                        "MERGE INTO UniScraper.UCM.meeting WITH (HOLDLOCK) AS Target " +
                        "USING (Values(@BeginTime, @EndTime, @BeginDate, @EndDate, @Building, @BuildingDescription, @Campus, @CampusDescription, @Room, @CreditHourSession, @HoursPerWeek, @InSession, @Type)) AS SOURCE(begin_time, end_time, begin_date, end_date, building, building_description, campus, campus_description, room, credit_hour_session, hours_per_week, in_session, meeting_type) " +
                        "ON Target.class_id = @Id AND Target.meeting_type = @Type WHEN MATCHED THEN " +
                        "UPDATE SET begin_time = SOURCE.begin_time, end_time = SOURCE.end_time, begin_date = SOURCE.begin_date, end_date = SOURCE.end_date, building = SOURCE.building, building_description = SOURCE.building_description, campus = SOURCE.campus, campus_description = SOURCE.campus_description, room = SOURCE.room, credit_hour_session = SOURCE.credit_hour_session, hours_per_week = SOURCE.hours_per_week, in_session = SOURCE.in_session, meeting_type = SOURCE.meeting_type " +
                        "WHEN NOT MATCHED THEN " +
                        "INSERT (class_id, begin_time, end_time, begin_date, end_date, building, building_description, campus, campus_description, room, credit_hour_session, hours_per_week, in_session, meeting_type) VALUES (@Id, @BeginTime, @EndTime, @BeginDate, @EndDate, @Building, @BuildingDescription, @Campus, @CampusDescription, @Room, @CreditHourSession, @HoursPerWeek, @InSession, @Type);",
                        new { item.Id, time.BeginTime, time.EndTime, time.BeginDate, time.EndDate, time.Building, 
                            time.BuildingDescription, time.Campus, time.CampusDescription, time.Room, time.CreditHourSession,
                            time.HoursPerWeek, time.InSession, time.Type });
                    ++countMeetings;
                }
                
                ++countClasses;
            }

            await connection.ExecuteAsync(
                "UPDATE UniScraper.UCM.stats SET last_update = SYSDATETIME() WHERE table_name = 'class';");
            Console.WriteLine($"Updated {countClasses} classes, {countProfessors} professors, and {countMeetings} meetings!");
        }
    }
}