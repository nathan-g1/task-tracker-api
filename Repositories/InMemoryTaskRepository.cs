using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskTrackerApi.Models;

namespace TaskTrackerApi.Repositories
{
    public class InMemoryTaskRepository : ITaskRepository
    {
        private readonly List<TaskItem> _tasks = new();
        private int _nextId = 1;

        public IEnumerable<TaskItem> GetAll(int page, int pageSize)
        {
            return _tasks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
        public int GetTotalCount() => _tasks.Count;

        public TaskItem? GetById(int id) => _tasks.FirstOrDefault(t => t.Id == id);

        public TaskItem Add(TaskItem task)
        {
            task.Id = _nextId++;
            _tasks.Add(task);
            return task;
        }

        public void MarkComplete(int id)
        {
            var task = GetById(id);
            if (task != null)
                task.IsCompleted = true;
        }
    }
}