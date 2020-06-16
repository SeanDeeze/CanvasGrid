using CanvasGridAPI.Models;
using CanvasGridAPI.Repositories;
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

        public GridController(ILogger<GridController> logger, GridContext context)
        {
            _logger = logger;
            _context = context;
            _gridRepository = new GridRepository(_context);
        }

        [HttpGet]
        public GridMessage LoadGrids()
        {
            return _gridRepository.LoadGrids();
        }
    }
}
