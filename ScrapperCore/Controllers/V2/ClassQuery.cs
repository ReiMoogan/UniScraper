using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using ScrapperCore.Models.V2.SQL;

namespace ScrapperCore.Controllers.V2;

[GraphQLDescription("Welcome to the V2 API for UniScraper!")]
public partial class Query
{
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [GraphQLDescription("Get classes for a given term.")]
    public async Task<IQueryable<Class>> GetClasses
    (
        [Service] IDbContextFactory<UniScraperContext> factory,
        [Service] ITopicEventSender eventSender,
        int term
    )
    {
        var ctx = await factory.CreateDbContextAsync();

        var classes = ctx.Classes
            .Include(o => o.Meetings)
            .Include(o => o.LinkedSections)
            .Include(o => o.Faculty)
            .ThenInclude(o => o.Professor)
            .Where(o => o.Term == term);
        
        await eventSender.SendAsync("Classes", classes);
        return classes;
    }
    
    [UseProjection]
    [UseFiltering]
    [GraphQLDescription("Get a specific class in a term.")]
    public async Task<Class?> GetClass(
        [Service] IDbContextFactory<UniScraperContext> factory,
        [Service] ITopicEventSender eventSender,
        int term, int crn
    )
    {
        var ctx = await factory.CreateDbContextAsync();

        var @class = ctx.Classes
            .Include(o => o.Meetings)
            .Include(o => o.LinkedSections)
            .Include(o => o.Faculty)
            .ThenInclude(o => o.Professor)
            .SingleOrDefault(o => o.Term == term && o.CourseReferenceNumber == crn);
        
        await eventSender.SendAsync("Class", @class);
        return @class;
    }
    
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [GraphQLDescription("Get all classes for a course.")]
    public async Task<IQueryable<Class>> GetClassesByCourseNumber
    (
        [Service] IDbContextFactory<UniScraperContext> factory,
        [Service] ITopicEventSender eventSender,
        string major,
        int number,
        int? term = null
    )
    {
        var ctx = await factory.CreateDbContextAsync();
        
        var courseNumber = $"{major}-{number}";
        var classes = ctx.Classes
            .Include(o => o.Meetings)
            .Include(o => o.LinkedSections)
            .Include(o => o.Faculty)
            .ThenInclude(o => o.Professor)
            .Where(o => o.CourseNumber.StartsWith(courseNumber));

        await eventSender.SendAsync("ClassesByCourseNumber", classes);
        return classes;
    }
    
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [GraphQLDescription("Search for classes by their course name.")]
    public async Task<IQueryable<Class>> GetClassesByCourseName
    (
        [Service] IDbContextFactory<UniScraperContext> factory,
        [Service] ITopicEventSender eventSender,
        string phrase,
        int term
    )
    {
        var ctx = await factory.CreateDbContextAsync();

        var query = CreateFullTextQuery(phrase);
        var classes = ctx.Classes
            .Include(o => o.Meetings)
            .Include(o => o.LinkedSections)
            .Include(o => o.Faculty)
            .ThenInclude(o => o.Professor)
            .Where(o => o.Term == term && o.CourseTitle != null && EF.Functions.Contains(o.CourseTitle, query));
        
        await eventSender.SendAsync("ClassesByCourseName", classes);
        return classes;
    }
    
    [GeneratedRegex("[()\"']", RegexOptions.Multiline)]
    private static partial Regex FullTextRegex();
    
    private static string CreateFullTextQuery(string query)
    {
        var queryTerms = query
            .Trim()
            .Split()
            .Select(o => FullTextRegex().Replace(o, ""))
            .Select(o => $"\"*{o}*\"");

        return string.Join(" AND ", queryTerms);
    }
}