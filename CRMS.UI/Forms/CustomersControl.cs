using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CRMS.Business.Services;
using CRMS.Entity.Models;

namespace CRMS.UI.Forms
{
    public partial class CustomersControl : UserControl
    {
        private readonly CustomerService _customerService;
        private DataGridView dgvCustomers;
        private TextBox txtSearch;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnSearch;
        private Button btnRefresh;
        private Label lblCount;

        public CustomersControl()
        {
            InitializeComponent();
            _customerService = new CustomerService(Program.UnitOfWork);
            InitializeCustomComponents();
            LoadCustomers();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "CustomersControl";
            this.Size = new System.Drawing.Size(1000, 620);
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.ResumeLayout(false);
        }

        private void InitializeCustomComponents()
        {
            // Title
            Label lblTitle = new Label
            {
                Text = "MÜŞTERİLER",
                Location = new Point(20, 20),
                Size = new Size(300, 30),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 45, 48)
            };
            this.Controls.Add(lblTitle);

            // Search TextBox
            txtSearch = new TextBox
            {
                Location = new Point(20, 65),
                Size = new Size(250, 28),
                Font = new Font("Arial", 10),
                PlaceholderText = "Müşteri arayınız..."
            };
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnSearch_Click(s, e); };
            this.Controls.Add(txtSearch);

            // Search Button
            btnSearch = CreateButton("Ara", 280, 65, Color.FromArgb(52, 152, 219), 70);
            btnSearch.Click += BtnSearch_Click;
            this.Controls.Add(btnSearch);

            // Add Button
            btnAdd = CreateButton("Yeni Ekle", 360, 65, Color.FromArgb(46, 204, 113), 100);
            btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(btnAdd);

            // Edit Button
            btnEdit = CreateButton("Düzenle", 470, 65, Color.FromArgb(241, 196, 15), 100);
            btnEdit.Click += BtnEdit_Click;
            this.Controls.Add(btnEdit);

            // Delete Button
            btnDelete = CreateButton("Sil", 580, 65, Color.FromArgb(231, 76, 60), 80);
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            // Refresh Button
            btnRefresh = CreateButton("Yenile", 670, 65, Color.FromArgb(149, 165, 166), 80);
            btnRefresh.Click += (s, e) => { txtSearch.Clear(); LoadCustomers(); };
            this.Controls.Add(btnRefresh);

            // Count label
            lblCount = new Label
            {
                Text = "",
                Location = new Point(760, 68),
                Size = new Size(200, 25),
                Font = new Font("Arial", 10),
                ForeColor = Color.FromArgb(100, 100, 100)
            };
            this.Controls.Add(lblCount);

            // DataGridView
            dgvCustomers = new DataGridView
            {
                Location = new Point(20, 105),
                Size = new Size(960, 490),
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
            dgvCustomers.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9, FontStyle.Bold);
            dgvCustomers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            dgvCustomers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCustomers.EnableHeadersVisualStyles = false;
            dgvCustomers.DoubleClick += (s, e) => BtnEdit_Click(s, e);

            dgvCustomers.Columns.Add("Id", "ID");
            dgvCustomers.Columns.Add("CustomerCode", "Müşteri Kodu");
            dgvCustomers.Columns.Add("FirstName", "Ad");
            dgvCustomers.Columns.Add("LastName", "Soyad");
            dgvCustomers.Columns.Add("CompanyName", "Şirket");
            dgvCustomers.Columns.Add("Email", "E-posta");
            dgvCustomers.Columns.Add("PhoneNumber", "Telefon");
            dgvCustomers.Columns.Add("City", "Şehir");
            dgvCustomers.Columns.Add("CustomerType", "Tip");
            dgvCustomers.Columns.Add("Status", "Durum");
            dgvCustomers.Columns.Add("Rating", "Puan");
            dgvCustomers.Columns.Add("CreatedDate", "Kayıt Tarihi");

