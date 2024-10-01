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
        Task<List<SchedulerTaskInfo>> GetTasksAsync(int page = 1, int pageSize = 10);

        Task<SchedulerTaskInfo> CreateTaskAsync(CreateSchedulerTaskRequest job);

        Task<bool> ResumeTaskAsync(string taskKey);

        Task<bool> PauseTaskAsync(string taskKey);

        Task<bool> DeleteTaskAsync(string taskKey);
    }
}
