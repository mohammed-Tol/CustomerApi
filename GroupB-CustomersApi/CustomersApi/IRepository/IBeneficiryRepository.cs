using System.Collections.Generic;
using System.Threading.Tasks;
using CustomersApi.Models;

namespace CustomersApi.Repositories
{
    public interface IBeneficiariesRepository
    {
        Task<Beneficiaries> AddBeneficiaryAsync(Beneficiaries beneficiary);
        Task<Beneficiaries> DeleteBeneficiaryByAccountIdAsync(long benefAccount);
        Task<IEnumerable<Beneficiaries>> GetBeneficiariesByCustomerIdAsync(int customerId);
    }
}