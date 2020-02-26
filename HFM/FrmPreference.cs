using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace HFM.Components
{
    public partial class FrmPreference : Form
    {
        public FrmPreference()
        {
            InitializeComponent();
            OleDbConnection oleDbConnection = new OleDbConnection(DbHelperAccess.connectionString);
            string SQL = "SELECT * FROM HFM_Preference";
            OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(SQL, oleDbConnection);
            System.Data.DataSet thisDataSet = new System.Data.DataSet();
            oleDbDataAdapter.Fill(thisDataSet, "HFM_Preference");
            DataTable dt = thisDataSet.Tables["HFM_Preference"];
            this.DgvAlphaSet.DataSource = dt;
            oleDbConnection.Close();

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void groupBox11_Enter(object sender, EventArgs e)
        {

        }
    }
}
