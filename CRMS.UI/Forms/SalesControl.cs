using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CRMS.Business.Services;
using CRMS.Entity.Models;

namespace CRMS.UI.Forms
{
    public class SalesControl : UserControl
    {
        private readonly SaleService _saleService;
        private readonly CustomerService _customerService;
        private DataGridView dgvSales;
        private ComboBox cmbStatusFilter;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnRefresh;
        private Label lblTotalAmount;

        public SalesControl()
        {
            _saleService = new SaleService(Program.UnitOfWork);
            _customerService = new CustomerService(Program.UnitOfWork);
            InitializeCustomComponents();
            LoadSales();
        }

        private void InitializeCustomComponents()
        {
            this.BackColor = Color.FromArgb(245, 245, 245);

            Label lblTitle = new Label
            {
                Text = "SATIŞLAR",
                Location = new Point(20, 20),
                Size = new Size(300, 30),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 45, 48)
            };
            this.Controls.Add(lblTitle);

            // Status filter
            Label lblFilter = new Label
            {
                Text = "Durum:",
                Location = new Point(20, 65),
                Size = new Size(60, 25),
                Font = new Font("Arial", 10),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblFilter);

            cmbStatusFilter = new ComboBox
            {
                Location = new Point(85, 65),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 10)
            };
            cmbStatusFilter.Items.AddRange(new object[] { "Tümü", "Prospect", "Quote", "Negotiation", "Won", "Lost", "Cancelled" });
            cmbStatusFilter.SelectedIndex = 0;
            cmbStatusFilter.SelectedIndexChanged += (s, e) => LoadSales();
            this.Controls.Add(cmbStatusFilter);

            // Buttons
            btnAdd = CreateButton("Yeni Ekle", 250, 65, Color.FromArgb(46, 204, 113));
            btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(btnAdd);

            btnEdit = CreateButton("Düzenle", 360, 65, Color.FromArgb(241, 196, 15));
            btnEdit.Click += BtnEdit_Click;
            this.Controls.Add(btnEdit);

            btnDelete = CreateButton("Sil", 470, 65, Color.FromArgb(231, 76, 60));
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            btnRefresh = CreateButton("Yenile", 580, 65, Color.FromArgb(52, 152, 219));
            btnRefresh.Click += (s, e) => LoadSales();
            this.Controls.Add(btnRefresh);

            // Total label
            lblTotalAmount = new Label
            {
                Text = "Toplam Kazanılan: ₺0,00",
                Location = new Point(700, 68),
                Size = new Size(250, 25),
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113)
            };
            this.Controls.Add(lblTotalAmount);

            // DataGridView
            dgvSales = new DataGridView
            {
                Location = new Point(20, 105),
                Size = new Size(960, 470),
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
            dgvSales.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9, FontStyle.Bold);
            dgvSales.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            dgvSales.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSales.EnableHeadersVisualStyles = false;

            dgvSales.Columns.Add("Id", "ID");
            dgvSales.Columns.Add("SaleCode", "Satış Kodu");
            dgvSales.Columns.Add("CustomerName", "Müşteri");
            dgvSales.Columns.Add("Status", "Durum");
            dgvSales.Columns.Add("Amount", "Tutar");
            dgvSales.Columns.Add("FinalAmount", "Net Tutar");
            dgvSales.Columns.Add("SaleDate", "Satış Tarihi");
            dgvSales.Columns.Add("ClosingDate", "Kapanış Tarihi");

            dgvSales.Columns["Id"].Visible = false;
            this.Controls.Add(dgvSales);
        }

        private Button CreateButton(string text, int x, int y, Color color)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(100, 28),
                Font = new Font("Arial", 9, FontStyle.Bold),
                BackColor = color,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat
            };
        }

        private void LoadSales()
        {
            try
            {
                dgvSales.Rows.Clear();
                var sales = _saleService.GetAllSales();
                var customers = _customerService.GetAllCustomers().ToDictionary(c => c.Id, c => c);

                string filter = cmbStatusFilter.SelectedItem?.ToString();
                if (filter != "Tümü" && !string.IsNullOrEmpty(filter))
                    sales = sales.Where(s => s.Status == filter).ToList();

                decimal totalWon = 0;
                foreach (var sale in sales)
                {
                    customers.TryGetValue(sale.CustomerId, out var customer);
                    string customerName = customer != null ? $"{customer.FirstName} {customer.LastName}".Trim() : "Bilinmiyor";

                    dgvSales.Rows.Add(
                        sale.Id,
                        sale.SaleCode,
                        customerName,
                        sale.Status,
                        $"₺{sale.Amount:N2}",
                        $"₺{sale.FinalAmount:N2}",
                        sale.SaleDate.ToString("dd/MM/yyyy"),
                        sale.ClosingDate.HasValue ? sale.ClosingDate.Value.ToString("dd/MM/yyyy") : "-"
                    );

                    if (sale.Status == "Won")
                        totalWon += sale.FinalAmount;
                }

                lblTotalAmount.Text = $"Toplam Kazanılan: ₺{totalWon:N2}";

                // Color rows by status
                foreach (DataGridViewRow row in dgvSales.Rows)
                {
                    string status = row.Cells["Status"].Value?.ToString();
                    if (status == "Won") row.DefaultCellStyle.ForeColor = Color.FromArgb(39, 174, 96);
                    else if (status == "Lost" || status == "Cancelled") row.DefaultCellStyle.ForeColor = Color.FromArgb(192, 57, 43);
                    else if (status == "Negotiation") row.DefaultCellStyle.ForeColor = Color.FromArgb(41, 128, 185);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Satışlar yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new SaleForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                    LoadSales();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvSales.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen düzenlemek istediğiniz satışı seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int saleId = (int)dgvSales.SelectedRows[0].Cells["Id"].Value;
            var sale = _saleService.GetSaleById(saleId);
            if (sale != null)
            {
                using (var form = new SaleForm(sale))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                        LoadSales();
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvSales.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silmek istediğiniz satışı seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Seçili satışı silmek istediğinizden emin misiniz?", "Onay",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    int saleId = (int)dgvSales.SelectedRows[0].Cells["Id"].Value;
                    _saleService.DeleteSale(saleId);
                    Program.LoggingService.LogDataDelete(Program.CurrentUser?.Id, "Sales", saleId, saleId.ToString());
                    LoadSales();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Silme sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
