using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Customer
    {
        public int Id { get; set; }
        [StringLength(500)]
        public string Name { get; set; }
        [StringLength(50)]
        public string Mobile { get; set; }
        [StringLength(500)]
        public string Email { get; set; }

    }
}
