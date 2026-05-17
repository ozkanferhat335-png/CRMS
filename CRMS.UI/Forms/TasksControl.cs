using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CRMS.Business.Services;
using CRMS.Entity.Models;

namespace CRMS.UI.Forms
{
    public class TasksControl : UserControl
    {
        private readonly TaskService _taskService;
        private readonly UserService _userService;
        private DataGridView dgvTasks;
        private ComboBox cmbStatusFilter;
        private ComboBox cmbPriorityFilter;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnComplete;
        private Label lblStats;

        public TasksControl()
        {
            _taskService = new TaskService(Program.UnitOfWork);
            _userService = new UserService(Program.UnitOfWork);
            InitializeCustomComponents();
            LoadTasks();
        }

        private void InitializeCustomComponents()
        {
            this.BackColor = Color.FromArgb(245, 245, 245);

            Label lblTitle = new Label
            {
                Text = "GÖREVLER",
                Location = new Point(20, 20),
                Size = new Size(300, 30),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 45, 48)
            };
            this.Controls.Add(lblTitle);

            // Filters
            AddLabel("Durum:", 20, 65);
            cmbStatusFilter = new ComboBox { Location = new Point(80, 65), Size = new Size(130, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 10) };
            cmbStatusFilter.Items.AddRange(new object[] { "Tümü", "Pending", "InProgress", "Completed", "Cancelled" });
            cmbStatusFilter.SelectedIndex = 0;
            cmbStatusFilter.SelectedIndexChanged += (s, e) => LoadTasks();
            this.Controls.Add(cmbStatusFilter);

            AddLabel("Öncelik:", 220, 65);
            cmbPriorityFilter = new ComboBox { Location = new Point(285, 65), Size = new Size(130, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 10) };
            cmbPriorityFilter.Items.AddRange(new object[] { "Tümü", "Low", "Medium", "High", "Urgent" });
            cmbPriorityFilter.SelectedIndex = 0;
            cmbPriorityFilter.SelectedIndexChanged += (s, e) => LoadTasks();
            this.Controls.Add(cmbPriorityFilter);

            btnAdd = CreateButton("Yeni Ekle", 430, 65, Color.FromArgb(46, 204, 113));
            btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(btnAdd);

            btnEdit = CreateButton("Düzenle", 540, 65, Color.FromArgb(241, 196, 15));
            btnEdit.Click += BtnEdit_Click;
            this.Controls.Add(btnEdit);

            btnComplete = CreateButton("Tamamla", 650, 65, Color.FromArgb(26, 188, 156));
            btnComplete.Click += BtnComplete_Click;
            this.Controls.Add(btnComplete);

            btnDelete = CreateButton("Sil", 760, 65, Color.FromArgb(231, 76, 60));
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            lblStats = new Label
            {
                Text = "",
                Location = new Point(870, 68),
                Size = new Size(200, 25),
                Font = new Font("Arial", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(231, 76, 60)
            };
            this.Controls.Add(lblStats);

            dgvTasks = new DataGridView
            {
                Location = new Point(20, 105),
                Size = new Size(960, 470),
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
            dgvTasks.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9, FontStyle.Bold);
            dgvTasks.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            dgvTasks.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvTasks.EnableHeadersVisualStyles = false;

            dgvTasks.Columns.Add("Id", "ID");
            dgvTasks.Columns.Add("Title", "Başlık");
            dgvTasks.Columns.Add("AssignedTo", "Atanan Kişi");
            dgvTasks.Columns.Add("Priority", "Öncelik");
            dgvTasks.Columns.Add("Status", "Durum");
            dgvTasks.Columns.Add("DueDate", "Son Tarih");
            dgvTasks.Columns.Add("CompletedDate", "Tamamlanma");
            dgvTasks.Columns.Add("Overdue", "Gecikme");

            dgvTasks.Columns["Id"].Visible = false;
            this.Controls.Add(dgvTasks);
        }

