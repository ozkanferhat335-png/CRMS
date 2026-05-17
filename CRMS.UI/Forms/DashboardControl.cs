using System;
using System.Windows.Forms;

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
            this.Size = new System.Drawing.Size(900, 600);
            this.ResumeLayout(false);
        }

        private void LoadDashboardData()
        {
            try
            {
                var dashboardService = new CRMS.Business.Services.DashboardService(Program.UnitOfWork);
                var dashboardData = dashboardService.GetDashboardData();

                // Create dashboard UI
                Label lblTitle = new Label
                {
                    Text = "DASHBOARD",
                    Location = new System.Drawing.Point(20, 20),
                    Size = new System.Drawing.Size(300, 30),
                    Font = new System.Drawing.Font("Arial", 16, System.Drawing.FontStyle.Bold)
                };
                this.Controls.Add(lblTitle);

                // Total Customers
                CreateStatisticPanel("Toplam Müşteriler", dashboardData.TotalCustomers.ToString(), 20, 70, System.Drawing.Color.FromArgb(52, 152, 219));

                // Active Customers
                CreateStatisticPanel("Aktif Müşteriler", dashboardData.ActiveCustomers.ToString(), 250, 70, System.Drawing.Color.FromArgb(46, 204, 113));

                // Total Sales
                CreateStatisticPanel("Toplam Satış", $"₺{dashboardData.TotalSalesAmount:N2}", 480, 70, System.Drawing.Color.FromArgb(155, 89, 182));

                // New Offers
                CreateStatisticPanel("Yeni Teklifler", dashboardData.NewOffers.ToString(), 710, 70, System.Drawing.Color.FromArgb(230, 126, 34));

                // Pending Tasks
                CreateStatisticPanel("Bekleyen Görevler", dashboardData.PendingTasks.ToString(), 20, 250, System.Drawing.Color.FromArgb(26, 188, 156));

                // Open Tickets
                CreateStatisticPanel("Açık Destek Talepleri", dashboardData.OpenTickets.ToString(), 250, 250, System.Drawing.Color.FromArgb(231, 76, 60));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dashboard yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateStatisticPanel(string title, string value, int x, int y, System.Drawing.Color color)
        {
            Panel panel = new Panel
            {
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(200, 150),
                BackColor = color,
                BorderStyle = BorderStyle.None
            };

            Label lblTitle = new Label
            {
                Text = title,
                Location = new System.Drawing.Point(10, 15),
                Size = new System.Drawing.Size(180, 50),
                Font = new System.Drawing.Font("Arial", 11, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.White,
                AutoSize = false,
                TextAlign = System.Windows.Forms.ContentAlignment.MiddleCenter
            };
            panel.Controls.Add(lblTitle);

            Label lblValue = new Label
            {
                Text = value,
                Location = new System.Drawing.Point(10, 65),
                Size = new System.Drawing.Size(180, 70),
                Font = new System.Drawing.Font("Arial", 24, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.White,
                AutoSize = false,
                TextAlign = System.Windows.Forms.ContentAlignment.MiddleCenter
            };
            panel.Controls.Add(lblValue);

            this.Controls.Add(panel);
        }
    }
}