using System.Linq;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using ScrapperCore.Models.V2.SQL;

namespace ScrapperCore.Controllers.V2;

public class Query
{
    [UsePaging]
    public async Task<IQueryable<Class>> GetClasses
    (
        [Service] IClassRepository classRepository,
        [Service] ITopicEventSender eventSender,
        int term
    )
    {
        var classes = classRepository.GetClasses(term);
        await eventSender.SendAsync("Classes", classes);
        return classes;
    }
    
    public async Task<Class?> GetClass(
        [Service] IClassRepository classRepository,
        [Service] ITopicEventSender eventSender,
        int term, int crn
    )
    {
        var @class = classRepository.GetClass(term, crn);
        await eventSender.SendAsync("Class", @class);
        return @class;
    }
    
    [UsePaging]
    public async Task<IQueryable<Class>> GetClassesByClassNumber
    (
        [Service] IClassRepository classRepository,
        [Service] ITopicEventSender eventSender,
        string major,
        int number,
        int? term = null
    )
    {
        var classes = classRepository.GetClassesByClassNumber(major, number, term);
        await eventSender.SendAsync("ClassesByClassNumber", classes);
        return classes;
    }
    
    [UsePaging]
    public async Task<IQueryable<Class>> GetClassesByCourseName
    (
        [Service] IClassRepository classRepository,
        [Service] ITopicEventSender eventSender,
        string phrase,
        int term
    )
    {
        var classes = classRepository.GetClassesByCourseName(phrase, term);
        await eventSender.SendAsync("ClassesByCourseName", classes);
        return classes;
    }
}