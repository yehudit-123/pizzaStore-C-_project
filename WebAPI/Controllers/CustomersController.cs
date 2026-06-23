using Entities;
using Microsoft.AspNetCore.Mvc;
using ServiceInterface;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly IPizzaService _service;

    public CustomersController(IPizzaService service)
    {
        _service = service;
    }

    [HttpPost]
    public ActionResult<int> Add([FromBody] Customer customer)
    {
        int result = _service.AddCustomer(customer);
        if (result == -1) return StatusCode(500, "שגיאה בהוספת לקוח");
        return Ok(result);
    }

    [HttpGet("search/{searchTerm}")]
    public ActionResult<List<Customer>> Search(string searchTerm)
    {
        var results = _service.SearchCustomers(searchTerm);
        return Ok(results);
    }

    [HttpGet("{id}")]
    public ActionResult<Customer> GetById(int id)
    {
        var customer = _service.GetCustomerById(id);
        if (customer == null) return NotFound();
        return Ok(customer);
    }
}