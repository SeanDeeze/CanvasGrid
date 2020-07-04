using CanvasGridAPI.Models;
using System.Linq;

namespace CanvasGridAPI.Repositories
{
    public static class DatabaseInitializer
    {
        public static void Initialize(GridContext context)
        {
            context.Database.EnsureCreated();
            if (context.Grids.Any())
            {
                return;
            }

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
    }
}
