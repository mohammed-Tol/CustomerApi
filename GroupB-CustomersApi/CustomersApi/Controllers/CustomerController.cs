using Microsoft.AspNetCore.Mvc;
using CustomersApi.IRepository;
using CustomersApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;

namespace CustomersApi.Controllers
{
    [Route("")]
    [ApiController]
    [EnableCors]
  
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository) 
        {
            _customerRepository = customerRepository;
        }
       



   
    [HttpGet("{id}")]
    [Authorize(Policy = "CustomerDataAccess")]
        public async Task<IActionResult> GetActiveCustomerById(int id)
        {
            try
            {
                var customer = await _customerRepository.GetActiveCustomerByIdAsync(id);
                if (customer == null)
                {
                    return BadRequest(new { Message = $"No active customer found with ID {id}." });
                }
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = $"An internal server error has occurred: {ex.Message}" });
            }
        }





        [HttpPut("{id}")]
        [EnableCors]
        [Authorize(Policy = "CustomerDataAccess")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] Customer customer)
        {
            // Check for null customer object
            if (customer == null)
            {
                return BadRequest(new { Error = "Customer data cannot be null." });
            }

            // Validate model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Ensure the customer ID in the route matches the ID in the body
            if (id != customer.CustomerId)
            {
                return BadRequest(new { Error = "Mismatched Customer ID." });
            }

            // Ensure the customer ID is valid and positive
            if (id <= 0)
            {
                return BadRequest(new { Error = "Invalid Customer ID." });
            }

            try
            {
                // Use the ID from the route to ensure the correct customer is updated
                var updatedCustomer = await _customerRepository.UpdateCustomerAsync(customer);
                if (updatedCustomer == null)
                {
                    return NotFound(new { Error = $"No active customer found with ID {id}." });
                }

                return Ok(new { Message = "Customer updated successfully.", Customer = updatedCustomer });
            }
            catch (Exception ex)
            {
                // Handle exceptions internally
                return StatusCode(500, new { Error = $"An internal error occurred: {ex.Message}" });
            }
        }






        [HttpDelete("{id}")]
        [Authorize(Policy = "CustomerDataAccess")]
        public async Task<IActionResult> DeactivateCustomer(int id)
        {
            try
            {
                var customer = await _customerRepository.DeactivateCustomerAsync(id);
                if (customer == null)
                {
                    var errorResponse = new Dictionary<string, string>
                {
                    {"Message", "No active customer found or customer could not be deactivated."}
                };
                    return BadRequest(errorResponse); 
                }

                var successResponse = new Dictionary<string, object>
            {
                {"Message", $"Customer and all related accounts with CustomerID {customer.CustomerId } deactivated successfully."}
               // {"CustomerID", customer.CustomerId}
            };
                return Ok(successResponse); // Status code 200
            }
            catch (Exception ex)
            {
                var exceptionResponse = new Dictionary<string, string>
            {
                {"Error", $"An error occurred: {ex.Message}"}
            };
                return StatusCode(500, exceptionResponse); // Status code 500
            }
        }
      
    }
}
   
