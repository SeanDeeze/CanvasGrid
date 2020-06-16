using System.ComponentModel.DataAnnotations;

namespace CanvasGridAPI.Models
{
    public class Grid
    {
        [Key]
        public int Id { get; set; }
        public int Order { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
    }
}
