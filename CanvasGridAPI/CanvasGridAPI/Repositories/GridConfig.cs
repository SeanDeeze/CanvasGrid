using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanvasGridAPI.Repositories
{
    public interface IGridConfig
    {
        public string XAxisWidth { get; set; }
        public string YAxisWidth { get; set; }
    }
}
