using CanvasGridAPI.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CanvasGridAPI.Repositories
{
    public class GridRepository
    {
        private const string REPOSITORY_NAME = "GridRepository";
        private string METHOD_NAME = string.Empty;
        private readonly GridContext _context;
        public GridRepository(GridContext context)
        {
            _context = context;
        }

        public GridMessage LoadGrids()
        {
            GridMessage returnGM = new GridMessage();
            METHOD_NAME = "LoadGrids";
            try
            {
                IEnumerable<Grid> grids = _context.Grids.OrderBy(g => g.Order).ToList();

                returnGM.ReturnObject = grids;
                returnGM.OperationStatus = true;
            }
            catch (Exception ex)
            {

                returnGM.Message = $"{REPOSITORY_NAME}.{METHOD_NAME}; Error: {ex.Message}";
            }

            return returnGM;
        }

        public GridMessage SaveGrid()
        {
            GridMessage returnGM = new GridMessage();

            return returnGM;
        }
    }
}
