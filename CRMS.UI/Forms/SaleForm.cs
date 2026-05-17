using System;
using System.Drawing;
using System.Windows.Forms;
using CRMS.Business.Services;
using CRMS.Entity.Models;

namespace CRMS.UI.Forms
{
    public class SaleForm : Form
    {
        private readonly SaleService _saleService;
        private readonly CustomerService _customerService;
        private readonly Sale _sale;
        private readonly bool _isEditMode;

        private ComboBox cmbCustomer;
        private ComboBox cmbStatus;
        private TextBox txtNotes;
        private NumericUpDown nudAmount;
        private NumericUpDown nudTaxAmount;
        private NumericUpDown nudFinalAmount;
        private DateTimePicker dtpSaleDate;
        private DateTimePicker dtpClosingDate;
        private CheckBox chkHasClosingDate;
        private Button btnSave;
        private Button btnCancel;

        public SaleForm(Sale sale = null)
        {
            _saleService = new SaleService(Program.UnitOfWork);
            _customerService = new CustomerService(Program.UnitOfWork);
            _sale = sale ?? new Sale();
            _isEditMode = sale != null;

            InitializeForm();
            LoadCustomers();
            if (_isEditMode)
                PopulateFields();
        }

        private void InitializeForm()
        {
            this.Text = _isEditMode ? "Satış Düzenle" : "Yeni Satış";
            this.Size = new Size(480, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            int lx = 20, fx = 160, fw = 280, rh = 38, y = 20;

            AddLabel("MÜŞTERİ SEÇ *", lx, y, 14, true);
            y += 35;

            AddLabel("Müşteri:", lx, y);
            cmbCustomer = new ComboBox { Location = new Point(fx, y), Size = new Size(fw, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 10) };
            this.Controls.Add(cmbCustomer);
            y += rh;

            AddLabel("Durum:", lx, y);
            cmbStatus = new ComboBox { Location = new Point(fx, y), Size = new Size(fw, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 10) };
            cmbStatus.Items.AddRange(new object[] { "Prospect", "Quote", "Negotiation", "Won", "Lost", "Cancelled" });
            cmbStatus.SelectedIndex = 0;
            this.Controls.Add(cmbStatus);
            y += rh;

            AddLabel("Tutar (₺):", lx, y);
            nudAmount = new NumericUpDown { Location = new Point(fx, y), Size = new Size(fw, 25), Maximum = 99999999, DecimalPlaces = 2, Font = new Font("Arial", 10) };
            nudAmount.ValueChanged += (s, e) => nudFinalAmount.Value = nudAmount.Value + nudTaxAmount.Value;
            this.Controls.Add(nudAmount);
            y += rh;

            AddLabel("KDV (₺):", lx, y);
            nudTaxAmount = new NumericUpDown { Location = new Point(fx, y), Size = new Size(fw, 25), Maximum = 99999999, DecimalPlaces = 2, Font = new Font("Arial", 10) };
            nudTaxAmount.ValueChanged += (s, e) => nudFinalAmount.Value = nudAmount.Value + nudTaxAmount.Value;
            this.Controls.Add(nudTaxAmount);
            y += rh;

            AddLabel("Net Tutar (₺):", lx, y);
            nudFinalAmount = new NumericUpDown { Location = new Point(fx, y), Size = new Size(fw, 25), Maximum = 99999999, DecimalPlaces = 2, Font = new Font("Arial", 10), ReadOnly = true, BackColor = Color.FromArgb(240, 240, 240) };
            this.Controls.Add(nudFinalAmount);
            y += rh;

            AddLabel("Satış Tarihi:", lx, y);
            dtpSaleDate = new DateTimePicker { Location = new Point(fx, y), Size = new Size(fw, 25), Format = DateTimePickerFormat.Short, Font = new Font("Arial", 10) };
            this.Controls.Add(dtpSaleDate);
            y += rh;

            chkHasClosingDate = new CheckBox { Text = "Kapanış Tarihi:", Location = new Point(lx, y + 3), Size = new Size(135, 20), Font = new Font("Arial", 10) };
            chkHasClosingDate.CheckedChanged += (s, e) => dtpClosingDate.Enabled = chkHasClosingDate.Checked;
            this.Controls.Add(chkHasClosingDate);
            dtpClosingDate = new DateTimePicker { Location = new Point(fx, y), Size = new Size(fw, 25), Format = DateTimePickerFormat.Short, Font = new Font("Arial", 10), Enabled = false };
            this.Controls.Add(dtpClosingDate);
            y += rh;

            AddLabel("Notlar:", lx, y);
            txtNotes = new TextBox { Location = new Point(fx, y), Size = new Size(fw, 55), Multiline = true, Font = new Font("Arial", 10), ScrollBars = ScrollBars.Vertical };
            this.Controls.Add(txtNotes);
            y += 65;

            btnSave = new Button { Text = "Kaydet", Location = new Point(fx, y), Size = new Size(130, 35), Font = new Font("Arial", 10, FontStyle.Bold), BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, Cursor = Cursors.Hand, FlatStyle = FlatStyle.Flat };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnCancel = new Button { Text = "İptal", Location = new Point(fx + 145, y), Size = new Size(130, 35), Font = new Font("Arial", 10, FontStyle.Bold), BackColor = Color.FromArgb(231, 76, 60), ForeColor = Color.White, Cursor = Cursors.Hand, FlatStyle = FlatStyle.Flat };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnCancel);

            this.ClientSize = new Size(460, y + 55);
        }

