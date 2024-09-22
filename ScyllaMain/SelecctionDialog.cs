using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Scylla
{
    public partial class SelecctionDialog : Form
    {
        private string dbName;

        public string DbName
        {
            get { return dbName; }
            set { dbName = value; }
        }
        public SelecctionDialog()
        {
            InitializeComponent();
        }
        public SelecctionDialog(List<DBInstance> dbs)
        {
            InitializeComponent();
            for(int i = 0; i<dbs.Count; i++)
            {
                foreach (DBInstance dbi in dbs)
                {
                    
                    comboBox1.Items.Add(dbi.instName);
                    foreach(DataBaseInfo di in dbi.dbs)
                    {
                        comboBox2.Items.Add(di.dbName + "("+di.dbAlias+")");
                    }
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(comboBox2.Text))
                return;
            DbName = comboBox2.Text.Split(new char[]{'('}, StringSplitOptions.RemoveEmptyEntries)[0];
            if (string.IsNullOrWhiteSpace(DbName))
            {
                DbName = comboBox2.SelectedText;
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
