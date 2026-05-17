using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CRMS.Business.Services;
using CRMS.Entity.Models;

namespace CRMS.UI.Forms
{
    public class OffersControl : UserControl
    {
        private readonly OfferService _offerService;
        private readonly CustomerService _customerService;
        private DataGridView dgvOffers;
        private ComboBox cmbStatusFilter;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnApprove;
        private Button btnReject;
        private Button btnDelete;
        private Label lblStats;

        public OffersControl()
        {
            _offerService = new OfferService(Program.UnitOfWork);
            _customerService = new CustomerService(Program.UnitOfWork);
            InitializeCustomComponents();
            LoadOffers();
        }

        private void InitializeCustomComponents()
        {
            this.BackColor = Color.FromArgb(245, 245, 245);

            Label lblTitle = new Label
            {
                Text = "TEKLİFLER",
                Location = new Point(20, 20),
                Size = new Size(300, 30),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 45, 48)
            };
            this.Controls.Add(lblTitle);

            this.Controls.Add(new Label { Text = "Durum:", Location = new Point(20, 65), Size = new Size(60, 25), Font = new Font("Arial", 10), TextAlign = ContentAlignment.MiddleLeft });

            cmbStatusFilter = new ComboBox { Location = new Point(85, 65), Size = new Size(140, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 10) };
            cmbStatusFilter.Items.AddRange(new object[] { "Tümü", "Pending", "Approved", "Rejected", "Expired" });
            cmbStatusFilter.SelectedIndex = 0;
            cmbStatusFilter.SelectedIndexChanged += (s, e) => LoadOffers();
            this.Controls.Add(cmbStatusFilter);

            btnAdd = CreateButton("Yeni Ekle", 240, 65, Color.FromArgb(46, 204, 113));
            btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(btnAdd);

            btnEdit = CreateButton("Düzenle", 350, 65, Color.FromArgb(241, 196, 15));
            btnEdit.Click += BtnEdit_Click;
            this.Controls.Add(btnEdit);

            btnApprove = CreateButton("Onayla", 460, 65, Color.FromArgb(26, 188, 156));
            btnApprove.Click += BtnApprove_Click;
            this.Controls.Add(btnApprove);

            btnReject = CreateButton("Reddet", 570, 65, Color.FromArgb(230, 126, 34));
            btnReject.Click += BtnReject_Click;
            this.Controls.Add(btnReject);

            btnDelete = CreateButton("Sil", 680, 65, Color.FromArgb(231, 76, 60));
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            lblStats = new Label { Text = "", Location = new Point(790, 68), Size = new Size(250, 25), Font = new Font("Arial", 9, FontStyle.Bold), ForeColor = Color.FromArgb(46, 204, 113) };
            this.Controls.Add(lblStats);

            dgvOffers = new DataGridView
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
            dgvOffers.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 9, FontStyle.Bold);
            dgvOffers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            dgvOffers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvOffers.EnableHeadersVisualStyles = false;

            dgvOffers.Columns.Add("Id", "ID");
            dgvOffers.Columns.Add("OfferCode", "Teklif Kodu");
            dgvOffers.Columns.Add("CustomerName", "Müşteri");
            dgvOffers.Columns.Add("Title", "Başlık");
            dgvOffers.Columns.Add("FinalAmount", "Tutar");
            dgvOffers.Columns.Add("Status", "Durum");
            dgvOffers.Columns.Add("OfferDate", "Teklif Tarihi");
            dgvOffers.Columns.Add("ExpiryDate", "Son Geçerlilik");

            dgvOffers.Columns["Id"].Visible = false;
            this.Controls.Add(dgvOffers);
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

        private void LoadOffers()
        {
            try
            {
                dgvOffers.Rows.Clear();
                var offers = _offerService.GetAllOffers();
                var customers = _customerService.GetAllCustomers().ToDictionary(c => c.Id, c => c);

                string filter = cmbStatusFilter.SelectedItem?.ToString();
                if (filter != "Tümü" && !string.IsNullOrEmpty(filter))
                    offers = offers.Where(o => o.Status == filter).ToList();

                decimal totalApproved = 0;
                foreach (var offer in offers)
                {
                    customers.TryGetValue(offer.CustomerId, out var customer);
                    string customerName = customer != null ? $"{customer.FirstName} {customer.LastName}".Trim() : "Bilinmiyor";

                    dgvOffers.Rows.Add(
                        offer.Id,
                        offer.OfferCode,
                        customerName,
                        offer.Title,
                        $"₺{offer.FinalAmount:N2}",
                        offer.Status,
                        offer.OfferDate.ToString("dd/MM/yyyy"),
                        offer.ExpiryDate.HasValue ? offer.ExpiryDate.Value.ToString("dd/MM/yyyy") : "-"
                    );

                    if (offer.Status == "Approved")
                        totalApproved += offer.FinalAmount;
                }

                lblStats.Text = $"Onaylanan: ₺{totalApproved:N2}";

                foreach (DataGridViewRow row in dgvOffers.Rows)
                {
                    string status = row.Cells["Status"].Value?.ToString();
                    if (status == "Approved") row.DefaultCellStyle.ForeColor = Color.FromArgb(39, 174, 96);
                    else if (status == "Rejected") row.DefaultCellStyle.ForeColor = Color.FromArgb(192, 57, 43);
                    else if (status == "Expired") row.DefaultCellStyle.ForeColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Teklifler yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new OfferForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                    LoadOffers();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvOffers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen düzenlemek istediğiniz teklifi seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int offerId = (int)dgvOffers.SelectedRows[0].Cells["Id"].Value;
            var offer = _offerService.GetOfferById(offerId);
            if (offer != null)
            {
                using (var form = new OfferForm(offer))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                        LoadOffers();
                }
            }
        }

        private void BtnApprove_Click(object sender, EventArgs e)
        {
            if (dgvOffers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen onaylamak istediğiniz teklifi seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int offerId = (int)dgvOffers.SelectedRows[0].Cells["Id"].Value;
            try
            {
                _offerService.ApproveOffer(offerId);
                Program.LoggingService.LogDataUpdate(Program.CurrentUser?.Id, "Offers", offerId, "", "Approved");
                LoadOffers();
                MessageBox.Show("Teklif onaylandı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnReject_Click(object sender, EventArgs e)
        {
            if (dgvOffers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen reddetmek istediğiniz teklifi seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int offerId = (int)dgvOffers.SelectedRows[0].Cells["Id"].Value;
            try
            {
                _offerService.RejectOffer(offerId);
                Program.LoggingService.LogDataUpdate(Program.CurrentUser?.Id, "Offers", offerId, "", "Rejected");
                LoadOffers();
                MessageBox.Show("Teklif reddedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvOffers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silmek istediğiniz teklifi seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Seçili teklifi silmek istediğinizden emin misiniz?", "Onay",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    int offerId = (int)dgvOffers.SelectedRows[0].Cells["Id"].Value;
                    _offerService.DeleteOffer(offerId);
                    Program.LoggingService.LogDataDelete(Program.CurrentUser?.Id, "Offers", offerId, offerId.ToString());
                    LoadOffers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Silme sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
