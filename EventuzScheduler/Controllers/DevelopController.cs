using EventuzScheduler.Application.DTOs;
using EventuzScheduler.Application.Enums;
using EventuzScheduler.Application.Interfaces;
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
            Action myAction = async () =>
            {
                var tasks = await _scheduler.GetTasksAsync();
                Console.WriteLine("Hello from delegate!");
                Console.WriteLine(tasks);
            };

            SchedulerJob job = new()
            {
                Action = myAction,
                Type = type,          
                Cron = cron
            };

            var task = await _scheduler.CreateTaskAsync(job);
            return Ok(task);
        }


        [HttpGet("Scheduler/GetTasks")]
        public async Task<IActionResult> GetTasksAsync()
        {
            var tasks = await _scheduler.GetTasksAsync();
            return Ok(tasks);
        }


        [HttpPost("Scheduler/ResumeTask")]
        public async Task<IActionResult> ResumeTaskAsync(string taskGuid)
        {
            var task = await _scheduler.ResumeTaskAsync(taskGuid);
            return Ok(task);
        }


        [HttpPost("Scheduler/PauseTask")]
        public async Task<IActionResult> PauseTaskAsync(string taskGuid)
        {
            var task = await _scheduler.PauseTaskAsync(taskGuid);
            return Ok(task);
        }


        [HttpPost("Scheduler/DeleteTask")]
        public async Task<IActionResult> DeleteTaskAsync(string taskGuid)
        {
            var task = await _scheduler.DeleteTaskAsync(taskGuid);
            return Ok(task);
        }
    }
}
