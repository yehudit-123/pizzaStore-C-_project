using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Entities;

namespace PizzaClient
{
    public partial class Form1
    {
        private List<Customer> _allCustomers = new();
        private DataGridView dgvCustomerOrders = null!;
        private Label lblCustomerInfo = null!;
        private int currentSelectedCustomerId = -1;

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

            btnLoadAll.Click += async (s, e) =>
            {
                txtSearch.Clear();
                await LoadAllCustomers();
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

            dgvSearchResults.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);
            dgvSearchResults.AlternatingRowsDefaultCellStyle.BackColor =
                Color.AliceBlue;

            // אירוע לחיצה על כפתור "הוסף הזמנה"
            dgvSearchResults.CellClick += DgvCustomers_CellClick;

            // אירוע לחיצה כפולה לפתיחת פרטי הלקוח וההזמנות שלו
            dgvSearchResults.CellDoubleClick += DgvSearchResults_CellDoubleClick;

            tab.Controls.Add(dgvSearchResults);

            // טעינת כל הלקוחות בכניסה ללשונית
            tab.Enter += async (s, e) => await LoadAllCustomers();

            return tab;
        }

        private async Task LoadAllCustomers()
        {
            try
            {
                var customers = await _client.GetFromJsonAsync<List<Customer>>("api/Customers");
                if (customers != null)
                {
                    _allCustomers = customers;
                    DisplayCustomers(_allCustomers);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("שגיאה בטעינת לקוחות מהשרת.", "שגיאת תקשורת", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            int firstRow = dgvSearchResults.FirstDisplayedScrollingRowIndex;

            dgvSearchResults.DataSource = null;
            dgvSearchResults.Columns.Clear();

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

            var btnCol = new DataGridViewButtonColumn
            {
                Name = "הוסף_הזמנה",
                HeaderText = "",
                Text = "➕ הוסף הזמנה",
                UseColumnTextForButtonValue = true,
                Width = 120
            };
            dgvSearchResults.Columns.Add(btnCol);

            foreach (var c in customers)
            {
                dgvSearchResults.Rows.Add(
                    c.CustomerId, c.Name, c.Gender,
                    c.Phone, c.Email, c.Age, "");
            }

            for (int i = 0; i < dgvSearchResults.Rows.Count; i++)
            {
                dgvSearchResults.Rows[i].Tag = customers[i].CustomerId;
            }

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

        private void DgvSearchResults_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvSearchResults.Rows[e.RowIndex].Tag is int customerId)
            {
                string customerName = dgvSearchResults.Rows[e.RowIndex].Cells["שם"].Value?.ToString() ?? "";
                OpenCustomerDetailsTab(customerId, customerName);
            }
        }

        private async void OpenCustomerDetailsTab(int customerId, string customerName)
        {
            currentSelectedCustomerId = customerId;

            TabPage? existingTab = _tabs.TabPages.Cast<TabPage>().FirstOrDefault(t => t.Text == $"לקוח: {customerName}");

            if (existingTab != null)
            {
                _tabs.SelectedTab = existingTab;
                await LoadCustomerOrdersData(customerId);
                return;
            }

            var tab = new TabPage($"לקוח: {customerName}");

            lblCustomerInfo = new Label
            {
                Text = $"פרטי הלקוח: {customerName} (מזהה: {customerId})",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            tab.Controls.Add(lblCustomerInfo);

            dgvCustomerOrders = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(820, 370),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };
            tab.Controls.Add(dgvCustomerOrders);

            var btnDeleteOrder = new Button
            {
                Text = "🗑 מחק הזמנה נבחרה",
                Location = new Point(20, 445),
                Size = new Size(150, 35),
                BackColor = Color.LightCoral,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnDeleteOrder.Click += BtnDeleteOrder_Click;
            tab.Controls.Add(btnDeleteOrder);

            _tabs.TabPages.Add(tab);
            _tabs.SelectedTab = tab;

            await LoadCustomerOrdersData(customerId);
        }

        private async Task LoadCustomerOrdersData(int customerId)
        {
            try
            {
                var allOrders = await _client.GetFromJsonAsync<List<Order>>("api/Orders") ?? new List<Order>();
                var customerOrders = allOrders.Where(o => o.CustomerId == customerId).ToList();

                dgvCustomerOrders.DataSource = customerOrders.Select(o => new
                {
                    מזהה_הזמנה = o.Id,
                    מזהה_פיצה = o.PizzaId,
                    תאריך_יצירה = o.CreatingDate?.ToString("dd/MM/yyyy HH:mm"),
                    משוב = o.Feedback ?? "",
                    סטטוס = o.IsClose ? "סגור ✅" : "פתוח 🟡"
                }).ToList();
            }
            catch (Exception)
            {
                MessageBox.Show("שגיאה בטעינת ההזמנות של הלקוח", "שגיאה", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnDeleteOrder_Click(object? sender, EventArgs e)
        {
            if (dgvCustomerOrders.SelectedRows.Count == 0)
            {
                MessageBox.Show("אנא בחרי הזמנה מתוך הטבלה למחיקה", "שים לב", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int orderId = (int)dgvCustomerOrders.SelectedRows[0].Cells["מזהה_הזמנה"].Value;

            var confirmResult = MessageBox.Show($"האם את בטוחה שברצונך למחוק את הזמנה מספר {orderId}?",
                                                 "אישור מחיקה",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    var response = await _client.DeleteAsync($"api/Orders/{orderId}");

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("ההזמנה נמחקה בהצלחה!", "הצלחה", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await LoadCustomerOrdersData(currentSelectedCustomerId);
                    }
                    else
                    {
                        MessageBox.Show("שגיאה במחיקת ההזמנה מול השרת.", "שגיאה", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("שגיאת תקשורת מול השרת.", "שגיאת תקשורת", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}