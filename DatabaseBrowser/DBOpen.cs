using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Npgsql;

namespace DatabaseBrowser
{
    public partial class DBOpen : Form
    {
        private static int[] ports = { 1521, 1433, 50000, 3306, 5432,10 };
        DbConnection connection;
        int type;

        public int Type
        {
            get { return type; }
            set { type = value; }
        }

        public DbConnection Connection
        {
            get { return connection; }
            set { connection = value; }
        }
        public DBOpen()
        {
            InitializeComponent();
            connection = null;
            this.comboBox1.Items.AddRange(Enum.GetNames(typeof(DBManager.DB_TYPE)));
            this.comboBox1.SelectedIndex = 0;
            this.comboBoxConnectAs.SelectedIndex = 0;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            groupBox1.Enabled = !checkBox1.Checked;
            textBox1.Enabled = checkBox1.Checked;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0)
                return;
            numericUpDownPort.Value = (decimal)ports[comboBox1.SelectedIndex];
            label7.Visible = comboBoxConnectAs.Visible = (comboBox1.SelectedIndex == (int)DatabaseBrowser.DBManager.DB_TYPE.ORACLE);
            if (comboBox1.SelectedIndex == (int)DatabaseBrowser.DBManager.DB_TYPE.SQLServerCE35)
            {
                label2.Text = "DB File";
                label3.Visible = numericUpDownPort.Visible = label4.Visible = textBoxDB.Visible = label5.Visible = textBoxUser.Visible = false;
                button3.Visible = true;
            }
            else
            {
                label2.Text = "Host";
                label3.Visible = numericUpDownPort.Visible = label4.Visible = textBoxDB.Visible = label5.Visible = textBoxUser.Visible = true;
                button3.Visible = false;
            }
        }
        //TODO:SQL HISTORY
        private DbConnection getConnection()
        {
            if (comboBox1.SelectedIndex < 0)
                return null;
            string conStr = string.Empty;
            switch (comboBox1.SelectedIndex)
            {
                case (int) DatabaseBrowser.DBManager.DB_TYPE.DB2:
                    if (checkBox1.Checked)
                        conStr = textBox1.Text;
                    else
                        conStr = "Server=" + textBoxHost.Text + ":" + numericUpDownPort.Value.ToString() + ";Database=" + textBoxDB.Text + ";UID=" + textBoxUser.Text + ";PWD=" + textBoxPasswd.Text + ";";
                    return new IBM.Data.DB2.DB2Connection(conStr);
                    
                case (int)DatabaseBrowser.DBManager.DB_TYPE.MSSQL:
                    if (checkBox1.Checked)
                        conStr = textBox1.Text;
                    else
                    {
                        conStr = "server=" + textBoxHost.Text + ", " + numericUpDownPort.Value.ToString() + ";Password=" + textBoxPasswd.Text + ";User ID=" + textBoxUser.Text;
                        if (!string.IsNullOrEmpty(textBoxDB.Text))
                            conStr += ";Initial Catalog=" + textBoxDB.Text;
                    }
                    return new SqlConnection(conStr);
                case (int)DatabaseBrowser.DBManager.DB_TYPE.ORACLE:
                    if (checkBox1.Checked)
                        conStr = textBox1.Text;
                    else
                        conStr = "User Id=" + textBoxUser.Text + "; Password=" + textBoxPasswd.Text + "; DBA Privilege = SYSDBA; Data Source=" + textBoxHost.Text + ":" + numericUpDownPort.Value + "/" + textBoxDB.Text;
                    return new Oracle.DataAccess.Client.OracleConnection(conStr);

                case (int)DatabaseBrowser.DBManager.DB_TYPE.MYSQL:
                    if (checkBox1.Checked)
                        conStr = textBox1.Text;
                    else
                    {
                        conStr = "Data Source=" + textBoxHost.Text + "; User Id=" + textBoxUser.Text + "; Password=" + textBoxPasswd.Text + "; Port =" + numericUpDownPort.Value.ToString();
                        if (!string.IsNullOrEmpty(textBoxDB.Text))
                            conStr += ";DataBase=" + textBoxDB.Text;
                    }
                    return new MySql.Data.MySqlClient.MySqlConnection(conStr);
                case (int)DatabaseBrowser.DBManager.DB_TYPE.POSTGRES:
                    if (checkBox1.Checked)
                        conStr = textBox1.Text;
                    else
                    {
                        conStr = "Host=" + textBoxHost.Text + "; User Id=" + textBoxUser.Text + "; Password=" + textBoxPasswd.Text + "; Port =" + numericUpDownPort.Value.ToString();
                        if (!string.IsNullOrEmpty(textBoxDB.Text))
                            conStr += ";DataBase=" + textBoxDB.Text;
                    }
                    return new NpgsqlConnection(conStr);
                case (int)DatabaseBrowser.DBManager.DB_TYPE.SQLServerCE35:
                    if (checkBox1.Checked)
                        conStr = textBox1.Text;
                    else
                    {
                        conStr = @"DataSource=" + textBoxHost.Text;
                        if (!string.IsNullOrEmpty(textBoxPasswd.Text))
                            conStr += @";Encrypt Database=True;Password=" + textBoxPasswd.Text + ";File Mode=shared read;Persist Security Info=False;";
                    }
                    return new SqlCeConnection(conStr);
            }
            return null;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            connection = getConnection();
            Type = comboBox1.SelectedIndex;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs ev)
        {
            try
            {
                DbConnection cn = getConnection();
                cn.Open();
                cn.Close();
                MessageBox.Show("Connection succesfull", "Succesfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception e)
            {
                MessageBox.Show("Connection unsuccesfull\n\n"+e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != (int)DatabaseBrowser.DBManager.DB_TYPE.SQLServerCE35)
                return;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Title = "Database file";
            ofd.Filter = "Dictionary files(*.sdf)|*.sdf|All files (*.*)|*.*"; 
            if (ScyllaDBBrowser.STAShowDialog(ofd) != System.Windows.Forms.DialogResult.OK)
                return;
            textBoxHost.Text = ofd.FileName;
        }
    }
}
