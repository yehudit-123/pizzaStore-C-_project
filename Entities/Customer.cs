using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{    public class Customer
    {
        public int CustomerId { get; set; }
        public string? Gender { get; set; }
        public string Name { get; set; } = null!;
        public string? Phone { get; set; }
        public int? Age { get; set; }
        public string? Email { get; set; }

        public virtual ICollection<Order> Orders { get; } = new List<Order>();
    }
}
