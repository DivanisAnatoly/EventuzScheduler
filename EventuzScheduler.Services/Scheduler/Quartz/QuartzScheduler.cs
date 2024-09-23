using EventuzScheduler.Application.DTOs;
using EventuzScheduler.Application.Enums;
using EventuzScheduler.Application.Interfaces;
using Quartz;
using Quartz.Impl.Matchers;

namespace EventuzScheduler.Services.Scheduler.Quartz
{
    public class QuartzScheduler : ICustomScheduler
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IScheduler _scheduler;

        public QuartzScheduler(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
            _scheduler = _schedulerFactory.GetScheduler().Result;
            _scheduler.Start().Wait();
        }

        public async Task<List<SchedulerTaskInfo>> GetTasksAsync()
        {
            var jobKeys = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
            var tasks = new List<SchedulerTaskInfo>();

            foreach (var jobKey in jobKeys)
            {
                var jobDetail = await _scheduler.GetJobDetail(jobKey);
                var triggers = await _scheduler.GetTriggersOfJob(jobKey);
                var trigger = triggers.FirstOrDefault();

                var trigerState = await _scheduler.GetTriggerState(trigger.Key);

                SchedulerTaskStatus taskStatus = SchedulerTaskStatus.Default;
                switch (trigerState)
                {
                    case TriggerState.Normal:
                        taskStatus = SchedulerTaskStatus.Running; break;
                    case TriggerState.Paused:
                        taskStatus = SchedulerTaskStatus.Paused; break;
                    case TriggerState.Complete:
                    case TriggerState.Error:
                    case TriggerState.Blocked:
                        throw new Exception("Unsupported TriggerState");
                    case TriggerState.None:
                        taskStatus = SchedulerTaskStatus.Deleted; break;
                }


                if (trigger != null)
                {
                    tasks.Add(new SchedulerTaskInfo
                    {
                        Guid = jobKey.Name,
                        Type = (SchedulerTaskType)Enum.Parse(typeof(SchedulerTaskType), jobDetail.JobDataMap.GetString("Type")),
                        Status = taskStatus,
                        //StatusStr = trigerState.ToString(),
                        CRON = (trigger as ICronTrigger)?.CronExpressionString,
                        LastRun = trigger.GetPreviousFireTimeUtc()?.DateTime ?? DateTime.MinValue
                    });
                }
            }

            return tasks;
        }

        public async Task<bool> EditTaskCronAsync(string taskGuid, string newCron)
        {
            var triggerKey = new TriggerKey(taskGuid);
            var newTrigger = TriggerBuilder.Create()
                .WithIdentity(taskGuid)
                .WithCronSchedule(newCron)
                .Build();

            await _scheduler.RescheduleJob(triggerKey, newTrigger);
            return true;
        }

        public async Task<SchedulerTaskInfo> CreateTaskAsync(SchedulerJob schedulerJob)
        {
            var taskGuid = Guid.NewGuid().ToString();
            var type = schedulerJob.Type;
            var cron = schedulerJob.Cron;
            var action = schedulerJob.Action;

            JobDataMap jobDataMap = new JobDataMap();
            jobDataMap.Put("action", action);

            IJobDetail job = JobBuilder.Create<QuartzJob>()
                .WithIdentity(taskGuid)
                .UsingJobData("Type", type.ToString())
                .UsingJobData(jobDataMap)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(taskGuid)
                .WithCronSchedule(cron, x => x.WithMisfireHandlingInstructionDoNothing())
                .Build();

            await _scheduler.ScheduleJob(job, trigger);

            var task = new SchedulerTaskInfo
            {
                Guid = taskGuid,
                Type = type,
                Status = SchedulerTaskStatus.Running,
                CRON = cron,
                LastRun = DateTime.MinValue
            };

            return task;
        }

        public async Task<bool> ResumeTaskAsync(string taskGuid)
        {
            var jobKey = new JobKey(taskGuid);
            await _scheduler.ResumeJob(jobKey);
            return true;
        }

        public async Task<bool> PauseTaskAsync(string taskGuid)
        {
            var jobKey = new JobKey(taskGuid);
            await _scheduler.PauseJob(jobKey);
            return true;
        }

        public async Task<bool> DeleteTaskAsync(string taskGuid)
        {
            await _scheduler.DeleteJob(new JobKey(taskGuid));
            return true;
        }

        //private async Task InitJobs()
        //{

        //}
    }
}
