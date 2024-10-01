using EventuzScheduler.Application.Interfaces;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventuzScheduler.Services.Scheduler.Quartz
{
    public class QuartzJobWrapper : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            string taskType = context.JobDetail.JobDataMap.GetString("taskType");
            Type type = Type.GetType(taskType);
            IScheduledTask task = (IScheduledTask)Activator.CreateInstance(type);
            task.Execute();
            return Task.CompletedTask;
        }
    }
}
