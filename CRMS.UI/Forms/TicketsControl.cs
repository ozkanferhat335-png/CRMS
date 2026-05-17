using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CRMS.Business.Services;
using CRMS.Entity.Models;

namespace CRMS.UI.Forms
{
    public class TicketsControl : UserControl
    {
        private readonly TicketService _ticketService;
        private readonly CustomerService _customerService;
        private readonly UserService _userService;
        private DataGridView dgvTickets;
        private ComboBox cmbStatusFilter;
        private ComboBox cmbPriorityFilter;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnResolve;
        private Button btnDelete;
        private Label lblStats;

        public TicketsControl()
        {
            _ticketService = new TicketService(Program.UnitOfWork);
            _customerService = new CustomerService(Program.UnitOfWork);
            _userService = new UserService(Program.UnitOfWork);
            InitializeCustomComponents();
            LoadTickets();
        }

        private void InitializeCustomComponents()
        {
            this.BackColor = Color.FromArgb(245, 245, 245);

            Label lblTitle = new Label
            {
                Text = "DESTEK TALEPLERİ",
                Location = new Point(20, 20),
                Size = new Size(350, 30),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 45, 48)
            };
            this.Controls.Add(lblTitle);

            this.Controls.Add(new Label { Text = "Durum:", Location = new Point(20, 65), Size = new Size(55, 25), Font = new Font("Arial", 10), TextAlign = ContentAlignment.MiddleLeft });
            cmbStatusFilter = new ComboBox { Location = new Point(80, 65), Size = new Size(130, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 10) };
            cmbStatusFilter.Items.AddRange(new object[] { "Tümü", "Open", "InProgress", "OnHold", "Resolved", "Closed" });
            cmbStatusFilter.SelectedIndex = 0;
            cmbStatusFilter.SelectedIndexChanged += (s, e) => LoadTickets();
            this.Controls.Add(cmbStatusFilter);

            this.Controls.Add(new Label { Text = "Öncelik:", Location = new Point(220, 65), Size = new Size(60, 25), Font = new Font("Arial", 10), TextAlign = ContentAlignment.MiddleLeft });
            cmbPriorityFilter = new ComboBox { Location = new Point(285, 65), Size = new Size(120, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 10) };
            cmbPriorityFilter.Items.AddRange(new object[] { "Tümü", "Low", "Medium", "High", "Urgent" });
            cmbPriorityFilter.SelectedIndex = 0;
            cmbPriorityFilter.SelectedIndexChanged += (s, e) => LoadTickets();
            this.Controls.Add(cmbPriorityFilter);

            btnAdd = CreateButton("Yeni Ekle", 420, 65, Color.FromArgb(46, 204, 113));
            btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(btnAdd);

            btnEdit = CreateButton("Düzenle", 530, 65, Color.FromArgb(241, 196, 15));
            btnEdit.Click += BtnEdit_Click;
            this.Controls.Add(btnEdit);

            btnResolve = CreateButton("Çöz", 640, 65, Color.FromArgb(26, 188, 156));
            btnResolve.Click += BtnResolve_Click;
            this.Controls.Add(btnResolve);

            btnDelete = CreateButton("Sil", 750, 65, Color.FromArgb(231, 76, 60));
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            lblStats = new Label { Text = "", Location = new Point(860, 68), Size = new Size(200, 25), Font = new Font("Arial", 9, FontStyle.Bold), ForeColor = Color.FromArgb(231, 76, 60) };
            this.Controls.Add(lblStats);

            dgvTickets = new DataGridView
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
            dgvTickets.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9, FontStyle.Bold);
            dgvTickets.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            dgvTickets.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvTickets.EnableHeadersVisualStyles = false;

            dgvTickets.Columns.Add("Id", "ID");
            dgvTickets.Columns.Add("TicketCode", "Talep Kodu");
            dgvTickets.Columns.Add("CustomerName", "Müşteri");
            dgvTickets.Columns.Add("Title", "Başlık");
            dgvTickets.Columns.Add("Priority", "Öncelik");
            dgvTickets.Columns.Add("Status", "Durum");
            dgvTickets.Columns.Add("Category", "Kategori");
            dgvTickets.Columns.Add("AssignedTo", "Atanan");
            dgvTickets.Columns.Add("CreatedDate", "Oluşturma");

