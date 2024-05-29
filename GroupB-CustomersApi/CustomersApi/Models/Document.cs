using System.ComponentModel.DataAnnotations;
namespace CustomersApi.Models
{
    public class Document
    {
        
        public IFormFile? Photo { get; set; }
        public string? BasePhoto { get; set; }
        public IFormFile? Aadhar { get; set; }
        public string? BaseAadhar { get; set; }
        public IFormFile? PanCard { get; set; }
        public string? BasePanCard { get; set; }
    }
}
