using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisualAcademy.Models.TextMessages
{
    [Table("TextMessages")]
    public class TextMessageModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long EmployeeID { get; set; }

        [Required]
        public string Message { get; set; } = string.Empty;

        public DateTimeOffset DateSent { get; set; } = DateTimeOffset.Now;

        public int? TextMessageType { get; set; } // 1: Vendor, 2: Vendor Employee
    }
}