        private void AddLabel(string text, int x, int y)
        {
            this.Controls.Add(new Label { Text = text, Location = new Point(x, y + 3), Size = new Size(60, 20), Font = new Font("Arial", 10) });
        }

        private Button CreateButton(string text, int x, int y, Color color)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(100, 28),
                Font = new Font("Arial", 9, FontStyle.Bold),
                BackColor = color,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat
            };
        }

        private void LoadTasks()
        {
            try
            {
                dgvTasks.Rows.Clear();
                var tasks = _taskService.GetAllTasks();
                var users = _userService.GetAllUsers().ToDictionary(u => u.Id, u => u);

                string statusFilter = cmbStatusFilter.SelectedItem?.ToString();
                string priorityFilter = cmbPriorityFilter.SelectedItem?.ToString();

                if (statusFilter != "Tümü" && !string.IsNullOrEmpty(statusFilter))
                    tasks = tasks.Where(t => t.Status == statusFilter).ToList();

                if (priorityFilter != "Tümü" && !string.IsNullOrEmpty(priorityFilter))
                    tasks = tasks.Where(t => t.Priority == priorityFilter).ToList();

                int overdueCount = 0;
                foreach (var task in tasks)
                {
                    users.TryGetValue(task.AssignedToUserId, out var user);
                    string assignedTo = user != null ? $"{user.FirstName} {user.LastName}".Trim() : "Bilinmiyor";
                    bool isOverdue = task.Status != "Completed" && task.Status != "Cancelled" && task.DueDate < DateTime.Now;
                    if (isOverdue) overdueCount++;

                    dgvTasks.Rows.Add(
                        task.Id,
                        task.Title,
                        assignedTo,
                        task.Priority,
                        task.Status,
                        task.DueDate.ToString("dd/MM/yyyy"),
                        task.CompletedDate.HasValue ? task.CompletedDate.Value.ToString("dd/MM/yyyy") : "-",
                        isOverdue ? "⚠ GECİKMİŞ" : ""
                    );
                }

                // Color rows
                foreach (DataGridViewRow row in dgvTasks.Rows)
                {
                    string status = row.Cells["Status"].Value?.ToString();
                    string priority = row.Cells["Priority"].Value?.ToString();
                    string overdue = row.Cells["Overdue"].Value?.ToString();

                    if (!string.IsNullOrEmpty(overdue))
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 235);
                    else if (status == "Completed")
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(39, 174, 96);
                    else if (priority == "Urgent")
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(192, 57, 43);
                    else if (priority == "High")
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(230, 126, 34);
                }

                lblStats.Text = overdueCount > 0 ? $"⚠ {overdueCount} gecikmiş görev!" : "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Görevler yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new TaskForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                    LoadTasks();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvTasks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen düzenlemek istediğiniz görevi seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int taskId = (int)dgvTasks.SelectedRows[0].Cells["Id"].Value;
            var task = _taskService.GetTaskById(taskId);
            if (task != null)
            {
                using (var form = new TaskForm(task))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                        LoadTasks();
                }
            }
        }

        private void BtnComplete_Click(object sender, EventArgs e)
        {
            if (dgvTasks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen tamamlamak istediğiniz görevi seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int taskId = (int)dgvTasks.SelectedRows[0].Cells["Id"].Value;
            try
            {
                _taskService.CompleteTask(taskId);
                Program.LoggingService.LogDataUpdate(Program.CurrentUser?.Id, "Tasks", taskId, "", "Completed");
                LoadTasks();
                MessageBox.Show("Görev tamamlandı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvTasks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silmek istediğiniz görevi seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Seçili görevi silmek istediğinizden emin misiniz?", "Onay",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    int taskId = (int)dgvTasks.SelectedRows[0].Cells["Id"].Value;
                    _taskService.DeleteTask(taskId);
                    Program.LoggingService.LogDataDelete(Program.CurrentUser?.Id, "Tasks", taskId, taskId.ToString());
                    LoadTasks();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Silme sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
