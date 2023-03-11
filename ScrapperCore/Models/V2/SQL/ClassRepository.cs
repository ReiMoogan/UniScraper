using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ScrapperCore.Models.V2.SQL;

public interface IClassRepository
{
    public IQueryable<Class> GetClasses(int term);
    public Class? GetClass(int term, int crn);
    public IQueryable<Class> GetClassesByClassNumber(string major, int number, int? term = null);
    public IQueryable<Class> GetClassesByCourseName(string phrase, int term);
}

public class ClassRepository : IClassRepository
{
    private readonly IDbContextFactory<UniScraperContext> _db;
    
    public ClassRepository(IDbContextFactory<UniScraperContext> db)
    {
        _db = db;
        
        // using var ctx = _db.CreateDbContext();
        // ctx.Database.EnsureCreated();
    }
    
    public IQueryable<Class> GetClasses(int term)
    {
        using var ctx = _db.CreateDbContext();
        return ctx.Classes.Where(o => o.Term == term);
    }

    public Class? GetClass(int term, int crn)
    {
        using var ctx = _db.CreateDbContext();
        return ctx.Classes.SingleOrDefault(o => o.Term == term && o.CourseReferenceNumber == crn);
    }
    
    public IQueryable<Class> GetClassesByClassNumber(string major, int number, int? term = null)
    {
        using var ctx = _db.CreateDbContext();
        var mukyu = $"{major}-{number}";
        return ctx.Classes.Where(o => o.CourseNumber.StartsWith(mukyu));
    }
    
    public IQueryable<Class> GetClassesByCourseName(string phrase, int term)
    {
        using var ctx = _db.CreateDbContext();
        return ctx.Classes.Where(o =>
            o.Term == term && o.CourseTitle != null && EF.Functions.Contains(o.CourseTitle, phrase));
    }
}