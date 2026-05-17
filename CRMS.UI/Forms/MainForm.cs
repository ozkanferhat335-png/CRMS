using System;
using System.Drawing;
using System.Windows.Forms;
using CRMS.Business.Services;

namespace CRMS.UI.Forms
{
    public partial class MainForm : Form
    {
        private Panel pnlSidebar;
        private Panel pnlContent;
        private Panel pnlHeader;
        private Label lblWelcome;
        private Label lblDateTime;
        private Label lblNotifications;
        private Button btnDashboard;
        private Button btnCustomers;
        private Button btnMeetings;
        private Button btnSales;
        private Button btnTasks;
        private Button btnOffers;
        private Button btnTickets;
        private Button btnReports;
        private Button btnSettings;
        private Button btnLogout;
        private Button _activeButton;
        private System.Windows.Forms.Timer _clockTimer;
        private System.Windows.Forms.Timer _notifTimer;
        private readonly NotificationService _notificationService;

        public MainForm()
        {
            InitializeComponent();
            _notificationService = new NotificationService(Program.UnitOfWork);
            InitializeCustomComponents();
            LoadDashboard();
            StartTimers();
        }

        private void InitializeCustomComponents()
        {
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Header Panel
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(30, 30, 35)
            };

            lblWelcome = new Label
            {
                Text = $"Hoşgeldiniz, {Program.CurrentUser?.FirstName} {Program.CurrentUser?.LastName}  |  Rol: {Program.CurrentUser?.RoleName}",
                Location = new Point(270, 0),
                Size = new Size(600, 50),
                ForeColor = Color.White,
                Font = new Font("Arial", 10),
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlHeader.Controls.Add(lblWelcome);

            lblDateTime = new Label
            {
                Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                Location = new Point(900, 0),
                Size = new Size(200, 50),
                ForeColor = Color.FromArgb(200, 200, 200),
                Font = new Font("Arial", 10),
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlHeader.Controls.Add(lblDateTime);

            lblNotifications = new Label
            {
                Text = "🔔 0",
                Location = new Point(1110, 0),
                Size = new Size(80, 50),
                ForeColor = Color.FromArgb(241, 196, 15),
                Font = new Font("Arial", 11, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            };
            lblNotifications.Click += (s, e) => ShowNotifications();
            pnlHeader.Controls.Add(lblNotifications);

            this.Controls.Add(pnlHeader);

            // Sidebar Panel
            pnlSidebar = new Panel
            {
                Location = new Point(0, 50),
                Width = 250,
                BackColor = Color.FromArgb(45, 45, 48),
                Dock = DockStyle.None
            };
            pnlSidebar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;

            // App Logo/Title
            var lblAppTitle = new Label
            {
                Text = "🏢 CRM SİSTEMİ",
                Location = new Point(0, 10),
                Size = new Size(250, 40),
                ForeColor = Color.FromArgb(52, 152, 219),
                Font = new Font("Arial", 13, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlSidebar.Controls.Add(lblAppTitle);

            var separator = new Panel { Location = new Point(10, 55), Size = new Size(230, 1), BackColor = Color.FromArgb(70, 70, 75) };
            pnlSidebar.Controls.Add(separator);

            // Navigation Buttons
            int btnY = 65;
            btnDashboard = CreateSidebarButton("📊  Dashboard", btnY); btnDashboard.Click += (s, e) => { SetActiveButton(btnDashboard); LoadDashboard(); }; pnlSidebar.Controls.Add(btnDashboard); btnY += 48;
            btnCustomers = CreateSidebarButton("👥  Müşteriler", btnY); btnCustomers.Click += (s, e) => { SetActiveButton(btnCustomers); LoadControl(new CustomersControl()); }; pnlSidebar.Controls.Add(btnCustomers); btnY += 48;
            btnMeetings = CreateSidebarButton("📞  Görüşmeler", btnY); btnMeetings.Click += (s, e) => { SetActiveButton(btnMeetings); LoadControl(new MeetingsControl()); }; pnlSidebar.Controls.Add(btnMeetings); btnY += 48;
            btnSales = CreateSidebarButton("💰  Satışlar", btnY); btnSales.Click += (s, e) => { SetActiveButton(btnSales); LoadControl(new SalesControl()); }; pnlSidebar.Controls.Add(btnSales); btnY += 48;
            btnTasks = CreateSidebarButton("✅  Görevler", btnY); btnTasks.Click += (s, e) => { SetActiveButton(btnTasks); LoadControl(new TasksControl()); }; pnlSidebar.Controls.Add(btnTasks); btnY += 48;
            btnOffers = CreateSidebarButton("📄  Teklifler", btnY); btnOffers.Click += (s, e) => { SetActiveButton(btnOffers); LoadControl(new OffersControl()); }; pnlSidebar.Controls.Add(btnOffers); btnY += 48;
            btnTickets = CreateSidebarButton("🎫  Destek Talepleri", btnY); btnTickets.Click += (s, e) => { SetActiveButton(btnTickets); LoadControl(new TicketsControl()); }; pnlSidebar.Controls.Add(btnTickets); btnY += 48;
            btnReports = CreateSidebarButton("📈  Raporlar", btnY); btnReports.Click += (s, e) => { SetActiveButton(btnReports); LoadControl(new ReportsControl()); }; pnlSidebar.Controls.Add(btnReports); btnY += 48;
            btnSettings = CreateSidebarButton("⚙️  Ayarlar", btnY); btnSettings.Click += (s, e) => { SetActiveButton(btnSettings); LoadControl(new SettingsControl()); }; pnlSidebar.Controls.Add(btnSettings);

            // Logout Button
            btnLogout = new Button
            {
                Text = "🚪  Çıkış Yap",
                Location = new Point(10, this.ClientSize.Height - 110),
                Size = new Size(230, 45),
                BackColor = Color.FromArgb(192, 57, 43),
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += BtnLogout_Click;
            pnlSidebar.Controls.Add(btnLogout);

            // Content Panel
            pnlContent = new Panel
            {
                BackColor = Color.FromArgb(240, 240, 240)
            };

            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlSidebar);

            this.Resize += MainForm_Resize;
            AdjustLayout();

            SetActiveButton(btnDashboard);
        }

        private void AdjustLayout()
        {
            pnlSidebar.Location = new Point(0, 50);
            pnlSidebar.Size = new Size(250, this.ClientSize.Height - 50);

            pnlContent.Location = new Point(250, 50);
            pnlContent.Size = new Size(this.ClientSize.Width - 250, this.ClientSize.Height - 50);

            if (btnLogout != null)
                btnLogout.Location = new Point(10, pnlSidebar.Height - 60);
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            AdjustLayout();
        }

        private Button CreateSidebarButton(string text, int yPosition)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(5, yPosition),
                Size = new Size(240, 42),
                BackColor = Color.FromArgb(60, 60, 65),
                ForeColor = Color.FromArgb(200, 200, 200),
                Font = new Font("Arial", 10),
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(75, 75, 80);
            return btn;
        }

        private void SetActiveButton(Button btn)
        {
            if (_activeButton != null)
            {
                _activeButton.BackColor = Color.FromArgb(60, 60, 65);
                _activeButton.ForeColor = Color.FromArgb(200, 200, 200);
            }

            _activeButton = btn;
            if (_activeButton != null)
            {
                _activeButton.BackColor = Color.FromArgb(52, 152, 219);
                _activeButton.ForeColor = Color.White;
            }
        }

        private void LoadControl(UserControl control)
        {
            pnlContent.Controls.Clear();
            control.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(control);
        }

        private void LoadDashboard()
        {
            pnlContent.Controls.Clear();
            var dashboard = new DashboardControl();
            dashboard.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(dashboard);
        }

        private void ShowNotifications()
        {
            try
            {
                var notifications = _notificationService.GetUnreadNotifications(Program.CurrentUser?.Id ?? 0);
                if (notifications.Count == 0)
                {
                    MessageBox.Show("Okunmamış bildiriminiz bulunmamaktadır.", "Bildirimler", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string msg = $"Okunmamış {notifications.Count} bildirim:\n\n";
                foreach (var n in notifications)
                    msg += $"• [{n.NotificationType}] {n.Title}: {n.Message}\n";

                if (MessageBox.Show(msg + "\nTümünü okundu olarak işaretle?", "Bildirimler", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    _notificationService.MarkAllAsRead(Program.CurrentUser?.Id ?? 0);
                    UpdateNotificationBadge();
                }
            }
            catch { }
        }

        private void UpdateNotificationBadge()
        {
            try
            {
                int count = _notificationService.GetUnreadCount(Program.CurrentUser?.Id ?? 0);
                lblNotifications.Text = $"🔔 {count}";
                lblNotifications.ForeColor = count > 0 ? Color.FromArgb(241, 196, 15) : Color.FromArgb(150, 150, 150);
            }
            catch { }
        }

        private void StartTimers()
        {
            _clockTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _clockTimer.Tick += (s, e) => lblDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            _clockTimer.Start();

            _notifTimer = new System.Windows.Forms.Timer { Interval = 30000 };
            _notifTimer.Tick += (s, e) => UpdateNotificationBadge();
            _notifTimer.Start();

            UpdateNotificationBadge();
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Çıkmak istediğinizden emin misiniz?", "Çıkış", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _clockTimer?.Stop();
                _notifTimer?.Stop();
                Program.LoggingService.LogLogout(Program.CurrentUser?.Id ?? 0);
                Program.CurrentUser = null;
                this.Close();
                new LoginForm().Show();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _clockTimer?.Stop();
            _notifTimer?.Stop();
            base.OnFormClosed(e);
        }
    }
}
