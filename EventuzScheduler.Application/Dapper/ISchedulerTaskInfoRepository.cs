using EventuzScheduler.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventuzScheduler.Application.Dapper
{
    public interface ISchedulerTaskInfoRepository : IGenericRepository<TaskInfo>
    {
    }
}
