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
                int DateTimeSeed = DateTime.Now.Millisecond;
                foreach (Grid grid in grids)
                {
                    if (grid != null && !string.IsNullOrEmpty(grid.Title))
                    { grid.Title = $"{grid.Title}?{DateTimeSeed}"; }
                }
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
                    string uniqueId = Guid.NewGuid().ToString();
                    string fileName = $"{uniqueId}.png";
                    string filePath = $"{_webHostEnvironment.ContentRootPath}/Images/{fileName}";

                    updateGrid.Title = fileName;
                    updateGrid.Used = true;
                    updateGrid.Image = filePath;
                    _context.SaveChanges();
                    _logger.LogDebug($"{REPOSITORY_NAME}.{MethodBase.GetCurrentMethod()}; Updated Grid Saved");

                    byte[] image = Convert.FromBase64String(gridDTO.base64File);
                    File.WriteAllBytes(filePath, image);
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
