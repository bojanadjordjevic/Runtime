﻿using System;
using Dolittle.Tasks;
using Machine.Specifications;

namespace Dolittle.Specs.Tasks.for_TaskManager
{
    public class when_stopping : given.a_task_manager_with_one_reporter
    {
        static TaskId task_id = Guid.NewGuid();
        static OurTask task;

        Establish context = () => {
            task = new OurTask
            {
                Id = task_id,
            };
            task_repository.Setup(t=>t.Load(task_id)).Returns(task);
        };

        Because of = () => task_manager.Stop(task_id);

        It should_call_end_on_the_task = () => task.EndCalled.ShouldBeTrue();
        It should_delete_the_task = () => task_repository.Verify(t => t.Delete(task), Moq.Times.Once());
        It should_call_the_status_reporter = () => task_status_reporter.Verify(t => t.Stopped(task), Moq.Times.Once());
    }
}
