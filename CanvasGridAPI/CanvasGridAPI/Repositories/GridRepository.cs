using CanvasGridAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CanvasGridAPI.Repositories
{
    public class GridRepository
    {
        private const string REPOSITORY_NAME = "GridRepository";

        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly GridContext _context;
        public GridRepository(ILogger logger, GridContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public GridMessage LoadGrids()
        {
            GridMessage returnGM = new GridMessage();
            try
            {
                IEnumerable<Grid> grids = _context.Grids.OrderBy(g => g.Order).ToList();

                returnGM.ReturnObject = grids;
                returnGM.OperationStatus = true;
            }
            catch (Exception ex)
            {

                returnGM.Message = $"{REPOSITORY_NAME}.{MethodBase.GetCurrentMethod()}; Error: {ex.Message}";
            }

            return returnGM;
        }

        public GridMessage SaveGrid(GridDTO gridDTO)
        {
            GridMessage returnGM = new GridMessage();

            try
            {
                Grid updateGrid = _context.Grids.FirstOrDefault(g => g.Id == gridDTO.id);
                if (updateGrid != null)
                {
                    string filePath = $"{_webHostEnvironment.WebRootPath}/Images/" + updateGrid.Title;

                    updateGrid.Title = gridDTO.title;
                    updateGrid.Used = true;
                    updateGrid.Image = filePath;
                    _context.SaveChanges();
                    _logger.LogDebug($"{REPOSITORY_NAME}.{MethodBase.GetCurrentMethod()}; Updated Grid Saved");

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        gridDTO.file.CopyTo(fileStream);
                    }
                    _logger.LogDebug($"{REPOSITORY_NAME}.{MethodBase.GetCurrentMethod()}; Updated Grid Image File Saved", filePath);

                    returnGM.OperationStatus = true;
                }
            }
            catch (Exception ex)
            {

                _logger.LogDebug($"{REPOSITORY_NAME}.{MethodBase.GetCurrentMethod()}; Error During Operation: {ex.Message}", ex);
            }

            return returnGM;
        }
    }
}
