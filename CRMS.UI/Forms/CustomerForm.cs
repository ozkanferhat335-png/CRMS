using System;
using System.Windows.Forms;
using CRMS.Business.Services;
using CRMS.Entity.Models;

namespace CRMS.UI.Forms
{
    public class CustomerForm : Form
    {
        private readonly CustomerService _customerService;
        private readonly Customer _customer;
        private readonly bool _isEditMode;

        private TextBox txtFirstName;
        private TextBox txtLastName;
        private TextBox txtCompanyName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private TextBox txtAddress;
        private TextBox txtCity;
        private TextBox txtCountry;
        private TextBox txtWebsite;
        private TextBox txtTaxNumber;
        private TextBox txtNotes;
        private ComboBox cmbCustomerType;
        private ComboBox cmbStatus;
        private NumericUpDown nudRating;
        private Button btnSave;
        private Button btnCancel;

        public CustomerForm(Customer customer = null)
        {
            _customerService = new CustomerService(Program.UnitOfWork);
            _customer = customer ?? new Customer();
            _isEditMode = customer != null;

            InitializeForm();
            if (_isEditMode)
                PopulateFields();
        }

        private void InitializeForm()
        {
            this.Text = _isEditMode ? "Müşteri Düzenle" : "Yeni Müşteri Ekle";
            this.Size = new System.Drawing.Size(520, 620);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = System.Drawing.Color.White;

            int labelX = 20, fieldX = 160, fieldW = 310, rowH = 35, startY = 20;

            Label lblTitle = new Label
            {
                Text = _isEditMode ? "MÜŞTERİ DÜZENLE" : "YENİ MÜŞTERİ",
                Location = new System.Drawing.Point(labelX, startY),
                Size = new System.Drawing.Size(460, 30),
                Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(45, 45, 48),
                TextAlign = System.Windows.Forms.ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblTitle);

            int y = startY + 45;

            AddRow("Ad *", ref y, rowH, labelX, fieldX, fieldW, out txtFirstName);
            AddRow("Soyad *", ref y, rowH, labelX, fieldX, fieldW, out txtLastName);
            AddRow("Şirket Adı", ref y, rowH, labelX, fieldX, fieldW, out txtCompanyName);
            AddRow("E-posta", ref y, rowH, labelX, fieldX, fieldW, out txtEmail);
            AddRow("Telefon", ref y, rowH, labelX, fieldX, fieldW, out txtPhone);
            AddRow("Adres", ref y, rowH, labelX, fieldX, fieldW, out txtAddress);
            AddRow("Şehir", ref y, rowH, labelX, fieldX, fieldW, out txtCity);
            AddRow("Ülke", ref y, rowH, labelX, fieldX, fieldW, out txtCountry);
            AddRow("Web Sitesi", ref y, rowH, labelX, fieldX, fieldW, out txtWebsite);
            AddRow("Vergi No", ref y, rowH, labelX, fieldX, fieldW, out txtTaxNumber);

            // Customer Type
            AddLabel("Müşteri Tipi", labelX, y);
            cmbCustomerType = new ComboBox
            {
                Location = new System.Drawing.Point(fieldX, y),
                Size = new System.Drawing.Size(fieldW, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new System.Drawing.Font("Arial", 10)
            };
            cmbCustomerType.Items.AddRange(new object[] { "Individual", "Corporate" });
            cmbCustomerType.SelectedIndex = 0;
            this.Controls.Add(cmbCustomerType);
            y += rowH;

            // Status
            AddLabel("Durum", labelX, y);
            cmbStatus = new ComboBox
            {
                Location = new System.Drawing.Point(fieldX, y),
                Size = new System.Drawing.Size(fieldW, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new System.Drawing.Font("Arial", 10)
            };
            cmbStatus.Items.AddRange(new object[] { "Active", "Inactive", "Prospect" });
            cmbStatus.SelectedIndex = 0;
            this.Controls.Add(cmbStatus);
            y += rowH;

            // Rating
            AddLabel("Puan (0-5)", labelX, y);
            nudRating = new NumericUpDown
            {
                Location = new System.Drawing.Point(fieldX, y),
                Size = new System.Drawing.Size(100, 25),
                Minimum = 0,
                Maximum = 5,
                DecimalPlaces = 1,
                Increment = 0.5m,
                Font = new System.Drawing.Font("Arial", 10)
            };
            this.Controls.Add(nudRating);
            y += rowH;

            // Notes
            AddLabel("Notlar", labelX, y);
            txtNotes = new TextBox
            {
                Location = new System.Drawing.Point(fieldX, y),
                Size = new System.Drawing.Size(fieldW, 60),
                Multiline = true,
                Font = new System.Drawing.Font("Arial", 10),
                ScrollBars = ScrollBars.Vertical
            };
            this.Controls.Add(txtNotes);
            y += 70;

            // Buttons
            btnSave = new Button
            {
                Text = "Kaydet",
                Location = new System.Drawing.Point(fieldX, y),
                Size = new System.Drawing.Size(140, 35),
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold),
                BackColor = System.Drawing.Color.FromArgb(46, 204, 113),
                ForeColor = System.Drawing.Color.White,
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnCancel = new Button
            {
                Text = "İptal",
                Location = new System.Drawing.Point(fieldX + 155, y),
                Size = new System.Drawing.Size(140, 35),
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold),
                BackColor = System.Drawing.Color.FromArgb(231, 76, 60),
                ForeColor = System.Drawing.Color.White,
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnCancel);

            this.ClientSize = new System.Drawing.Size(500, y + 55);
        }

        private void AddLabel(string text, int x, int y)
        {
            var lbl = new Label
            {
                Text = text + ":",
                Location = new System.Drawing.Point(x, y + 3),
                Size = new System.Drawing.Size(135, 20),
                Font = new System.Drawing.Font("Arial", 10),
                ForeColor = System.Drawing.Color.FromArgb(60, 60, 60)
            };
            this.Controls.Add(lbl);
        }

        private void AddRow(string label, ref int y, int rowH, int labelX, int fieldX, int fieldW, out TextBox textBox)
        {
            AddLabel(label, labelX, y);
            textBox = new TextBox
            {
                Location = new System.Drawing.Point(fieldX, y),
                Size = new System.Drawing.Size(fieldW, 25),
                Font = new System.Drawing.Font("Arial", 10)
            };
            this.Controls.Add(textBox);
            y += rowH;
        }

        private void PopulateFields()
        {
            txtFirstName.Text = _customer.FirstName ?? string.Empty;
            txtLastName.Text = _customer.LastName ?? string.Empty;
            txtCompanyName.Text = _customer.CompanyName ?? string.Empty;
            txtEmail.Text = _customer.Email ?? string.Empty;
            txtPhone.Text = _customer.PhoneNumber ?? string.Empty;
            txtAddress.Text = _customer.Address ?? string.Empty;
            txtCity.Text = _customer.City ?? string.Empty;
            txtCountry.Text = _customer.Country ?? string.Empty;
            txtWebsite.Text = _customer.Website ?? string.Empty;
            txtTaxNumber.Text = _customer.TaxNumber ?? string.Empty;
            txtNotes.Text = _customer.Notes ?? string.Empty;
            nudRating.Value = (decimal)_customer.Rating;

            int typeIdx = cmbCustomerType.Items.IndexOf(_customer.CustomerType ?? "Individual");
            cmbCustomerType.SelectedIndex = typeIdx >= 0 ? typeIdx : 0;

            int statusIdx = cmbStatus.Items.IndexOf(_customer.Status ?? "Active");
            cmbStatus.SelectedIndex = statusIdx >= 0 ? statusIdx : 0;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) && string.IsNullOrWhiteSpace(txtCompanyName.Text))
            {
                MessageBox.Show("Lütfen en az Ad veya Şirket Adı giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _customer.FirstName = txtFirstName.Text.Trim();
                _customer.LastName = txtLastName.Text.Trim();
                _customer.CompanyName = txtCompanyName.Text.Trim();
                _customer.Email = txtEmail.Text.Trim();
                _customer.PhoneNumber = txtPhone.Text.Trim();
                _customer.Address = txtAddress.Text.Trim();
                _customer.City = txtCity.Text.Trim();
                _customer.Country = txtCountry.Text.Trim();
                _customer.Website = txtWebsite.Text.Trim();
                _customer.TaxNumber = txtTaxNumber.Text.Trim();
                _customer.Notes = txtNotes.Text.Trim();
                _customer.CustomerType = cmbCustomerType.SelectedItem?.ToString() ?? "Individual";
                _customer.Status = cmbStatus.SelectedItem?.ToString() ?? "Active";
                _customer.Rating = nudRating.Value;
                _customer.CreatedByUserId = Program.CurrentUser?.Id;

                bool success;
                if (_isEditMode)
                {
                    _customer.ModifiedByUserId = Program.CurrentUser?.Id;
                    success = _customerService.UpdateCustomer(_customer);
                    if (success)
                        Program.LoggingService.LogDataUpdate(Program.CurrentUser?.Id, "Customers", _customer.Id, "", _customer.FirstName + " " + _customer.LastName);
                }
                else
                {
                    success = _customerService.AddCustomer(_customer);
                    if (success)
                        Program.LoggingService.LogDataCreate(Program.CurrentUser?.Id, "Customers", _customer.Id, _customer.FirstName + " " + _customer.LastName);
                }

                if (success)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kayıt sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
