using Entities;
using InfrastructureAPI;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class PizzaInfrastructure : IPizzaInfrastructure
{
    private readonly PizzaStoreContext _ctx;

    public PizzaInfrastructure(PizzaStoreContext context)
    {
        _ctx = context;
    }

    // =================== פיצות ===================

    public List<Pizza> GetAllPizzas()
    {
        try { return _ctx.Pizzas.ToList(); }
        catch { return new List<Pizza>(); }
    }

    public Pizza? GetPizzaById(int id)
    {
        try { return _ctx.Pizzas.Find(id); }
        catch { return null; }
    }

    // =================== לקוחות ===================

    public int AddCustomer(Customer customer)
    {
        try
        {
            _ctx.Customers.Add(customer);
            _ctx.SaveChanges();
            return customer.CustomerId;
        }
        catch { return -1; }
    }

    public Customer? GetCustomerById(int id)
    {
        try { return _ctx.Customers.Find(id); }
        catch { return null; }
    }
    public List<Customer> GetAllCustomers()
    {
        return _ctx.Customers.ToList();
    }
    public List<Customer> SearchCustomers(string searchTerm)
    {
        try
        {
            return _ctx.Customers
                .Where(c => c.Name.Contains(searchTerm) ||
                            c.Phone!.Contains(searchTerm) ||
                            c.Email!.Contains(searchTerm))
                .ToList();
        }
        catch { return new List<Customer>(); }
    }

    // =================== הזמנות ===================

    public int AddOrder(Order order)
    {
        try
        {
            _ctx.Orders.Add(order);
            _ctx.SaveChanges();
            return order.Id;
        }
        catch { return -1; }
    }

    public List<Order> GetAllOrders()
    {
        try
        {
            return _ctx.Orders
                .Include(o => o.Customer)
                .Include(o => o.Pizza)
                .ToList();
        }
        catch { return new List<Order>(); }
    }

    public int UpdateOrder(Order order)
    {
        try
        {
            _ctx.Orders.Update(order);
            return _ctx.SaveChanges();
        }
        catch { return -1; }
    }

    public List<Order> GetPendingReminders(int days)
    {
        try
        {
            DateTime cutoff = DateTime.Now.AddDays(-days);
            return _ctx.Orders
                .Include(o => o.Customer)
                .Include(o => o.Pizza)
                .Where(o => !o.IsClose &&
                            o.CreatingDate <= cutoff &&
                            !o.ReminderSent)
                .ToList();
        }
        catch { return new List<Order>(); }
    }
    public int DeleteOrder(int id)
    {
        // 1. חיפוש ההזמנה במסד הנתונים לפי ה-id שהתקבל
        var order = _ctx.Orders.Find(id);

        // אם ההזמנה לא קיימת במסד, מחזירים -1 (שגיאה)
        if (order == null)
        {
            return -1;
        }

        // 2. הסרת ההזמנה מטבלת ההזמנות בעזרת Remove
        _ctx.Orders.Remove(order);

        // 3. שמירת השינויים במסד הנתונים (כדי שהמחיקה תתבצע בפועל ב-SQL)
        _ctx.SaveChanges();

        // החזרת ערך חיובי המציין הצלחה
        return 1;
    }
}