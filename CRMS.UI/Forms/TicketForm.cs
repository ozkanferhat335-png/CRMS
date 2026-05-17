using System;
using System.Drawing;
using System.Windows.Forms;
using CRMS.Business.Services;
using CRMS.Entity.Models;

namespace CRMS.UI.Forms
{
    public class TicketForm : Form
    {
        private readonly TicketService _ticketService;
        private readonly CustomerService _customerService;
        private readonly UserService _userService;
        private readonly Ticket _ticket;
        private readonly bool _isEditMode;

        private ComboBox cmbCustomer;
        private ComboBox cmbAssignedTo;
        private TextBox txtTitle;
        private TextBox txtDescription;
        private ComboBox cmbPriority;
        private ComboBox cmbStatus;
        private TextBox txtCategory;
        private TextBox txtDepartment;
        private Button btnSave;
        private Button btnCancel;

        public TicketForm(Ticket ticket = null)
        {
            _ticketService = new TicketService(Program.UnitOfWork);
            _customerService = new CustomerService(Program.UnitOfWork);
            _userService = new UserService(Program.UnitOfWork);
            _ticket = ticket ?? new Ticket();
            _isEditMode = ticket != null;

            InitializeForm();
            LoadCustomers();
            LoadUsers();
            if (_isEditMode)
                PopulateFields();
        }

        private void InitializeForm()
        {
            this.Text = _isEditMode ? "Talep Düzenle" : "Yeni Destek Talebi";
            this.Size = new Size(480, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            int lx = 20, fx = 160, fw = 280, rh = 38, y = 20;

            AddLabel("YENİ DESTEK TALEBİ", lx, y, 13, true);
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
            txtDescription = new TextBox { Location = new Point(fx, y), Size = new Size(fw, 60), Multiline = true, Font = new Font("Arial", 10), ScrollBars = ScrollBars.Vertical };
            this.Controls.Add(txtDescription);
            y += 70;

            AddLabel("Öncelik:", lx, y);
            cmbPriority = new ComboBox { Location = new Point(fx, y), Size = new Size(fw, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 10) };
            cmbPriority.Items.AddRange(new object[] { "Low", "Medium", "High", "Urgent" });
            cmbPriority.SelectedIndex = 1;
            this.Controls.Add(cmbPriority);
            y += rh;

            AddLabel("Durum:", lx, y);
            cmbStatus = new ComboBox { Location = new Point(fx, y), Size = new Size(fw, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 10) };
            cmbStatus.Items.AddRange(new object[] { "Open", "InProgress", "OnHold", "Resolved", "Closed" });
            cmbStatus.SelectedIndex = 0;
            this.Controls.Add(cmbStatus);
            y += rh;

            AddLabel("Kategori:", lx, y);
            txtCategory = new TextBox { Location = new Point(fx, y), Size = new Size(fw, 25), Font = new Font("Arial", 10) };
            this.Controls.Add(txtCategory);
            y += rh;

            AddLabel("Departman:", lx, y);
            txtDepartment = new TextBox { Location = new Point(fx, y), Size = new Size(fw, 25), Font = new Font("Arial", 10) };
            this.Controls.Add(txtDepartment);
            y += rh;

            AddLabel("Atanan Kişi:", lx, y);
            cmbAssignedTo = new ComboBox { Location = new Point(fx, y), Size = new Size(fw, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 10) };
            this.Controls.Add(cmbAssignedTo);
            y += rh;

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
                    cmbCustomer.Items.Add(new ComboBoxItem { Text = $"{c.FirstName} {c.LastName}".Trim(), Value = c.Id });

                if (cmbCustomer.Items.Count > 0)
                    cmbCustomer.SelectedIndex = 0;
            }
            catch { }
        }

        private void LoadUsers()
        {
            try
            {
                cmbAssignedTo.Items.Clear();
                cmbAssignedTo.Items.Add(new ComboBoxItem { Text = "(Atanmadı)", Value = 0 });
                var users = _userService.GetAllUsers();
                foreach (var u in users)
                    cmbAssignedTo.Items.Add(new ComboBoxItem { Text = $"{u.FirstName} {u.LastName}".Trim(), Value = u.Id });

                cmbAssignedTo.SelectedIndex = 0;
            }
            catch { }
        }

        private void PopulateFields()
        {
            for (int i = 0; i < cmbCustomer.Items.Count; i++)
            {
                if (((ComboBoxItem)cmbCustomer.Items[i]).Value == _ticket.CustomerId)
                {
                    cmbCustomer.SelectedIndex = i;
                    break;
                }
            }

            txtTitle.Text = _ticket.Title ?? string.Empty;
            txtDescription.Text = _ticket.Description ?? string.Empty;
            txtCategory.Text = _ticket.Category ?? string.Empty;
            txtDepartment.Text = _ticket.Department ?? string.Empty;

            int priIdx = cmbPriority.Items.IndexOf(_ticket.Priority ?? "Medium");
            cmbPriority.SelectedIndex = priIdx >= 0 ? priIdx : 1;

            int statusIdx = cmbStatus.Items.IndexOf(_ticket.Status ?? "Open");
            cmbStatus.SelectedIndex = statusIdx >= 0 ? statusIdx : 0;

            if (_ticket.AssignedToUserId.HasValue)
            {
                for (int i = 0; i < cmbAssignedTo.Items.Count; i++)
                {
                    if (((ComboBoxItem)cmbAssignedTo.Items[i]).Value == _ticket.AssignedToUserId.Value)
                    {
                        cmbAssignedTo.SelectedIndex = i;
                        break;
                    }
                }
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
                _ticket.CustomerId = ((ComboBoxItem)cmbCustomer.SelectedItem).Value;
                _ticket.Title = txtTitle.Text.Trim();
                _ticket.Description = txtDescription.Text.Trim();
                _ticket.Priority = cmbPriority.SelectedItem?.ToString() ?? "Medium";
                _ticket.Status = cmbStatus.SelectedItem?.ToString() ?? "Open";
                _ticket.Category = txtCategory.Text.Trim();
                _ticket.Department = txtDepartment.Text.Trim();
                _ticket.CreatedByUserId = Program.CurrentUser?.Id;

                int assignedId = ((ComboBoxItem)cmbAssignedTo.SelectedItem).Value;
                _ticket.AssignedToUserId = assignedId > 0 ? assignedId : (int?)null;

                bool success = _isEditMode ? _ticketService.UpdateTicket(_ticket) : _ticketService.AddTicket(_ticket);

                if (success)
                {
                    Program.LoggingService.LogDataCreate(Program.CurrentUser?.Id, "Tickets", _ticket.Id, _ticket.Title);
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
