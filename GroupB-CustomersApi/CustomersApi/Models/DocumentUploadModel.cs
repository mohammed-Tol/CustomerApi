
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace CustomersApi.Models
    {
        public class DocumentUploadModel
        {
      
            public IFormFile? Photo { get; set; }
            public IFormFile? Aadhar { get; set; }
            public IFormFile? PanCard { get; set; }
        }
    }
    

