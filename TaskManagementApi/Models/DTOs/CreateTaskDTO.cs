namespace TaskManagementApi.Models.DTOs
{
    public class CreateTaskDTO
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsDone { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime DueDate { get; set; }
    }
}
