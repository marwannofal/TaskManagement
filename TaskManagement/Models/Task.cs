using System.ComponentModel.DataAnnotations;
using TaskManagement.Data.Enums;

namespace TaskManagement.Models
{
    public class Task
    {
        [Key]
        public int TaskId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Display(Name = "End Date")]
        public DateTime DueDate { get; set; }
        [Required]
        public Status Status { get; set; }
        public string Category { get; set; }
    }
}