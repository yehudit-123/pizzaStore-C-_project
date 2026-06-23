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
}