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

namespace STPLapp
{
    public partial class FormMenu : Form
    {
        string connectionString = "Server = localhost; database = SI_STPL_DB; UID = root; " +
            "Password = 21914113";

        public FormMenu()
        {
            InitializeComponent();
        }

        private void FormMenu_Load(object sender, EventArgs e)
        {
            HitungTotalData();
        }

        private void HitungTotalData()
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();

                string queryHilang = "SELECT COUNT(*) FROM tb_laporan_hilang";
                MySqlCommand cmdHilang = new MySqlCommand(queryHilang, conn);

                int totalHilang = Convert.ToInt32(cmdHilang.ExecuteScalar());
                lblHilang.Text = "Total Laporan Hilang: " + totalHilang.ToString();

                string queryTemuan = "SELECT COUNT(*) FROM tb_barang_temuan";
                MySqlCommand cmdTemuan = new MySqlCommand(queryTemuan, conn);

                int totalTemuan = Convert.ToInt32(cmdTemuan.ExecuteScalar());
                lblTemu.Text = "Total Barang Temuan: " + totalTemuan.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat statistik: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void btnInput_Click(object sender, EventArgs e)
        {
            FormInput formInput = new FormInput();
            formInput.Show();
            this.Hide();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            FormSearch FormSearch = new FormSearch();
            FormSearch.Show();
            this.Hide();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Apakah Anda yakin ingin logout?", "Konfirmasi Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialog == DialogResult.Yes)
            {
                FormLogin formLogin = new FormLogin();
                formLogin.Show();
                this.Hide();
            }
        }
    }
}
