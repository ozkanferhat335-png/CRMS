using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CRMS.Business.Services;
using CRMS.Entity.Models;

namespace CRMS.UI.Forms
{
    public class MeetingsControl : UserControl
    {
        private readonly MeetingService _meetingService;
        private readonly CustomerService _customerService;
        private readonly UserService _userService;
        private DataGridView dgvMeetings;
        private ComboBox cmbStatusFilter;
        private ComboBox cmbTypeFilter;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnComplete;
        private Button btnDelete;

        public MeetingsControl()
        {
            _meetingService = new MeetingService(Program.UnitOfWork);
            _customerService = new CustomerService(Program.UnitOfWork);
            _userService = new UserService(Program.UnitOfWork);
            InitializeCustomComponents();
            LoadMeetings();
        }

        private void InitializeCustomComponents()
        {
            this.BackColor = Color.FromArgb(245, 245, 245);

            Label lblTitle = new Label
            {
                Text = "GÖRÜŞMELER",
                Location = new Point(20, 20),
                Size = new Size(300, 30),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 45, 48)
            };
            this.Controls.Add(lblTitle);

            this.Controls.Add(new Label { Text = "Durum:", Location = new Point(20, 65), Size = new Size(55, 25), Font = new Font("Arial", 10), TextAlign = ContentAlignment.MiddleLeft });
            cmbStatusFilter = new ComboBox { Location = new Point(80, 65), Size = new Size(130, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 10) };
            cmbStatusFilter.Items.AddRange(new object[] { "Tümü", "Planned", "Completed", "Cancelled" });
            cmbStatusFilter.SelectedIndex = 0;
            cmbStatusFilter.SelectedIndexChanged += (s, e) => LoadMeetings();
            this.Controls.Add(cmbStatusFilter);

            this.Controls.Add(new Label { Text = "Tür:", Location = new Point(220, 65), Size = new Size(40, 25), Font = new Font("Arial", 10), TextAlign = ContentAlignment.MiddleLeft });
            cmbTypeFilter = new ComboBox { Location = new Point(265, 65), Size = new Size(130, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 10) };
            cmbTypeFilter.Items.AddRange(new object[] { "Tümü", "Call", "Meeting", "Email", "WhatsApp" });
            cmbTypeFilter.SelectedIndex = 0;
            cmbTypeFilter.SelectedIndexChanged += (s, e) => LoadMeetings();
            this.Controls.Add(cmbTypeFilter);

            btnAdd = CreateButton("Yeni Ekle", 410, 65, Color.FromArgb(46, 204, 113));
            btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(btnAdd);

            btnEdit = CreateButton("Düzenle", 520, 65, Color.FromArgb(241, 196, 15));
            btnEdit.Click += BtnEdit_Click;
            this.Controls.Add(btnEdit);

            btnComplete = CreateButton("Tamamla", 630, 65, Color.FromArgb(26, 188, 156));
            btnComplete.Click += BtnComplete_Click;
            this.Controls.Add(btnComplete);

            btnDelete = CreateButton("Sil", 740, 65, Color.FromArgb(231, 76, 60));
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            dgvMeetings = new DataGridView
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
            dgvMeetings.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9, FontStyle.Bold);
            dgvMeetings.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            dgvMeetings.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvMeetings.EnableHeadersVisualStyles = false;

            dgvMeetings.Columns.Add("Id", "ID");
            dgvMeetings.Columns.Add("CustomerName", "Müşteri");
            dgvMeetings.Columns.Add("Title", "Başlık");
            dgvMeetings.Columns.Add("MeetingType", "Tür");
            dgvMeetings.Columns.Add("Status", "Durum");
            dgvMeetings.Columns.Add("MeetingDate", "Tarih");
            dgvMeetings.Columns.Add("AssignedUser", "Sorumlu");
            dgvMeetings.Columns.Add("Result", "Sonuç");

            dgvMeetings.Columns["Id"].Visible = false;
            this.Controls.Add(dgvMeetings);
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

        private void LoadMeetings()
        {
            try
            {
                dgvMeetings.Rows.Clear();
                var meetings = _meetingService.GetAllMeetings();
                var customers = _customerService.GetAllCustomers().ToDictionary(c => c.Id, c => c);
                var users = _userService.GetAllUsers().ToDictionary(u => u.Id, u => u);

                string statusFilter = cmbStatusFilter.SelectedItem?.ToString();
                string typeFilter = cmbTypeFilter.SelectedItem?.ToString();

                if (statusFilter != "Tümü" && !string.IsNullOrEmpty(statusFilter))
                    meetings = meetings.Where(m => m.Status == statusFilter).ToList();

                if (typeFilter != "Tümü" && !string.IsNullOrEmpty(typeFilter))
                    meetings = meetings.Where(m => m.MeetingType == typeFilter).ToList();

                foreach (var meeting in meetings)
                {
                    customers.TryGetValue(meeting.CustomerId, out var customer);
                    string customerName = customer != null ? $"{customer.FirstName} {customer.LastName}".Trim() : "Bilinmiyor";

                    users.TryGetValue(meeting.UserId, out var user);
                    string userName = user != null ? $"{user.FirstName} {user.LastName}".Trim() : "Bilinmiyor";

                    dgvMeetings.Rows.Add(
                        meeting.Id,
                        customerName,
                        meeting.Title,
                        meeting.MeetingType,
                        meeting.Status,
                        meeting.MeetingDate.ToString("dd/MM/yyyy HH:mm"),
                        userName,
                        meeting.Result ?? "-"
                    );
                }

                foreach (DataGridViewRow row in dgvMeetings.Rows)
                {
                    string status = row.Cells["Status"].Value?.ToString();
                    if (status == "Completed") row.DefaultCellStyle.ForeColor = Color.FromArgb(39, 174, 96);
                    else if (status == "Cancelled") row.DefaultCellStyle.ForeColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Görüşmeler yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new MeetingForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                    LoadMeetings();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvMeetings.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen düzenlemek istediğiniz görüşmeyi seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int meetingId = (int)dgvMeetings.SelectedRows[0].Cells["Id"].Value;
            var meeting = _meetingService.GetMeetingById(meetingId);
            if (meeting != null)
            {
                using (var form = new MeetingForm(meeting))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                        LoadMeetings();
                }
            }
        }

        private void BtnComplete_Click(object sender, EventArgs e)
        {
            if (dgvMeetings.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen tamamlamak istediğiniz görüşmeyi seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string result = Microsoft.VisualBasic.Interaction.InputBox("Görüşme sonucunu giriniz:", "Görüşme Sonucu", "");
            int meetingId = (int)dgvMeetings.SelectedRows[0].Cells["Id"].Value;
            try
            {
                _meetingService.CompleteMeeting(meetingId, result);
                Program.LoggingService.LogDataUpdate(Program.CurrentUser?.Id, "Meetings", meetingId, "", "Completed");
                LoadMeetings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvMeetings.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silmek istediğiniz görüşmeyi seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Seçili görüşmeyi silmek istediğinizden emin misiniz?", "Onay",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    int meetingId = (int)dgvMeetings.SelectedRows[0].Cells["Id"].Value;
                    _meetingService.DeleteMeeting(meetingId);
                    Program.LoggingService.LogDataDelete(Program.CurrentUser?.Id, "Meetings", meetingId, meetingId.ToString());
                    LoadMeetings();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Silme sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
