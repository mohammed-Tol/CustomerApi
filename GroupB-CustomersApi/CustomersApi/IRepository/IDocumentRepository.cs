using CustomersApi.Models;

namespace CustomersApi.IRepository
{
    public interface IDocumentRepository
    {

        Task<Document> GetDocumentsByCustomer(int customerId);
        Task InsertDocument(int customerId, byte[] document, int docType);
        Task UpdateDocument(int docId, byte[] document, int docType);

        Task<int> GetDocumentId(int customerId, int docType);

    }
}
