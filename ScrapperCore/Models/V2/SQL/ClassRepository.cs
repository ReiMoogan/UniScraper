using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ScrapperCore.Models.V2.SQL;

public interface IClassRepository
{
    public List<Class> GetClasses();
    public Class? GetClass(int term, int crn);
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
    
    public List<Class> GetClasses()
    {
        using var ctx = _db.CreateDbContext();
        return ctx.Classes.ToList();
    }

    public Class? GetClass(int term, int crn)
    {
        using var ctx = _db.CreateDbContext();
        return ctx.Classes.SingleOrDefault(o => o.Term == term && o.CourseReferenceNumber == crn);
    }
}