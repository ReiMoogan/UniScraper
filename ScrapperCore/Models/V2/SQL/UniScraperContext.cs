using Microsoft.EntityFrameworkCore;

namespace ScrapperCore.Models.V2.SQL;

public class UniScraperContext : DbContext
{
    public UniScraperContext(DbContextOptions<UniScraperContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Class> Classes { get; set; } = null!;

    public virtual DbSet<Description> Descriptions { get; set; } = null!;

    public virtual DbSet<Faculty> Faculties { get; set; } = null!;

    public virtual DbSet<LinkedSection> LinkedSections { get; set; } = null!;

    public virtual DbSet<Meeting> Meetings { get; set; } = null!;

    public virtual DbSet<MeetingType> MeetingTypes { get; set; } = null!;

    public virtual DbSet<Professor> Professors { get; set; } = null!;

    public virtual DbSet<Reminder> Reminders { get; set; } = null!;

    public virtual DbSet<Stat> Stats { get; set; } = null!;

    public virtual DbSet<Subject> Subjects { get; set; } = null!;

    public virtual DbSet<V1api> V1apis { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Class>(entity =>
        {
            entity.ToTable("class", "UCM");

            entity.HasIndex(e => new { e.Term, e.CourseReferenceNumber }, "IX_crn_term").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasComputedColumnSql("([term]*(10000)+[course_reference_number])", true)
                .HasColumnName("id");
            entity.Property(e => e.CampusDescription)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("campus_description");
            entity.Property(e => e.CourseNumber)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("course_number");
            entity.Property(e => e.CourseReferenceNumber).HasColumnName("course_reference_number");
            entity.Property(e => e.CourseTitle)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("course_title");
            entity.Property(e => e.CreditHours).HasColumnName("credit_hours");
            entity.Property(e => e.Enrollment).HasColumnName("enrollment");
            entity.Property(e => e.MaximumEnrollment).HasColumnName("maximum_enrollment");
            entity.Property(e => e.SeatsAvailable).HasColumnName("seats_available");
            entity.Property(e => e.Term).HasColumnName("term");
            entity.Property(e => e.WaitAvailable)
                .HasDefaultValueSql("((0))")
                .HasColumnName("wait_available");
            entity.Property(e => e.WaitCapacity)
                .HasDefaultValueSql("((0))")
                .HasColumnName("wait_capacity");
        });

        modelBuilder.Entity<Description>(entity =>
        {
            entity.ToTable("description", "UCM");

            entity.HasIndex(e => e.CourseNumber, "IX_description").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CourseDescription)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("course_description");
            entity.Property(e => e.CourseNumber)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("course_number");
            entity.Property(e => e.Department)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("department");
            entity.Property(e => e.DepartmentCode)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("department_code");
            entity.Property(e => e.TermEnd).HasColumnName("term_end");
            entity.Property(e => e.TermStart).HasColumnName("term_start");
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.ToTable("faculty", "UCM");

            entity.HasIndex(e => new { e.ClassId, e.ProfessorEmail }, "class_professor_unique")
                .IsUnique()
                .IsClustered();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.ProfessorEmail)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("professor_email");

            /*
            entity.HasOne(d => d.Class).WithMany(o => o.Faculty)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_faculty_class");*/

            entity.HasOne(d => d.Professor).WithMany(o => o.Classes)
                .HasForeignKey(d => d.ProfessorEmail)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_faculty_professor");

            entity.Navigation(o => o.Professor).AutoInclude();
        });

        modelBuilder.Entity<LinkedSection>(entity =>
        {
            entity.ToTable("linked_section", "UCM");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Child).HasColumnName("child");
            entity.Property(e => e.Parent).HasColumnName("parent");

            entity.HasOne(d => d.ChildNavigation).WithMany(o => o.LinkedSections)
                .HasForeignKey(d => d.Child)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_linked_section_class1");

            // Don't implement o => o.LinkedSections for backwards navigation, EF doesn't like that
            entity.HasOne(d => d.ParentNavigation).WithMany()
                .HasForeignKey(d => d.Parent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_linked_section_class");
        });

        modelBuilder.Entity<Meeting>(entity =>
        {
            entity.ToTable("meeting", "UCM");

            entity.HasIndex(e => new { e.ClassId, e.MeetingType }, "IX_class").IsUnique();

            entity.HasIndex(e => e.ClassId, "meeting_class_id_index");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BeginDate)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('00/00/0000')")
                .HasColumnName("begin_date");
            entity.Property(e => e.BeginTime)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasDefaultValueSql("('0000')")
                .HasColumnName("begin_time");
            entity.Property(e => e.Building)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("building");
            entity.Property(e => e.BuildingDescription)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("building_description");
            entity.Property(e => e.Campus)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("campus");
            entity.Property(e => e.CampusDescription)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("campus_description");
            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.CreditHourSession).HasColumnName("credit_hour_session");
            entity.Property(e => e.EndDate)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('00/00/0000')")
                .HasColumnName("end_date");
            entity.Property(e => e.EndTime)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasDefaultValueSql("('0000')")
                .HasColumnName("end_time");
            entity.Property(e => e.HoursPerWeek).HasColumnName("hours_per_week");
            entity.Property(e => e.InSession).HasColumnName("in_session");
            entity.Property(e => e.MeetingType)
                .HasDefaultValueSql("((1))")
                .HasColumnName("meeting_type");
            entity.Property(e => e.Room)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("room");

