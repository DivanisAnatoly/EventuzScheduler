using EventuzScheduler.Application.DTOs;
using EventuzScheduler.Application.Enums;
using EventuzScheduler.Application.Interfaces;
using Hangfire;
using Hangfire.Storage;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventuzScheduler.Services.Scheduler.Hangfire
{
    public class HangfireScheduler : ICustomScheduler
    {
        private readonly IRecurringJobManager _recurringJobManager;

        public HangfireScheduler(IRecurringJobManager recurringJobManager)
        {
            _recurringJobManager = recurringJobManager;
        }

        public Task<List<SchedulerTaskInfo>> GetTasksAsync(int page = 1, int pageSize = 10)
        {
            List<SchedulerTaskInfo> result = new();

            using (var connection = JobStorage.Current.GetConnection())
            {
                var jobs = connection.GetRecurringJobs()
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();


                var pausedJobs = connection.GetAllItemsFromSet("paused-jobs");

                

                foreach (var job in jobs)
                {
                    SchedulerTaskStatus status;
                    if (pausedJobs.Contains(job.Id)) {
                        status = SchedulerTaskStatus.Paused;
                    }
                    else
                    {
                        status = job.Removed ? SchedulerTaskStatus.Deleted : SchedulerTaskStatus.Running;
                    }

                    result.Add(new SchedulerTaskInfo()
                    {
                        Key = job.Id,
                        Type = (SchedulerTaskType)Enum.Parse(typeof(SchedulerTaskType), job.Id),
                        CRON = job.Cron,
                        LastRun = job.LastExecution ?? DateTime.MinValue,
                        Status = status
                    });
                }
            }

            return Task.FromResult(result);
        }

        public Task<SchedulerTaskInfo> CreateTaskAsync(CreateSchedulerTaskRequest job)
        {
            var jobId = job.Type.GetStringName();
            _recurringJobManager.AddOrUpdate(jobId, () => job.Task.Execute(), job.Cron);
            return Task.FromResult(new SchedulerTaskInfo
            {
                Key = jobId,
                Type = job.Type,
                Status = SchedulerTaskStatus.Default,
                CRON = job.Cron,
                LastRun = DateTime.MinValue
            });
        }

        public Task<bool> ResumeTaskAsync(string taskKey)
        {
            HangfireJobExtensions.ResumeJob(taskKey);
            return Task.FromResult(true);
        }

        public Task<bool> PauseTaskAsync(string taskKey)
        {
            HangfireJobExtensions.PauseJob(taskKey);
            return Task.FromResult(true);
        }

        public Task<bool> DeleteTaskAsync(string taskKey)
        {
            _recurringJobManager.RemoveIfExists(taskKey);
            HangfireJobExtensions.RemoveJobFromPaused(taskKey);
            return Task.FromResult(true);
        }
    }
}
