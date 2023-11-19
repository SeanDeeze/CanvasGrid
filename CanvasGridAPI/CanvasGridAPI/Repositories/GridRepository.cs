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
    public class GridRepository(ILogger logger, GridContext context, IWebHostEnvironment webHostEnvironment)
    {
        private const string ClassName = "GridRepository";

        private readonly ILogger _logger = logger;
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

        public bool CreateData()
        {
            string methodName = $"{ClassName}.CreateData";
            bool returnBool = false;
            try
            {
                context.Database.EnsureCreated();
                if (context.Grids.Any() == false)
                {

                    for (int i = 0; i < 5000; i++)
                    {
                        context.Grids.Add(new Grid
                        {
                            Order = i,
                            Title = "newimage.png"
                        });
                    }

                    context.SaveChanges();
                }
                returnBool = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{methodName}; Error Loading Grids. Error: {ex.Message}");
            }

            return returnBool;
        }

        public GridMessage LoadGrids()
        {
            string methodName = $"{ClassName}.LoadGrids";
            GridMessage returnGM = new();

            if(CreateData() == false)
            {
                returnGM.Message = $"{methodName}; Error Creating Grid Data Records! See Other Logs for Details.";
                return returnGM;
            }

            try
            {
                IEnumerable<Grid> grids = context.Grids.OrderBy(g => g.Order).ToList();
                int DateTimeSeed = DateTime.Now.Millisecond;
                foreach (Grid grid in grids)
                {
                    if (grid != null && !string.IsNullOrEmpty(grid.Title))
                    {
                        grid.Title = $"{grid.Title}?{DateTimeSeed}";
                    }
                }
                returnGM.ReturnObject = grids;
                returnGM.OperationStatus = true;
            }
            catch (Exception ex)
            {
                returnGM.Message = $"{methodName}; Error Encountered Loading Grids. See logs for full detials. Error: {ex.Message}";
                _logger.LogError(ex, $"{methodName}; Error Loading Grids. Error: {ex.Message}");
            }

            return returnGM;
        }

        public GridMessage SaveGrid(GridDTO gridDTO)
        {
            string methodName = $"{ClassName}.SaveGrid";
            GridMessage returnGM = new();

            try
            {
                Grid updateGrid = context.Grids.FirstOrDefault(g => g.Id == gridDTO.id);
                if (updateGrid != null)
                {
                    string uniqueId = Guid.NewGuid().ToString();
                    string fileName = $"{uniqueId}.png";
                    string filePath = $"{_webHostEnvironment.ContentRootPath}/wwwroot/Images/{fileName}";

                    updateGrid.Title = fileName;
                    updateGrid.Used = true;
                    updateGrid.Image = filePath;
                    context.SaveChanges();
                    _logger.LogDebug($"{ClassName}.{MethodBase.GetCurrentMethod()}; Updated Grid Saved");

                    byte[] image = Convert.FromBase64String(gridDTO.base64File);
                    File.WriteAllBytes(filePath, image);
                    _logger.LogDebug($"{methodName}; Updated Grid Image File Saved", filePath);

                    returnGM.OperationStatus = true;
                }
            }
            catch (Exception ex)
            {
                returnGM.Message = $"{methodName}; Error Encountered Savid Grid. See logs for full detials. Error: {ex.Message}";
                _logger.LogError(ex, $"{methodName}; Error Saving Grid. Error: {ex.Message}");
            }

            return returnGM;
        }
    }
}
