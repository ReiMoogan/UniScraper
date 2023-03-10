using System.Collections.Generic;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Subscriptions;
using ScrapperCore.Models.V2.SQL;

namespace ScrapperCore.Controllers.V2;

public class Query
{
    public async Task<List<Class>> GetClasses
    (
        [Service] IClassRepository classRepository,
        [Service] ITopicEventSender eventSender
    )
    {
        var classes = classRepository.GetClasses();
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
}