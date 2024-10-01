using EventuzScheduler.Application.Enums;
using EventuzScheduler.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventuzScheduler.Application.DTOs
{
    public class CreateSchedulerTaskRequest
    {
        public IScheduledTask Task { get; set; }
        public SchedulerTaskType Type { get; set; }
        public string Cron { get; set; }
    }
}
