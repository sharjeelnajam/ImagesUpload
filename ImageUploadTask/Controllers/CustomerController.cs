using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImageUploadTask.Data;
using ImageUploadTask.Models;
using ImageUploadTask.Models.DTOs;

namespace ImageUploadTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ApplicationDbContext context, ILogger<CustomerController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all customers
        /// </summary>
        /// <returns>List of customers</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Customer>>>> GetCustomers()
        {
            try
            {
                var customers = await _context.Customers
                    .Include(c => c.Images)
                    .OrderBy(c => c.LastName)
                    .ThenBy(c => c.FirstName)
                    .ToListAsync();

                return Ok(ApiResponse<List<Customer>>.SuccessResult(customers, "Customers retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customers");
                return StatusCode(500, ApiResponse<List<Customer>>.ErrorResult(
                    "An error occurred while retrieving customers"));
            }
        }

        /// <summary>
        /// Get a specific customer by ID
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <returns>Customer details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Customer>>> GetCustomer(int id)
        {
            try
            {
                var customer = await _context.Customers
                    .Include(c => c.Images)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (customer == null)
                {
                    return NotFound(ApiResponse<Customer>.ErrorResult($"Customer with ID {id} not found"));
                }

                return Ok(ApiResponse<Customer>.SuccessResult(customer, "Customer retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer {CustomerId}", id);
                return StatusCode(500, ApiResponse<Customer>.ErrorResult(
                    "An error occurred while retrieving the customer"));
            }
        }

        /// <summary>
        /// Create a new customer
        /// </summary>
        /// <param name="customer">Customer data</param>
        /// <returns>Created customer</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Customer>>> CreateCustomer([FromBody] Customer customer)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponse<Customer>.ErrorResult("Invalid customer data", errors));
                }

                customer.CreatedAt = DateTime.UtcNow;
                customer.UpdatedAt = DateTime.UtcNow;

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created new customer {CustomerId} with name {FirstName} {LastName}", 
                    customer.Id, customer.FirstName, customer.LastName);

                return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, 
                    ApiResponse<Customer>.SuccessResult(customer, "Customer created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                return StatusCode(500, ApiResponse<Customer>.ErrorResult(
                    "An error occurred while creating the customer"));
            }
        }

        /// <summary>
        /// Update an existing customer
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <param name="customer">Updated customer data</param>
        /// <returns>Updated customer</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Customer>>> UpdateCustomer(int id, [FromBody] Customer customer)
        {
            try
            {
                if (id != customer.Id)
                {
                    return BadRequest(ApiResponse<Customer>.ErrorResult("Customer ID mismatch"));
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponse<Customer>.ErrorResult("Invalid customer data", errors));
                }

                var existingCustomer = await _context.Customers.FindAsync(id);
                if (existingCustomer == null)
                {
                    return NotFound(ApiResponse<Customer>.ErrorResult($"Customer with ID {id} not found"));
                }

                existingCustomer.FirstName = customer.FirstName;
                existingCustomer.LastName = customer.LastName;
                existingCustomer.Email = customer.Email;
                existingCustomer.Phone = customer.Phone;
                existingCustomer.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated customer {CustomerId}", id);

                return Ok(ApiResponse<Customer>.SuccessResult(existingCustomer, "Customer updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer {CustomerId}", id);
                return StatusCode(500, ApiResponse<Customer>.ErrorResult(
                    "An error occurred while updating the customer"));
            }
        }

        /// <summary>
        /// Delete a customer and all their images
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteCustomer(int id)
        {
            try
            {
                var customer = await _context.Customers
                    .Include(c => c.Images)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (customer == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult($"Customer with ID {id} not found"));
                }

                // Delete physical image files
                foreach (var image in customer.Images)
                {
                    if (System.IO.File.Exists(image.FilePath))
                    {
                        System.IO.File.Delete(image.FilePath);
                    }
                }

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted customer {CustomerId} and all associated images", id);

                return Ok(ApiResponse<object?>.SuccessResult(null, "Customer and all associated images deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer {CustomerId}", id);
                return StatusCode(500, ApiResponse<object?>.ErrorResult(
                    "An error occurred while deleting the customer"));
            }
        }
    }
}
