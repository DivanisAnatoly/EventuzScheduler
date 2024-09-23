using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventuzScheduler.Application.Enums
{
    public enum SchedulerTaskStatus
    {
        Default=0,
        Running=1,
        Paused=2,
        Deleted=3
    }
}
