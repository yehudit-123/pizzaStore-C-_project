using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Entities;

namespace PizzaClient
{
    public partial class Form1
    {
        private TabPage BuildTabCreateOrder()
        {
            var tab = new TabPage("יצירת הזמנה");
            int y = 20;
            int labelX = 700, inputX = 450;

            tab.Controls.Add(new Label { Text = "לקוח:", Location = new Point(labelX, y), AutoSize = true });
            cmbOrderCustomer = new ComboBox
            {
                Location = new Point(inputX, y),
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            tab.Controls.Add(cmbOrderCustomer);
            y += 40;

            tab.Controls.Add(new Label { Text = "פיצה:", Location = new Point(labelX, y), AutoSize = true });
            cmbOrderPizza = new ComboBox
            {
                Location = new Point(inputX, y),
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            tab.Controls.Add(cmbOrderPizza);
            y += 40;

            tab.Controls.Add(new Label { Text = "משוב:", Location = new Point(labelX, y), AutoSize = true });
            txtFeedback = new TextBox { Location = new Point(inputX, y), Width = 220 };
            tab.Controls.Add(txtFeedback);
            y += 40;

            tab.Controls.Add(new Label { Text = "תזכורת:", Location = new Point(labelX, y), AutoSize = true });
            dtpReminder = new DateTimePicker
            {
                Location = new Point(inputX, y),
                Width = 220,
                Value = DateTime.Now.AddDays(3)
            };
            tab.Controls.Add(dtpReminder);
            y += 50;

            var btnCreate = new Button
            {
                Text = "צור הזמנה",
                Location = new Point(inputX, y),
                Size = new Size(140, 40),
                BackColor = Color.LightGreen,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnCreate.Click += BtnCreateOrder_Click;
            tab.Controls.Add(btnCreate);
            y += 60;

            lblOrderResult = new Label
            {
                Location = new Point(inputX, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            tab.Controls.Add(lblOrderResult);

            // הפעלת טעינת הנתונים מהשרת ברגע שנכנסים ללשונית
            tab.Enter += async (s, e) => await LoadOrderCombos();

            return tab;
        }

        private async Task LoadOrderCombos()
        {
            try
            {
                // בקשת רשימת הפיצות מהשרת
                var pizzas = await _client.GetFromJsonAsync<List<Pizza>>("api/Pizzas") ?? new List<Pizza>();
                cmbOrderPizza.DataSource = pizzas;
                cmbOrderPizza.DisplayMember = "Name";
                cmbOrderPizza.ValueMember = "PizzaId";

                // בקשת רשימת הלקוחות מהשרת
                var customers = await _client.GetFromJsonAsync<List<Customer>>("api/Customers") ?? new List<Customer>();
                cmbOrderCustomer.DataSource = customers;
                cmbOrderCustomer.DisplayMember = "Name";
                cmbOrderCustomer.ValueMember = "CustomerId";
            }
            catch (Exception)
            {
                MessageBox.Show("שגיאה בטעינת הנתונים (לקוחות/פיצות) מהשרת", "שגיאת תקשורת", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnCreateOrder_Click(object? sender, EventArgs e)
        {
            if (cmbOrderCustomer.SelectedItem == null || cmbOrderPizza.SelectedItem == null)
            {
                lblOrderResult.Text = "שגיאה: יש לבחור לקוח ופיצה";
                lblOrderResult.ForeColor = Color.Red;
                return;
            }

            var order = new Order
            {
                CustomerId = (int)cmbOrderCustomer.SelectedValue!,
                PizzaId = (int)cmbOrderPizza.SelectedValue!,
                Feedback = txtFeedback.Text.Trim(),
                CreatingDate = DateTime.Now,
                ReminderDate = dtpReminder.Value,
                IsClose = false,
                ReminderSent = false
            };

            try
            {
                // שליחת ההזמנה לשרת לשמירה במסד הנתונים
                var response = await _client.PostAsJsonAsync("api/Orders", order);

                if (response.IsSuccessStatusCode)
                {
                    int result = await response.Content.ReadFromJsonAsync<int>();
                    lblOrderResult.Text = $"✔ הזמנה נוצרה בהצלחה! מזהה: {result}";
                    lblOrderResult.ForeColor = Color.DarkGreen;
                    txtFeedback.Text = "";
                }
                else
                {
                    lblOrderResult.Text = "שגיאה ביצירת הזמנה דרך השרת";
                    lblOrderResult.ForeColor = Color.Red;
                }
            }
            catch (Exception)
            {
                lblOrderResult.Text = "שגיאת תקשורת - השרת לא זמין";
                lblOrderResult.ForeColor = Color.Red;
            }
        }
    }
}