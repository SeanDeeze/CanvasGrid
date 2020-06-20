using CanvasGridAPI.Models;
using CanvasGridAPI.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CanvasGridAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class GridController : ControllerBase
    {
        private readonly ILogger<GridController> _logger;
        private readonly GridRepository _gridRepository;
        private readonly GridContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public GridController(ILogger<GridController> logger, GridContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _gridRepository = new GridRepository(_logger, _context, webHostEnvironment);
        }

        [HttpGet]
        public GridMessage LoadGrids()
        {
            return _gridRepository.LoadGrids();
        }

        [HttpPost]
        public GridMessage SaveGrid(IFormFile file, [FromForm]GridDTO grid)
        {
            // return _gridRepository.SaveGrid(grid);
            return null;
        }
    }
}
