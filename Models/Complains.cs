using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Complain
    {
        public int Id { get; set; }
        [StringLength(500)]
        [Required]
        public string Title { get; set; }
        [StringLength(500)]
        [Required]
        public string Description { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime? Createddate { get; set; }
    }
}
