using Hangfire;
using Hangfire.Common;
using Hangfire.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventuzScheduler.Services.Scheduler.Hangfire
{
    public class CanBePausedAttribute : JobFilterAttribute, IServerFilter
    {
        public void OnPerforming(PerformingContext filterContext)
        {
            using (var connection = JobStorage.Current.GetConnection())
            {
                var pausedJobs = connection.GetAllItemsFromSet("paused-jobs");

                if (filterContext.BackgroundJob.ParametersSnapshot.ContainsKey("RecurringJobId"))
                {
                    var parameter = filterContext.BackgroundJob.ParametersSnapshot.First(x => x.Key == "RecurringJobId");
                    var recurringJobId = parameter.Value.Replace("\"","");


                    if (pausedJobs.Contains(recurringJobId))
                    {
                        filterContext.Canceled = true;
                    }
                }
            }
        }

        public void OnPerformed(PerformedContext filterContext) { }
    }
}
