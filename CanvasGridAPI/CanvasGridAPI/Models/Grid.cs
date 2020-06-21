using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace CanvasGridAPI.Models
{
    public class Grid
    {
        [Key]
        public int Id { get; set; }
        public int Order { get; set; }
        public bool Used { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
    }

    public class GridDTO
    {
        public int id { get; set; }
        public int order { get; set; }
        public bool used { get; set; }
        public string image { get; set; }
        public string title { get; set; }
        public string base64File { get; set; }
    }
}
