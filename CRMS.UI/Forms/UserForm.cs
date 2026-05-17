using System;
using System.Drawing;
using System.Windows.Forms;
using CRMS.Business.Services;
using CRMS.Entity.Models;

namespace CRMS.UI.Forms
{
    public class UserForm : Form
    {
        private readonly UserService _userService;
        private readonly User _user;
        private readonly bool _isEditMode;

        private TextBox txtUsername;
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private TextBox txtPassword;
        private ComboBox cmbRole;
        private CheckBox chkIsActive;
        private Button btnSave;
        private Button btnCancel;

        public UserForm(User user = null)
        {
            _userService = new UserService(Program.UnitOfWork);
            _user = user ?? new User { IsActive = true };
            _isEditMode = user != null;

            InitializeForm();
            LoadRoles();
            if (_isEditMode)
                PopulateFields();
        }

        private void InitializeForm()
        {
            this.Text = _isEditMode ? "Kullanıcı Düzenle" : "Yeni Kullanıcı";
            this.Size = new Size(460, 430);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            int lx = 20, fx = 155, fw = 265, rh = 38, y = 20;

            AddLabel("KULLANICI BİLGİLERİ", lx, y, 13, true);
            y += 35;

            AddLabel("Kullanıcı Adı *:", lx, y);
            txtUsername = new TextBox { Location = new Point(fx, y), Size = new Size(fw, 25), Font = new Font("Arial", 10) };
            this.Controls.Add(txtUsername);
            y += rh;

            AddLabel("Ad *:", lx, y);
            txtFirstName = new TextBox { Location = new Point(fx, y), Size = new Size(fw, 25), Font = new Font("Arial", 10) };
            this.Controls.Add(txtFirstName);
            y += rh;

            AddLabel("Soyad *:", lx, y);
            txtLastName = new TextBox { Location = new Point(fx, y), Size = new Size(fw, 25), Font = new Font("Arial", 10) };
            this.Controls.Add(txtLastName);
            y += rh;

            AddLabel("E-posta *:", lx, y);
            txtEmail = new TextBox { Location = new Point(fx, y), Size = new Size(fw, 25), Font = new Font("Arial", 10) };
            this.Controls.Add(txtEmail);
            y += rh;

            AddLabel("Telefon:", lx, y);
            txtPhone = new TextBox { Location = new Point(fx, y), Size = new Size(fw, 25), Font = new Font("Arial", 10) };
            this.Controls.Add(txtPhone);
            y += rh;

            AddLabel(_isEditMode ? "Yeni Şifre:" : "Şifre *:", lx, y);
            txtPassword = new TextBox { Location = new Point(fx, y), Size = new Size(fw, 25), Font = new Font("Arial", 10), UseSystemPasswordChar = true };
            this.Controls.Add(txtPassword);
            y += rh;

            AddLabel("Rol *:", lx, y);
            cmbRole = new ComboBox { Location = new Point(fx, y), Size = new Size(fw, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 10) };
            this.Controls.Add(cmbRole);
            y += rh;

            chkIsActive = new CheckBox { Text = "Aktif Kullanıcı", Location = new Point(fx, y), Size = new Size(200, 22), Font = new Font("Arial", 10), Checked = true };
            this.Controls.Add(chkIsActive);
            y += rh;

            btnSave = new Button { Text = "Kaydet", Location = new Point(fx, y), Size = new Size(125, 35), Font = new Font("Arial", 10, FontStyle.Bold), BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, Cursor = Cursors.Hand, FlatStyle = FlatStyle.Flat };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnCancel = new Button { Text = "İptal", Location = new Point(fx + 140, y), Size = new Size(125, 35), Font = new Font("Arial", 10, FontStyle.Bold), BackColor = Color.FromArgb(231, 76, 60), ForeColor = Color.White, Cursor = Cursors.Hand, FlatStyle = FlatStyle.Flat };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnCancel);

            this.ClientSize = new Size(440, y + 55);
        }

        private void AddLabel(string text, int x, int y, int fontSize = 10, bool bold = false)
        {
            this.Controls.Add(new Label
            {
                Text = text,
                Location = new Point(x, y + 3),
                Size = new Size(130, 22),
                Font = new Font("Arial", fontSize, bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = Color.FromArgb(60, 60, 60)
            });
        }

        private void LoadRoles()
        {
            try
            {
                var roles = Program.UnitOfWork.Roles.GetAll();
                cmbRole.Items.Clear();
                foreach (var r in roles)
                    cmbRole.Items.Add(new ComboBoxItem { Text = r.RoleName, Value = r.Id });

                if (cmbRole.Items.Count > 0)
                    cmbRole.SelectedIndex = 0;
            }
            catch { }
        }

        private void PopulateFields()
        {
            txtUsername.Text = _user.Username ?? string.Empty;
            txtFirstName.Text = _user.FirstName ?? string.Empty;
            txtLastName.Text = _user.LastName ?? string.Empty;
            txtEmail.Text = _user.Email ?? string.Empty;
            txtPhone.Text = _user.PhoneNumber ?? string.Empty;
            chkIsActive.Checked = _user.IsActive;

            for (int i = 0; i < cmbRole.Items.Count; i++)
            {
                if (((ComboBoxItem)cmbRole.Items[i]).Value == _user.RoleId)
                {
                    cmbRole.SelectedIndex = i;
                    break;
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) || cmbRole.SelectedItem == null)
            {
                MessageBox.Show("Lütfen zorunlu alanları doldurunuz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_isEditMode && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Lütfen şifre giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _user.Username = txtUsername.Text.Trim();
                _user.FirstName = txtFirstName.Text.Trim();
                _user.LastName = txtLastName.Text.Trim();
                _user.Email = txtEmail.Text.Trim();
                _user.PhoneNumber = txtPhone.Text.Trim();
                _user.RoleId = ((ComboBoxItem)cmbRole.SelectedItem).Value;
                _user.IsActive = chkIsActive.Checked;
                _user.CreatedByUserId = Program.CurrentUser?.Id;

                if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                    _user.PasswordHash = CRMS.Business.Services.Security.PasswordHasher.HashPassword(txtPassword.Text);

                bool success = _isEditMode ? _userService.UpdateUser(_user) : _userService.AddUser(_user);

                if (success)
                {
                    Program.LoggingService.LogDataCreate(Program.CurrentUser?.Id, "Users", _user.Id, _user.Username);
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
