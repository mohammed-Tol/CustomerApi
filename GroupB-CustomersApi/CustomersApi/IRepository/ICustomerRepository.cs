using CustomersApi.Models;

namespace CustomersApi.IRepository
{
    public interface ICustomerRepository
    {
       // Task<List<Customer>> GetAllActiveCustomersAsync();

        Task<Customer> GetActiveCustomerByIdAsync(int id);

        Task<Customer> UpdateCustomerAsync(Customer customer);

        Task<Customer> DeactivateCustomerAsync(int customerId);
    }
}
