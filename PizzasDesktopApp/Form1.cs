using System.Drawing;
using System.Windows.Forms;
using Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Service;
using ServiceInterface;

namespace PizzasDesktopApp
{
    public partial class Form1 : Form
    {
        private readonly IPizzaService _pizzaService;
        private TabControl _tabs = null!;

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

        public Form1()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<PizzaStoreContext>();
            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\MSSQLLocalDB;Database=OfakimPizzaDB;" +
                "Trusted_Connection=True;TrustServerCertificate=True;");

            PizzaStoreContext context = new PizzaStoreContext(optionsBuilder.Options);
            PizzaInfrastructure infra = new PizzaInfrastructure(context);
            _pizzaService = new PizzaServices(infra);

            BuildUI();

            this.Load += (s, e) => LoadReminders();
        }

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