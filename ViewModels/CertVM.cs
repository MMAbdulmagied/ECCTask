using API.Models;
using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class CertVM
    {
        public int Id { get; set; }
        public int AcademicYear { get; set; }
        
        public string SchoolName { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
    }
}
