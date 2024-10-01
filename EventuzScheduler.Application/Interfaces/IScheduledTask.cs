using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventuzScheduler.Application.Interfaces
{
    public interface IScheduledTask
    {
        void Execute();
    }
}
