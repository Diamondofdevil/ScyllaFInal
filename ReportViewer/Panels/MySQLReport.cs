using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DBManagement;

namespace ReportViewer.Panels
{
    public partial class MySQLReport : IReportPanel//UserControl//
    {
        private Session session;
        public enum MySQLMessageType
        {
            //Defaults
            NoMessage = -1,
            USER_PASSWORD_FOUND = 0,
            ERROR_MESSAGE = 1,
            //MySQL
            Table = 2,
            UserInfo = 3,
            Version = 4,
            Shell = 50,
        }
        public MySQLReport()
        {
            session = Session.getInstance();
            InitializeComponent();
        }
        int id = -1;
        int actualModule = -1;
        string host = string.Empty;
        internal override void setData(int id, int module, string host__)
        {
            this.id = id;
            this.actualModule = module;
            this.host = host__;
            string query = "SELECT username FROM USER_PASS WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host__ +"'";
            List<string> strs = session.getStrings(query);
            foreach (string s in strs)
            {
                listBox1.Items.Add(s);
            }
            //found aditional users? :/ gotta find a better way to manage this
            query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND Type =" + (int)MySQLMessageType.UserInfo;
            List<Messages> mes = session.getMessages(query);
            foreach (Messages message in mes)
            {
                if (!listBox1.Items.Contains(message.User))
                    listBox1.Items.Add(message.User);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
                return;

            textBox1.Text = listBox1.SelectedItem.ToString();
            string query = "SELECT pass FROM USER_PASS WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username = '" + listBox1.SelectedItem.ToString() + "'";
            textBox3.Text = session.getString(query);

            textBox2.Text = "";
            query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username = '" + listBox1.SelectedItem.ToString() + "'";
            List<Messages> mes = session.getMessages(query);
            foreach (Messages message in mes)
            {
                if (message.Type == (int)MySQLMessageType.Table){
                    textBox2.Text += "\n"+message.Message + "\n\n";
                    continue;
                }
                if (message.Type == (int)MySQLMessageType.UserInfo)
                {
                    //'Host','Password', 'Select_priv', 'Insert_priv', 'Update_priv', 'Delete_priv', 'Create_priv', 'Drop_priv', 'Process_priv', 'File_priv', 'Grant_priv', 'References_priv', 'Index_priv', 'Alter_priv', 'Show_db_priv', 'Create_tmp_table_priv', 'Show_view_priv', 'Create_user_priv'
                    string[] strings = message.Message.Split(new char[] { ':' });
                    textBox6.Text = strings[0];
                    textBox5.Text = strings[1];
                    //maybe true, "yes", "y", bla bla... depends on the version
                    checkBox1.Checked = strings[2].ToLower().StartsWith("y");
                    checkBox2.Checked = strings[3].ToLower().StartsWith("y");
                    checkBox3.Checked = strings[4].ToLower().StartsWith("y");
                    checkBox4.Checked = strings[5].ToLower().StartsWith("y");
                    checkBox5.Checked = strings[6].ToLower().StartsWith("y");
                    checkBox6.Checked = strings[7].ToLower().StartsWith("y");
                    checkBox7.Checked = strings[8].ToLower().StartsWith("y");
                    checkBox8.Checked = strings[9].ToLower().StartsWith("y");
                    checkBox9.Checked = strings[10].ToLower().StartsWith("y");
                    checkBox10.Checked = strings[11].ToLower().StartsWith("y");
                    checkBox11.Checked = strings[12].ToLower().StartsWith("y");
                    checkBox12.Checked = strings[13].ToLower().StartsWith("y");
                    checkBox13.Checked = strings[14].ToLower().StartsWith("y");
                    checkBox14.Checked = strings[15].ToLower().StartsWith("y");
                    checkBox15.Checked = strings[16].ToLower().StartsWith("y");
                    checkBox16.Checked = strings[17].ToLower().StartsWith("y");
                }
                if (message.Type == (int)MySQLMessageType.Version)
                {
                    textBox4.Text = message.Message;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ListBoxForm lbf = new ListBoxForm("Command Shell Execution Log", "");
            lbf.addItemTruncated(session.getMessages("SELECT * FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND type = " + (int)MySQLMessageType.Shell).ToArray());
            lbf.ShowDialog();
        }
    }
}
