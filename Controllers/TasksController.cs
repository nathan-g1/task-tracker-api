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
        private readonly ILogger<TasksController> _logger;

        public TasksController(TaskService service, ILogger<TasksController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET /tasks
        [HttpGet]
        public ActionResult<IEnumerable<TaskItem>> GetTasks([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("GET /tasks requested with page={Page}, pageSize={PageSize}", page, pageSize);

            if (page < 1 || pageSize < 1)
            {
                _logger.LogWarning("Invalid pagination parameters: page={Page}, pageSize={PageSize}", page, pageSize);
                throw new ArgumentException("Page and pageSize must be greater than 0.");
            }

            var (tasks, totalCount) = _service.GetTasks(page, pageSize);

            var response = new
            {
                totalCount,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                data = tasks
            };

            _logger.LogInformation("GET /tasks returning {Count} items (total={Total})", response.data.Count(), totalCount);
            return Ok(response);
        }

        // POST /tasks
        [HttpPost]
        public ActionResult<TaskItem> CreateTask([FromBody] TaskItemRequest newTask)
        {
            _logger.LogInformation("POST /tasks with title length {TitleLength}", newTask?.Title?.Length ?? 0);

            if (newTask == null || string.IsNullOrWhiteSpace(newTask.Title))
            {
                _logger.LogWarning("Task creation failed: missing or empty title");
                throw new ArgumentException("The 'title' field is required.");
            }

            var created = _service.AddTask(newTask.Title);
            _logger.LogInformation("Task created with id={TaskId}", created.Id);
            return CreatedAtAction(nameof(GetTasks), new { id = created.Id }, created);
        }

        // PUT /tasks/{id}/complete
        [HttpPut("{id}/complete")]
        public IActionResult CompleteTask(int id)
        {
            _logger.LogInformation("PUT /tasks/{Id}/complete invoked", id);

            var task = _service.GetTask(id);
            if (task == null)
            {
                _logger.LogWarning("Complete failed: task id={Id} not found", id);
                throw new KeyNotFoundException($"Task with ID: {id} does not exist.");
            }

            if (task.IsCompleted)
            {
                _logger.LogInformation("Complete skipped: task id={Id} already completed", id);
                throw new InvalidOperationException($"Task with ID: {id} is already completed.");
            }

            _service.CompleteTask(id);
            _logger.LogInformation("Task id={Id} marked as completed", id);
            return Ok(task);
        }
    }
}
