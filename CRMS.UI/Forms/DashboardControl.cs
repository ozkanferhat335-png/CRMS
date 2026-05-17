using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CRMS.Business.Services;

namespace CRMS.UI.Forms
{
    public partial class DashboardControl : UserControl
    {
        public DashboardControl()
        {
            InitializeComponent();
            LoadDashboardData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "DashboardControl";
            this.Size = new Size(1000, 650);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.ResumeLayout(false);
        }

        private void LoadDashboardData()
        {
            try
            {
                var dashboardService = new DashboardService(Program.UnitOfWork);
                var data = dashboardService.GetDashboardData();

                // Title
                Label lblTitle = new Label
                {
                    Text = "DASHBOARD",
                    Location = new Point(20, 20),
                    Size = new Size(400, 35),
                    Font = new Font("Arial", 18, FontStyle.Bold),
                    ForeColor = Color.FromArgb(45, 45, 48)
                };
                this.Controls.Add(lblTitle);

                Label lblDate = new Label
                {
                    Text = DateTime.Now.ToString("dddd, dd MMMM yyyy"),
                    Location = new Point(20, 58),
                    Size = new Size(400, 22),
                    Font = new Font("Arial", 10),
                    ForeColor = Color.FromArgb(120, 120, 120)
                };
                this.Controls.Add(lblDate);

                // Stat Cards Row 1
                int cardY = 90;
                CreateStatCard("Toplam Müşteri", data.TotalCustomers.ToString(), "👥", 20, cardY, Color.FromArgb(52, 152, 219));
                CreateStatCard("Aktif Müşteri", data.ActiveCustomers.ToString(), "✅", 230, cardY, Color.FromArgb(46, 204, 113));
                CreateStatCard("Toplam Satış", $"₺{data.TotalSalesAmount:N0}", "💰", 440, cardY, Color.FromArgb(155, 89, 182));
                CreateStatCard("Yeni Teklifler", data.NewOffers.ToString(), "📄", 650, cardY, Color.FromArgb(230, 126, 34));
                CreateStatCard("Bekleyen Görevler", data.PendingTasks.ToString(), "📋", 860, cardY, Color.FromArgb(26, 188, 156));

                // Stat Cards Row 2
                cardY = 240;
                CreateStatCard("Açık Destek Talepleri", data.OpenTickets.ToString(), "🎫", 20, cardY, Color.FromArgb(231, 76, 60));

                // Recent Activities
                Label lblActivities = new Label
                {
                    Text = "Son Aktiviteler",
                    Location = new Point(20, 390),
                    Size = new Size(300, 28),
                    Font = new Font("Arial", 13, FontStyle.Bold),
                    ForeColor = Color.FromArgb(45, 45, 48)
                };
                this.Controls.Add(lblActivities);

                var dgvActivities = new DataGridView
                {
                    Location = new Point(20, 425),
                    Size = new Size(600, 200),
                    Font = new Font("Arial", 9),
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    ReadOnly = true,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    BackgroundColor = Color.White,
                    BorderStyle = BorderStyle.None,
                    RowHeadersVisible = false,
                    AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(248, 248, 248) }
                };
                dgvActivities.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9, FontStyle.Bold);
                dgvActivities.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
                dgvActivities.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvActivities.EnableHeadersVisualStyles = false;
                dgvActivities.Columns.Add("Action", "İşlem");
                dgvActivities.Columns.Add("Description", "Açıklama");
                dgvActivities.Columns.Add("Date", "Tarih");

                if (data.RecentActivities != null)
                {
                    foreach (var activity in data.RecentActivities)
                    {
                        dgvActivities.Rows.Add(
                            activity.ActivityType,
                            activity.Description,
                            activity.CreatedDate.ToString("dd/MM/yyyy HH:mm")
                        );
                    }
                }
                this.Controls.Add(dgvActivities);

                // Quick Stats Panel
                Label lblQuickStats = new Label
                {
                    Text = "Hızlı İstatistikler",
                    Location = new Point(640, 390),
                    Size = new Size(300, 28),
                    Font = new Font("Arial", 13, FontStyle.Bold),
                    ForeColor = Color.FromArgb(45, 45, 48)
                };
                this.Controls.Add(lblQuickStats);

                var statsPanel = new Panel
                {
                    Location = new Point(640, 425),
                    Size = new Size(380, 200),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.None
                };

                AddQuickStat(statsPanel, "Toplam Müşteri", data.TotalCustomers.ToString(), 10, 10, Color.FromArgb(52, 152, 219));
                AddQuickStat(statsPanel, "Aktif Müşteri", data.ActiveCustomers.ToString(), 10, 50, Color.FromArgb(46, 204, 113));
                AddQuickStat(statsPanel, "Bekleyen Görev", data.PendingTasks.ToString(), 10, 90, Color.FromArgb(230, 126, 34));
                AddQuickStat(statsPanel, "Açık Destek Talebi", data.OpenTickets.ToString(), 10, 130, Color.FromArgb(231, 76, 60));
                AddQuickStat(statsPanel, "Bekleyen Teklif", data.NewOffers.ToString(), 10, 170, Color.FromArgb(155, 89, 182));

                this.Controls.Add(statsPanel);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dashboard yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateStatCard(string title, string value, string icon, int x, int y, Color color)
        {
            Panel panel = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(195, 130),
                BackColor = color
            };

            Label lblIcon = new Label
            {
                Text = icon,
                Location = new Point(10, 10),
                Size = new Size(40, 40),
                Font = new Font("Arial", 20),
                ForeColor = Color.FromArgb(255, 255, 255, 180),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panel.Controls.Add(lblIcon);

            Label lblTitle = new Label
            {
                Text = title,
                Location = new Point(5, 55),
                Size = new Size(185, 30),
                Font = new Font("Arial", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 220, 220),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panel.Controls.Add(lblTitle);

            Label lblValue = new Label
            {
                Text = value,
                Location = new Point(5, 85),
                Size = new Size(185, 38),
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panel.Controls.Add(lblValue);

            this.Controls.Add(panel);
        }

        private void AddQuickStat(Panel parent, string label, string value, int x, int y, Color color)
        {
            var lbl = new Label
            {
                Text = label + ":",
                Location = new Point(x, y + 3),
                Size = new Size(200, 22),
                Font = new Font("Arial", 10),
                ForeColor = Color.FromArgb(80, 80, 80)
            };
            parent.Controls.Add(lbl);

            var val = new Label
            {
                Text = value,
                Location = new Point(x + 210, y + 3),
                Size = new Size(80, 22),
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = color,
                TextAlign = ContentAlignment.MiddleRight
            };
            parent.Controls.Add(val);
        }
    }
}
