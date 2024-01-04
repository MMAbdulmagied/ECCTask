using API.Models;
using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class ComplainVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? Createddate { get; set; }
    }
}
