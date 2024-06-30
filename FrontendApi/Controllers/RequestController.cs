using FrontendApi.Models;
using FrontendApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrontendApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RequestController : ControllerBase
    {
        private readonly ServiceBusClientService _serviceBusClientService;

        public RequestController(ServiceBusClientService serviceBusClientService)
        {
            _serviceBusClientService = serviceBusClientService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RequestData requestData)
        {
            requestData.Id = Guid.NewGuid().ToString();
            await _serviceBusClientService.SendMessageAsync(requestData);
            return AcceptedAtAction(nameof(StatusController.GetStatus), "Status", new { id = requestData.Id });
        }
    }
}
