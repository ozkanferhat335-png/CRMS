using System;
using System.Windows.Forms;
using CRMS.Business.Services;
using CRMS.Entity.Models;

namespace CRMS.UI.Forms
{
    public partial class CustomersControl : UserControl
    {
        private CustomerService _customerService;
        private DataGridView dgvCustomers;
        private TextBox txtSearch;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnSearch;

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
            this.Size = new System.Drawing.Size(900, 600);
            this.ResumeLayout(false);
        }

        private void InitializeCustomComponents()
        {
            // Title
            Label lblTitle = new Label
            {
                Text = "MÜŞTERİLER",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(300, 30),
                Font = new System.Drawing.Font("Arial", 16, System.Drawing.FontStyle.Bold)
            };
            this.Controls.Add(lblTitle);

            // Search TextBox
            txtSearch = new TextBox
            {
                Location = new System.Drawing.Point(20, 60),
                Size = new System.Drawing.Size(250, 25),
                Font = new System.Drawing.Font("Arial", 10),
                PlaceholderText = "Müşteri arayınız..."
            };
            this.Controls.Add(txtSearch);

            // Search Button
            btnSearch = new Button
            {
                Text = "Ara",
                Location = new System.Drawing.Point(280, 60),
                Size = new System.Drawing.Size(80, 25),
                Font = new System.Drawing.Font("Arial", 10),
                BackColor = System.Drawing.Color.FromArgb(52, 152, 219),
                ForeColor = System.Drawing.Color.White,
                Cursor = System.Windows.Forms.Cursors.Hand
            };
            btnSearch.Click += BtnSearch_Click;
            this.Controls.Add(btnSearch);

            // Add Button
            btnAdd = new Button
            {
                Text = "Yeni Ekle",
                Location = new System.Drawing.Point(370, 60),
                Size = new System.Drawing.Size(100, 25),
                Font = new System.Drawing.Font("Arial", 10),
                BackColor = System.Drawing.Color.FromArgb(46, 204, 113),
                ForeColor = System.Drawing.Color.White,
                Cursor = System.Windows.Forms.Cursors.Hand
            };
            btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(btnAdd);

            // Edit Button
            btnEdit = new Button
            {
                Text = "Düzenle",
                Location = new System.Drawing.Point(480, 60),
                Size = new System.Drawing.Size(100, 25),
                Font = new System.Drawing.Font("Arial", 10),
                BackColor = System.Drawing.Color.FromArgb(241, 196, 15),
                ForeColor = System.Drawing.Color.White,
                Cursor = System.Windows.Forms.Cursors.Hand
            };
            btnEdit.Click += BtnEdit_Click;
            this.Controls.Add(btnEdit);

            // Delete Button
            btnDelete = new Button
            {
                Text = "Sil",
                Location = new System.Drawing.Point(590, 60),
                Size = new System.Drawing.Size(100, 25),
                Font = new System.Drawing.Font("Arial", 10),
                BackColor = System.Drawing.Color.FromArgb(231, 76, 60),
                ForeColor = System.Drawing.Color.White,
                Cursor = System.Windows.Forms.Cursors.Hand
            };
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            // DataGridView
            dgvCustomers = new DataGridView
            {
                Location = new System.Drawing.Point(20, 100),
                Size = new System.Drawing.Size(860, 480),
                Font = new System.Drawing.Font("Arial", 10),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvCustomers.Columns.Add("Id", "ID");
            dgvCustomers.Columns.Add("CustomerCode", "Müşteri Kodu");
            dgvCustomers.Columns.Add("FirstName", "Ad");
            dgvCustomers.Columns.Add("LastName", "Soyad");
            dgvCustomers.Columns.Add("Email", "E-posta");
            dgvCustomers.Columns.Add("PhoneNumber", "Telefon");
            dgvCustomers.Columns.Add("Status", "Durum");
            this.Controls.Add(dgvCustomers);
        }

        private void LoadCustomers()
        {
            try
            {
                dgvCustomers.Rows.Clear();
                var customers = _customerService.GetAllCustomers();

                foreach (var customer in customers)
                {
                    dgvCustomers.Rows.Add(
                        customer.Id,
                        customer.CustomerCode,
                        customer.FirstName,
                        customer.LastName,
                        customer.Email,
                        customer.PhoneNumber,
                        customer.Status
                    );
                }
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
                        customer.Email,
                        customer.PhoneNumber,
                        customer.Status
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Arama sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Yeni müşteri ekleme formu yakında eklenecek.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count > 0)
            {
                MessageBox.Show("Müşteri düzenleme formu yakında eklenecek.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Lütfen düzenlemek istediğiniz müşteri seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Seçili müşteriyi silmek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MessageBox.Show("Müşteri silinecek.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek istediğiniz müşteri seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}