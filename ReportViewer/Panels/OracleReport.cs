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
    public partial class OracleReport : IReportPanel//UserControl//
    {
        private Session session;
        public enum OracleMessageType
        {
            //Defaults
            NoMessage = -1,
            USER_PASSWORD_FOUND = 0,
            ERROR_MESSAGE = 1,
            //Oracle
            //username:created_time
            USER = 2,
            //version
            VERSION = 3,
            //sids
            SID = 4,
            //just role name
            ROLE = 5,
            //Profile:pwdPolicy
            PASSWORD_POLICY = 6,
            //username:status:profile
            STATUS = 7,
            //User:role
            USER_ROLE = 8,
            //Tables user can see
            TABLES = 9,
            //username:hash
            HASHES = 10,
            LINKS = 11,
            //username:schemaname:osuser:machine:terminal:program:module:logon_time
            AUDIT_INFO = 12
        }
        public OracleReport()
        {
            session = Session.getInstance();
            InitializeComponent();
        }
        int id = -1;
        int actualModule = -1;
        string host = "";
        internal override void setData(int id, int p, string p_3)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            listBox4.Items.Clear();
            listBox5.Items.Clear();
            this.id = id;
            this.actualModule = p;
            this.host = p_3;
            string query = "SELECT * FROM Messages WHERE id = "+id +" AND module = "+p+" AND host_ = '" + p_3 + "' AND type = "+(int)OracleMessageType.USER;
            List<Messages> mes = session.getMessages(query);
            foreach (Messages message in mes)
            {
                    listBox1.Items.Add(message.User);
            }
            query = "SELECT usename FROM USER_PASS WHERE id = " + id + " AND module = " + p + " AND host_ = '" + p_3 + "'";
            List<string> users = session.getStrings(query);
            foreach (string message in users)
            {
                if(!listBox1.Items.Contains(message))
                    listBox1.Items.Add(message);
            }
            query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + p + " AND host_ = '" + p_3 + "' AND type = " + (int)OracleMessageType.ROLE;
            mes = session.getMessages(query);
            foreach (Messages message in mes)
            {
                listBox2.Items.Add(message.Message);
            }
            query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + p + " AND host_ = '" + p_3 + "' AND type = " + (int)OracleMessageType.PASSWORD_POLICY;
            mes = session.getMessages(query);
            foreach (Messages message in mes)
            {
                string prof = message.Message.Split(new char[] { ':' })[0];
                if (!listBox4.Items.Contains(prof))
                {
                    listBox4.Items.Add(prof);
                }
            }
            query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + p + " AND host_ = '" + p_3 + "' AND type = " + (int)OracleMessageType.VERSION;
            mes = session.getMessages(query);
            if(mes.Count > 0)
                textBox8.Text = mes[0].Message;

            query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + p + " AND host_ = '" + p_3 + "' AND type = " + (int)OracleMessageType.LINKS;
            mes = session.getMessages(query);
            foreach (Messages message in mes)
            {
                string prof = message.Message.Split(new char[] { ':' })[0];
                if (!listBox5.Items.Contains(message.Message))
                {
                    listBox5.Items.Add(message.Message);
                }
                if (!listBox3.Items.Contains(prof))
                {
                    listBox3.Items.Add(prof);
                }
            }

            query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + p + " AND host_ = '" + p_3 + "' AND type = " + (int)OracleMessageType.SID;
            mes = session.getMessages(query);
            foreach (Messages message in mes)
            {
                if (!listBox3.Items.Contains(message.Message))
                {
                    listBox3.Items.Add(message.Message);
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
                return;

            textBox1.Text = listBox1.SelectedItem.ToString();
            string query = "SELECT pass FROM USER_PASS WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username = '" + listBox1.SelectedItem.ToString() + "'";
            textBox2.Text = session.getString(query);

            query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username = '" + listBox1.SelectedItem.ToString() + "'";
            List<Messages> mes = session.getMessages(query);
            textBox4.Text ="";
            richTextBoxDB.Text = "";
            foreach (Messages message in mes)
            {
                if (message.Type == (int)OracleMessageType.USER){
                    textBox9.Text = message.Message;
                    continue;
                }
                if (message.Type == (int)OracleMessageType.HASHES)
                {
                    textBox3.Text = message.Message;
                    continue;
                }
                if (message.Type == (int)OracleMessageType.USER_ROLE)
                {
                    textBox4.Text += message.Message + ", ";
                    continue;
                }
                if (message.Type == (int)OracleMessageType.STATUS)
                {
                    textBox7.Text = message.Message.Split(new char[] { ':' })[1];
                    textBox10.Text = message.Message.Split(new char[] { ':' })[0];
                }
                if (message.Type == (int)OracleMessageType.TABLES)
                {
                    richTextBoxDB.Text += message.Message +Environment.NewLine;
                }
            }
        }



        private void listBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex < 0)
                return;
            textBox6.Text = "";
            string query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND type = " + (int)OracleMessageType.PASSWORD_POLICY;
            List<Messages> mes = session.getMessages(query);
            foreach (Messages message in mes)
            {
                string prof = message.Message.Split(new char[] { ':' })[1];
                if (message.Message.StartsWith( listBox4.SelectedItem.ToString()))
                {
                    textBox6.Text += prof + "\n";
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ListBoxForm lbf = new ListBoxForm("Audit Information", "UserName : SchemaName : OSUser : Machine : Terminal : Program : Module : Logon_time");
            lbf.addItem(session.getMessages("SELECT * FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND type = " + (int)OracleMessageType.AUDIT_INFO).ToArray());
            lbf.ShowDialog();
        }
    }
}
