using System;
using System.Drawing;
using System.Net.Http.Json;
using System.Windows.Forms;
using Entities;

namespace PizzaClient
{
    public partial class Form1
    {
        private TabPage BuildTabAddCustomer()
        {
            var tab = new TabPage("קליטת לקוח");
            int y = 20;
            int labelX = 700, inputX = 450;

            tab.Controls.Add(new Label { Text = "שם מלא:", Location = new Point(labelX, y), AutoSize = true });
            txtCustName = new TextBox { Location = new Point(inputX, y), Width = 200 };
            tab.Controls.Add(txtCustName);
            y += 40;

            tab.Controls.Add(new Label { Text = "טלפון:", Location = new Point(labelX, y), AutoSize = true });
            txtCustPhone = new TextBox { Location = new Point(inputX, y), Width = 200 };
            tab.Controls.Add(txtCustPhone);
            y += 40;

            tab.Controls.Add(new Label { Text = "אימייל:", Location = new Point(labelX, y), AutoSize = true });
            txtCustEmail = new TextBox { Location = new Point(inputX, y), Width = 200 };
            tab.Controls.Add(txtCustEmail);
            y += 40;

            tab.Controls.Add(new Label { Text = "גיל:", Location = new Point(labelX, y), AutoSize = true });
            txtCustAge = new TextBox { Location = new Point(inputX, y), Width = 200 };
            tab.Controls.Add(txtCustAge);
            y += 40;

            tab.Controls.Add(new Label { Text = "מגדר:", Location = new Point(labelX, y), AutoSize = true });
            cmbGender = new ComboBox
            {
                Location = new Point(inputX, y),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbGender.Items.AddRange(new[] { "בן", "בת" });
            cmbGender.SelectedIndex = 0;
            tab.Controls.Add(cmbGender);
            y += 50;

            var btnSave = new Button
            {
                Text = "שמור לקוח",
                Location = new Point(inputX, y),
                Size = new Size(140, 40),
                BackColor = Color.LightGreen,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSave.Click += BtnSaveCustomer_Click;
            tab.Controls.Add(btnSave);

            var btnClear = new Button
            {
                Text = "נקה",
                Location = new Point(inputX + 150, y),
                Size = new Size(80, 40),
                Font = new Font("Segoe UI", 10)
            };
            btnClear.Click += (s, e) => ClearCustomerForm();
            tab.Controls.Add(btnClear);
            y += 60;

            lblCustResult = new Label
            {
                Location = new Point(inputX, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            tab.Controls.Add(lblCustResult);

            return tab;
        }

        private async void BtnSaveCustomer_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustName.Text))
            {
                ShowCustomerResult("שגיאה: יש להזין שם", Color.Red);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCustPhone.Text) || txtCustPhone.Text.Length < 9)
            {
                ShowCustomerResult("שגיאה: טלפון לא תקין", Color.Red);
                return;
            }

            if (!string.IsNullOrWhiteSpace(txtCustEmail.Text) && !txtCustEmail.Text.Contains("@"))
            {
                ShowCustomerResult("שגיאה: אימייל לא תקין", Color.Red);
                return;
            }

            if (!string.IsNullOrWhiteSpace(txtCustAge.Text) && !int.TryParse(txtCustAge.Text, out _))
            {
                ShowCustomerResult("שגיאה: גיל חייב להיות מספר", Color.Red);
                return;
            }

            var customer = new Customer
            {
                Name = txtCustName.Text.Trim(),
                Phone = txtCustPhone.Text.Trim(),
                Email = txtCustEmail.Text.Trim(),
                Age = string.IsNullOrWhiteSpace(txtCustAge.Text) ? null : int.Parse(txtCustAge.Text),
                Gender = cmbGender.SelectedItem?.ToString()
            };

            try
            {
                var response = await _client.PostAsJsonAsync("api/Customers", customer);

                if (response.IsSuccessStatusCode)
                {
                    int result = await response.Content.ReadFromJsonAsync<int>();
                    ShowCustomerResult($"✔ לקוח נוסף בהצלחה! מזהה: {result}", Color.DarkGreen);
                    ClearCustomerForm();
                }
                else
                {
                    ShowCustomerResult("שגיאה בשמירה דרך השרת", Color.Red);
                }
            }
            catch (Exception)
            {
                ShowCustomerResult("שגיאת תקשורת - השרת לא זמין", Color.Red);
            }
        }

        private void ShowCustomerResult(string msg, Color color)
        {
            lblCustResult.Text = msg;
            lblCustResult.ForeColor = color;
        }

        private void ClearCustomerForm()
        {
            txtCustName.Text = "";
            txtCustPhone.Text = "";
            txtCustEmail.Text = "";
            txtCustAge.Text = "";
            cmbGender.SelectedIndex = 0;
            lblCustResult.Text = "";
        }
    }
}