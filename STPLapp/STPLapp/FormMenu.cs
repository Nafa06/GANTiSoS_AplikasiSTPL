using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace STPLapp
{
    public partial class FormMenu : Form
    {
        string connectionString => DatabaseHelper.ConnectionString;

        private string nrpPetugas;

        public FormMenu(string nrp)
        {
            InitializeComponent();
            this.nrpPetugas = nrp;
        }

        public FormMenu()
        {
            InitializeComponent();
        }

        private void FormMenu_Load(object sender, EventArgs e)
        {
            HitungTotalData();
            if (!string.IsNullOrEmpty(nrpPetugas))
            {
                simpanLogSesi(nrpPetugas, "LOGIN");
            }
        }

        public void simpanLogSesi(string nrp, string aktivitas)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("sp_LogSesiPetugas", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_nrp", nrp);
                    cmd.Parameters.AddWithValue("@p_aktivitas", aktivitas);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal mencatat log sesi: " + ex.Message);
                }
            }
        }

        private void HitungTotalData()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    int totalHilang = Convert.ToInt32(new MySqlCommand("SELECT COUNT(*) FROM tb_laporan_hilang", conn).ExecuteScalar());
                    lblHilang.Text = "Total Laporan Hilang: " + totalHilang.ToString();
                    int totalTemuan = Convert.ToInt32(new MySqlCommand("SELECT COUNT(*) FROM tb_barang_temuan", conn).ExecuteScalar());
                    lblTemu.Text = "Total Barang Temuan: " + totalTemuan.ToString();
                    RenderChart(totalHilang, totalTemuan);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal memuat statistik: " + ex.Message);
                }
            }
        }

        private void RenderChart(int totalHilang, int totalTemu)
        {
            Control oldChart = this.Controls["chartStats"];
            if (oldChart != null)
            {
                this.Controls.Remove(oldChart);
                oldChart.Dispose();
            }
            this.Size = new Size(770, 340);
            Chart chartStats = new Chart();
            chartStats.Name = "chartStats";
            chartStats.Location = new Point(360, 20);
            chartStats.Size = new Size(380, 250);
            ChartArea chartArea = new ChartArea("MainArea");
            chartStats.ChartAreas.Add(chartArea);
            Legend legend = new Legend("MainLegend");
            legend.Docking = Docking.Bottom;
            chartStats.Legends.Add(legend);
            Series series = new Series("Statistik")
            {
                ChartArea = "MainArea",
                ChartType = SeriesChartType.Pie,
                Legend = "MainLegend",
                IsValueShownAsLabel = true
            };
            series.Points.AddXY($"Laporan Hilang ({totalHilang})", totalHilang);
            series.Points.AddXY($"Barang Temuan ({totalTemu})", totalTemu);
            series.Points[0].Color = Color.FromArgb(239, 83, 80);
            series.Points[1].Color = Color.FromArgb(102, 187, 106);
            chartStats.Series.Add(series);
            Title title = new Title("Grafik Perbandingan Data STPL", Docking.Top, new Font("Segoe UI", 11, FontStyle.Bold), Color.FromArgb(33, 33, 33));
            chartStats.Titles.Add(title);
            this.Controls.Add(chartStats);
        }

        private void btnInput_Click(object sender, EventArgs e)
        {
            FormInput formInput = new FormInput(nrpPetugas); 
            formInput.Show();
            this.Hide();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            FormSearch FormSearch = new FormSearch(nrpPetugas);
            FormSearch.Show();
            this.Hide();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Apakah Anda yakin ingin logout?", "Konfirmasi Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialog == DialogResult.Yes)
            {
                simpanLogSesi(nrpPetugas, "LOGOUT");
                FormLogin formLogin = new FormLogin();
                formLogin.Show();
                this.Hide();
            }
        }
    }
}
