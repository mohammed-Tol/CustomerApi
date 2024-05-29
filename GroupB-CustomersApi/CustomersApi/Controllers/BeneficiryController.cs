using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CustomersApi.Models;
using CustomersApi.Repositories;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;

namespace CustomersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    
    public class BeneficiariesController : ControllerBase
    {
        private readonly IBeneficiariesRepository _beneficiariesRepository;

        public BeneficiariesController(IBeneficiariesRepository beneficiariesRepository)
        {
            _beneficiariesRepository = beneficiariesRepository;
        }

        [HttpPost("add")]
        [Authorize(Policy = "CustomerDataAccess")]
        public async Task<IActionResult> AddBeneficiary(Beneficiaries beneficiary)
        {
            if (beneficiary == null)
            {
                return BadRequest(new { message = "Invalid beneficiary data." });
            }

            try
            {
                var result = await _beneficiariesRepository.AddBeneficiaryAsync(beneficiary);
                if (result == null)
                {
                    return BadRequest(new { message = "Failed to add beneficiary. The account may not exist, or you may be trying to add your own account." });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
  
                return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
            }
        }

        [HttpDelete("{benefAccount}")]
        [Authorize(Policy = "CustomerDataAccess")]
        public async Task<IActionResult> DeleteBeneficiaryByAccountId(long benefAccount)
        {
            try
            {
                var result = await _beneficiariesRepository.DeleteBeneficiaryByAccountIdAsync(benefAccount);
                if (result != null)
                {
                    return Ok(new { message = "Beneficiary deactivated." });
                }

                return BadRequest(new { message = "Beneficiary not found or could not be deactivated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{customerId}")]
        [Authorize(Policy = "CustomerDataAccess")]
        public async Task<IActionResult> GetBeneficiariesByCustomerId(int customerId)
        {
            try
            {
                var beneficiaries = await _beneficiariesRepository.GetBeneficiariesByCustomerIdAsync(customerId);
                return Ok(beneficiaries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}