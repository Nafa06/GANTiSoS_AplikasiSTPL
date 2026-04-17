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
    }
}
