using CanvasGridAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            //Make a 1000 X 1000 Grid
            for (int i = 0; i < 1000; i++)
            {
                context.Grids.Add(new Grid
                {
                    Order = i
                });
            }

            context.SaveChanges();
        }
    }
}
