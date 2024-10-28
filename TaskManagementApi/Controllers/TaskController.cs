
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Models;
using TaskManagementApi.Models.DTOs;
using TaskManagementApi.Services;
using TaskManagementApi.Services.Interfaces;

namespace TaskManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        protected APIResponse _response;
        private readonly RabbitMqConsumerService _rabbitMqConsumerService;

        public TaskController(ITaskService taskService, RabbitMqConsumerService rabbitMqConsumerService)
        {
            _response = new APIResponse();
            _taskService = taskService;
            _rabbitMqConsumerService = rabbitMqConsumerService;
        }

        [HttpPost]

        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDTO task)
        {
            try
            {
                var newTask = await _taskService.CreateTaskAsync(task);

                return Ok(newTask);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskModel>> GetTaskById(int id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessage = "task not found.";
                    return NotFound(_response);
                }
                _response.Result = task;
                _response.ErrorMessage = "task retrieved successfully.";
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Status = "failed";
                _response.IsSuccess = false;
                _response.ErrorMessage = $"An error occurred: {ex.Message}";
                return StatusCode(500, _response.ErrorMessage);
            }

        }

        [HttpGet]
        public async Task<ActionResult<List<TaskModel>>> GetAll()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDTO taskDto)
        {
            var updatedTask = await _taskService.UpdateTaskAsync(id, taskDto);
            if (updatedTask == null)
            {
                return NotFound(); 
            }
            return Ok(updatedTask);
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var result = await _taskService.DeleteTaskAsync(id);
            if (!result)
            {
                return NotFound(); 
            }
            return NoContent(); 
        }

        [HttpGet("consume")]
        public async Task<IActionResult> StartConsuming()
        {
            // Tüketici başlatma
            _rabbitMqConsumerService.StartConsuming();
            return Ok("Consuming has started.");
        }

    }
}
