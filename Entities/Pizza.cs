using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{

    public class Pizza
    {
        public int PizzaId { get; set; }
        public string? Type { get; set; }
        public string Name { get; set; } = null!;
        public int Price { get; set; }

        public virtual ICollection<Order> Orders { get; } = new List<Order>();
    }
}
