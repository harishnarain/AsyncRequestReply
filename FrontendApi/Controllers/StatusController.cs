using FrontendApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrontendApi.Controllers
{
    [ApiController]
    [Route("status")]
    public class StatusController : ControllerBase
    {
        private readonly ServiceBusClientService _serviceBusClientService;

        public StatusController(ServiceBusClientService serviceBusClientService)
        {
            _serviceBusClientService = serviceBusClientService;
        }

        [HttpGet("{id}")]
        public IActionResult GetStatus(string id)
        {
            var status = _serviceBusClientService.GetRequestStatus(id);
            if (status == null)
            {
                return NotFound();
            }

            if (status.IsCompleted)
            {
                return Created(status.RedirectUrl, new { id = status.Id, redirectUrl = status.RedirectUrl });
            }

            return Accepted();
        }
    }
}
