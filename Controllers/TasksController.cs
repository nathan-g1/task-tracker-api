using Microsoft.AspNetCore.Mvc;
using TaskTrackerApi.Models;
using TaskTrackerApi.Services;

namespace TaskTrackerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly TaskService _service;

        public TasksController(TaskService service)
        {
            _service = service;
        }

        // GET /tasks
        [HttpGet]
        public ActionResult<IEnumerable<TaskItem>> GetTasks([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                throw new ArgumentException("Page and pageSize must be greater than 0.");

            var (tasks, totalCount) = _service.GetTasks(page, pageSize);

            var response = new
            {
                totalCount,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                data = tasks
            };

            return Ok(response);
        }

        // POST /tasks
        [HttpPost]
        public ActionResult<TaskItem> CreateTask([FromBody] TaskItemRequest newTask)
        {
            if (newTask == null || string.IsNullOrWhiteSpace(newTask.Title))
                throw new ArgumentException("The 'title' field is required.");

            var created = _service.AddTask(newTask.Title);
            return CreatedAtAction(nameof(GetTasks), new { id = created.Id }, created);
        }

        // PUT /tasks/{id}/complete
        [HttpPut("{id}/complete")]
        public IActionResult CompleteTask(int id)
        {
            var task = _service.GetTask(id);
            if (task == null)
                throw new KeyNotFoundException($"Task with ID: {id} does not exist.");

            if (task.IsCompleted)
                throw new InvalidOperationException($"Task with ID: {id} is already completed.");

            _service.CompleteTask(id);
            return Ok(task); 
        }
    }
}
