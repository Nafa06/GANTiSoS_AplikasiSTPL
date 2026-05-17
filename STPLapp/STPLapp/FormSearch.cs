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
        string connectionString = "Server = localhost; database = SI_STPL_DB; UID = root; Password = 21914113";
        string idTerpilih = "";
        private string nrpPetugas; 

        private BindingSource bindingSourceGudang = new BindingSource();

        public FormSearch(string nrp)
        {
            InitializeComponent();
            this.nrpPetugas = nrp;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        public FormSearch()
        {
            InitializeComponent();
        }

        private void FormSearch_Load(object sender, EventArgs e)
        {
            if (cmbKategori.Items.Count > 0 && cmbKategori.SelectedIndex == -1)
            {
                cmbKategori.SelectedIndex = 0;
            }
            TampilData();
        }

        private void TampilData(string keyword = "")
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                string query = "";

                if (cmbKategori.Text == "Laporan Hilang")
                {
                    query = "SELECT * FROM vw_laporan_hilang_lengkap WHERE no_stpl LIKE @key OR nama_pelapor LIKE @key OR jenis_barang LIKE @key";
                }
                else
                {
                    query = "SELECT * FROM vw_barang_temuan_lengkap WHERE id_temuan LIKE @key OR nama_penemu LIKE @key OR jenis_barang LIKE @key";
                }

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@key", "%" + keyword + "%");

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                bindingSourceGudang.DataSource = dt;
                dgvGudang.DataSource = bindingSourceGudang;
                bindingNavigator1.BindingSource = bindingSourceGudang;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data dari VIEW: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        private void cmbKategori_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbStatus.Items.Clear();

            if (cmbKategori.Text == "Laporan Hilang")
            {
                cmbStatus.Items.Add("Dicari");
                cmbStatus.Items.Add("Tersimpan");
            }
            else
            {
                cmbStatus.Items.Add("Tersimpan");
                cmbStatus.Items.Add("Dikembalikan");
            }

            BersihkanForm();
            TampilData(); 
        }

        private void dgvGudang_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvGudang.Rows[e.RowIndex];
                idTerpilih = row.Cells[0].Value.ToString(); 

                if (cmbKategori.Text == "Laporan Hilang")
                {
                    lblId.Text = row.Cells["no_stpl"].Value.ToString();
                    lblNik.Text = row.Cells["nik_pelapor"].Value.ToString();
                    lblNama.Text = row.Cells["nama_pelapor"].Value.ToString();
                    lblTanggal.Text = row.Cells["waktu_kejadian"].Value.ToString();
                    lblNrp.Text = row.Cells["nrp_petugas"].Value.ToString();

                    txtJenis.Text = row.Cells["jenis_barang"].Value.ToString();
                    txtCiri.Text = row.Cells["ciri_khusus"].Value.ToString();
                    txtLokasi.Text = row.Cells["tkp"].Value.ToString();
                    cmbStatus.Text = row.Cells["status_pencarian"].Value.ToString();
                }
                else
                {
                    lblId.Text = row.Cells["id_temuan"].Value.ToString();
                    lblNik.Text = row.Cells["nik_penemu"].Value.ToString();
                    lblNama.Text = row.Cells["nama_penemu"].Value.ToString();
                    lblTanggal.Text = row.Cells["waktu_ditemukan"].Value.ToString();
                    lblNrp.Text = row.Cells["nrp_petugas"].Value.ToString();

                    txtJenis.Text = row.Cells["jenis_barang"].Value.ToString();
                    txtCiri.Text = row.Cells["ciri_ciri"].Value.ToString();
                    txtLokasi.Text = row.Cells["lokasi_ditemukan"].Value.ToString();
                    cmbStatus.Text = row.Cells["status_gudang"].Value.ToString();
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Apakah Anda yakin ingin mengubah detail data ini?", "Konfirmasi Ubah", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialog == DialogResult.Yes)
            {
                MySqlConnection conn = new MySqlConnection(connectionString);
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (cmbKategori.Text == "Laporan Hilang")
                    {
                        cmd.CommandText = "SP_UpdateStatusLaporan";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("p_no_stpl", idTerpilih);
                        cmd.Parameters.AddWithValue("p_jenis_barang", txtJenis.Text);
                        cmd.Parameters.AddWithValue("p_ciri_khusus", txtCiri.Text);
                        cmd.Parameters.AddWithValue("p_tkp", txtLokasi.Text);
                        cmd.Parameters.AddWithValue("p_status", cmbStatus.Text);
                    }
                    else
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "UPDATE tb_barang_temuan SET jenis_barang=@brg, ciri_ciri=@ciri, lokasi_ditemukan=@loc, status_gudang=@stat WHERE id_temuan=@id";
                        cmd.Parameters.AddWithValue("@brg", txtJenis.Text);
                        cmd.Parameters.AddWithValue("@ciri", txtCiri.Text);
                        cmd.Parameters.AddWithValue("@loc", txtLokasi.Text);
                        cmd.Parameters.AddWithValue("@stat", cmbStatus.Text);
                        cmd.Parameters.AddWithValue("@id", idTerpilih);
                    }

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Data berhasil diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    BersihkanForm();
                    TampilData();
                }
                catch (Exception ex) { MessageBox.Show("Gagal update via SP: " + ex.Message); }
                finally { conn.Close(); }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Yakin mau hapus data " + idTerpilih + "?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dialog == DialogResult.Yes)
            {
                MySqlConnection conn = new MySqlConnection(connectionString);
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;

                    if (cmbKategori.Text == "Laporan Hilang")
                    {
                        cmd.CommandText = "SP_DeleteLaporanHilang";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@p_no_stpl", idTerpilih);
                    }
                    else
                    {
                        cmd.CommandText = "DELETE FROM tb_barang_temuan WHERE id_temuan=@id";
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@id", idTerpilih);
                    }

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Data berhasil dihapus dari sistem!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    BersihkanForm();
                    TampilData();
                }
                catch (Exception ex) { MessageBox.Show("Gagal hapus: " + ex.Message); }
                finally { conn.Close(); }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e) { TampilData(txtSearch.Text); }

        private void btnRefresh_Click(object sender, EventArgs e) { TampilData(txtSearch.Text); }

        private void BersihkanForm()
        {
            lblId.Text = "-";
            lblNik.Text = "-";
            lblNama.Text = "-";
            txtJenis.Clear();
            txtCiri.Clear();
            txtLokasi.Clear();
            cmbStatus.SelectedIndex = -1;
            idTerpilih = "";
            txtSearch.Clear();
            lblTanggal.Text = "-";
            lblNrp.Text = "-";
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            FormMenu menu = new FormMenu(nrpPetugas);
            menu.Show();
            this.Hide();
        }

        private void FormSearch_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            BersihkanForm();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
          
        }
    }
}