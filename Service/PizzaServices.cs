using Entities;
using InfrastructureAPI;
using ServiceInterface;

namespace Service;

public class PizzaServices : IPizzaService
{
    private readonly IPizzaInfrastructure _infra;

    public PizzaServices(IPizzaInfrastructure infra)
    {
        _infra = infra;
    }

    // =================== פיצות ===================

    public List<Pizza> GetAllPizzas()
    {
        try { return _infra.GetAllPizzas(); }
        catch { return new List<Pizza>(); }
    }

    public Pizza? GetPizzaById(int id)
    {
        try { return _infra.GetPizzaById(id); }
        catch { return null; }
    }

    // =================== לקוחות ===================

    public int AddCustomer(Customer customer)
    {
        try { return _infra.AddCustomer(customer); }
        catch { return -1; }
    }

    public Customer? GetCustomerById(int id)
    {
        try { return _infra.GetCustomerById(id); }
        catch { return null; }
    }

    public List<Customer> SearchCustomers(string searchTerm)
    {
        try { return _infra.SearchCustomers(searchTerm); }
        catch { return new List<Customer>(); }
    }

    // =================== הזמנות ===================

    public int AddOrder(Order order)
    {
        try { return _infra.AddOrder(order); }
        catch { return -1; }
    }

    public List<Order> GetAllOrders()
    {
        try { return _infra.GetAllOrders(); }
        catch { return new List<Order>(); }
    }

    public int UpdateOrder(Order order)
    {
        try { return _infra.UpdateOrder(order); }
        catch { return -1; }
    }

    public List<Order> GetPendingReminders(int days)
    {
        try { return _infra.GetPendingReminders(days); }
        catch { return new List<Order>(); }
    }
}