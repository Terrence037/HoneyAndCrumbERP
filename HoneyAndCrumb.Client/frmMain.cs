using System;
using System.Windows.Forms;

namespace HoneyAndCrumb.Client
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            var renderer = Theme.CreateMenuRenderer();
            menuStrip1.Renderer = renderer;
            statusStrip1.Renderer = renderer;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            lblUser.Text = string.Format("Signed in: {0} ({1})", Session.FullName, Session.Role);
            lblPlan.Text = string.Format("{0}  •  Plan: {1}", Session.CompanyName, Session.SubscriptionPlan);

            menuDashboard.Visible = Session.CanAccess("Dashboard");
            menuSales.Visible = Session.CanAccess("Sales");
            menuPayment.Visible = Session.CanAccess("Record payment");
            menuOrderStatus.Visible = Session.CanAccess("Track order status");
            menuCustomers.Visible = Session.CanAccess("Customers");
            menuMenu.Visible = Session.CanAccess("Menu");
            menuIngredients.Visible = Session.CanAccess("Ingredients");
            menuSupplyChain.Visible = Session.CanAccess("Supply chain");
            menuOpReports.Visible = Session.CanAccess("Operational reports");
            menuBI.Visible = Session.CanAccess("Business intelligence");
            menuHR.Visible = Session.CanAccess("Human resource");
            menuPayroll.Visible = Session.CanAccess("Payroll");
            menuFinancials.Visible = Session.CanAccess("Financial statements");
            menuBranching.Visible = Session.CanAccess("Branching");
            menuStaffPerms.Visible = string.Equals(Session.Role, "Owner", StringComparison.OrdinalIgnoreCase);
            menuClients.Visible = Session.CanAccess("Manage clients");
            menuSubscriptions.Visible = Session.CanAccess("Subscriptions");
        }

        private void OpenChildForm(Form child)
        {
            foreach (Control c in pnlContent.Controls)
            {
                if (c is Form oldForm) oldForm.Close();
            }
            pnlContent.Controls.Clear();

            child.TopLevel = false;
            child.FormBorderStyle = FormBorderStyle.None;
            child.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(child);
            child.Show();
        }

        private void menuLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Application.Restart();
        }
    }
}