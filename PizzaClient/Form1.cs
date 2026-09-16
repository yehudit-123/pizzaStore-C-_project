using System.Drawing;
using System.Windows.Forms;
using Entities;
using System.Net.Http.Json;

namespace PizzaClient
{
    public partial class Form1 : Form
    {
        private readonly HttpClient _client;
        private TabControl _tabs = null!;

        public Form1()
        {
            InitializeComponent();

            // 2. אתחול הלקוח עם הכתובת של ה-Swagger שלך
            _client = new HttpClient();
            _client.BaseAddress = new Uri("https://localhost:7205/");

            // 3. מחיקת כל הקוד הישן שהגדיר DbContext וחיבור ל-SQL Server!

            BuildUI();
            this.Load += (s, e) => LoadReminders();
        }

        // לשונית 1 - קליטת לקוח
        private TextBox txtCustName = null!;
        private TextBox txtCustPhone = null!;
        private TextBox txtCustEmail = null!;
        private TextBox txtCustAge = null!;
        private ComboBox cmbGender = null!;
        private Label lblCustResult = null!;

        // לשונית 2 - חיפוש לקוח
        private TextBox txtSearch = null!;
        private DataGridView dgvSearchResults = null!;

        // לשונית 3 - יצירת הזמנה
        private ComboBox cmbOrderCustomer = null!;
        private ComboBox cmbOrderPizza = null!;
        private TextBox txtFeedback = null!;
        private DateTimePicker dtpReminder = null!;
        private Label lblOrderResult = null!;

        // לשונית 4 - תזכורות
        private DataGridView dgvReminders = null!;

        private void BuildUI()
        {
            this.Text = "מערכת ניהול פיצות - אופקים";
            this.Size = new Size(900, 600);
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = Color.WhiteSmoke;

            _tabs = new TabControl   
            {
                Location = new Point(10, 10),
                Size = new Size(860, 540),
                Font = new Font("Segoe UI", 10)
            };

            _tabs.TabPages.Add(BuildTabAddCustomer());
            _tabs.TabPages.Add(BuildTabSearchCustomer());
            _tabs.TabPages.Add(BuildTabCreateOrder());
            _tabs.TabPages.Add(BuildTabReminders());

            this.Controls.Add(_tabs);
        }
        // ניווט ללשונית הזמנה עם לקוח מוכן
        public void NavigateToOrderWithCustomer(int customerId)
        {
            _tabs.SelectedIndex = 2; // לשונית "יצירת הזמנה"
            LoadOrderCombos();

            // בחירת הלקוח הנכון ב-ComboBox
            foreach (var item in cmbOrderCustomer.Items)
            {
                if (item is Entities.Customer c && c.CustomerId == customerId)
                {
                    cmbOrderCustomer.SelectedItem = item;
                    break;
                }
            }
        }
    }
}