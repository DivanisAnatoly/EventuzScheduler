using EventuzScheduler.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventuzScheduler.Application.DTOs
{
    public class SchedulerJob
    {
        public Action Action { get; set; }
        public SchedulerTaskType Type { get; set; }
        public string Cron { get; set; }
    }
}
