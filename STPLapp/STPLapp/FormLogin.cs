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
    public partial class FormLogin : Form
    {
        string connectionString = "Server = localhost; database = SI_STPL_DB; UID = root; " +
            "Password = 21914113";

        public FormLogin()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                MessageBox.Show("Terhubung ke database", "Status Koneksi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Koneksi ke database gagal.\nDetail: " + ex.Message, "Status Koneksi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtNrp.Text == "" || txtPass.Text == "")
            {
                MessageBox.Show("NRP dan Password tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                string query = "SELECT * FROM tb_petugas WHERE nrp = @nrp AND password_petugas = @password";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@nrp", txtNrp.Text);
                cmd.Parameters.AddWithValue("@password", txtPass.Text);

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string namaPetugas = reader["nama_petugas"].ToString();
                    MessageBox.Show("Login Berhasil! Selamat bertugas, " + namaPetugas, "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    FormMenu menuUtama = new FormMenu();
                    menuUtama.Show();

                    this.Hide();
                }
                else
                {
                    MessageBox.Show("NRP atau Password salah!", "Akses Ditolak", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Database: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