            dgvTickets.Columns["Id"].Visible = false;
            this.Controls.Add(dgvTickets);
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

        private void LoadTickets()
        {
            try
            {
                dgvTickets.Rows.Clear();
                var tickets = _ticketService.GetAllTickets();
                var customers = _customerService.GetAllCustomers().ToDictionary(c => c.Id, c => c);
                var users = _userService.GetAllUsers().ToDictionary(u => u.Id, u => u);

                string statusFilter = cmbStatusFilter.SelectedItem?.ToString();
                string priorityFilter = cmbPriorityFilter.SelectedItem?.ToString();

                if (statusFilter != "Tümü" && !string.IsNullOrEmpty(statusFilter))
                    tickets = tickets.Where(t => t.Status == statusFilter).ToList();

                if (priorityFilter != "Tümü" && !string.IsNullOrEmpty(priorityFilter))
                    tickets = tickets.Where(t => t.Priority == priorityFilter).ToList();

                int urgentCount = 0;
                foreach (var ticket in tickets)
                {
                    customers.TryGetValue(ticket.CustomerId, out var customer);
                    string customerName = customer != null ? $"{customer.FirstName} {customer.LastName}".Trim() : "Bilinmiyor";

                    string assignedTo = "-";
                    if (ticket.AssignedToUserId.HasValue && users.TryGetValue(ticket.AssignedToUserId.Value, out var assignedUser))
                        assignedTo = $"{assignedUser.FirstName} {assignedUser.LastName}".Trim();

                    if (ticket.Priority == "Urgent") urgentCount++;

                    dgvTickets.Rows.Add(
                        ticket.Id,
                        ticket.TicketCode,
                        customerName,
                        ticket.Title,
                        ticket.Priority,
                        ticket.Status,
                        ticket.Category ?? "-",
                        assignedTo,
                        ticket.CreatedDate.ToString("dd/MM/yyyy")
                    );
                }

                lblStats.Text = urgentCount > 0 ? $"⚠ {urgentCount} acil talep!" : "";

                foreach (DataGridViewRow row in dgvTickets.Rows)
                {
                    string priority = row.Cells["Priority"].Value?.ToString();
                    string status = row.Cells["Status"].Value?.ToString();

                    if (priority == "Urgent") row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 235);
                    else if (status == "Resolved" || status == "Closed") row.DefaultCellStyle.ForeColor = Color.Gray;
                    else if (priority == "High") row.DefaultCellStyle.ForeColor = Color.FromArgb(192, 57, 43);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Destek talepleri yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new TicketForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                    LoadTickets();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvTickets.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen düzenlemek istediğiniz talebi seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int ticketId = (int)dgvTickets.SelectedRows[0].Cells["Id"].Value;
            var ticket = _ticketService.GetTicketById(ticketId);
            if (ticket != null)
            {
                using (var form = new TicketForm(ticket))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                        LoadTickets();
                }
            }
        }

        private void BtnResolve_Click(object sender, EventArgs e)
        {
            if (dgvTickets.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen çözmek istediğiniz talebi seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int ticketId = (int)dgvTickets.SelectedRows[0].Cells["Id"].Value;
            try
            {
                _ticketService.ResolveTicket(ticketId);
                Program.LoggingService.LogDataUpdate(Program.CurrentUser?.Id, "Tickets", ticketId, "", "Resolved");
                LoadTickets();
                MessageBox.Show("Destek talebi çözüldü.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvTickets.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silmek istediğiniz talebi seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Seçili destek talebini silmek istediğinizden emin misiniz?", "Onay",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    int ticketId = (int)dgvTickets.SelectedRows[0].Cells["Id"].Value;
                    _ticketService.DeleteTicket(ticketId);
                    Program.LoggingService.LogDataDelete(Program.CurrentUser?.Id, "Tickets", ticketId, ticketId.ToString());
                    LoadTickets();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Silme sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
