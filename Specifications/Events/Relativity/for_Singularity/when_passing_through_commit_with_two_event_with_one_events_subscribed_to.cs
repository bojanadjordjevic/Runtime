using System;
using System.Collections.Generic;
using Dolittle.Artifacts;
using Dolittle.PropertyBags;
using Machine.Specifications;
using Dolittle.Execution;
using Dolittle.Runtime.Events;
using Dolittle.Collections;
using specs = Dolittle.Runtime.Events.Specs.given;

namespace Dolittle.Runtime.Events.Relativity.for_Singularity
{
    // public class when_passing_through_commit_with_two_event_with_one_events_subscribed_to : given.a_singularity_with_a_tunnel_and_no_events_to_subscribe_to
    // {
    //     static Store.CommittedEventStream stream;
    //     static bool result;

    //     Establish context = () => 
    //     {
    //         var versionedEventSource = new VersionedEventSource(Guid.NewGuid(), ArtifactId.New());
    //         var correlationId = CorrelationId.New();
    //         var firstEventArtifact = Artifact.New();
    //         var secondEventArtifact = Artifact.New();
    //         stream = new Store.CommittedEventStream(
    //             0L,
    //             versionedEventSource,
    //             Store.CommitId.New(),
    //             correlationId,
    //             DateTimeOffset.UtcNow,
    //             new Store.EventStream(new[] {
    //                 new EventEnvelope(
    //                     new EventMetadata(EventId.New(),versionedEventSource,correlationId, firstEventArtifact, DateTimeOffset.UtcNow, specs.Events.an_original_context()),
    //                     new PropertyBag(new NullFreeDictionary<string, object>())
    //                 ),
    //                 new EventEnvelope(
    //                     new EventMetadata(EventId.New(),versionedEventSource,correlationId, secondEventArtifact, DateTimeOffset.UtcNow, specs.Events.an_original_context()),
    //                     new PropertyBag(new NullFreeDictionary<string, object>())
    //                 )
    //             })
    //         );

    //         singularity = new Singularity(
    //             application,
    //             bounded_context,
    //             tunnel.Object,
    //             new EventParticleSubscription(new[] { secondEventArtifact })
    //         );
    //     };

    //     Because of = () => result = singularity.PassThrough(stream);

    //     It should_pass_through_tunnel = () => tunnel.Verify(_ => _.PassThrough(stream), Moq.Times.Once());
    // }
}