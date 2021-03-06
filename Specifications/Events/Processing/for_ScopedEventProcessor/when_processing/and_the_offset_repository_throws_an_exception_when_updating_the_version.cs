﻿// namespace Dolittle.Runtime.Processing.for_ScopedEventProcessor.when_processing
// {
//     using Dolittle.Runtime.Events.Processing;
//     using Dolittle.Runtime.Events.Store;
//     using Machine.Specifications;
//     using Moq;
//     using It = Machine.Specifications.It;
//     using specs = Dolittle.Runtime.Events.Specs.given;
//     using System.Linq;
//     using System;
//     using Dolittle.Runtime.Events.Specs.Processing;

//     [Subject(typeof(ScopedEventProcessor),"Process")]
//     public class and_the_offset_repository_throws_an_exception_when_updating_the_version : scoped_event_processors
//     {
//         static CommittedEventVersion original_version = new CommittedEventVersion(1,1,0);
//         static CommittedEventEnvelope event_to_process;

//         static Exception exception;

//         Establish context = () => 
//         {
//             var event_artifact = specs.Artifacts.artifact_for_simple_event;
//             var first_commit = specs.Events.Build(original_version);
//             event_to_process = first_commit.Events.Where(e => e.Metadata.Artifact == event_artifact).Select(e => e.ToCommittedEventEnvelope(first_commit.Sequence)).Last();
//             offset_repository_simple_tenant.Setup(r => r.Get(simple_scoped_processor.ProcessorId)).Returns(original_version);
//             offset_repository_simple_tenant.Setup(r => r.Set(simple_scoped_processor.ProcessorId,event_to_process.Version)).Throws(new Exception());
//             simple_scoped_processor.CatchUp();
//         };
//         Because of = () => exception = Catch.Exception(() => simple_scoped_processor.Process(event_to_process));

//         It should_not_let_the_exception_bubble_up = () => exception.ShouldBeNull();
//         It should_have_the_version_of_the_event_processed = () => simple_scoped_processor.LastVersionProcessed.ShouldEqual(event_to_process.Version);
//         It should_set_the_offset_with_the_repository_for_the_event_it_is_processing = () => offset_repository_simple_tenant.Verify(r => r.Set(simple_scoped_processor.ProcessorId,event_to_process.Version),Times.Once());
//         It should_pass_the_event_to_the_actual_event_processor_for_processing = () => simple_event_processor.Verify(p => p.Process(event_to_process),Times.Once());
//     }    
// }