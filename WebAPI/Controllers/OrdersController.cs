using Entities;
using Microsoft.AspNetCore.Mvc;
using ServiceInterface;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IPizzaService _service;

    public OrdersController(IPizzaService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<List<Order>> GetAll()
    {
        return Ok(_service.GetAllOrders());
    }

    [HttpPost]
    public ActionResult<int> Add([FromBody] Order order)
    {
        int result = _service.AddOrder(order);
        if (result == -1) return StatusCode(500, "שגיאה ביצירת הזמנה");
        return Ok(result);
    }

    [HttpPut("{id}")]
    public ActionResult<int> Update(int id, [FromBody] Order order)
    {
        int result = _service.UpdateOrder(order);
        if (result == -1) return StatusCode(500, "שגיאה בעדכון הזמנה");
        return Ok(result);
    }

    [HttpGet("reminders/{days}")]
    public ActionResult<List<Order>> GetReminders(int days)
    {
        return Ok(_service.GetPendingReminders(days));
    }
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        // קריאה לפונקציה שיצרנו ב-Service
        int result = _service.DeleteOrder(id);

        // אם הפונקציה החזירה -1, סימן שההזמנה לא נמצאה או נכשלה
        if (result == -1)
        {
            return NotFound("ההזמנה לא נמצאה או ששגיאה אירעה במחיקה");
        }

        // מחזירים תשובת הצלחה
        return Ok(true);
    }
}