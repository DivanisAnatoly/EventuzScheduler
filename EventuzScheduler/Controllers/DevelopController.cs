using EventuzScheduler.Application.DTOs;
using EventuzScheduler.Application.Enums;
using EventuzScheduler.Application.Interfaces;
using EventuzScheduler.Services.Scheduler.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EventuzScheduler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevelopController : ControllerBase
    {
        private readonly ICustomScheduler _scheduler;
        public DevelopController(ICustomScheduler scheduler)
        {
            _scheduler = scheduler;
        }


        [HttpPost("Scheduler/CreateTask")]
        public async Task<IActionResult> CreateTaskAsync(SchedulerTaskType type, string cron)
        {
            CreateSchedulerTaskRequest job = new()
            {
                Task = new MyTask(),
                Type = type,          
                Cron = cron
            };

            var task = await _scheduler.CreateTaskAsync(job);
            return Ok(task);
        }


        [HttpGet("Scheduler/GetTasks")]
        public async Task<IActionResult> GetTasksAsync(int page = 1, int pageSize = 10)
        {
            var tasks = await _scheduler.GetTasksAsync(page, pageSize);
            return Ok(tasks);
        }


        [HttpPost("Scheduler/ResumeTask")]
        public async Task<IActionResult> ResumeTaskAsync(string taskKey)
        {
            var task = await _scheduler.ResumeTaskAsync(taskKey);
            return Ok(task);
        }


        [HttpPost("Scheduler/PauseTask")]
        public async Task<IActionResult> PauseTaskAsync(string taskKey)
        {
            var task = await _scheduler.PauseTaskAsync(taskKey);
            return Ok(task);
        }


        [HttpPost("Scheduler/DeleteTask")]
        public async Task<IActionResult> DeleteTaskAsync(string taskKey)
        {
            var task = await _scheduler.DeleteTaskAsync(taskKey);
            return Ok(task);
        }
    }
}
