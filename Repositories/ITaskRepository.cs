using TaskTrackerApi.Models;

namespace TaskTrackerApi.Repositories
{
    public interface ITaskRepository
    {
        IEnumerable<TaskItem> GetAll(int page, int pageSize);
        TaskItem? GetById(int id);
        TaskItem Add(TaskItem task);
        void MarkComplete(int id);
        int GetTotalCount();
    }
}
