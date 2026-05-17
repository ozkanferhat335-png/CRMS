using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CRMS.Business.Services;

namespace CRMS.UI.Forms
{
    public class ReportsControl : UserControl
    {
        private readonly ReportService _reportService;
        private TabControl tabReports;

        public ReportsControl()
        {
            _reportService = new ReportService(Program.UnitOfWork);
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            this.BackColor = Color.FromArgb(245, 245, 245);

            Label lblTitle = new Label
            {
                Text = "RAPORLAR",
                Location = new Point(20, 20),
                Size = new Size(300, 30),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 45, 48)
            };
            this.Controls.Add(lblTitle);

            tabReports = new TabControl
            {
                Location = new Point(20, 60),
                Size = new Size(960, 520),
                Font = new Font("Arial", 10)
            };

            tabReports.TabPages.Add(CreateSalesReportTab());
            tabReports.TabPages.Add(CreateCustomerReportTab());
            tabReports.TabPages.Add(CreateTaskReportTab());
            tabReports.TabPages.Add(CreateTicketReportTab());
            tabReports.TabPages.Add(CreateSummaryTab());

            this.Controls.Add(tabReports);
        }

        private TabPage CreateSalesReportTab()
        {
            var tab = new TabPage("Satış Raporu");
            tab.BackColor = Color.White;

            // Filters
            var pnlFilter = new Panel { Location = new Point(5, 5), Size = new Size(940, 45), BackColor = Color.FromArgb(240, 240, 240) };

            pnlFilter.Controls.Add(new Label { Text = "Başlangıç:", Location = new Point(5, 12), Size = new Size(70, 20), Font = new Font("Arial", 9) });
            var dtpStart = new DateTimePicker { Location = new Point(80, 8), Size = new Size(130, 25), Format = DateTimePickerFormat.Short, Font = new Font("Arial", 9), Value = DateTime.Now.AddMonths(-1) };
            pnlFilter.Controls.Add(dtpStart);

            pnlFilter.Controls.Add(new Label { Text = "Bitiş:", Location = new Point(220, 12), Size = new Size(45, 20), Font = new Font("Arial", 9) });
            var dtpEnd = new DateTimePicker { Location = new Point(270, 8), Size = new Size(130, 25), Format = DateTimePickerFormat.Short, Font = new Font("Arial", 9) };
            pnlFilter.Controls.Add(dtpEnd);

            pnlFilter.Controls.Add(new Label { Text = "Durum:", Location = new Point(410, 12), Size = new Size(50, 20), Font = new Font("Arial", 9) });
            var cmbStatus = new ComboBox { Location = new Point(465, 8), Size = new Size(120, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 9) };
            cmbStatus.Items.AddRange(new object[] { "Tümü", "Prospect", "Quote", "Negotiation", "Won", "Lost", "Cancelled" });
            cmbStatus.SelectedIndex = 0;
            pnlFilter.Controls.Add(cmbStatus);

            var dgv = CreateStyledGrid();
            dgv.Location = new Point(5, 55);
            dgv.Size = new Size(940, 420);
            dgv.Columns.Add("CustomerName", "Müşteri");
            dgv.Columns.Add("SaleCode", "Satış Kodu");
            dgv.Columns.Add("Status", "Durum");
            dgv.Columns.Add("Amount", "Tutar");
            dgv.Columns.Add("FinalAmount", "Net Tutar");
            dgv.Columns.Add("SaleDate", "Tarih");

            var btnLoad = new Button { Text = "Raporu Getir", Location = new Point(600, 8), Size = new Size(120, 28), Font = new Font("Arial", 9, FontStyle.Bold), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, Cursor = Cursors.Hand, FlatStyle = FlatStyle.Flat };
            btnLoad.Click += (s, e) =>
            {
                try
                {
                    dgv.Rows.Clear();
                    string statusFilter = cmbStatus.SelectedItem?.ToString();
                    var data = _reportService.GetSalesReport(dtpStart.Value, dtpEnd.Value, statusFilter == "Tümü" ? null : statusFilter);
                    foreach (var item in data)
                        dgv.Rows.Add(item.CustomerName, item.SaleCode, item.Status, $"₺{item.Amount:N2}", $"₺{item.FinalAmount:N2}", item.SaleDate.ToString("dd/MM/yyyy"));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            pnlFilter.Controls.Add(btnLoad);

            var btnExport = new Button { Text = "CSV Dışa Aktar", Location = new Point(730, 8), Size = new Size(120, 28), Font = new Font("Arial", 9, FontStyle.Bold), BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, Cursor = Cursors.Hand, FlatStyle = FlatStyle.Flat };
            btnExport.Click += (s, e) => ExportGridToCsv(dgv, "satis_raporu.csv");
            pnlFilter.Controls.Add(btnExport);

            tab.Controls.Add(pnlFilter);
            tab.Controls.Add(dgv);

            // Auto-load
            btnLoad.PerformClick();

            return tab;
        }

        private TabPage CreateCustomerReportTab()
        {
            var tab = new TabPage("Müşteri Raporu");
            tab.BackColor = Color.White;

            var pnlFilter = new Panel { Location = new Point(5, 5), Size = new Size(940, 45), BackColor = Color.FromArgb(240, 240, 240) };

            pnlFilter.Controls.Add(new Label { Text = "Durum:", Location = new Point(5, 12), Size = new Size(50, 20), Font = new Font("Arial", 9) });
            var cmbStatus = new ComboBox { Location = new Point(60, 8), Size = new Size(120, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 9) };
            cmbStatus.Items.AddRange(new object[] { "Tümü", "Active", "Inactive", "Prospect" });
            cmbStatus.SelectedIndex = 0;
            pnlFilter.Controls.Add(cmbStatus);

            pnlFilter.Controls.Add(new Label { Text = "Tip:", Location = new Point(190, 12), Size = new Size(35, 20), Font = new Font("Arial", 9) });
            var cmbType = new ComboBox { Location = new Point(230, 8), Size = new Size(120, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 9) };
            cmbType.Items.AddRange(new object[] { "Tümü", "Individual", "Corporate" });
            cmbType.SelectedIndex = 0;
            pnlFilter.Controls.Add(cmbType);

            var dgv = CreateStyledGrid();
            dgv.Location = new Point(5, 55);
            dgv.Size = new Size(940, 420);
            dgv.Columns.Add("CustomerCode", "Kod");
            dgv.Columns.Add("FullName", "Ad Soyad");
            dgv.Columns.Add("CompanyName", "Şirket");
            dgv.Columns.Add("Status", "Durum");
            dgv.Columns.Add("CustomerType", "Tip");
            dgv.Columns.Add("TotalSales", "Satış Sayısı");
            dgv.Columns.Add("TotalSalesAmount", "Toplam Satış");
            dgv.Columns.Add("TotalTickets", "Talep Sayısı");
            dgv.Columns.Add("CreatedDate", "Kayıt Tarihi");

            var btnLoad = new Button { Text = "Raporu Getir", Location = new Point(370, 8), Size = new Size(120, 28), Font = new Font("Arial", 9, FontStyle.Bold), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, Cursor = Cursors.Hand, FlatStyle = FlatStyle.Flat };
            btnLoad.Click += (s, e) =>
            {
                try
                {
                    dgv.Rows.Clear();
                    string statusFilter = cmbStatus.SelectedItem?.ToString();
                    string typeFilter = cmbType.SelectedItem?.ToString();
                    var data = _reportService.GetCustomerReport(statusFilter == "Tümü" ? null : statusFilter, typeFilter == "Tümü" ? null : typeFilter);
                    foreach (var item in data)
                        dgv.Rows.Add(item.CustomerCode, item.FullName, item.CompanyName, item.Status, item.CustomerType, item.TotalSales, $"₺{item.TotalSalesAmount:N2}", item.TotalTickets, item.CreatedDate.ToString("dd/MM/yyyy"));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            pnlFilter.Controls.Add(btnLoad);

            var btnExport = new Button { Text = "CSV Dışa Aktar", Location = new Point(500, 8), Size = new Size(120, 28), Font = new Font("Arial", 9, FontStyle.Bold), BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, Cursor = Cursors.Hand, FlatStyle = FlatStyle.Flat };
            btnExport.Click += (s, e) => ExportGridToCsv(dgv, "musteri_raporu.csv");
            pnlFilter.Controls.Add(btnExport);

            tab.Controls.Add(pnlFilter);
            tab.Controls.Add(dgv);
            btnLoad.PerformClick();
            return tab;
        }

        private TabPage CreateTaskReportTab()
        {
            var tab = new TabPage("Görev Raporu");
            tab.BackColor = Color.White;

            var pnlFilter = new Panel { Location = new Point(5, 5), Size = new Size(940, 45), BackColor = Color.FromArgb(240, 240, 240) };

            pnlFilter.Controls.Add(new Label { Text = "Durum:", Location = new Point(5, 12), Size = new Size(50, 20), Font = new Font("Arial", 9) });
            var cmbStatus = new ComboBox { Location = new Point(60, 8), Size = new Size(120, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 9) };
            cmbStatus.Items.AddRange(new object[] { "Tümü", "Pending", "InProgress", "Completed", "Cancelled" });
            cmbStatus.SelectedIndex = 0;
            pnlFilter.Controls.Add(cmbStatus);

            pnlFilter.Controls.Add(new Label { Text = "Öncelik:", Location = new Point(190, 12), Size = new Size(55, 20), Font = new Font("Arial", 9) });
            var cmbPriority = new ComboBox { Location = new Point(250, 8), Size = new Size(120, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 9) };
            cmbPriority.Items.AddRange(new object[] { "Tümü", "Low", "Medium", "High", "Urgent" });
            cmbPriority.SelectedIndex = 0;
            pnlFilter.Controls.Add(cmbPriority);

            var dgv = CreateStyledGrid();
            dgv.Location = new Point(5, 55);
            dgv.Size = new Size(940, 420);
            dgv.Columns.Add("Title", "Başlık");
            dgv.Columns.Add("AssignedTo", "Atanan");
            dgv.Columns.Add("Priority", "Öncelik");
            dgv.Columns.Add("Status", "Durum");
            dgv.Columns.Add("DueDate", "Son Tarih");
            dgv.Columns.Add("CompletedDate", "Tamamlanma");
            dgv.Columns.Add("IsOverdue", "Gecikme");

            var btnLoad = new Button { Text = "Raporu Getir", Location = new Point(385, 8), Size = new Size(120, 28), Font = new Font("Arial", 9, FontStyle.Bold), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, Cursor = Cursors.Hand, FlatStyle = FlatStyle.Flat };
            btnLoad.Click += (s, e) =>
            {
                try
                {
                    dgv.Rows.Clear();
                    string statusFilter = cmbStatus.SelectedItem?.ToString();
                    string priorityFilter = cmbPriority.SelectedItem?.ToString();
                    var data = _reportService.GetTaskReport(statusFilter == "Tümü" ? null : statusFilter, priorityFilter == "Tümü" ? null : priorityFilter);
                    foreach (var item in data)
                        dgv.Rows.Add(item.Title, item.AssignedTo, item.Priority, item.Status, item.DueDate.ToString("dd/MM/yyyy"), item.CompletedDate.HasValue ? item.CompletedDate.Value.ToString("dd/MM/yyyy") : "-", item.IsOverdue ? "⚠ Evet" : "");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            pnlFilter.Controls.Add(btnLoad);

            tab.Controls.Add(pnlFilter);
            tab.Controls.Add(dgv);
            btnLoad.PerformClick();
            return tab;
        }

        private TabPage CreateTicketReportTab()
        {
            var tab = new TabPage("Destek Talebi Raporu");
            tab.BackColor = Color.White;

            var pnlFilter = new Panel { Location = new Point(5, 5), Size = new Size(940, 45), BackColor = Color.FromArgb(240, 240, 240) };

            pnlFilter.Controls.Add(new Label { Text = "Durum:", Location = new Point(5, 12), Size = new Size(50, 20), Font = new Font("Arial", 9) });
            var cmbStatus = new ComboBox { Location = new Point(60, 8), Size = new Size(120, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 9) };
            cmbStatus.Items.AddRange(new object[] { "Tümü", "Open", "InProgress", "OnHold", "Resolved", "Closed" });
            cmbStatus.SelectedIndex = 0;
            pnlFilter.Controls.Add(cmbStatus);

            var dgv = CreateStyledGrid();
            dgv.Location = new Point(5, 55);
            dgv.Size = new Size(940, 420);
            dgv.Columns.Add("TicketCode", "Kod");
            dgv.Columns.Add("CustomerName", "Müşteri");
            dgv.Columns.Add("Title", "Başlık");
            dgv.Columns.Add("Priority", "Öncelik");
            dgv.Columns.Add("Status", "Durum");
            dgv.Columns.Add("Category", "Kategori");
            dgv.Columns.Add("CreatedDate", "Oluşturma");
            dgv.Columns.Add("ClosedDate", "Kapanış");

            var btnLoad = new Button { Text = "Raporu Getir", Location = new Point(195, 8), Size = new Size(120, 28), Font = new Font("Arial", 9, FontStyle.Bold), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, Cursor = Cursors.Hand, FlatStyle = FlatStyle.Flat };
            btnLoad.Click += (s, e) =>
            {
                try
                {
                    dgv.Rows.Clear();
                    string statusFilter = cmbStatus.SelectedItem?.ToString();
                    var data = _reportService.GetTicketReport(statusFilter == "Tümü" ? null : statusFilter);
                    foreach (var item in data)
                        dgv.Rows.Add(item.TicketCode, item.CustomerName, item.Title, item.Priority, item.Status, item.Category, item.CreatedDate.ToString("dd/MM/yyyy"), item.ClosedDate.HasValue ? item.ClosedDate.Value.ToString("dd/MM/yyyy") : "-");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            pnlFilter.Controls.Add(btnLoad);

            tab.Controls.Add(pnlFilter);
            tab.Controls.Add(dgv);
            btnLoad.PerformClick();
            return tab;
        }

        private TabPage CreateSummaryTab()
        {
            var tab = new TabPage("Özet");
            tab.BackColor = Color.White;

            int y = 20;

            // Sales Pipeline
            var lblPipeline = new Label { Text = "Satış Pipeline Özeti", Location = new Point(20, y), Size = new Size(300, 25), Font = new Font("Arial", 12, FontStyle.Bold), ForeColor = Color.FromArgb(45, 45, 48) };
            tab.Controls.Add(lblPipeline);
            y += 30;

            try
            {
                var pipeline = _reportService.GetSalesPipelineSummary();
                Color[] colors = { Color.FromArgb(52, 152, 219), Color.FromArgb(155, 89, 182), Color.FromArgb(230, 126, 34), Color.FromArgb(46, 204, 113), Color.FromArgb(231, 76, 60) };
                int colorIdx = 0;
                foreach (var kvp in pipeline)
                {
                    var pnl = new Panel { Location = new Point(20 + (colorIdx * 175), y), Size = new Size(165, 70), BackColor = colors[colorIdx % colors.Length] };
                    pnl.Controls.Add(new Label { Text = kvp.Key, Location = new Point(5, 8), Size = new Size(155, 20), Font = new Font("Arial", 10, FontStyle.Bold), ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter });
                    pnl.Controls.Add(new Label { Text = kvp.Value.ToString(), Location = new Point(5, 30), Size = new Size(155, 30), Font = new Font("Arial", 18, FontStyle.Bold), ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter });
                    tab.Controls.Add(pnl);
                    colorIdx++;
                }
            }
            catch { }

            y += 90;

            // Ticket Status
            var lblTickets = new Label { Text = "Destek Talebi Durum Özeti", Location = new Point(20, y), Size = new Size(300, 25), Font = new Font("Arial", 12, FontStyle.Bold), ForeColor = Color.FromArgb(45, 45, 48) };
            tab.Controls.Add(lblTickets);
            y += 30;

            try
            {
                var ticketSummary = _reportService.GetTicketStatusSummary();
                Color[] tColors = { Color.FromArgb(231, 76, 60), Color.FromArgb(230, 126, 34), Color.FromArgb(241, 196, 15), Color.FromArgb(46, 204, 113), Color.FromArgb(149, 165, 166) };
                int tIdx = 0;
                foreach (var kvp in ticketSummary)
                {
                    var pnl = new Panel { Location = new Point(20 + (tIdx * 175), y), Size = new Size(165, 70), BackColor = tColors[tIdx % tColors.Length] };
                    pnl.Controls.Add(new Label { Text = kvp.Key, Location = new Point(5, 8), Size = new Size(155, 20), Font = new Font("Arial", 10, FontStyle.Bold), ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter });
                    pnl.Controls.Add(new Label { Text = kvp.Value.ToString(), Location = new Point(5, 30), Size = new Size(155, 30), Font = new Font("Arial", 18, FontStyle.Bold), ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter });
                    tab.Controls.Add(pnl);
                    tIdx++;
                }
            }
            catch { }

            y += 90;

            // Monthly Sales
            var lblMonthly = new Label { Text = $"Aylık Satış Özeti ({DateTime.Now.Year})", Location = new Point(20, y), Size = new Size(400, 25), Font = new Font("Arial", 12, FontStyle.Bold), ForeColor = Color.FromArgb(45, 45, 48) };
            tab.Controls.Add(lblMonthly);
            y += 30;

            try
            {
                var monthly = _reportService.GetMonthlySalesSummary(DateTime.Now.Year);
                var dgvMonthly = CreateStyledGrid();
                dgvMonthly.Location = new Point(20, y);
                dgvMonthly.Size = new Size(900, 200);
                dgvMonthly.Columns.Add("Month", "Ay");
                dgvMonthly.Columns.Add("Amount", "Satış Tutarı");

                foreach (var kvp in monthly)
                    dgvMonthly.Rows.Add(kvp.Key, $"₺{kvp.Value:N2}");

                tab.Controls.Add(dgvMonthly);
            }
            catch { }

            return tab;
        }

        private DataGridView CreateStyledGrid()
        {
            var dgv = new DataGridView
            {
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
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;
            return dgv;
        }

        private void ExportGridToCsv(DataGridView dgv, string fileName)
        {
            try
            {
                using (var dialog = new SaveFileDialog { FileName = fileName, Filter = "CSV Dosyası|*.csv" })
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        using (var writer = new System.IO.StreamWriter(dialog.FileName, false, System.Text.Encoding.UTF8))
                        {
                            // Header
                            var headers = new System.Collections.Generic.List<string>();
                            foreach (DataGridViewColumn col in dgv.Columns)
                                if (col.Visible) headers.Add(col.HeaderText);
                            writer.WriteLine(string.Join(";", headers));

                            // Rows
                            foreach (DataGridViewRow row in dgv.Rows)
                            {
                                var cells = new System.Collections.Generic.List<string>();
                                foreach (DataGridViewCell cell in row.Cells)
                                    if (dgv.Columns[cell.ColumnIndex].Visible)
                                        cells.Add(cell.Value?.ToString() ?? "");
                                writer.WriteLine(string.Join(";", cells));
                            }
                        }
                        MessageBox.Show($"Rapor dışa aktarıldı: {dialog.FileName}", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dışa aktarma hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
