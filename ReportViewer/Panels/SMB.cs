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
    public partial class SMB : IReportPanel//UserControl//
    {
        Session session;
        public enum SMBMessageType
        {
            //Defaults
            NoMessage = -1,
            USER_PASSWORD_FOUND = 0,
            ERROR_MESSAGE = 1,
            /*
             *
            public bool RPC;
            public bool patches;
             * */
            //username:comment:flags,...:full_name:group:uid
            USER = 2,
            //group:comment:GUID
            GROUP = 3,
            //disk
            DISK = 4,
            //id:timegen:timewritten:type:sname:cname
            EVENT = 5,
            //minLen:maxAge:minAge:forceLogOf:passHistLen:attenpts:timeBetwFail:lockDuration
            PASSWORD_POLICY = 6,
            //#.#:type:comment:adGUID:adName:adOrg:adOwner:adProc:adProcCount
            OS = 7,
            //dispName:servName:status
            SERVICES = 8,
            //usrName:connMachine:secIdle:secConn
            SESSIONS = 9,
            //date
            DATE = 10,
            //name:add:netadd:domain
            NETDEV = 11,
            NULLSESSION = 12,
            //name:host:comment:type
            SHARES = 13,
            LMHash = 14,
            NTHash = 15
        }
        public SMB()
        {
            InitializeComponent();
            session = Session.getInstance();
            
        }
        int actualModule;
        int id;
        string host;
        internal override void setData(int id, int module, string host)
        {
            listBox1.Items.Clear();
            this.id = id; this.actualModule = module; this.host = host;
            string query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host + "' AND type = " + (int)SMBMessageType.USER;
            List<Messages> mes = session.getMessages(query);
            foreach (Messages message in mes)
            {
                if(!listBox1.Items.Contains(message.Message.Split(new char[] { ':' })[0]))
                    listBox1.Items.Add(message.Message.Split(new char[] { ':' })[0]);
            }
            query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host + "' AND type = " + (int)SMBMessageType.OS;
            mes = session.getMessages(query);
            if (mes.Count > 0)
                textBox12.Text = mes[0].Message;
            query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host + "' AND type = " + (int)SMBMessageType.EVENT;
            mes = session.getMessages(query);
            foreach (Messages m in mes)
            {
                string[] row = m.Message.Split(new char[] { ':' });
                dataGridView1.Rows.Add(row);
            }
            this.radioButton3.Checked = true;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            string query = "";
            dataGridView1.Rows.Clear();
            //dataGridView1.Columns.Clear();
            if (radioButton1.Checked)
            {
                C1.HeaderText = "ID";
                C2.HeaderText = "Time Generated";
                C3.HeaderText = "Time Written";
                C4.HeaderText = "Type";
                C5.HeaderText = "Source Name";
                C6.HeaderText = "Computer Name";
                C7.HeaderText = "";
                C8.HeaderText = "";

                query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND type = " + (int)SMBMessageType.EVENT;
                List<string> l = session.getStrings(query);
                foreach (string s in l)
                {
                    string[] strs = s.Split(new char[] { ':' });
                    dataGridView1.Rows.Add(strs);
                    //id:timegen:timewritten:type:sname:cname
                }
                return;
            }
            if (radioButton2.Checked)
            {
                C1.HeaderText = "Name";
                C2.HeaderText = "Host";
                C3.HeaderText = "Comment";
                C4.HeaderText = "Type";
                C5.HeaderText = "";
                C6.HeaderText = "";
                C7.HeaderText = "";
                C8.HeaderText = "";

                query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND type = " + (int)SMBMessageType.SHARES;
                List<string> l = session.getStrings(query);
                foreach (string s in l)
                {
                    string[] strs = s.Split(new char[] { ':' });
                    dataGridView1.Rows.Add(strs);
                    //name:host:comment:type
                }
                return;
            }
            if (radioButton3.Checked)
            {
                C1.HeaderText = "Min Len";
                C2.HeaderText = "Max Age";
                C3.HeaderText = "Min Age";
                C4.HeaderText = "Force Log Off";
                C5.HeaderText = "Pwd History Len";
                C6.HeaderText = "Attempts";
                C7.HeaderText = "Time Between Fail";
                C8.HeaderText = "Lock Down Duration";

                query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND type = " + (int)SMBMessageType.PASSWORD_POLICY;
                List<string> l = session.getStrings(query);
                foreach (string s in l)
                {
                    string[] strs = s.Split(new char[] { ':' });
                    dataGridView1.Rows.Add(strs);
                    //name:host:comment:type
                }
                return;
            }
            if (radioButton4.Checked)
            {
                C1.HeaderText = "Display Name";
                C2.HeaderText = "Service Name";
                C3.HeaderText = "Status";
                C4.HeaderText = "";
                C5.HeaderText = "";
                C6.HeaderText = "";
                C7.HeaderText = "";
                C8.HeaderText = "";

                query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND type = " + (int)SMBMessageType.SERVICES;
                List<string> l = session.getStrings(query);
                foreach (string s in l)
                {
                    string[] strs = s.Split(new char[] { ':' });
                    dataGridView1.Rows.Add(strs);
                    //dispName:servName:status
                }
                return;
            }
            if (radioButton5.Checked)
            {
                C1.HeaderText = "Device Name";
                C2.HeaderText = "Address";
                C3.HeaderText = "Network Address";
                C4.HeaderText = "Domain";
                C5.HeaderText = "";
                C6.HeaderText = "";
                C7.HeaderText = "";
                C8.HeaderText = "";

                query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND type = " + (int)SMBMessageType.NETDEV;
                List<string> l = session.getStrings(query);
                foreach (string s in l)
                {
                    string[] strs = s.Split(new char[] { ':' });
                    dataGridView1.Rows.Add(strs);
                    //name:add:netadd:domain
                }
                return;
            }
            if (radioButton6.Checked)
            {
                C1.HeaderText = "User Name";
                C2.HeaderText = "Machine";
                C3.HeaderText = "Idle Time (sec)";
                C4.HeaderText = "Seconds Connected";
                C5.HeaderText = "";
                C6.HeaderText = "";
                C7.HeaderText = "";
                C8.HeaderText = "";

                query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND type = " + (int)SMBMessageType.SESSIONS;
                List<string> l = session.getStrings(query);
                foreach (string s in l)
                {
                    string[] strs = s.Split(new char[] { ':' });
                    dataGridView1.Rows.Add(strs);
                    //usrName:connMachine:secIdle:secConn
                }
                return;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
                return;
            textBox1.Text = listBox1.SelectedItem.ToString();
            string query = "SELECT pass FROM USER_PASS WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username LIKE '%" + listBox1.SelectedItem.ToString() + "%'";
            textBox2.Text = session.getString(query);

            if (String.IsNullOrEmpty(textBox2.Text))
            {
                textBox4.Text = "localhost";
            }
            else
            {
                query = "SELECT username FROM USER_PASS WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username LIKE '%" + listBox1.SelectedItem.ToString() + "%'";
                textBox4.Text = session.getString(query).Split(new char[] { '\\'})[0];
            }
            query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND Message LIKE '%" + listBox1.SelectedItem.ToString() + "%' AND type = " + (int)SMBMessageType.USER;
            String mes = session.getString(query);
            //username:comment:full_name:uid:flags,...:g:group
            string [] messages  =  mes.Split(new char[] { ':' });
            textBox7.Text =messages[1];
            textBox9.Text = messages[4];
            textBox10.Text = messages[3];
            richTextBox2.Text = "";
            textBox4.Text = "";
            richTextBox1.Text = "";

            int i = 5;
            while (messages[i] != "g")
            {
                richTextBox2.Text += messages[i] + "\n";
                i++;
            }
            for (i = i+1; i < messages.Length; i++)
            {
                richTextBox1.Text += messages[i] + "\n";
            }
            query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username = '" + listBox1.SelectedItem.ToString() + "' AND type = " + (int)SMBMessageType.LMHash;
            textBox6.Text = session.getString(query);
            query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username = '" + listBox1.SelectedItem.ToString() + "' AND type = " + (int)SMBMessageType.NTHash;
            textBox8.Text = session.getString(query);
        }
    }
}
