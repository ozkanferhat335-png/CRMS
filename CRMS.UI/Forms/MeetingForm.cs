using System;
using System.Drawing;
using System.Windows.Forms;
using CRMS.Business.Services;
using CRMS.Entity.Models;

namespace CRMS.UI.Forms
{
    public class MeetingForm : Form
    {
        private readonly MeetingService _meetingService;
        private readonly CustomerService _customerService;
        private readonly UserService _userService;
        private readonly Meeting _meeting;
        private readonly bool _isEditMode;

        private ComboBox cmbCustomer;
        private ComboBox cmbUser;
        private ComboBox cmbMeetingType;
        private ComboBox cmbStatus;
        private TextBox txtTitle;
        private TextBox txtDescription;
        private TextBox txtNotes;
        private TextBox txtResult;
        private DateTimePicker dtpMeetingDate;
        private Button btnSave;
        private Button btnCancel;

        public MeetingForm(Meeting meeting = null)
        {
            _meetingService = new MeetingService(Program.UnitOfWork);
            _customerService = new CustomerService(Program.UnitOfWork);
            _userService = new UserService(Program.UnitOfWork);
            _meeting = meeting ?? new Meeting();
            _isEditMode = meeting != null;

            InitializeForm();
            LoadCustomers();
            LoadUsers();
            if (_isEditMode)
                PopulateFields();
        }

        private void InitializeForm()
        {
            this.Text = _isEditMode ? "Görüşme Düzenle" : "Yeni Görüşme";
            this.Size = new Size(480, 490);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            int lx = 20, fx = 160, fw = 280, rh = 38, y = 20;

            AddLabel("YENİ GÖRÜŞME", lx, y, 13, true);
            y += 35;

            AddLabel("Müşteri *:", lx, y);
            cmbCustomer = new ComboBox { Location = new Point(fx, y), Size = new Size(fw, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 10) };
            this.Controls.Add(cmbCustomer);
            y += rh;

            AddLabel("Sorumlu *:", lx, y);
            cmbUser = new ComboBox { Location = new Point(fx, y), Size = new Size(fw, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 10) };
            this.Controls.Add(cmbUser);
            y += rh;

            AddLabel("Başlık *:", lx, y);
            txtTitle = new TextBox { Location = new Point(fx, y), Size = new Size(fw, 25), Font = new Font("Arial", 10) };
            this.Controls.Add(txtTitle);
            y += rh;

            AddLabel("Açıklama:", lx, y);
            txtDescription = new TextBox { Location = new Point(fx, y), Size = new Size(fw, 50), Multiline = true, Font = new Font("Arial", 10) };
            this.Controls.Add(txtDescription);
            y += 60;

            AddLabel("Görüşme Türü:", lx, y);
            cmbMeetingType = new ComboBox { Location = new Point(fx, y), Size = new Size(fw, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 10) };
            cmbMeetingType.Items.AddRange(new object[] { "Call", "Meeting", "Email", "WhatsApp" });
            cmbMeetingType.SelectedIndex = 0;
            this.Controls.Add(cmbMeetingType);
            y += rh;

            AddLabel("Durum:", lx, y);
            cmbStatus = new ComboBox { Location = new Point(fx, y), Size = new Size(fw, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 10) };
            cmbStatus.Items.AddRange(new object[] { "Planned", "Completed", "Cancelled" });
            cmbStatus.SelectedIndex = 0;
            this.Controls.Add(cmbStatus);
            y += rh;

            AddLabel("Tarih *:", lx, y);
            dtpMeetingDate = new DateTimePicker { Location = new Point(fx, y), Size = new Size(fw, 25), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm", Font = new Font("Arial", 10), ShowUpDown = true };
            this.Controls.Add(dtpMeetingDate);
            y += rh;

            AddLabel("Sonuç:", lx, y);
            txtResult = new TextBox { Location = new Point(fx, y), Size = new Size(fw, 25), Font = new Font("Arial", 10) };
            this.Controls.Add(txtResult);
            y += rh;

            AddLabel("Notlar:", lx, y);
            txtNotes = new TextBox { Location = new Point(fx, y), Size = new Size(fw, 45), Multiline = true, Font = new Font("Arial", 10) };
            this.Controls.Add(txtNotes);
            y += 55;

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
                var users = _userService.GetAllUsers();
                cmbUser.Items.Clear();
                foreach (var u in users)
                    cmbUser.Items.Add(new ComboBoxItem { Text = $"{u.FirstName} {u.LastName}".Trim(), Value = u.Id });

                // Default to current user
                for (int i = 0; i < cmbUser.Items.Count; i++)
                {
                    if (((ComboBoxItem)cmbUser.Items[i]).Value == (Program.CurrentUser?.Id ?? 0))
                    {
                        cmbUser.SelectedIndex = i;
                        break;
                    }
                }

                if (cmbUser.SelectedIndex < 0 && cmbUser.Items.Count > 0)
                    cmbUser.SelectedIndex = 0;
            }
            catch { }
        }

        private void PopulateFields()
        {
            for (int i = 0; i < cmbCustomer.Items.Count; i++)
            {
                if (((ComboBoxItem)cmbCustomer.Items[i]).Value == _meeting.CustomerId)
                {
                    cmbCustomer.SelectedIndex = i;
                    break;
                }
            }

            for (int i = 0; i < cmbUser.Items.Count; i++)
            {
                if (((ComboBoxItem)cmbUser.Items[i]).Value == _meeting.UserId)
                {
                    cmbUser.SelectedIndex = i;
                    break;
                }
            }

            txtTitle.Text = _meeting.Title ?? string.Empty;
            txtDescription.Text = _meeting.Description ?? string.Empty;
            txtResult.Text = _meeting.Result ?? string.Empty;
            txtNotes.Text = _meeting.Notes ?? string.Empty;
            dtpMeetingDate.Value = _meeting.MeetingDate == DateTime.MinValue ? DateTime.Now : _meeting.MeetingDate;

            int typeIdx = cmbMeetingType.Items.IndexOf(_meeting.MeetingType ?? "Call");
            cmbMeetingType.SelectedIndex = typeIdx >= 0 ? typeIdx : 0;

            int statusIdx = cmbStatus.Items.IndexOf(_meeting.Status ?? "Planned");
            cmbStatus.SelectedIndex = statusIdx >= 0 ? statusIdx : 0;
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
                _meeting.CustomerId = ((ComboBoxItem)cmbCustomer.SelectedItem).Value;
                _meeting.UserId = ((ComboBoxItem)cmbUser.SelectedItem).Value;
                _meeting.Title = txtTitle.Text.Trim();
                _meeting.Description = txtDescription.Text.Trim();
                _meeting.MeetingType = cmbMeetingType.SelectedItem?.ToString() ?? "Call";
                _meeting.Status = cmbStatus.SelectedItem?.ToString() ?? "Planned";
                _meeting.MeetingDate = dtpMeetingDate.Value;
                _meeting.Result = txtResult.Text.Trim();
                _meeting.Notes = txtNotes.Text.Trim();
                _meeting.CreatedByUserId = Program.CurrentUser?.Id;

                bool success = _isEditMode ? _meetingService.UpdateMeeting(_meeting) : _meetingService.AddMeeting(_meeting);

                if (success)
                {
                    Program.LoggingService.LogDataCreate(Program.CurrentUser?.Id, "Meetings", _meeting.Id, _meeting.Title);
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
