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
    public partial class FormSearch : Form
    {
        string connectionString = "Server = localhost; database = SI_STPL_DB; UID = root; " +
            "Password = 21914113";
        string idTerpilih = "";

        public FormSearch()
        {
            InitializeComponent();
        }

        private void FormSearch_Load(object sender, EventArgs e)
        {
            cmbKategori.SelectedIndex = 0;
        }
        private void TampilData(string keyword = "")
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                string query = "";

                // Cari di semua kolom (Global Search)
                if (cmbKategori.Text == "Laporan Hilang")
                {
                    query = @"SELECT * FROM tb_laporan_hilang 
                              WHERE no_stpl LIKE @key OR nik_pelapor LIKE @key 
                              OR nama_pelapor LIKE @key OR jenis_barang LIKE @key 
                              OR ciri_khusus LIKE @key OR tkp LIKE @key 
                              OR status_pencarian LIKE @key";
                }
                else
                {
                    query = @"SELECT * FROM tb_barang_temuan 
                              WHERE id_temuan LIKE @key OR nik_penemu LIKE @key 
                              OR nama_penemu LIKE @key OR jenis_barang LIKE @key 
                              OR ciri_ciri LIKE @key OR lokasi_ditemukan LIKE @key 
                              OR status_gudang LIKE @key";
                }

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@key", "%" + keyword + "%");

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgvGudang.DataSource = dt; // Masukkan data ke tabel
            }
            catch (Exception ex) { MessageBox.Show("Error Tampil Data: " + ex.Message); }
            finally { conn.Close(); }
        }

        private void cmbKategori_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbStatus.Items.Clear(); // Kosongkan pilihan status

            if (cmbKategori.Text == "Laporan Hilang")
            {
                // Isi pilihan status untuk Barang Hilang
                cmbStatus.Items.Add("Dicari");
                cmbStatus.Items.Add("Ditemukan");
                cmbStatus.Items.Add("Selesai");

                // Ubah awalan label
                lblId.Text = "No STPL: -";
                lblNik.Text = "NIK Pelapor: -";
                lblNama.Text = "Nama Pelapor: -";
            }
            else
            {
                // Isi pilihan status untuk Barang Temuan
                cmbStatus.Items.Add("Tersimpan");
                cmbStatus.Items.Add("Sudah Diambil");
                cmbStatus.Items.Add("Diserahkan ke Pemilik");

                // Ubah awalan label
                lblId.Text = "No Temuan: -";
                lblNik.Text = "NIK Penemu: -";
                lblNama.Text = "Nama Penemu: -";
            }

            BersihkanForm(); // Kosongkan kotak input
            TampilData();    // Muat ulang tabel
        }
        private void dgvGudang_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvGudang.Rows[e.RowIndex];
                idTerpilih = row.Cells[0].Value.ToString(); // Kunci ID

                // Lempar data ke Label
                if (cmbKategori.Text == "Laporan Hilang")
                {
                    lblId.Text = "No STPL: " + idTerpilih;
                    lblNik.Text = "NIK Pelapor: " + row.Cells[1].Value.ToString();
                    lblNama.Text = "Nama Pelapor: " + row.Cells[2].Value.ToString();
                }
                else
                {
                    lblId.Text = "No Temuan: " + idTerpilih;
                    lblNik.Text = "NIK Penemu: " + row.Cells[1].Value.ToString();
                    lblNama.Text = "Nama Penemu: " + row.Cells[2].Value.ToString();
                }

                // Asumsi urutan kolom: Tanggal = 4, NRP = 7
                lblTanggal.Text = "Tanggal: " + row.Cells[4].Value.ToString();
                lblNrp.Text = "NRP Petugas: " + row.Cells[7].Value.ToString();

                // Lempar data ke TextBox untuk diedit (Asumsi: Barang=3, Ciri=5, Lokasi/TKP=6)
                txtJenis.Text = row.Cells[3].Value.ToString();
                txtCiri.Text = row.Cells[5].Value.ToString();
                txtLokasi.Text = row.Cells[6].Value.ToString();
                cmbStatus.Text = row.Cells[row.Cells.Count - 1].Value.ToString(); // Status
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (idTerpilih == "") { MessageBox.Show("Pilih data di tabel dulu!"); return; }

            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                string query = "";

                if (cmbKategori.Text == "Laporan Hilang")
                {
                    query = "UPDATE tb_laporan_hilang SET jenis_barang=@brg, ciri_khusus=@ciri, tkp=@loc, status_pencarian=@stat WHERE no_stpl=@id";
                }
                else
                {
                    query = "UPDATE tb_barang_temuan SET jenis_barang=@brg, ciri_ciri=@ciri, lokasi_ditemukan=@loc, status_gudang=@stat WHERE id_temuan=@id";
                }

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@brg", txtJenis.Text);
                cmd.Parameters.AddWithValue("@ciri", txtCiri.Text);
                cmd.Parameters.AddWithValue("@loc", txtLokasi.Text);
                cmd.Parameters.AddWithValue("@stat", cmbStatus.Text);
                cmd.Parameters.AddWithValue("@id", idTerpilih);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Data berhasil diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                BersihkanForm();
                TampilData();
            }
            catch (Exception ex) { MessageBox.Show("Gagal update: " + ex.Message); }
            finally { conn.Close(); }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (idTerpilih == "") { MessageBox.Show("Pilih data yang mau dihapus!"); return; }

            DialogResult dialog = MessageBox.Show("Yakin mau hapus data " + idTerpilih + "?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dialog == DialogResult.Yes)
            {
                MySqlConnection conn = new MySqlConnection(connectionString);
                try
                {
                    conn.Open();
                    string query = cmbKategori.Text == "Laporan Hilang" ?
                                   "DELETE FROM tb_laporan_hilang WHERE no_stpl=@id" :
                                   "DELETE FROM tb_barang_temuan WHERE id_temuan=@id";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", idTerpilih);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Data berhasil dihapus!");
                    BersihkanForm();
                    TampilData();
                }
                catch (Exception ex) { MessageBox.Show("Gagal hapus: " + ex.Message); }
                finally { conn.Close(); }
            }
        }
        private void btnSearch_Click(object sender, EventArgs e) { TampilData(txtSearch.Text); }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            BersihkanForm();
            TampilData();
        }

        private void BersihkanForm()
        {
            txtJenis.Clear();
            txtCiri.Clear();
            txtLokasi.Clear();
            cmbStatus.SelectedIndex = -1;
            idTerpilih = "";
            txtSearch.Clear();
            lblTanggal.Text = "Tanggal: -";
            lblNrp.Text = "NRP Petugas: -";
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            FormMenu menu = new FormMenu();
            menu.Show();
            this.Hide();
        }

        private void FormSearch_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMenu menu = new FormMenu();
            menu.Show();
        }
    }
}
