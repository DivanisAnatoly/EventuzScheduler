using Hangfire;
using Hangfire.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventuzScheduler.Services.Scheduler.Hangfire
{
    public static class HangfireJobExtensions
    {
        public static void PauseJob(string jobId)
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                var recurringJob = connection.GetRecurringJobs().FirstOrDefault(job => job.Id == jobId);
                if (recurringJob != null)
                {
                    using (var transaction = connection.CreateWriteTransaction())
                    {
                        transaction.AddToSet("paused-jobs", jobId);
                        transaction.Commit();
                    }
                }
            }
        }


        public static void ResumeJob(string jobId)
        {
            RemoveJobFromPaused(jobId);
        }


        public static void RemoveJobFromPaused(string jobId)
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                var recurringJob = connection.GetRecurringJobs().FirstOrDefault(job => job.Id == jobId);
                if (recurringJob != null)
                {
                    using (var transaction = connection.CreateWriteTransaction())
                    {
                        transaction.RemoveFromSet("paused-jobs", jobId);
                        transaction.Commit();
                    }
                }
            }
        }
    }
}
