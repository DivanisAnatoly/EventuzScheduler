using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventuzScheduler.Core.Entities
{
    public class TaskInfo
    {
        public int Id { get; set; }
        public string Guid { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Cron { get; set; }
        public DateTime LastRun { get; set; }
    }
}
