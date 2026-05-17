using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CRMS.Business.Services;
using CRMS.Entity.Models;

namespace CRMS.UI.Forms
{
    public class SettingsControl : UserControl
    {
        private readonly SettingsService _settingsService;
        private readonly UserService _userService;
        private readonly AuthenticationService _authService;
        private TabControl tabSettings;

        public SettingsControl()
        {
            _settingsService = new SettingsService(Program.UnitOfWork);
            _userService = new UserService(Program.UnitOfWork);
            _authService = new AuthenticationService(Program.UnitOfWork);
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            this.BackColor = Color.FromArgb(245, 245, 245);

            Label lblTitle = new Label
            {
                Text = "AYARLAR",
                Location = new Point(20, 20),
                Size = new Size(300, 30),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 45, 48)
            };
            this.Controls.Add(lblTitle);

            tabSettings = new TabControl
            {
                Location = new Point(20, 60),
                Size = new Size(960, 520),
                Font = new Font("Arial", 10)
            };

            tabSettings.TabPages.Add(CreateUserManagementTab());
            tabSettings.TabPages.Add(CreateSystemSettingsTab());
            tabSettings.TabPages.Add(CreateChangePasswordTab());
            tabSettings.TabPages.Add(CreateLogsTab());

            this.Controls.Add(tabSettings);
        }

