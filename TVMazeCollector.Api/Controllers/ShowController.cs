using Microsoft.AspNetCore.Mvc;
using TVMazeCollector.DataCollector.Models;
using TVMazeCollector.DataCollector.Services;

namespace TvMazeCollector.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowController : ControllerBase
    {
        private readonly IShowService _showService;
        private readonly ILogger<ShowController> _logger;

        public ShowController(IShowService showService, ILogger<ShowController> logger)
        {
            _showService = showService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<PageModel<GetShowModel>> Get([FromQuery] int startRow = 0, [FromQuery] int endRow = 100)
        {
            var result = await _showService.GetShowsAsync(startRow, endRow);
            return result;
        }
    }
}