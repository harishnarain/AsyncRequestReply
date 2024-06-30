using Microsoft.AspNetCore.Mvc;
using Worker.Repositories;

namespace Worker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IRecordRepository _repository;

        public MessagesController(IRecordRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_repository.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var record = _repository.Get(id);
            if (record == null)
            {
                return NotFound();
            }
            return Ok(record);
        }
    }
}
