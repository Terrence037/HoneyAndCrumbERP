using System;
using System.Windows.Forms;

namespace HoneyAndCrumb.Client
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
            lblError.Text = string.Empty;
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            lblError.Text = string.Empty;

            if (string.IsNullOrWhiteSpace(txtUser.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblError.Text = "Please enter your username and password.";
                return;
            }

            btnLogin.Enabled = false;
            btnLogin.Text = "Signing in...";

            try
            {
                var req = new LoginRequest
                {
                    CompanyCode = txtCompany.Text.Trim(),
                    Username = txtUser.Text.Trim(),
                    Password = txtPassword.Text
                };

                var res = await ApiClient.PostAsync<LoginRequest, LoginResponse>("auth/login", req);

                Session.CompanyId = res.companyId;
                Session.CompanyCode = res.companyCode;
                Session.CompanyName = res.companyName;
                Session.Username = res.username;
                Session.FullName = res.fullName;
                Session.Role = res.role;
                Session.SubscriptionPlan = res.subscriptionPlan;
                Session.SetPermissions(res.permissions);

                this.Hide();
                var main = new frmMain();
                main.FormClosed += (s, args) => this.Close();
                main.Show();
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.Contains("401")
                    ? "Invalid company code, username, or password."
                    : "Unable to sign in: " + ex.Message;
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "Log in";
            }
        }

        private class LoginRequest
        {
            public string CompanyCode { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }

        private class LoginResponse
        {
            public int companyId { get; set; }
            public string companyCode { get; set; }
            public string companyName { get; set; }
            public string username { get; set; }
            public string fullName { get; set; }
            public string role { get; set; }
            public string subscriptionPlan { get; set; }
            public string[] permissions { get; set; }
        }
    }
}