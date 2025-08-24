
namespace TaskTrackerApi.Models
{
    using System.ComponentModel.DataAnnotations;

    public class TaskItemRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;
    }
}
