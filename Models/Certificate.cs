using System.ComponentModel.DataAnnotations;
using API.DB;

namespace API.Models
{
    public class Certificate
    {
        public int Id { get; set; }
        public int AcademicYear { get; set; }
        [StringLength(500)]
        public string SchoolName { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