        private TabPage CreateUserManagementTab()
        {
            var tab = new TabPage("Kullanıcı Yönetimi");
            tab.BackColor = Color.White;

            var dgv = new DataGridView
            {
                Location = new Point(5, 50),
                Size = new Size(940, 380),
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

            dgv.Columns.Add("Id", "ID");
            dgv.Columns.Add("Username", "Kullanıcı Adı");
            dgv.Columns.Add("FullName", "Ad Soyad");
            dgv.Columns.Add("Email", "E-posta");
            dgv.Columns.Add("RoleName", "Rol");
            dgv.Columns.Add("IsActive", "Aktif");
            dgv.Columns.Add("IsLocked", "Kilitli");
            dgv.Columns.Add("LastLogin", "Son Giriş");
            dgv.Columns["Id"].Visible = false;

            Action loadUsers = () =>
            {
                try
                {
                    dgv.Rows.Clear();
                    var users = _userService.GetAllUsers();
                    var roles = Program.UnitOfWork.Roles.GetAll().ToDictionary(r => r.Id, r => r);
                    foreach (var u in users)
                    {
                        roles.TryGetValue(u.RoleId, out var role);
                        dgv.Rows.Add(u.Id, u.Username, $"{u.FirstName} {u.LastName}".Trim(), u.Email, role?.RoleName ?? "-", u.IsActive ? "Evet" : "Hayır", u.IsLocked ? "Evet" : "Hayır", u.LastLoginDate.HasValue ? u.LastLoginDate.Value.ToString("dd/MM/yyyy HH:mm") : "-");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Kullanıcılar yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            // Buttons
            var btnAdd = CreateSmallButton("Yeni Kullanıcı", 5, 10, Color.FromArgb(46, 204, 113));
            btnAdd.Click += (s, e) =>
            {
                using (var form = new UserForm())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                        loadUsers();
                }
            };
            tab.Controls.Add(btnAdd);

            var btnEdit = CreateSmallButton("Düzenle", 135, 10, Color.FromArgb(241, 196, 15));
            btnEdit.Click += (s, e) =>
            {
                if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Kullanıcı seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                int userId = (int)dgv.SelectedRows[0].Cells["Id"].Value;
                var user = _userService.GetUserById(userId);
                if (user != null)
                {
                    using (var form = new UserForm(user))
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                            loadUsers();
                    }
                }
            };
            tab.Controls.Add(btnEdit);

            var btnUnlock = CreateSmallButton("Kilidi Aç", 265, 10, Color.FromArgb(26, 188, 156));
            btnUnlock.Click += (s, e) =>
            {
                if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Kullanıcı seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                int userId = (int)dgv.SelectedRows[0].Cells["Id"].Value;
                _authService.UnlockUser(userId);
                loadUsers();
                MessageBox.Show("Kullanıcı kilidi açıldı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            tab.Controls.Add(btnUnlock);

            var btnResetPwd = CreateSmallButton("Şifre Sıfırla", 395, 10, Color.FromArgb(230, 126, 34));
            btnResetPwd.Click += (s, e) =>
            {
                if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Kullanıcı seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                int userId = (int)dgv.SelectedRows[0].Cells["Id"].Value;
                string newPwd = Microsoft.VisualBasic.Interaction.InputBox("Yeni şifreyi giriniz:", "Şifre Sıfırla", "");
                if (!string.IsNullOrWhiteSpace(newPwd))
                {
                    _authService.ResetPassword(userId, newPwd);
                    MessageBox.Show("Şifre sıfırlandı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };
            tab.Controls.Add(btnResetPwd);

            var btnDelete = CreateSmallButton("Sil", 525, 10, Color.FromArgb(231, 76, 60));
            btnDelete.Click += (s, e) =>
            {
                if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Kullanıcı seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                int userId = (int)dgv.SelectedRows[0].Cells["Id"].Value;
                if (userId == Program.CurrentUser?.Id) { MessageBox.Show("Kendi hesabınızı silemezsiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                if (MessageBox.Show("Kullanıcıyı silmek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _userService.DeleteUser(userId);
                    loadUsers();
                }
            };
            tab.Controls.Add(btnDelete);

            tab.Controls.Add(dgv);
            loadUsers();
            return tab;
        }

        private TabPage CreateSystemSettingsTab()
        {
            var tab = new TabPage("Sistem Ayarları");
            tab.BackColor = Color.White;

            int y = 20;
            var settingControls = new System.Collections.Generic.Dictionary<string, TextBox>();

            string[] keys = { "CompanyName", "AppTitle", "SessionTimeoutMinutes", "MaxFailedLoginAttempts", "AccountLockoutMinutes" };
            string[] labels = { "Şirket Adı:", "Uygulama Başlığı:", "Oturum Zaman Aşımı (dk):", "Maks. Başarısız Giriş:", "Hesap Kilitleme Süresi (dk):" };

            for (int i = 0; i < keys.Length; i++)
            {
                tab.Controls.Add(new Label { Text = labels[i], Location = new Point(20, y + 3), Size = new Size(220, 22), Font = new Font("Arial", 10) });
                var txt = new TextBox { Location = new Point(250, y), Size = new Size(300, 25), Font = new Font("Arial", 10), Text = _settingsService.GetSettingValue(keys[i], "") };
                tab.Controls.Add(txt);
                settingControls[keys[i]] = txt;
                y += 38;
            }

            var btnSave = new Button { Text = "Ayarları Kaydet", Location = new Point(250, y + 10), Size = new Size(160, 35), Font = new Font("Arial", 10, FontStyle.Bold), BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, Cursor = Cursors.Hand, FlatStyle = FlatStyle.Flat };
            btnSave.Click += (s, e) =>
            {
                try
                {
                    foreach (var kvp in settingControls)
                        _settingsService.SetSettingValue(kvp.Key, kvp.Value.Text.Trim(), Program.CurrentUser?.Id);

                    MessageBox.Show("Ayarlar kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            tab.Controls.Add(btnSave);

            return tab;
        }

        private TabPage CreateChangePasswordTab()
        {
            var tab = new TabPage("Şifre Değiştir");
            tab.BackColor = Color.White;

            int y = 30;

            tab.Controls.Add(new Label { Text = "Mevcut Şifre:", Location = new Point(100, y + 3), Size = new Size(150, 22), Font = new Font("Arial", 10) });
            var txtOld = new TextBox { Location = new Point(260, y), Size = new Size(250, 25), Font = new Font("Arial", 10), UseSystemPasswordChar = true };
            tab.Controls.Add(txtOld);
            y += 40;

            tab.Controls.Add(new Label { Text = "Yeni Şifre:", Location = new Point(100, y + 3), Size = new Size(150, 22), Font = new Font("Arial", 10) });
            var txtNew = new TextBox { Location = new Point(260, y), Size = new Size(250, 25), Font = new Font("Arial", 10), UseSystemPasswordChar = true };
            tab.Controls.Add(txtNew);
            y += 40;

            tab.Controls.Add(new Label { Text = "Yeni Şifre (Tekrar):", Location = new Point(100, y + 3), Size = new Size(150, 22), Font = new Font("Arial", 10) });
            var txtConfirm = new TextBox { Location = new Point(260, y), Size = new Size(250, 25), Font = new Font("Arial", 10), UseSystemPasswordChar = true };
            tab.Controls.Add(txtConfirm);
            y += 50;

            var btnChange = new Button { Text = "Şifreyi Değiştir", Location = new Point(260, y), Size = new Size(160, 35), Font = new Font("Arial", 10, FontStyle.Bold), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, Cursor = Cursors.Hand, FlatStyle = FlatStyle.Flat };
            btnChange.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtOld.Text) || string.IsNullOrWhiteSpace(txtNew.Text))
                {
                    MessageBox.Show("Lütfen tüm alanları doldurunuz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtNew.Text != txtConfirm.Text)
                {
                    MessageBox.Show("Yeni şifreler eşleşmiyor.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtNew.Text.Length < 6)
                {
                    MessageBox.Show("Şifre en az 6 karakter olmalıdır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool success = _authService.ChangePassword(Program.CurrentUser?.Id ?? 0, txtOld.Text, txtNew.Text);
                if (success)
                {
                    MessageBox.Show("Şifre başarıyla değiştirildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtOld.Clear(); txtNew.Clear(); txtConfirm.Clear();
                }
                else
                {
                    MessageBox.Show("Mevcut şifre hatalı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            tab.Controls.Add(btnChange);

            return tab;
        }

        private TabPage CreateLogsTab()
        {
            var tab = new TabPage("Sistem Logları");
            tab.BackColor = Color.White;

            var dgv = new DataGridView
            {
                Location = new Point(5, 45),
                Size = new Size(940, 430),
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

            dgv.Columns.Add("Action", "İşlem");
            dgv.Columns.Add("TableName", "Tablo");
            dgv.Columns.Add("Description", "Açıklama");
            dgv.Columns.Add("IpAddress", "IP Adresi");
            dgv.Columns.Add("CreatedDate", "Tarih");

            var btnLoad = CreateSmallButton("Logları Yükle", 5, 8, Color.FromArgb(52, 152, 219));
            btnLoad.Click += (s, e) =>
            {
                try
                {
                    dgv.Rows.Clear();
                    var logs = Program.UnitOfWork.Logs.GetAll();
                    foreach (var log in logs)
                        dgv.Rows.Add(log.Action, log.TableName, log.Description, log.IpAddress, log.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss"));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Loglar yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            tab.Controls.Add(btnLoad);
            tab.Controls.Add(dgv);

            btnLoad.PerformClick();
            return tab;
        }

        private Button CreateSmallButton(string text, int x, int y, Color color)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(120, 28),
                Font = new Font("Arial", 9, FontStyle.Bold),
                BackColor = color,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat
            };
        }
    }
}
