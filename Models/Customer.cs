using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Customer
    {
        public int Id { get; set; }
        [StringLength(500)]
        [Required]
        public string Name { get; set; }
        [StringLength(50)]
        [Required]
        public string Mobile { get; set; }
        [StringLength(500)]
        [Required]
        public string Email { get; set; }

    }
}
