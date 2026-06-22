using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace STPLapp
{
    public partial class FormDbSettings : Form
    {
        public FormDbSettings()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void FormDbSettings_Load(object sender, EventArgs e)
        {
            // Load nilai default dari db_config.txt (jika ada) ke textboxes
            try
            {
                string rawStr = DatabaseHelper.ConnectionString;
                var builder = new MySqlConnectionStringBuilder(rawStr);
                txtHost.Text = builder.Server;
                txtDatabase.Text = builder.Database;
                txtUsername.Text = builder.UserID;
                txtPassword.Text = builder.Password;
            }
            catch { }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            string tempConn = $"Server={txtHost.Text};database={txtDatabase.Text};UID={txtUsername.Text};Password={txtPassword.Text}";
            using (MySqlConnection conn = new MySqlConnection(tempConn))
            {
                try
                {
                    conn.Open();
                    MessageBox.Show("Koneksi berhasil terhubung!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Koneksi Gagal!\nDetail: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHost.Text) || string.IsNullOrWhiteSpace(txtDatabase.Text) || string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Kolom Host, Database, dan Username tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DatabaseHelper.SaveConnectionString(txtHost.Text, txtDatabase.Text, txtUsername.Text, txtPassword.Text);
                MessageBox.Show("Pengaturan database berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan konfigurasi: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}