            entity.HasOne(d => d.Class).WithMany(o => o.Meetings)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_meeting_class");
        });

        modelBuilder.Entity<MeetingType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("meeting_type_pk");

            entity.ToTable("meeting_type", "UCM");

            entity.HasIndex(e => e.Id, "meeting_type_id_uindex").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Type)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("type");
        });

        modelBuilder.Entity<Professor>(entity =>
        {
            entity.HasKey(e => e.Email).HasName("PK_professor").IsClustered(false);

            entity.ToTable("professor", "UCM");

            entity.HasIndex(e => new { e.FirstName, e.LastName }, "IX_professor");

            entity.HasIndex(e => e.FullName, "IX_professor_1");

            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Department)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("department");
            entity.Property(e => e.Difficulty).HasColumnName("difficulty");
            entity.Property(e => e.FirstName)
                .HasMaxLength(64)
                .HasColumnName("first_name");
            entity.Property(e => e.FullName)
                .HasMaxLength(129)
                .HasComputedColumnSql("(([first_name]+' ')+[last_name])", true)
                .HasColumnName("full_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(64)
                .HasColumnName("last_name");
            entity.Property(e => e.NumRatings).HasColumnName("num_ratings");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.RmpId)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("rmp_id");
            entity.Property(e => e.WouldTakeAgainPercent).HasColumnName("would_take_again_percent");
        });

        modelBuilder.Entity<Reminder>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("reminder", "UCM");

            entity.HasIndex(e => new { e.UserId, e.ClassId }, "user_crn_unique")
                .IsUnique()
                .IsClustered();

            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.ForWaitlist)
                .HasDefaultValueSql("((0))")
                .HasColumnName("for_waitlist");
            entity.Property(e => e.MinTrigger)
                .HasDefaultValueSql("((1))")
                .HasColumnName("min_trigger");
            entity.Property(e => e.Triggered)
                .HasDefaultValueSql("((0))")
                .HasColumnName("triggered");
            entity.Property(e => e.UserId)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("user_id");

            entity.HasOne(d => d.Class).WithMany()
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_reminder_class");
        });

        modelBuilder.Entity<Stat>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("stats", "UCM");

            entity.HasIndex(e => e.TableName, "stats_table_name_uindex").IsUnique();

            entity.Property(e => e.LastUpdate)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("last_update");
            entity.Property(e => e.TableName)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("table_name");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Subject1).HasName("subject_pk");

            entity.ToTable("subject", "UCM");

            entity.HasIndex(e => e.Subject1, "subject_subject_uindex").IsUnique();

            entity.Property(e => e.Subject1)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("subject");
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<V1api>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("v1api", "UCM");

            entity.Property(e => e.AttachedCrn)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("attached_crn");
            entity.Property(e => e.Available).HasColumnName("available");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.CourseId)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("course_id");
            entity.Property(e => e.CourseName)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("course_name");
            entity.Property(e => e.Crn).HasColumnName("crn");
            entity.Property(e => e.Dates)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("dates");
            entity.Property(e => e.Days)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("days");
            entity.Property(e => e.Enrolled).HasColumnName("enrolled");
            entity.Property(e => e.FinalDates)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("final_dates");
            entity.Property(e => e.FinalDays)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("final_days");
            entity.Property(e => e.FinalHours)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("final_hours");
            entity.Property(e => e.FinalRoom)
                .HasMaxLength(33)
                .IsUnicode(false)
                .HasColumnName("final_room");
            entity.Property(e => e.FinalType)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("final_type");
            entity.Property(e => e.Hours)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("hours");
            entity.Property(e => e.Instructor)
                .HasMaxLength(4000)
                .HasColumnName("instructor");
            entity.Property(e => e.LectureCrn)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("lecture_crn");
            entity.Property(e => e.LinkedCourses).HasColumnName("linked_courses");
            entity.Property(e => e.Room)
                .HasMaxLength(33)
                .IsUnicode(false)
                .HasColumnName("room");
            entity.Property(e => e.SimpleName)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("simple_name");
            entity.Property(e => e.Subject)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("subject");
            entity.Property(e => e.Term).HasColumnName("term");
            entity.Property(e => e.Type)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("type");
            entity.Property(e => e.Units).HasColumnName("units");
        });
    }
}
