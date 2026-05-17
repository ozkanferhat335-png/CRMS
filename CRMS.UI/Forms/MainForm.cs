using System;
using System.Windows.Forms;

namespace CRMS.UI.Forms
{
    public partial class MainForm : Form
    {
        private Panel pnlSidebar;
        private Panel pnlContent;
        private Label lblWelcome;
        private Button btnCustomers;
        private Button btnSales;
        private Button btnTasks;
        private Button btnOffers;
        private Button btnTickets;
        private Button btnReports;
        private Button btnSettings;
        private Button btnLogout;

        public MainForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            LoadDashboard();
        }

        private void InitializeCustomComponents()
        {
            // Sidebar Panel
            pnlSidebar = new Panel
            {
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(250, this.ClientSize.Height),
                BackColor = System.Drawing.Color.FromArgb(45, 45, 48),
                Dock = DockStyle.Left
            };

            // Welcome Label
            lblWelcome = new Label
            {
                Text = $"Hoşgeldiniz, {Program.CurrentUser?.FirstName}!",
                Location = new System.Drawing.Point(10, 20),
                Size = new System.Drawing.Size(230, 60),
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold),
                AutoSize = false,
                TextAlign = System.Windows.Forms.ContentAlignment.MiddleLeft
            };
            pnlSidebar.Controls.Add(lblWelcome);

            // Customers Button
            btnCustomers = CreateSidebarButton("Müşteriler", 90);
            btnCustomers.Click += BtnCustomers_Click;
            pnlSidebar.Controls.Add(btnCustomers);

            // Sales Button
            btnSales = CreateSidebarButton("Satışlar", 140);
            btnSales.Click += BtnSales_Click;
            pnlSidebar.Controls.Add(btnSales);

            // Tasks Button
            btnTasks = CreateSidebarButton("Görevler", 190);
            btnTasks.Click += BtnTasks_Click;
            pnlSidebar.Controls.Add(btnTasks);

            // Offers Button
            btnOffers = CreateSidebarButton("Teklifler", 240);
            btnOffers.Click += BtnOffers_Click;
            pnlSidebar.Controls.Add(btnOffers);

            // Tickets Button
            btnTickets = CreateSidebarButton("Destek Talepleri", 290);
            btnTickets.Click += BtnTickets_Click;
            pnlSidebar.Controls.Add(btnTickets);

            // Reports Button
            btnReports = CreateSidebarButton("Raporlar", 340);
            btnReports.Click += BtnReports_Click;
            pnlSidebar.Controls.Add(btnReports);

            // Settings Button
            btnSettings = CreateSidebarButton("Ayarlar", 390);
            btnSettings.Click += BtnSettings_Click;
            pnlSidebar.Controls.Add(btnSettings);

            // Logout Button
            btnLogout = new Button
            {
                Text = "Çıkış Yap",
                Location = new System.Drawing.Point(10, this.ClientSize.Height - 60),
                Size = new System.Drawing.Size(230, 45),
                BackColor = System.Drawing.Color.FromArgb(220, 53, 69),
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold),
                Cursor = System.Windows.Forms.Cursors.Hand
            };
            btnLogout.Click += BtnLogout_Click;
            pnlSidebar.Controls.Add(btnLogout);

            // Content Panel
            pnlContent = new Panel
            {
                Location = new System.Drawing.Point(250, 0),
                Size = new System.Drawing.Size(this.ClientSize.Width - 250, this.ClientSize.Height),
                BackColor = System.Drawing.Color.FromArgb(240, 240, 240),
                Dock = DockStyle.Fill
            };

            // Add panels to form
            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlSidebar);
        }

        private Button CreateSidebarButton(string text, int yPosition)
        {
            return new Button
            {
                Text = text,
                Location = new System.Drawing.Point(10, yPosition),
                Size = new System.Drawing.Size(230, 40),
                BackColor = System.Drawing.Color.FromArgb(60, 60, 65),
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Arial", 11),
                Cursor = System.Windows.Forms.Cursors.Hand,
                FlatStyle = FlatStyle.Flat
            };
        }

        private void LoadDashboard()
        {
            pnlContent.Controls.Clear();
            var dashboard = new DashboardControl();
            dashboard.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(dashboard);
        }

        private void BtnCustomers_Click(object sender, EventArgs e)
        {
            pnlContent.Controls.Clear();
            var customersControl = new CustomersControl();
            customersControl.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(customersControl);
        }

        private void BtnSales_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Satışlar modülü yakında eklenecek.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnTasks_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Görevler modülü yakında eklenecek.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnOffers_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Teklifler modülü yakında eklenecek.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnTickets_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Destek Talepleri modülü yakında eklenecek.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnReports_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Raporlar modülü yakında eklenecek.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Ayarlar modülü yakında eklenecek.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Çıkmak istediğinizden emin misiniz?", "Çıkış", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Program.LoggingService.LogLogout(Program.CurrentUser?.Id ?? 0);
                Program.CurrentUser = null;
                this.Close();
                new LoginForm().Show();
            }
        }
    }
}