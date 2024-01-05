using System.ComponentModel.DataAnnotations;
using API.DB;

namespace API.Models
{
    public class Certificate
    {
        public int Id { get; set; }
        [Range(1990,2023)]
        [Required]
        public int AcademicYear { get; set; }
        [StringLength(500)]
        [Required]
        public string SchoolName { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
    }
}
