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
        [NotMapped]
        public byte[] File { get; set; }
    }
}
