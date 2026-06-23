using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime? CreatingDate { get; set; }
        public int? CustomerId { get; set; }
        public int? PizzaId { get; set; }
        public string? Feedback { get; set; }
        public bool IsClose { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool ReminderSent { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual Pizza? Pizza { get; set; }
    }
}
