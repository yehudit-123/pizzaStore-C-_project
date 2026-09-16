using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization; // הוספנו את השורה הזו כדי שהמערכת תכיר את הפקודה להתעלמות

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

        [JsonIgnore] // הוראה להתעלם מהלקוח המלא בתקשורת
        public virtual Customer? Customer { get; set; }

        [JsonIgnore] // הוראה להתעלם מהפיצה המלאה בתקשורת
        public virtual Pizza? Pizza { get; set; }
    }
}