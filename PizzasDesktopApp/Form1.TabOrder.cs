using System.Drawing;
using System.Windows.Forms;
using Entities;

namespace PizzasDesktopApp
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

            tab.Enter += (s, e) => LoadOrderCombos();

            return tab;
        }

        private void LoadOrderCombos()
        {
            var pizzas = _pizzaService.GetAllPizzas();
            cmbOrderPizza.DataSource = pizzas;
            cmbOrderPizza.DisplayMember = "Name";
            cmbOrderPizza.ValueMember = "PizzaId";

            var customers = _pizzaService.SearchCustomers("");
            cmbOrderCustomer.DataSource = customers;
            cmbOrderCustomer.DisplayMember = "Name";
            cmbOrderCustomer.ValueMember = "CustomerId";
        }

        private void BtnCreateOrder_Click(object? sender, EventArgs e)
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

            int result = _pizzaService.AddOrder(order);

            if (result > 0)
            {
                lblOrderResult.Text = $"✔ הזמנה נוצרה בהצלחה! מזהה: {result}";
                lblOrderResult.ForeColor = Color.DarkGreen;
                txtFeedback.Text = "";
            }
            else
            {
                lblOrderResult.Text = "שגיאה ביצירת הזמנה";
                lblOrderResult.ForeColor = Color.Red;
            }
        }
    }
}