            dgvCustomers.Columns["Id"].Visible = false;
            this.Controls.Add(dgvCustomers);
        }

        private Button CreateButton(string text, int x, int y, Color color, int width = 100)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 28),
                Font = new Font("Arial", 9, FontStyle.Bold),
                BackColor = color,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat
            };
        }

        private void LoadCustomers()
        {
            try
            {
                dgvCustomers.Rows.Clear();
                var customers = _customerService.GetAllCustomers();

                foreach (var customer in customers)
                {
                    int rowIdx = dgvCustomers.Rows.Add(
                        customer.Id,
                        customer.CustomerCode,
                        customer.FirstName,
                        customer.LastName,
                        customer.CompanyName ?? "",
                        customer.Email ?? "",
                        customer.PhoneNumber ?? "",
                        customer.City ?? "",
                        customer.CustomerType ?? "",
                        customer.Status,
                        customer.Rating.ToString("F1"),
                        customer.CreatedDate.ToString("dd/MM/yyyy")
                    );

                    // Color by status
                    if (customer.Status == "Inactive")
                        dgvCustomers.Rows[rowIdx].DefaultCellStyle.ForeColor = Color.Gray;
                    else if (customer.Status == "Prospect")
                        dgvCustomers.Rows[rowIdx].DefaultCellStyle.ForeColor = Color.FromArgb(41, 128, 185);
                }

                lblCount.Text = $"Toplam: {customers.Count} müşteri";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Müşteriler yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                LoadCustomers();
                return;
            }

            try
            {
                dgvCustomers.Rows.Clear();
                var customers = _customerService.SearchCustomers(txtSearch.Text);

                foreach (var customer in customers)
                {
                    dgvCustomers.Rows.Add(
                        customer.Id,
                        customer.CustomerCode,
                        customer.FirstName,
                        customer.LastName,
                        customer.CompanyName ?? "",
                        customer.Email ?? "",
                        customer.PhoneNumber ?? "",
                        customer.City ?? "",
                        customer.CustomerType ?? "",
                        customer.Status,
                        customer.Rating.ToString("F1"),
                        customer.CreatedDate.ToString("dd/MM/yyyy")
                    );
                }

                lblCount.Text = $"Bulunan: {customers.Count} müşteri";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Arama sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new CustomerForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                    LoadCustomers();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen düzenlemek istediğiniz müşteriyi seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int customerId = (int)dgvCustomers.SelectedRows[0].Cells["Id"].Value;
            var customer = _customerService.GetCustomerById(customerId);

            if (customer != null)
            {
                // Map DTO back to entity for editing
                var customerEntity = new Customer
                {
                    Id = customer.Id,
                    CustomerCode = customer.CustomerCode,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    CompanyName = customer.CompanyName,
                    TaxNumber = customer.TaxNumber,
                    Email = customer.Email,
                    PhoneNumber = customer.PhoneNumber,
                    Address = customer.Address,
                    City = customer.City,
                    Country = customer.Country,
                    Website = customer.Website,
                    CustomerType = customer.CustomerType,
                    Status = customer.Status,
                    Rating = customer.Rating,
                    Notes = customer.Notes,
                    LastContactDate = customer.LastContactDate,
                    NextContactDate = customer.NextContactDate,
                    CreatedDate = customer.CreatedDate,
                    ModifiedDate = customer.ModifiedDate,
                    CreatedByUserId = customer.CreatedByUserId,
                    ModifiedByUserId = customer.ModifiedByUserId
                };

                using (var form = new CustomerForm(customerEntity))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                        LoadCustomers();
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silmek istediğiniz müşteriyi seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string customerName = $"{dgvCustomers.SelectedRows[0].Cells["FirstName"].Value} {dgvCustomers.SelectedRows[0].Cells["LastName"].Value}".Trim();

            if (MessageBox.Show($"'{customerName}' müşterisini silmek istediğinizden emin misiniz?\n\nBu işlem geri alınamaz!", "Onay",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    int customerId = (int)dgvCustomers.SelectedRows[0].Cells["Id"].Value;
                    _customerService.DeleteCustomer(customerId);
                    Program.LoggingService.LogDataDelete(Program.CurrentUser?.Id, "Customers", customerId, customerName);
                    LoadCustomers();
                    MessageBox.Show("Müşteri başarıyla silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Silme sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
