using System;
using System.Drawing;
using System.Windows.Forms;
using CRMS.Business.Services;
using CRMS.Entity.Models;

namespace CRMS.UI.Forms
{
    public class TaskForm : Form
    {
        private readonly TaskService _taskService;
        private readonly UserService _userService;
        private readonly CustomerService _customerService;
        private readonly Task _task;
        private readonly bool _isEditMode;

        private TextBox txtTitle;
        private TextBox txtDescription;
        private ComboBox cmbAssignedTo;
        private ComboBox cmbCustomer;
        private ComboBox cmbPriority;
        private ComboBox cmbStatus;
        private DateTimePicker dtpDueDate;
        private Button btnSave;
        private Button btnCancel;

        public TaskForm(Task task = null)
        {
            _taskService = new TaskService(Program.UnitOfWork);
            _userService = new UserService(Program.UnitOfWork);
            _customerService = new CustomerService(Program.UnitOfWork);
            _task = task ?? new Task();
            _isEditMode = task != null;

            InitializeForm();
            LoadUsers();
            LoadCustomers();
            if (_isEditMode)
                PopulateFields();
        }

        private void InitializeForm()
        {
            this.Text = _isEditMode ? "Görev Düzenle" : "Yeni Görev";
            this.Size = new Size(480, 430);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            int lx = 20, fx = 160, fw = 280, rh = 38, y = 20;

            AddLabel("YENİ GÖREV", lx, y, 14, true);
            y += 35;

            AddLabel("Başlık *:", lx, y);
            txtTitle = new TextBox { Location = new Point(fx, y), Size = new Size(fw, 25), Font = new Font("Arial", 10) };
            this.Controls.Add(txtTitle);
            y += rh;

            AddLabel("Açıklama:", lx, y);
            txtDescription = new TextBox { Location = new Point(fx, y), Size = new Size(fw, 55), Multiline = true, Font = new Font("Arial", 10), ScrollBars = ScrollBars.Vertical };
            this.Controls.Add(txtDescription);
            y += 65;

            AddLabel("Atanan Kişi *:", lx, y);
            cmbAssignedTo = new ComboBox { Location = new Point(fx, y), Size = new Size(fw, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 10) };
            this.Controls.Add(cmbAssignedTo);
            y += rh;

            AddLabel("Müşteri:", lx, y);
            cmbCustomer = new ComboBox { Location = new Point(fx, y), Size = new Size(fw, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 10) };
            this.Controls.Add(cmbCustomer);
            y += rh;

            AddLabel("Öncelik:", lx, y);
            cmbPriority = new ComboBox { Location = new Point(fx, y), Size = new Size(fw, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 10) };
            cmbPriority.Items.AddRange(new object[] { "Low", "Medium", "High", "Urgent" });
            cmbPriority.SelectedIndex = 1;
            this.Controls.Add(cmbPriority);
            y += rh;

            AddLabel("Durum:", lx, y);
            cmbStatus = new ComboBox { Location = new Point(fx, y), Size = new Size(fw, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 10) };
            cmbStatus.Items.AddRange(new object[] { "Pending", "InProgress", "Completed", "Cancelled" });
            cmbStatus.SelectedIndex = 0;
            this.Controls.Add(cmbStatus);
            y += rh;

            AddLabel("Son Tarih *:", lx, y);
            dtpDueDate = new DateTimePicker { Location = new Point(fx, y), Size = new Size(fw, 25), Format = DateTimePickerFormat.Short, Font = new Font("Arial", 10), Value = DateTime.Now.AddDays(7) };
            this.Controls.Add(dtpDueDate);
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

        private void LoadUsers()
        {
            try
            {
                var users = _userService.GetAllUsers();
                cmbAssignedTo.Items.Clear();
                foreach (var u in users)
                    cmbAssignedTo.Items.Add(new ComboBoxItem { Text = $"{u.FirstName} {u.LastName}".Trim(), Value = u.Id });

                if (cmbAssignedTo.Items.Count > 0)
                    cmbAssignedTo.SelectedIndex = 0;
            }
            catch { }
        }

        private void LoadCustomers()
        {
            try
            {
                cmbCustomer.Items.Clear();
                cmbCustomer.Items.Add(new ComboBoxItem { Text = "(Müşteri Yok)", Value = 0 });
                var customers = _customerService.GetAllCustomers();
                foreach (var c in customers)
                    cmbCustomer.Items.Add(new ComboBoxItem { Text = $"{c.FirstName} {c.LastName}".Trim(), Value = c.Id });

                cmbCustomer.SelectedIndex = 0;
            }
            catch { }
        }

        private void PopulateFields()
        {
            txtTitle.Text = _task.Title ?? string.Empty;
            txtDescription.Text = _task.Description ?? string.Empty;
            dtpDueDate.Value = _task.DueDate == DateTime.MinValue ? DateTime.Now.AddDays(7) : _task.DueDate;

            for (int i = 0; i < cmbAssignedTo.Items.Count; i++)
            {
                if (((ComboBoxItem)cmbAssignedTo.Items[i]).Value == _task.AssignedToUserId)
                {
                    cmbAssignedTo.SelectedIndex = i;
                    break;
                }
            }

            for (int i = 0; i < cmbCustomer.Items.Count; i++)
            {
                if (((ComboBoxItem)cmbCustomer.Items[i]).Value == (_task.CustomerId ?? 0))
                {
                    cmbCustomer.SelectedIndex = i;
                    break;
                }
            }

            int priIdx = cmbPriority.Items.IndexOf(_task.Priority ?? "Medium");
            cmbPriority.SelectedIndex = priIdx >= 0 ? priIdx : 1;

            int statusIdx = cmbStatus.Items.IndexOf(_task.Status ?? "Pending");
            cmbStatus.SelectedIndex = statusIdx >= 0 ? statusIdx : 0;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Lütfen görev başlığını giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbAssignedTo.SelectedItem == null)
            {
                MessageBox.Show("Lütfen atanacak kişiyi seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _task.Title = txtTitle.Text.Trim();
                _task.Description = txtDescription.Text.Trim();
                _task.AssignedToUserId = ((ComboBoxItem)cmbAssignedTo.SelectedItem).Value;
                _task.AssignedByUserId = Program.CurrentUser?.Id;
                int customerId = ((ComboBoxItem)cmbCustomer.SelectedItem).Value;
                _task.CustomerId = customerId > 0 ? customerId : (int?)null;
                _task.Priority = cmbPriority.SelectedItem?.ToString() ?? "Medium";
                _task.Status = cmbStatus.SelectedItem?.ToString() ?? "Pending";
                _task.DueDate = dtpDueDate.Value;

                bool success = _isEditMode ? _taskService.UpdateTask(_task) : _taskService.AddTask(_task);

                if (success)
                {
                    Program.LoggingService.LogDataCreate(Program.CurrentUser?.Id, "Tasks", _task.Id, _task.Title);
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
