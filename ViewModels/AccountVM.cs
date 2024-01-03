using API.Models;
using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class AccountVM
    {
        public int Id { get; set; }
        [Required]
        public string Mobile { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
