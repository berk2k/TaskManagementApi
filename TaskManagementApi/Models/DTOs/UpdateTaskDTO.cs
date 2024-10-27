namespace TaskManagementApi.Models.DTOs
{
    public class UpdateTaskDTO
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsDone { get; set; }

        public DateTime DueDate { get; set; }
    }
}
