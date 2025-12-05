using ChochoNest.Controller;
using ChochoNest.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ChochoNest.View
{
    public partial class AdminDashboard : Form
    {
        public User LoggedInUser { get; private set; }
        private readonly ProdukContext _produkContext = new ProdukContext();
        private int _leftMenuWidth = 320;

        public AdminDashboard(User user)
        {
            InitializeComponent();
            LoggedInUser = user;

            this.Resize += AdminDashboard_Resize;
        }

        private void AdminDashboard_Resize(object? sender, EventArgs e)
        {
            PositionDashboardContainer();
        }

        private void btn_RiwayatTransaksi_Click(object sender, EventArgs e)
        {
            RiwayatTransaksiForm riwayatTrxForm = new RiwayatTransaksiForm(LoggedInUser);
            riwayatTrxForm.Show();
            this.Hide();
        }

        private void btn_Dashboard_Click(object sender, EventArgs e)
        {
            AdminDashboard AD = new AdminDashboard(LoggedInUser);
            AD.Show();
            this.Hide();
        }

        private void btn_KelolaKatalog_Click(object sender, EventArgs e)
        {
            KelolaKatalog FK = new KelolaKatalog();
            FK.Show();
            this.Hide();
        }

        private void btn_Logout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Yakin ingin logout?", "Konfirmasi Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
                this.Close();
            }
        }

        private void AdminDashboard_Load(object sender, EventArgs e)
        {
            PopulateDashboard();
        }

        private FlowLayoutPanel GetOrCreateDashboardContainer()
        {
            var existing = this.Controls.Find("flowLayoutPanel1", true).FirstOrDefault() as FlowLayoutPanel
                           ?? this.Controls.OfType<FlowLayoutPanel>().FirstOrDefault();

            if (existing != null)
            {
                existing.Dock = DockStyle.None;
                existing.AutoScroll = true;
                existing.FlowDirection = FlowDirection.TopDown;
                existing.WrapContents = false;
                existing.Padding = new Padding(10);
                existing.BorderStyle = BorderStyle.None;
            }
            else
            {
                existing = new FlowLayoutPanel
                {
                    Name = "flowLayoutPanel1",
                    AutoScroll = true,
                    FlowDirection = FlowDirection.TopDown,
                    WrapContents = false,
                    Padding = new Padding(10),
                    BorderStyle = BorderStyle.None
                };
                this.Controls.Add(existing);
            }

            return existing;
        }

        private void PositionDashboardContainer()
        {
            var container = GetOrCreateDashboardContainer();
            int left = _leftMenuWidth + 16; 
            int top = 20;
            int rightMargin = 20;
            int bottomMargin = 20;

            int width = Math.Max(300, this.ClientSize.Width - left - rightMargin);
            int height = Math.Max(200, this.ClientSize.Height - top - bottomMargin);

            container.Location = new Point(left, top);
            container.Size = new Size(width, height);
            container.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }

        private void PopulateDashboard()
        {
            var container = GetOrCreateDashboardContainer();
            PositionDashboardContainer();

            container.SuspendLayout();
            container.Controls.Clear();

            List<TransaksiModel> allTx;
            try
            {
                allTx = _produkContext.GetRiwayatTransaksi() ?? new List<TransaksiModel>();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mengambil data transaksi: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                container.ResumeLayout();
                return;
            }

            var pnlSummary = CreatePanel(container.Width - 20, 100, Color.FromArgb(255, 250, 245));
            var lblTitle = CreateLabel("Dashboard Summary", 14F, FontStyle.Bold, new Point(12, 12));
            var lblTotalTx = CreateLabel($"Total Transactions: {allTx.Count:N0}", 11F, FontStyle.Regular, new Point(12, 40));
            var totalRevenue = allTx.Sum(t => t.TotalBayar);
            var lblRevenue = CreateLabel($"Total Revenue: Rp {totalRevenue:N0}", 11F, FontStyle.Regular, new Point(250, 40));
            pnlSummary.Controls.Add(lblTitle);
            pnlSummary.Controls.Add(lblTotalTx);
            pnlSummary.Controls.Add(lblRevenue);
            container.Controls.Add(pnlSummary);

            // --- Top products ---
            var productSums = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var trx in allTx)
            {
                if (trx.ListDetail == null) continue;
                foreach (var det in trx.ListDetail)
                {
                    var name = string.IsNullOrWhiteSpace(det.NamaProduk) ? $"Product-{det.IdProduk}" : det.NamaProduk!;
                    productSums.TryGetValue(name, out var cur);
                    productSums[name] = cur + det.JumlahBeli;
                }
            }

            var topProducts = productSums.OrderByDescending(kv => kv.Value).Take(8).ToList();
            var maxQty = topProducts.Any() ? topProducts.Max(kv => kv.Value) : 1;

            var pnlTop = CreatePanel(container.Width - 20, 260, Color.White);
            pnlTop.Controls.Add(CreateLabel("Top Sold Products", 12F, FontStyle.Bold, new Point(12, 12)));
            int y = 40;
            foreach (var kv in topProducts)
            {
                var row = new Panel { Location = new Point(12, y), Size = new Size(pnlTop.Width - 40, 28), BackColor = Color.Transparent };
                var lblName = CreateLabel(kv.Key, 9.5F, FontStyle.Regular, new Point(0, 4));
                lblName.Size = new Size((int)(row.Width * 0.55), 20);
                var lblQty = CreateLabel(kv.Value.ToString(), 9.5F, FontStyle.Bold, new Point((int)(row.Width * 0.58), 4));
                lblQty.AutoSize = true;

                var barBg = new Panel { Location = new Point((int)(row.Width * 0.65), 8), Size = new Size((int)(row.Width * 0.32), 12), BackColor = Color.FromArgb(240, 240, 240) };
                int barWidth = (int)((kv.Value / (double)maxQty) * barBg.Width);
                var barFill = new Panel { Location = new Point(0, 0), Size = new Size(Math.Max(4, barWidth), 12), BackColor = Color.SaddleBrown };
                barBg.Controls.Add(barFill);

                row.Controls.Add(lblName);
                row.Controls.Add(lblQty);
                row.Controls.Add(barBg);
                pnlTop.Controls.Add(row);
                y += 34;
            }
            container.Controls.Add(pnlTop);

            // --- Recent orders ---
            var pnlRecent = CreatePanel(container.Width - 20, 300, Color.White);
            pnlRecent.Controls.Add(CreateLabel("Recent Orders (latest 10)", 12F, FontStyle.Bold, new Point(12, 12)));
            var list = new ListView
            {
                Location = new Point(12, 40),
                Size = new Size(pnlRecent.Width - 40, 240),
                View = System.Windows.Forms.View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            list.Columns.Add("Date", 140);
            list.Columns.Add("Invoice / ID", 100);
            list.Columns.Add("User", 150);
            list.Columns.Add("Items", list.Width - 560);
            list.Columns.Add("Total (Rp)", 120, System.Windows.Forms.HorizontalAlignment.Right);

            var recent = allTx.OrderByDescending(t => t.TanggalTransaksi).Take(10).ToList();
            foreach (var trx in recent)
            {
                string items = trx.ListDetail != null && trx.ListDetail.Any()
                    ? string.Join(", ", trx.ListDetail.Select(d => $"{(d.NamaProduk ?? "Produk Terhapus")}({d.JumlahBeli}x)"))
                    : "(no items)";
                var lvItem = new ListViewItem(trx.TanggalTransaksi.ToString("g"));
                lvItem.SubItems.Add(trx.IdTransaksi.ToString());
                lvItem.SubItems.Add(trx.Username ?? trx.IdUser.ToString());
                lvItem.SubItems.Add(items);
                lvItem.SubItems.Add(trx.TotalBayar.ToString("N0"));
                list.Items.Add(lvItem);
            }
            pnlRecent.Controls.Add(list);
            container.Controls.Add(pnlRecent);

            // --- Last 7 days stats ---
            var pnlStats = CreatePanel(container.Width - 20, 140, Color.FromArgb(255, 250, 245));
            pnlStats.Controls.Add(CreateLabel("Orders (last 7 days)", 12F, FontStyle.Bold, new Point(12, 12)));
            var last7 = Enumerable.Range(0, 7).Select(i => DateTime.Today.AddDays(-i)).OrderBy(d => d).ToList();
            int sx = 12;
            foreach (var day in last7)
            {
                int count = allTx.Count(t => t.TanggalTransaksi.Date == day.Date);
                pnlStats.Controls.Add(CreateLabel(day.ToString("dd MMM"), 9F, FontStyle.Regular, new Point(sx, 40)));
                pnlStats.Controls.Add(CreateLabel(count.ToString(), 11F, FontStyle.Bold, new Point(sx, 62)));
                sx += 110;
            }
            container.Controls.Add(pnlStats);

            container.ResumeLayout();
        }

        private Panel CreatePanel(int width, int height, Color back)
        {
            return new Panel
            {
                Size = new Size(width, height),
                BackColor = back,
                Margin = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private Label CreateLabel(string text, float fontSize, FontStyle style, Point location)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", fontSize, style),
                Location = location,
                AutoSize = false,
                Size = new Size(760, (int)(fontSize * 2 + 6)),
                ForeColor = Color.FromArgb(60, 60, 60)
            };
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}