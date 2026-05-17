using System;
using System.Drawing;
using System.Windows.Forms;
using CRMS.Business.Services;
using CRMS.Entity.Models;

namespace CRMS.UI.Forms
{
    public class OfferForm : Form
    {
        private readonly OfferService _offerService;
        private readonly CustomerService _customerService;
        private readonly Offer _offer;
        private readonly bool _isEditMode;

        private ComboBox cmbCustomer;
        private TextBox txtTitle;
        private TextBox txtDescription;
        private TextBox txtNotes;
        private NumericUpDown nudTotalAmount;
        private NumericUpDown nudTaxAmount;
        private NumericUpDown nudDiscountAmount;
        private NumericUpDown nudFinalAmount;
        private DateTimePicker dtpOfferDate;
        private DateTimePicker dtpExpiryDate;
        private CheckBox chkHasExpiry;
        private Button btnSave;
        private Button btnCancel;

        public OfferForm(Offer offer = null)
        {
            _offerService = new OfferService(Program.UnitOfWork);
            _customerService = new CustomerService(Program.UnitOfWork);
            _offer = offer ?? new Offer();
            _isEditMode = offer != null;

            InitializeForm();
            LoadCustomers();
            if (_isEditMode)
                PopulateFields();
        }

        private void InitializeForm()
        {
            this.Text = _isEditMode ? "Teklif Düzenle" : "Yeni Teklif";
            this.Size = new Size(500, 530);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            int lx = 20, fx = 165, fw = 295, rh = 38, y = 20;

            AddLabel("YENİ TEKLİF", lx, y, 14, true);
            y += 35;

            AddLabel("Müşteri *:", lx, y);
            cmbCustomer = new ComboBox { Location = new Point(fx, y), Size = new Size(fw, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 10) };
            this.Controls.Add(cmbCustomer);
            y += rh;

            AddLabel("Başlık *:", lx, y);
            txtTitle = new TextBox { Location = new Point(fx, y), Size = new Size(fw, 25), Font = new Font("Arial", 10) };
            this.Controls.Add(txtTitle);
            y += rh;

            AddLabel("Açıklama:", lx, y);
            txtDescription = new TextBox { Location = new Point(fx, y), Size = new Size(fw, 50), Multiline = true, Font = new Font("Arial", 10) };
            this.Controls.Add(txtDescription);
            y += 60;

            AddLabel("Toplam Tutar (₺):", lx, y);
            nudTotalAmount = new NumericUpDown { Location = new Point(fx, y), Size = new Size(fw, 25), Maximum = 99999999, DecimalPlaces = 2, Font = new Font("Arial", 10) };
            nudTotalAmount.ValueChanged += RecalcFinal;
            this.Controls.Add(nudTotalAmount);
            y += rh;

            AddLabel("KDV (₺):", lx, y);
            nudTaxAmount = new NumericUpDown { Location = new Point(fx, y), Size = new Size(fw, 25), Maximum = 99999999, DecimalPlaces = 2, Font = new Font("Arial", 10) };
            nudTaxAmount.ValueChanged += RecalcFinal;
            this.Controls.Add(nudTaxAmount);
            y += rh;

            AddLabel("İndirim (₺):", lx, y);
            nudDiscountAmount = new NumericUpDown { Location = new Point(fx, y), Size = new Size(fw, 25), Maximum = 99999999, DecimalPlaces = 2, Font = new Font("Arial", 10) };
            nudDiscountAmount.ValueChanged += RecalcFinal;
            this.Controls.Add(nudDiscountAmount);
            y += rh;

            AddLabel("Net Tutar (₺):", lx, y);
            nudFinalAmount = new NumericUpDown { Location = new Point(fx, y), Size = new Size(fw, 25), Maximum = 99999999, DecimalPlaces = 2, Font = new Font("Arial", 10), ReadOnly = true, BackColor = Color.FromArgb(240, 240, 240) };
            this.Controls.Add(nudFinalAmount);
            y += rh;

            AddLabel("Teklif Tarihi:", lx, y);
            dtpOfferDate = new DateTimePicker { Location = new Point(fx, y), Size = new Size(fw, 25), Format = DateTimePickerFormat.Short, Font = new Font("Arial", 10) };
            this.Controls.Add(dtpOfferDate);
            y += rh;

            chkHasExpiry = new CheckBox { Text = "Son Geçerlilik:", Location = new Point(lx, y + 3), Size = new Size(140, 20), Font = new Font("Arial", 10) };
            chkHasExpiry.CheckedChanged += (s, e) => dtpExpiryDate.Enabled = chkHasExpiry.Checked;
            this.Controls.Add(chkHasExpiry);
            dtpExpiryDate = new DateTimePicker { Location = new Point(fx, y), Size = new Size(fw, 25), Format = DateTimePickerFormat.Short, Font = new Font("Arial", 10), Enabled = false, Value = DateTime.Now.AddDays(30) };
            this.Controls.Add(dtpExpiryDate);
            y += rh;

            AddLabel("Notlar:", lx, y);
            txtNotes = new TextBox { Location = new Point(fx, y), Size = new Size(fw, 45), Multiline = true, Font = new Font("Arial", 10) };
            this.Controls.Add(txtNotes);
            y += 55;

            btnSave = new Button { Text = "Kaydet", Location = new Point(fx, y), Size = new Size(140, 35), Font = new Font("Arial", 10, FontStyle.Bold), BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, Cursor = Cursors.Hand, FlatStyle = FlatStyle.Flat };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnCancel = new Button { Text = "İptal", Location = new Point(fx + 155, y), Size = new Size(140, 35), Font = new Font("Arial", 10, FontStyle.Bold), BackColor = Color.FromArgb(231, 76, 60), ForeColor = Color.White, Cursor = Cursors.Hand, FlatStyle = FlatStyle.Flat };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnCancel);

