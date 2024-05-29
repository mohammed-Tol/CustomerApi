using CustomersApi.IRepository;
using CustomersApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace CustomersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
  
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentRepository _documentRepository;

        public DocumentController(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        // GET: api/Document/{customerId}
        [HttpGet("{customerId}")]
        [Authorize(Policy = "CustomerDataAccess")]
        public async Task<IActionResult> GetDocumentsByCustomer(int customerId)
        {
            try
            {
                var documents = await _documentRepository.GetDocumentsByCustomer(customerId);
                if (documents == null)
                {
                    return NotFound();
                }
                return Ok(documents);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Internal server error: {ex.Message}" });
            }
        }

        // POST: api/Document/{customerId}
        [HttpPost("{customerId}")]
        [Authorize(Policy = "CustomerDataAccess")]
        public async Task<IActionResult> InsertDocuments(int customerId, [FromForm] DocumentUploadModel model)
        {
            try
            {
                if (model.Photo == null && model.Aadhar == null && model.PanCard == null)
                {
                    return BadRequest("At least one document must be provided.");
                }

                if (model.Photo != null)
                {
                    await SaveDocument(customerId, model.Photo, 1); // 1 = Photo
                }

                if (model.Aadhar != null)
                {
                    await SaveDocument(customerId, model.Aadhar, 2); // 2 = Aadhar
                }

                if (model.PanCard != null)
                {
                    await SaveDocument(customerId, model.PanCard, 3); // 3 = PAN Card
                }

                return Ok(new { Message = "Documents inserted successfully." });
            }
            
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Internal server error: {ex.Message}" });
            }
        }
        [HttpPut("{customerId}")]
        [Authorize(Policy = "CustomerDataAccess")]
        public async Task<IActionResult> UpdateDocuments(int customerId, [FromForm] DocumentUploadModel model)
        {
            bool anyUpdates = false;

            if (model.Photo != null)
            {
                anyUpdates = true;
                await UpdateDocumentData(customerId, model.Photo, 1); // 1 = Photo
            }

            if (model.Aadhar != null)
            {
                anyUpdates = true;
                await UpdateDocumentData(customerId, model.Aadhar, 2); // 2 = Aadhar
            }

            if (model.PanCard != null)
            {
                anyUpdates = true;
                await UpdateDocumentData(customerId, model.PanCard, 3); // 3 = PAN Card
            }

            if (!anyUpdates)
            {
                return BadRequest(new { Message = "No documents were provided for update." });
            }

            return Ok(new { Message = "Documents updated successfully." });
        }
        private async Task UpdateDocumentData(int customerId, IFormFile document, int docType)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            byte[] documentData;
            using (var ms = new MemoryStream())
            {
                await document.CopyToAsync(ms);
                documentData = ms.ToArray();
            }

            // Assuming you have a method to retrieve the document ID:
            int docId = await _documentRepository.GetDocumentId(customerId, docType);
            await _documentRepository.UpdateDocument(docId, documentData, docType);
        }
        private async Task SaveDocument(int customerId, IFormFile document, int docType)
        {
            byte[] documentData;
            using (var ms = new MemoryStream())
            {
                await document.CopyToAsync(ms);
                documentData = ms.ToArray();
            }

            await _documentRepository.InsertDocument(customerId, documentData, docType);
        }


        // Note: Connection will be closed by the calling method or use 'finally' block to ensure it closes


    }
}