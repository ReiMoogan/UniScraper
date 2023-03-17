using System.Linq;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using ScrapperCore.Models.V2.SQL;

namespace ScrapperCore.Controllers.V2;

public partial class Query
{
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [GraphQLDescription("Get all professors.")]
    public async Task<IQueryable<Professor>> GetProfessors
    (
        [Service] IDbContextFactory<UniScraperContext> factory,
        [Service] ITopicEventSender eventSender
    )
    {
        var ctx = await factory.CreateDbContextAsync();

        var professors = ctx.Professors;
        
        await eventSender.SendAsync("Professors", professors);
        return professors;
    }
    
    [UseProjection]
    [UseFiltering]
    [GraphQLDescription("Get a specific professor.")]
    public async Task<Professor?> GetProfessor(
        [Service] IDbContextFactory<UniScraperContext> factory,
        [Service] ITopicEventSender eventSender,
        string email
    )
    {
        var ctx = await factory.CreateDbContextAsync();

        var professor = await ctx.Professors
            .Include(o => o.Classes)
            .SingleOrDefaultAsync(o => o.Email == email);
        
        await eventSender.SendAsync("Professor", professor);
        return professor;
    }
    
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [GraphQLDescription("Search for professors by their name.")]
    public async Task<IQueryable<Professor>> GetProfessorsByName
    (
        [Service] IDbContextFactory<UniScraperContext> factory,
        [Service] ITopicEventSender eventSender,
        string name
    )
    {
        var ctx = await factory.CreateDbContextAsync();

        var query = CreateFullTextQuery(name);
        var professors = ctx.Professors
            .Include(o => o.Classes)
            .Where(o => EF.Functions.Contains(o.FullName, query));
        
        await eventSender.SendAsync("ProfessorsByName", professors);
        return professors;
    }
}