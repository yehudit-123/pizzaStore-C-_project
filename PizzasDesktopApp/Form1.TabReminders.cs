using System.Drawing;
using System.Windows.Forms;

namespace PizzasDesktopApp
{
    public partial class Form1
    {
        private TabPage BuildTabReminders()
        {
            var tab = new TabPage("תזכורות");

            tab.Controls.Add(new Label
            {
                Text = "כל ההזמנות — אדום = דורשות תשומת לב",
                Location = new Point(500, 15),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DarkRed
            });

            var btnRefresh = new Button
            {
                Text = "רענן",
                Location = new Point(20, 10),
                Size = new Size(80, 30),
                BackColor = Color.LightSkyBlue
            };
            btnRefresh.Click += (s, e) => LoadReminders();
            tab.Controls.Add(btnRefresh);

            dgvReminders = new DataGridView
            {
                Location = new Point(20, 50),
                Size = new Size(820, 390),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };

            // צביעת שורות לפי מצב
            dgvReminders.CellFormatting += DgvReminders_CellFormatting;

            dgvReminders.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);

            tab.Controls.Add(dgvReminders);

            var btnClose = new Button
            {
                Text = "✔ סמן כטופל",
                Location = new Point(20, 455),
                Size = new Size(130, 35),
                BackColor = Color.LightGreen,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnClose.Click += BtnCloseOrder_Click;
            tab.Controls.Add(btnClose);

            // מקרא צבעים
            tab.Controls.Add(new Label
            {
                Text = "🔴 = פתוח מעל 3 ימים    🟡 = פתוח פחות מ-3 ימים    ✅ = סגור",
                Location = new Point(200, 462),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            });

            tab.Enter += (s, e) => LoadReminders();

            return tab;
        }

        private void LoadReminders()
        {
            var allOrders = _pizzaService.GetAllOrders();
            var pending = allOrders.Where(o =>
                !o.IsClose &&
                o.CreatingDate <= DateTime.Now.AddDays(-3) &&
                !o.ReminderSent).ToList();

            // הצגת התראה אוטומטית בפתיחה
            if (pending.Count > 0)
            {
                MessageBox.Show(
                    $"⚠️ יש {pending.Count} הזמנות פתוחות הממתינות לטיפול!",
                    "תזכורות ממתינות",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            dgvReminders.DataSource = allOrders.Select(o => new
            {
                מזהה = o.Id,
                לקוח = o.Customer?.Name ?? "",
                פיצה = o.Pizza?.Name ?? "",
                תאריך_הזמנה = o.CreatingDate?.ToString("dd/MM/yyyy"),
                משוב = o.Feedback ?? "",
                סטטוס = o.IsClose ? "סגור ✅" :
                         o.CreatingDate <= DateTime.Now.AddDays(-3)
                             ? "דחוף 🔴"
                             : "פתוח 🟡",
                // שדה עזר נסתר לצביעה
                _isClose = o.IsClose,
                _isUrgent = !o.IsClose && o.CreatingDate <= DateTime.Now.AddDays(-3)
            }).ToList();

            // הסתרת עמודות עזר
            if (dgvReminders.Columns.Contains("_isClose"))
                dgvReminders.Columns["_isClose"]!.Visible = false;
            if (dgvReminders.Columns.Contains("_isUrgent"))
                dgvReminders.Columns["_isUrgent"]!.Visible = false;
        }

        private void DgvReminders_CellFormatting(object? sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvReminders.Rows[e.RowIndex];

            bool isClosed = false, isUrgent = false;

            if (row.Cells["_isClose"].Value is bool closed) isClosed = closed;
            if (row.Cells["_isUrgent"].Value is bool urgent) isUrgent = urgent;

            if (isClosed)
            {
                // סגור — ירוק בהיר
                row.DefaultCellStyle.BackColor = Color.Honeydew;
                row.DefaultCellStyle.ForeColor = Color.DarkGreen;
            }
            else if (isUrgent)
            {
                // דחוף — אדום בהיר
                row.DefaultCellStyle.BackColor = Color.MistyRose;
                row.DefaultCellStyle.ForeColor = Color.DarkRed;
                row.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            else
            {
                // פתוח רגיל — צהוב בהיר
                row.DefaultCellStyle.BackColor = Color.LightYellow;
                row.DefaultCellStyle.ForeColor = Color.DarkOrange;
            }
        }

        private void BtnCloseOrder_Click(object? sender, EventArgs e)
        {
            if (dgvReminders.SelectedRows.Count == 0)
            {
                MessageBox.Show("אנא בחרי הזמנה לסגירה");
                return;
            }

            int orderId = (int)dgvReminders.SelectedRows[0].Cells["מזהה"].Value;
            var orders = _pizzaService.GetAllOrders();
            var order = orders.FirstOrDefault(o => o.Id == orderId);

            if (order != null)
            {
                if (order.IsClose)
                {
                    MessageBox.Show("ההזמנה כבר סגורה!");
                    return;
                }

                order.IsClose = true;
                order.ReminderSent = true;
                int result = _pizzaService.UpdateOrder(order);

                if (result > 0)
                {
                    MessageBox.Show("✔ ההזמנה סומנה כטופלה!");
                    LoadReminders();
                }
            }
        }
    }
}