            this.ClientSize = new Size(480, y + 55);
        }

        private void RecalcFinal(object sender, EventArgs e)
        {
            nudFinalAmount.Value = nudTotalAmount.Value + nudTaxAmount.Value - nudDiscountAmount.Value;
        }

        private void AddLabel(string text, int x, int y, int fontSize = 10, bool bold = false)
        {
            this.Controls.Add(new Label
            {
                Text = text,
                Location = new Point(x, y + 3),
                Size = new Size(140, 22),
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
                if (((ComboBoxItem)cmbCustomer.Items[i]).Value == _offer.CustomerId)
                {
                    cmbCustomer.SelectedIndex = i;
                    break;
                }
            }

            txtTitle.Text = _offer.Title ?? string.Empty;
            txtDescription.Text = _offer.Description ?? string.Empty;
            txtNotes.Text = _offer.Notes ?? string.Empty;
            nudTotalAmount.Value = _offer.TotalAmount;
            nudTaxAmount.Value = _offer.TaxAmount;
            nudDiscountAmount.Value = _offer.DiscountAmount;
            nudFinalAmount.Value = _offer.FinalAmount;
            dtpOfferDate.Value = _offer.OfferDate == DateTime.MinValue ? DateTime.Now : _offer.OfferDate;

            if (_offer.ExpiryDate.HasValue)
            {
                chkHasExpiry.Checked = true;
                dtpExpiryDate.Value = _offer.ExpiryDate.Value;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (cmbCustomer.SelectedItem == null || string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Lütfen müşteri ve başlık alanlarını doldurunuz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _offer.CustomerId = ((ComboBoxItem)cmbCustomer.SelectedItem).Value;
                _offer.UserId = Program.CurrentUser?.Id ?? 1;
                _offer.Title = txtTitle.Text.Trim();
                _offer.Description = txtDescription.Text.Trim();
                _offer.Notes = txtNotes.Text.Trim();
                _offer.TotalAmount = nudTotalAmount.Value;
                _offer.TaxAmount = nudTaxAmount.Value;
                _offer.DiscountAmount = nudDiscountAmount.Value;
                _offer.FinalAmount = nudFinalAmount.Value;
                _offer.OfferDate = dtpOfferDate.Value;
                _offer.ExpiryDate = chkHasExpiry.Checked ? dtpExpiryDate.Value : (DateTime?)null;
                _offer.CreatedByUserId = Program.CurrentUser?.Id;

                if (!_isEditMode)
                    _offer.Status = "Pending";

                bool success = _isEditMode ? _offerService.UpdateOffer(_offer) : _offerService.AddOffer(_offer);

                if (success)
                {
                    Program.LoggingService.LogDataCreate(Program.CurrentUser?.Id, "Offers", _offer.Id, _offer.Title);
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
