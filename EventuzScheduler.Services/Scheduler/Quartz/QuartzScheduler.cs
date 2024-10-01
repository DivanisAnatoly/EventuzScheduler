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
            if (_scheduler != null && !_scheduler.IsStarted)
            {
                _scheduler.Start().Wait();
            }
        }

        public async Task<List<SchedulerTaskInfo>> GetTasksAsync(int page = 1, int pageSize = 10)
        {
            var jobKeys = (await _scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup())).Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList(); ;
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
                        Key = jobKey.Name,
                        Type = (SchedulerTaskType)Enum.Parse(typeof(SchedulerTaskType), jobDetail.JobDataMap.GetString("Type")),
                        Status = taskStatus,
                        CRON = (trigger as ICronTrigger)?.CronExpressionString,
                        LastRun = trigger.GetPreviousFireTimeUtc()?.DateTime ?? DateTime.MinValue
                    });
                }
            }

            return tasks;
        }

        public async Task<SchedulerTaskInfo> CreateTaskAsync(CreateSchedulerTaskRequest schedulerJob)
        {
            var type = schedulerJob.Type;
            var cron = schedulerJob.Cron;
            var task = schedulerJob.Task;

            var taskKey = $"{type.GetStringName()}";

            IJobDetail job = JobBuilder.Create<QuartzJobWrapper>()
                .WithIdentity(taskKey)
                .UsingJobData("Type", type.ToString())
                .UsingJobData("taskType", task.GetType().AssemblyQualifiedName)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(Guid.NewGuid().ToString())
                .WithCronSchedule(cron, x => x.WithMisfireHandlingInstructionDoNothing())
                .Build();

            await _scheduler.ScheduleJob(job, trigger);

            var taskInfo = new SchedulerTaskInfo
            {
                Key = taskKey,
                Type = type,
                Status = SchedulerTaskStatus.Running,
                CRON = cron,
                LastRun = DateTime.MinValue
            };

            return taskInfo;
        }

        public async Task<bool> ResumeTaskAsync(string taskKey)
        {
            var jobKey = new JobKey(taskKey);
            await _scheduler.ResumeJob(jobKey);
            return true;
        }

        public async Task<bool> PauseTaskAsync(string taskKey)
        {
            var jobKey = new JobKey(taskKey);
            await _scheduler.PauseJob(jobKey);
            return true;
        }

        public async Task<bool> DeleteTaskAsync(string taskKey)
        {
            await _scheduler.DeleteJob(new JobKey(taskKey));
            return true;
        }
    }
}
