using Microsoft.EntityFrameworkCore;

namespace CanvasGridAPI.Models
{
    public class GridContext(DbContextOptions dbContextOptions) : DbContext(dbContextOptions)
    {
        public DbSet<Grid> Grids { get; set; }
    }
}
