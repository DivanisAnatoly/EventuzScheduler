using EventuzScheduler.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EventuzScheduler.Application.DTOs
{
    public class SchedulerTaskInfo
    {
        public string Key { get; set; }
        public SchedulerTaskType Type { get; set; }
        public SchedulerTaskStatus Status { get; set; }
        public string CRON { get; set; }
        public DateTime LastRun { get; set; }
    }
}
