using EventuzScheduler.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventuzScheduler.Services.Scheduler.Tasks
{
    public class MyTask : IScheduledTask
    {
        public void Execute()
        {
            Console.WriteLine("Task executed");
        }
    }
}
