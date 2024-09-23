using EventuzScheduler.Application.Enums;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EventuzScheduler.Services.Scheduler.Quartz
{
    public class QuartzJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            // Получаем делегат из JobDataMap
            var dataMap = context.JobDetail.JobDataMap;
            var jobName = context.JobDetail.Key.Name;

            var type = (SchedulerTaskType)Enum.Parse(typeof(SchedulerTaskType), context.JobDetail.JobDataMap.GetString("Type"));
            var action = (Action)dataMap["action"];

            Console.WriteLine($"Starting to execute job: {jobName}. Type: {type.GetStringName()}");

            Thread.Sleep(1000);
            // Выполняем делегат
            action();

            Console.WriteLine($"Executed job: {jobName}. Type: {type.GetStringName()}");

            return Task.CompletedTask;
        }
    }
}
