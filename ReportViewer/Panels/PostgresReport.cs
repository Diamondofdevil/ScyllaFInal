using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DBManagement;

namespace ReportViewer.Panels
{
    public enum PostgresMessageType
    {
        //Defaults
        NoMessage = -1,
        USER_PASSWORD_FOUND = 0,
        ERROR_MESSAGE = 1,
        //Oracle
        //username:created_time
        Table = 2,
        UserInfo = 3,
        GroupInfo = 4,
        RoleInfo = 5,
        DBInfo = 6,
        Version = 7,
        ShadowOrSAM = 8,
        SSPI = 9
    }
    public partial class PostgresReport : IReportPanel//UserControl//
    {
        Session session;

        public PostgresReport()
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
            string query = "SELECT username FROM USER_PASS WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host__ + "'";
            List<string> strs = session.getStrings(query);
            foreach (string s in strs)
            {
                listBox1.Items.Add(s);
            }
            richTextBox1.Text = richTextBox2.Text = string.Empty;
            //found aditional users? :/ gotta find a better way to manage this
            query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "'";
            List<Messages> mes = session.getMessages(query);
            foreach (Messages message in mes)
            {
                if ((message.Type == (int)PostgresMessageType.UserInfo || message.Type == (int)PostgresMessageType.RoleInfo) && !listBox1.Items.Contains(message.User))
                {
                    listBox1.Items.Add(message.User);
                    continue;
                }
                message.Message = message.Message.Substring(0, message.Message.Length - 1);
                if (message.Type == (int)PostgresMessageType.Table)
                {
                    richTextBox1.Text += message.Message + "\n";
                    continue;
                }
                if (message.Type == (int)PostgresMessageType.SSPI)
                {
                    checkBox17.Checked = true;
                    continue;
                }
                if (message.Type == (int)PostgresMessageType.DBInfo)
                {
                    //'password_encryption' OR name ='server_version' OR name ='ssl_ciphers'"
                    if (message.User.ToLower().Equals("config_file"))
                    {
                        textBox8.Text = message.Message;
                        continue;
                    }
                    if (message.User.ToLower().Equals("hba_file"))
                    {
                        textBox9.Text = message.Message;
                        continue;
                    }
                    if (message.User.ToLower().Equals("ident_file"))
                    {
                        textBox10.Text = message.Message;
                        continue;
                    }
                    if (message.User.ToLower().Equals("krb_srvname"))
                    {
                        textBox11.Text = message.Message;
                        continue;
                    }
                    if (message.User.ToLower().Equals("password_encryption"))
                    {
                        textBox12.Text = message.Message;
                        continue;
                    }
                    if (message.User.ToLower().Equals("server_version"))
                    {
                        textBox13.Text = message.Message;
                        continue;
                    }
                    if (message.User.ToLower().Equals("ssl_ciphers"))
                    {
                        textBox14.Text = message.Message;
                        continue;
                    }
                }
                if (message.Type == (int)PostgresMessageType.Version)
                {
                    textBox4.Text = message.Message;
                }
                if (message.Type == (int)PostgresMessageType.GroupInfo)
                {
                    richTextBox2.Text += message.Message + "\n";
                    continue;
                }
            }

            //query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "'";
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
                return;
            clearData();
            string query = "SELECT pass FROM USER_PASS WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username = '" + listBox1.SelectedItem.ToString() + "'";
            textBox3.Text = session.getString(query);

            query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username = '" + listBox1.SelectedItem.ToString() + "'";
            List<Messages> mes = session.getMessages(query);
            foreach (Messages message in mes)
            {


                //isSuper, Inherit, createRol, CreateDB, Update, login, sh.usesysid, sh.passwd
                // usesysid, usecreatedb, usesuper, usecatupd
                string[] strings = message.Message.Split(new char[] { ':' });
                if (message.Type == (int)PostgresMessageType.UserInfo)
                {
                    textBox15.Text = strings[0];
                    checkBox1.Checked = strings[1].ToLower().StartsWith("t");
                    checkBox2.Checked = strings[2].ToLower().StartsWith("t");
                    checkBox3.Checked = strings[3].ToLower().StartsWith("t");
                }
                else if (message.Type == (int)PostgresMessageType.RoleInfo)
                {
                    checkBox2.Checked = strings[0].ToLower().StartsWith("t");
                    checkBox4.Checked = strings[1].ToLower().StartsWith("t");
                    checkBox5.Checked = strings[2].ToLower().StartsWith("t");
                    checkBox1.Checked = strings[3].ToLower().StartsWith("t");
                    checkBox3.Checked = strings[4].ToLower().StartsWith("t");
                    checkBox6.Checked = strings[5].ToLower().StartsWith("t");
                    textBox15.Text = strings[6];
                    textBox5.Text = strings[7];
                }
            }
        }

        private void clearData()
        {
            checkBox1.Checked = checkBox2.Checked = checkBox3.Checked = checkBox4.Checked = checkBox5.Checked = checkBox6.Checked = checkBox17.Checked = false;
            textBox15.Text = textBox3.Text = textBox5.Text = string.Empty;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND type = "+(int)PostgresMessageType.ShadowOrSAM;
            string mes = session.getString(query);

            ListBoxForm form = new ListBoxForm("SAM or Shadow File", "");
            form.addItem(mes);
            form.ShowDialog();
        }
    }
}
