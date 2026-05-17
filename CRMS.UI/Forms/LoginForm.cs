using System;
using System.Windows.Forms;
using CRMS.Business.Services;
using CRMS.Entity.DTOs;

namespace CRMS.UI.Forms
{
    public partial class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnExit;
        private Label lblUsername;
        private Label lblPassword;
        private CheckBox chkRememberMe;
        private Label lblTitle;

        public LoginForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            this.FormClosed += LoginForm_FormClosed;
        }

        private void InitializeCustomComponents()
        {
            // Title
            lblTitle = new Label
            {
                Text = "CRM SİSTEMİ GİRİŞ",
                Location = new System.Drawing.Point(50, 30),
                Size = new System.Drawing.Size(300, 30),
                Font = new System.Drawing.Font("Arial", 16, System.Drawing.FontStyle.Bold),
                TextAlign = System.Windows.Forms.ContentAlignment.MiddleCenter
            };

            // Username Label
            lblUsername = new Label
            {
                Text = "Kullanıcı Adı:",
                Location = new System.Drawing.Point(50, 80),
                Size = new System.Drawing.Size(300, 20),
                Font = new System.Drawing.Font("Arial", 10)
            };

            // Username TextBox
            txtUsername = new TextBox
            {
                Location = new System.Drawing.Point(50, 105),
                Size = new System.Drawing.Size(300, 25),
                Font = new System.Drawing.Font("Arial", 10),
                Text = "admin" // Default for testing
            };

            // Password Label
            lblPassword = new Label
            {
                Text = "Parola:",
                Location = new System.Drawing.Point(50, 140),
                Size = new System.Drawing.Size(300, 20),
                Font = new System.Drawing.Font("Arial", 10)
            };

            // Password TextBox
            txtPassword = new TextBox
            {
                Location = new System.Drawing.Point(50, 165),
                Size = new System.Drawing.Size(300, 25),
                Font = new System.Drawing.Font("Arial", 10),
                UseSystemPasswordChar = true,
                Text = "admin123" // Default for testing
            };

            // Remember Me CheckBox
            chkRememberMe = new CheckBox
            {
                Text = "Beni Hatırla",
                Location = new System.Drawing.Point(50, 200),
                Size = new System.Drawing.Size(150, 20),
                Font = new System.Drawing.Font("Arial", 10)
            };

            // Login Button
            btnLogin = new Button
            {
                Text = "Giriş Yap",
                Location = new System.Drawing.Point(50, 240),
                Size = new System.Drawing.Size(140, 35),
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold),
                BackColor = System.Drawing.Color.FromArgb(0, 120, 215),
                ForeColor = System.Drawing.Color.White,
                Cursor = System.Windows.Forms.Cursors.Hand
            };
            btnLogin.Click += BtnLogin_Click;

            // Exit Button
            btnExit = new Button
            {
                Text = "Çıkış",
                Location = new System.Drawing.Point(210, 240),
                Size = new System.Drawing.Size(140, 35),
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold),
                BackColor = System.Drawing.Color.FromArgb(220, 53, 69),
                ForeColor = System.Drawing.Color.White,
                Cursor = System.Windows.Forms.Cursors.Hand
            };
            btnExit.Click += BtnExit_Click;

            // Add controls to form
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(chkRememberMe);
            this.Controls.Add(btnLogin);
            this.Controls.Add(btnExit);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                PerformLogin();
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Lütfen kullanıcı adını giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Lütfen parolayı giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void PerformLogin()
        {
            try
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                var response = Program.AuthenticationService.Login(txtUsername.Text, txtPassword.Text);

                if (response.IsSuccess)
                {
                    // Save user info
                    Program.CurrentUser = new UserDTO
                    {
                        Id = response.User.Id,
                        Username = response.User.Username,
                        FirstName = response.User.FirstName,
                        LastName = response.User.LastName,
                        RoleId = response.User.RoleId,
                        RoleName = response.User.RoleName,
                        LastLoginDate = response.User.LastLoginDate
                    };

                    // Log login
                    Program.LoggingService.LogLogin(Program.CurrentUser.Id);

                    // Open Main Form
                    MainForm mainForm = new MainForm();
                    this.Hide();
                    mainForm.Show();
                }
                else
                {
                    MessageBox.Show(response.Message, "Giriş Başarısız", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Giriş sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Uygulamadan çıkmak istiyor musunuz?", "Çıkış", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}