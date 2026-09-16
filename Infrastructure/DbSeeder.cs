using Entities;
using Infrastructure;

public static class DbSeeder
{
    public static void SeedData(PizzaStoreContext context)
    {
        // בדיקה אם כבר יש לקוחות במסד הנתונים
        if (context.Customers.Any())
        {
            return;
        }

        // 1. יצירת 10 לקוחות (עם נתונים שמותאמים להגבלות ה-DB)
        var customers = new List<Customer>();
        for (int i = 1; i <= 10; i++)
        {
            customers.Add(new Customer
            {
                Name = $"לקוח {i}", // עד 20 תווים (תקין)
                Phone = $"050-1234{i:D2}", // בדיוק 10 תווים (למשל 050-123401)
                Email = $"customer{i}@test.com", // עד 50 תווים (תקין)
                Age = 20 + i,
                Gender = i % 2 == 0 ? "זכר" : "נקבה" // עד 10 תווים (תקין)
            });
        }
        context.Customers.AddRange(customers);
        context.SaveChanges();

        // 2. יצירת 2 פיצות לדוגמה
        if (!context.Pizzas.Any())
        {
            context.Pizzas.AddRange(
                new Pizza { Name = "מרגריטה", Type = "רגיל", Price = 40 },
                new Pizza { Name = "פפרוני", Type = "בשרי", Price = 50 }
            );
            context.SaveChanges();
        }
        var firstPizzaId = context.Pizzas.First().PizzaId;

        // 3. יצירת 30 הזמנות שמקושרות ללקוחות
        var orders = new List<Order>();
        var random = new Random();

        for (int i = 1; i <= 30; i++)
        {
            var randomCustomer = customers[random.Next(customers.Count)];

            orders.Add(new Order
            {
                CustomerId = randomCustomer.CustomerId,
                PizzaId = firstPizzaId,
                CreatingDate = DateTime.Now.AddDays(-random.Next(1, 30)),
                IsClose = random.Next(2) == 0,
                Feedback = "נוצר אוטומטית", // עד 50 תווים (תקין)
                ReminderSent = false
            });
        }
        context.Orders.AddRange(orders);
        context.SaveChanges();
    }
}