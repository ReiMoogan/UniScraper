﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using ScrapperCore.Models.V2.SQL;

namespace ScrapperCore.Controllers.V2;

public class Subscription
{
    [Subscribe(MessageType = typeof(string))]
    public async ValueTask<ISourceStream<IQueryable<Class>>> OnClassesGet
    (
        [Service] ITopicEventReceiver eventReceiver,
        CancellationToken cancellationToken
    )
    {
        return await eventReceiver.SubscribeAsync<IQueryable<Class>>("Classes", cancellationToken);
    }
    
    [Subscribe(MessageType = typeof(string))]
    public async ValueTask<ISourceStream<Class>> OnClassGet
    (
        [Service] ITopicEventReceiver eventReceiver,
        CancellationToken cancellationToken
    )
    {
        return await eventReceiver.SubscribeAsync<Class>("Class", cancellationToken);
    }
}