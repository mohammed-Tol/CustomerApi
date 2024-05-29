using CustomersApi.DataAccess;
using CustomersApi.IRepository;
using CustomersApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace CustomersApi.IRepository
{
    public class DocumentRepository : BaseDataAccess, IDocumentRepository
    {
        public DocumentRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<Document> GetDocumentsByCustomer(int customerId)
        {
            var model = new Document();
            using (var reader = await Task.Run(() => ExecuteReader("sp_GetDocumentsByCustomerID", CommandType.StoredProcedure, new SqlParameter("@CustomerId", customerId))))
            {
                while (await reader.ReadAsync())
                {
                    int docType = (int)reader["DocType"];
                    if (reader["Document"] != DBNull.Value)
                    {
                        byte[] bytes = (byte[])reader["Document"];
                        string fileName = DetermineFileName(docType);
                        IFormFile formFile = CreateFormFileFromBytes(bytes, fileName);
                        string base64Image = Convert.ToBase64String(bytes);

                        switch (docType)
                        {
                            case 1:
                                model.Photo = formFile;
                                model.BasePhoto = base64Image;
                                break;
                            case 2:
                                model.Aadhar = formFile;
                                model.BaseAadhar = base64Image;
                                break;
                            case 3:
                                model.PanCard = formFile;
                                model.BasePanCard = base64Image;
                                break;
                        }
                    }
                }
            }
            return model;
        }

        public async Task InsertDocument(int customerId, byte[] document, int docType)
        {
            await Task.Run(() => ExecuteNonQuery("sp_InsertCustomerDocument", CommandType.StoredProcedure,
                new SqlParameter("@CustomerId", customerId),
                new SqlParameter("@Document", document),
                new SqlParameter("@DocType", docType)));
        }

        public async Task UpdateDocument(int docId, byte[] document, int docType)
        {
            await Task.Run(() => ExecuteNonQuery("sp_UpdateCustomerDocument", CommandType.StoredProcedure,
                new SqlParameter("@DocId", docId),
                new SqlParameter("@Document", document),
                new SqlParameter("@DocType", docType)));
        }

        private IFormFile CreateFormFileFromBytes(byte[] bytes, string fileName)
        {
            if (bytes == null) return null;
            var stream = new MemoryStream(bytes);
            return new FormFile(stream, 0, bytes.Length, fileName, fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/octet-stream",
                ContentDisposition = $"form-data; name=\"{fileName}\"; filename=\"{fileName}\""
            };
        }

        private string DetermineFileName(int docType)
        {
            return docType switch
            {
                1 => "photo.png",
                2 => "aadhar.png",
                3 => "pancard.png",
                _ => "unknown",
            };
        }

        public async Task<int> GetDocumentId(int customerId, int docType)
        {
            OpenConnection();
            using (var command = _connection.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "sp_GetDocumentIdByCustomerAndType"; // Your stored procedure name
                command.Parameters.AddWithValue("@CustomerId", customerId);
                command.Parameters.AddWithValue("@DocType", docType);

                var result = command.ExecuteScalar();
                CloseConnection();
                return (result != DBNull.Value) ? Convert.ToInt32(result) : 0;
            }
        }
    }
}