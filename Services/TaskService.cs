using TaskTrackerApi.Models;
using TaskTrackerApi.Repositories;

namespace TaskTrackerApi.Services
{
    public class TaskService
    {
        private readonly ITaskRepository _repository;

        public TaskService(ITaskRepository repository)
        {
            _repository = repository;
        }

        public (IEnumerable<TaskItem> tasks, int totalCount) GetTasks(int page, int pageSize)
        {
            var tasks = _repository.GetAll(page, pageSize);
            var totalCount = _repository.GetTotalCount();
            return (tasks, totalCount);
        }

        public TaskItem? GetTask(int id) => _repository.GetById(id);

        public TaskItem AddTask(string title)
        {
            return _repository.Add(new TaskItem
            {
                Title = title,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            });
        }

        public void CompleteTask(int id) => _repository.MarkComplete(id);
    }
}
