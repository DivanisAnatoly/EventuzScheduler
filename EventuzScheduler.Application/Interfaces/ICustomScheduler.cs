using EventuzScheduler.Application.DTOs;
using EventuzScheduler.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventuzScheduler.Application.Interfaces
{
    public interface ICustomScheduler
    {
        Task<List<SchedulerTaskInfo>> GetTasksAsync();

        Task<bool> EditTaskCronAsync(string taskGuid, string cron);

        Task<SchedulerTaskInfo> CreateTaskAsync(SchedulerJob job);

        Task<bool> ResumeTaskAsync(string taskGuid);

        Task<bool> PauseTaskAsync(string taskGuid);

        Task<bool> DeleteTaskAsync(string taskGuid);
    }
}
