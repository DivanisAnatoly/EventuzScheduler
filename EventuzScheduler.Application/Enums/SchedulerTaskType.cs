using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventuzScheduler.Application.Enums
{
    public enum SchedulerTaskType
    {
        PuskinCardTicketsRegistration,
        CancelReservation
    }

    public static class SchedulerTaskTypeExtensions
    {
        public static string GetStringName(this SchedulerTaskType me)
        {
            return me switch
            {
                SchedulerTaskType.PuskinCardTicketsRegistration => "PuskinCardTicketsRegistration",
                SchedulerTaskType.CancelReservation => "CancelReservation",
                _ => "Unknown"
            };
        }
    }
}