        private void AddLabel(string text, int x, int y, int fontSize = 10, bool bold = false)
        {
            this.Controls.Add(new Label
            {
                Text = text,
                Location = new Point(x, y + 3),
                Size = new Size(135, 22),
                Font = new Font("Arial", fontSize, bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = Color.FromArgb(60, 60, 60)
            });
        }

        private void LoadCustomers()
        {
            try
            {
                var customers = _customerService.GetAllCustomers();
                cmbCustomer.Items.Clear();
                foreach (var c in customers)
                    cmbCustomer.Items.Add(new ComboBoxItem { Text = $"{c.FirstName} {c.LastName} - {c.CompanyName}".Trim(' ', '-'), Value = c.Id });

                if (cmbCustomer.Items.Count > 0)
                    cmbCustomer.SelectedIndex = 0;
            }
            catch { }
        }

        private void PopulateFields()
        {
            for (int i = 0; i < cmbCustomer.Items.Count; i++)
            {
                if (((ComboBoxItem)cmbCustomer.Items[i]).Value == _sale.CustomerId)
                {
                    cmbCustomer.SelectedIndex = i;
                    break;
                }
            }

            int statusIdx = cmbStatus.Items.IndexOf(_sale.Status ?? "Prospect");
            cmbStatus.SelectedIndex = statusIdx >= 0 ? statusIdx : 0;

            nudAmount.Value = _sale.Amount;
            nudTaxAmount.Value = _sale.TaxAmount;
            nudFinalAmount.Value = _sale.FinalAmount;
            dtpSaleDate.Value = _sale.SaleDate == DateTime.MinValue ? DateTime.Now : _sale.SaleDate;
            txtNotes.Text = _sale.Notes ?? string.Empty;

            if (_sale.ClosingDate.HasValue)
            {
                chkHasClosingDate.Checked = true;
                dtpClosingDate.Value = _sale.ClosingDate.Value;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (cmbCustomer.SelectedItem == null)
            {
                MessageBox.Show("Lütfen müşteri seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _sale.CustomerId = ((ComboBoxItem)cmbCustomer.SelectedItem).Value;
                _sale.UserId = Program.CurrentUser?.Id ?? 1;
                _sale.Status = cmbStatus.SelectedItem?.ToString() ?? "Prospect";
                _sale.Amount = nudAmount.Value;
                _sale.TaxAmount = nudTaxAmount.Value;
                _sale.FinalAmount = nudFinalAmount.Value;
                _sale.SaleDate = dtpSaleDate.Value;
                _sale.ClosingDate = chkHasClosingDate.Checked ? dtpClosingDate.Value : (DateTime?)null;
                _sale.Notes = txtNotes.Text.Trim();
                _sale.CreatedByUserId = Program.CurrentUser?.Id;

                bool success = _isEditMode ? _saleService.UpdateSale(_sale) : _saleService.AddSale(_sale);

                if (success)
                {
                    Program.LoggingService.LogDataCreate(Program.CurrentUser?.Id, "Sales", _sale.Id, _sale.SaleCode);
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

    internal class ComboBoxItem
    {
        public string Text { get; set; }
        public int Value { get; set; }
        public override string ToString() => Text;
    }
}
