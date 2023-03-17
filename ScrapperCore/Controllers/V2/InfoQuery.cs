using System.Linq;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Subscriptions;
using Microsoft.EntityFrameworkCore;
using ScrapperCore.Models.V2.GraphQL;
using ScrapperCore.Models.V2.SQL;
using MeetingType = ScrapperCore.Models.V2.SQL.MeetingType;

namespace ScrapperCore.Controllers.V2;

public partial class Query
{
    [GraphQLDescription("Get update times from when the scraper last ran.")]
    public async Task<IQueryable<Stat>> GetLastUpdate
    (
        [Service] IDbContextFactory<UniScraperContext> factory,
        [Service] ITopicEventSender eventSender
    )
    {
        var ctx = await factory.CreateDbContextAsync();

        var stats = ctx.Stats;
        
        await eventSender.SendAsync("Stats", stats);
        return stats;
    }
    
    [GraphQLDescription("Get update times from when the scraper last ran.")]
    public async Task<DBStat> GetStats
    (
        [Service] IDbContextFactory<UniScraperContext> factory,
        [Service] ITopicEventSender eventSender
    )
    {
        var ctx = await factory.CreateDbContextAsync();

        var totalClasses = await ctx.Classes.CountAsync();
        var totalProfessors = await ctx.Professors.CountAsync();
        var totalMeetings = await ctx.Meetings.CountAsync();

        var output = new DBStat(totalClasses, totalProfessors, totalMeetings);
        
        await eventSender.SendAsync("Stats", output);
        return output;
    }
    
    [GraphQLDescription("Get the list of mappable subjects from the database.")]
    public async Task<IQueryable<Subject>> GetSubjects
    (
        [Service] IDbContextFactory<UniScraperContext> factory,
        [Service] ITopicEventSender eventSender
    )
    {
        var ctx = await factory.CreateDbContextAsync();

        var subjects = ctx.Subjects;
        
        await eventSender.SendAsync("Subjects", subjects);
        return subjects;
    }
    
    [GraphQLDescription("Get all available terms in the class database.")]
    public async Task<IQueryable<int>> GetTerms
    (
        [Service] IDbContextFactory<UniScraperContext> factory,
        [Service] ITopicEventSender eventSender
    )
    {
        var ctx = await factory.CreateDbContextAsync();

        var stats = ctx.Classes.Select(o => o.Term).Distinct();
        
        await eventSender.SendAsync("Terms", stats);
        return stats;
    }
    
    [GraphQLDescription("Get all meeting types in the class database.")]
    public async Task<IQueryable<MeetingType>> GetMeetingTypes
    (
        [Service] IDbContextFactory<UniScraperContext> factory,
        [Service] ITopicEventSender eventSender
    )
    {
        var ctx = await factory.CreateDbContextAsync();

        var stats = ctx.MeetingTypes;
        
        await eventSender.SendAsync("MeetingTypes", stats);
        return stats;
    }
}