using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CanvasGridAPI.Models
{
    public class GridContext(DbContextOptions<GridContext> dbContextOptions) : DbContext(dbContextOptions)
    {
        public DbSet<Grid> Grids { get; set; }
    }
}
