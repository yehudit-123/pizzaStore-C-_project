using System.Drawing;
using System.Windows.Forms;
using Entities;

namespace PizzasDesktopApp
{
    public partial class Form1
    {
        private List<Customer> _allCustomers = new();

        private TabPage BuildTabSearchCustomer()
        {
            var tab = new TabPage("חיפוש לקוח");

            // שורת חיפוש
            tab.Controls.Add(new Label
            {
                Text = "חיפוש:",
                Location = new Point(760, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            });

            txtSearch = new TextBox
            {
                Location = new Point(480, 17),
                Width = 260,
                Font = new Font("Segoe UI", 10)
            };
            txtSearch.TextChanged += FilterCustomers;
            tab.Controls.Add(txtSearch);

            // כפתור טעינה מחדש
            var btnLoadAll = new Button
            {
                Text = "טען הכל",
                Location = new Point(380, 14),
                Size = new Size(85, 30),
                BackColor = Color.LightSkyBlue
            };
            btnLoadAll.Click += (s, e) =>
            {
                txtSearch.Clear();
                LoadAllCustomers();
            };
            tab.Controls.Add(btnLoadAll);

            // טבלת לקוחות
            dgvSearchResults = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(820, 420),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };

            // עיצוב כותרות
            dgvSearchResults.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);
            dgvSearchResults.AlternatingRowsDefaultCellStyle.BackColor =
                Color.AliceBlue;

            // אירוע לחיצה על כפתור "הוסף הזמנה"
            dgvSearchResults.CellClick += DgvCustomers_CellClick;

            tab.Controls.Add(dgvSearchResults);

            // טעינת כל הלקוחות בכניסה ללשונית
            tab.Enter += (s, e) => LoadAllCustomers();

            return tab;
        }

        private void LoadAllCustomers()
        {
            _allCustomers = _pizzaService.SearchCustomers("");
            DisplayCustomers(_allCustomers);
        }

        private void FilterCustomers(object? sender, EventArgs e)
        {
            string term = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(term))
            {
                DisplayCustomers(_allCustomers);
                return;
            }

            var filtered = _allCustomers
                .Where(c =>
                    (c.Name?.ToLower().Contains(term) ?? false) ||
                    (c.Phone?.Contains(term) ?? false) ||
                    (c.Email?.ToLower().Contains(term) ?? false))
                .ToList();

            DisplayCustomers(filtered);
        }

        private void DisplayCustomers(List<Customer> customers)
        {
            // שמירת מיקום גלילה
            int firstRow = dgvSearchResults.FirstDisplayedScrollingRowIndex;

            // בניית DataTable ידני כדי לאפשר עמודת כפתור
            dgvSearchResults.DataSource = null;
            dgvSearchResults.Columns.Clear();

            // עמודות נתונים
            dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "קוד", HeaderText = "קוד", Width = 50 });
            dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "שם", HeaderText = "שם", Width = 150 });
            dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "מגדר", HeaderText = "מגדר", Width = 70 });
            dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "טלפון", HeaderText = "טלפון", Width = 110 });
            dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "אימייל", HeaderText = "אימייל", Width = 180 });
            dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "גיל", HeaderText = "גיל", Width = 50 });

            // עמודת כפתור "הוסף הזמנה"
            var btnCol = new DataGridViewButtonColumn
            {
                Name = "הוסף_הזמנה",
                HeaderText = "",
                Text = "➕ הוסף הזמנה",
                UseColumnTextForButtonValue = true,
                Width = 120
            };
            dgvSearchResults.Columns.Add(btnCol);

            // מילוי שורות
            foreach (var c in customers)
            {
                dgvSearchResults.Rows.Add(
                    c.CustomerId, c.Name, c.Gender,
                    c.Phone, c.Email, c.Age, "");
            }

            // שמירת CustomerId בתג של כל שורה לשימוש בלחיצה
            for (int i = 0; i < dgvSearchResults.Rows.Count; i++)
            {
                dgvSearchResults.Rows[i].Tag = customers[i].CustomerId;
            }

            // שחזור מיקום גלילה
            if (firstRow > 0 && firstRow < dgvSearchResults.Rows.Count)
                dgvSearchResults.FirstDisplayedScrollingRowIndex = firstRow;
        }

        private void DgvCustomers_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvSearchResults.Columns[e.ColumnIndex].Name != "הוסף_הזמנה") return;

            int customerId = (int)dgvSearchResults.Rows[e.RowIndex].Tag!;
            NavigateToOrderWithCustomer(customerId);
        }
    }
}