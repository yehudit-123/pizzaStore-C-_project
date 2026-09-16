using Entities;
using Microsoft.AspNetCore.Mvc;
using ServiceInterface;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PizzasController : ControllerBase
{
    private readonly IPizzaService _service;

    public PizzasController(IPizzaService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<List<Pizza>> GetAll()
    {
        
        return Ok(_service.GetAllPizzas());
    }

    [HttpGet("{id}")]
    public ActionResult<Pizza> GetById(int id)
    {
        var pizza = _service.GetPizzaById(id);
        if (pizza == null) return NotFound();
        return Ok(pizza);
    }
}