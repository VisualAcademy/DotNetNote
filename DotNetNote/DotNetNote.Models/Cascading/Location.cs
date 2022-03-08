using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisualAcademy.Models
{
    [Table("Location")]
    public partial class Location
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Location Name")]
        public string Name { get; set; } = "";

        public bool Active { get; set; } = false;

        public string? Property { get; set; } = "";

        [Display(Name = "Property")]
        public Property? PropertyRef { get; set; }
        public int? PropertyId { get; set; }
    }
}
