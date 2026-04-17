using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace STPLapp
{
    public partial class FormInput : Form
    {
        string connectionString = "Server = localhost; database = SI_STPL_DB; UID = root; " +
            "Password = 21914113";

        public FormInput()
        {
            InitializeComponent();
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (txtStpl.Text == "" || txtNik.Text == "" || txtNama.Text == "")
            {
                MessageBox.Show("Mohon lengkapi data No STPL, NIK, dan Nama Pelapor!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                string query = "INSERT INTO tb_laporan_hilang (no_stpl, nik_pelapor, nama_pelapor, jenis_barang, waktu_kejadian, ciri_khusus, tkp, nrp_petugas) " +
                               "VALUES (@no, @nik, @nama, @barang, @tgl, @ciri, @tkp, @nrp)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@no", txtStpl.Text);
                cmd.Parameters.AddWithValue("@nik", txtNik.Text);
                cmd.Parameters.AddWithValue("@nama", txtNama.Text);
                cmd.Parameters.AddWithValue("@barang", txtJenis.Text);
                cmd.Parameters.AddWithValue("@tgl", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@ciri", txtCiri.Text);
                cmd.Parameters.AddWithValue("@tkp", txtTkp.Text);
                cmd.Parameters.AddWithValue("@nrp", txtNrp.Text);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Laporan Hilang berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtStpl.Clear(); txtNik.Clear(); txtNama.Clear(); txtJenis.Clear(); txtCiri.Clear(); txtTkp.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan data: " + ex.Message, "Error Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        private void btnSimpanNemu_Click(object sender, EventArgs e)
        {
            if (txtTemuan.Text == "" || txtNikPenemu.Text == "" || txtPenemu.Text == "")
            {
                MessageBox.Show("Mohon lengkapi data No Temuan, NIK, dan Nama Penemu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                // Query Simpan Data Temuan. SESUAIKAN NAMA TABEL DAN KOLOM DENGAN DATABASEMU!
                string query = "INSERT INTO tb_barang_temuan (id_temuan, nik_penemu, nama_penemu, jenis_barang, waktu_kejadian, ciri_ciri, lokasi_temu, nrp_petugas) " +
                               "VALUES (@no, @nik, @nama, @barang, @tgl, @ciri, @lokasi, @nrp)";
                
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@no", txtTemuan.Text);
                cmd.Parameters.AddWithValue("@nik", txtNikPenemu.Text);
                cmd.Parameters.AddWithValue("@nama", txtPenemu.Text);
                cmd.Parameters.AddWithValue("@barang", txtBarang.Text);
                cmd.Parameters.AddWithValue("@tgl", dateTimePicker2.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@ciri", txtCiriciri.Text);
                cmd.Parameters.AddWithValue("@lokasi", txtLokasi.Text);
                cmd.Parameters.AddWithValue("@nrp", txtNrpNemu.Text);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Data Temuan berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Kosongkan form setelah simpan
                txtTemuan.Clear(); txtNikPenemu.Clear(); txtPenemu.Clear(); txtBarang.Clear(); txtCiriciri.Clear(); txtLokasi.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan data: " + ex.Message, "Error Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
