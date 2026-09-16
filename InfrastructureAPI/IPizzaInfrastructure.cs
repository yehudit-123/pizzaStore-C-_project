using Entities;

namespace InfrastructureAPI
{
    public interface IPizzaInfrastructure
    {
        // פיצות
        List<Pizza> GetAllPizzas();
        Pizza? GetPizzaById(int id);

        // לקוחות
        int AddCustomer(Customer customer);
        Customer? GetCustomerById(int id);
        List<Customer> GetAllCustomers();
        List<Customer> SearchCustomers(string searchTerm);

        // הזמנות
        int AddOrder(Order order);
        List<Order> GetAllOrders();
        int UpdateOrder(Order order);
        List<Order> GetPendingReminders(int days);

        int DeleteOrder(int id);
    }
}