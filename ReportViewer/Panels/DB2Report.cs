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
    public partial class DB2Report : IReportPanel//UserControl//
    {
        public enum DB2MessageType
        {
            FoundNoMessage = -2,
            NoMessage = -1,
            USER_PASSWORD_FOUND = 0,
            ERROR_MESSAGE = 1,

            EXCSAT_INFO = 2,
            Table = 3,
            DASInfo = 4,
            AdminIface = 5,
            Instance = 6,
            SecPolicy = 7,
            AuditPolicy = 8,
            DataBaseInfo = 9,
            UserAuth = 10

        }
        private Session session;
        private int id;
        private int module;
        private string host;
        public DB2Report()
        {
            session = Session.getInstance();
            InitializeComponent();
        }

        internal override void setData(int id, int module, string host)
        {
            this.id = id; this.module = module; this.host = host;
            comboBox1.Items.Clear();
            string query = "SELECT username FROM USER_PASS WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host + "'";
            List<string> messages = session.getStrings(query);
            foreach (string message in messages)
            {
                if (!comboBox1.Items.Contains(message))
                    comboBox1.Items.Add(message);
            }
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
                comboBox1_SelectedIndexChanged(null, new EventArgs());
            }
            query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host + "' AND type = " + (int)DB2MessageType.DASInfo;
            string[] strings = session.getString(query).Split(new char[] { ':' });
            //di.app + ":" + di.Db2Syst + ":" + di.plat + ":" + di.ServType; 1,4,3,2
            if (strings.Length > 3)
            {
                textBox1.Text = strings[0];
                textBox2.Text = strings[3];
                textBox3.Text = strings[2];
                textBox4.Text = strings[1];
            }
            query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host + "' AND type = " + (int)DB2MessageType.AdminIface;
            strings = session.getString(query).Split(new char[] { ':' });
            if (strings.Length > 3)
            {
                textBox31.Text = strings[0];
                textBox30.Text = strings[3];
                textBox29.Text = strings[2];
                textBox28.Text = strings[1];
            }
            query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host + "' AND type = " + (int)DB2MessageType.Instance;
            messages = session.getStrings(query);
            listBox1.Items.Clear();
            foreach (string message in messages)
            {
                listBox1.Items.Add(message.Substring(0, message.IndexOf(':')));
            }
            if (listBox1.Items.Count > 0)
                listBox1.SelectedIndex = 0;

            query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host + "' AND type = " + (int)DB2MessageType.EXCSAT_INFO;
            List<Messages> mess = session.getMessages(query);
            foreach (Messages ms in mess)
            {
                if (ms.User == "PRL")
                    textBox35.Text = ms.Message;
                if (ms.User == "SN")
                    textBox32.Text = ms.Message;
                if (ms.User == "SCN")
                    textBox33.Text = ms.Message;
                if (ms.User == "EN")
                    textBox34.Text = ms.Message;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
                return;

            string query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host + "' AND type = " + (int)DB2MessageType.Instance + " AND message like '" + listBox1.SelectedItem.ToString() + ":%'";
            string[] ifaceInfo = session.getString(query).Split(':');
            if (ifaceInfo.Length > 6)
            {
                //din.instName+ ":"+din.authType+ ":"+din.connType+ ":"+din.hostName+ ":"+din.ip+ ":"+din.port+ ":"+din.serviceName;
                textBox8.Text = ifaceInfo[0];
                textBox5.Text = ifaceInfo[1];
                textBox6.Text = ifaceInfo[2];
                textBox7.Text = ifaceInfo[3];
                textBox9.Text = ifaceInfo[4];
                textBox10.Text = ifaceInfo[5];
                textBox11.Text = ifaceInfo[6];
            }

            query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host + "' AND type = " + (int)DB2MessageType.DataBaseInfo + " AND userName = '"+listBox1.SelectedItem.ToString()+"'";
            List<string> messages = session.getStrings(query);
            listBox2.Items.Clear();
            foreach (string message in messages)
            {
                //dbi.dbName + ":" + dbi.dbAlias + ":" + dbi.authType + ":" + dbi.driver.Replace(':', '-');
                listBox2.Items.Add(message.Substring(0, message.IndexOf(':')));
            }
            if (listBox2.Items.Count > 0)
                listBox2.SelectedIndex = 0;
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex < 0)
                return;
            string query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host + "' AND type = " + (int)DB2MessageType.DataBaseInfo + " AND message like '" + listBox2.SelectedItem.ToString() + ":%' AND userName = '" + listBox1.SelectedItem.ToString() + "'";
            string[] dbInfo = session.getString(query).Split(':');
            if (dbInfo.Length > 3)
            {
                //dbi.dbName + ":" + dbi.dbAlias + ":" + dbi.authType + ":" + dbi.driver ; 18,15,17
                textBox18.Text = dbInfo[0] + "(" + dbInfo[1] + ")";
                textBox15.Text = dbInfo[2];
                textBox17.Text = dbInfo[3];
            }
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string query = "SELECT pass FROM USER_PASS WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host + "'" + " AND username = '" + comboBox1.Text + "'";
            string pass = session.getString(query);
            if(!string.IsNullOrWhiteSpace(pass)) textBox16.Text = pass;

            query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host + "' AND type = " + (int)DB2MessageType.Table + " AND userName = '" + comboBox1.SelectedItem.ToString() + "'";
            List<string> tables = session.getStrings(query);
            dataGridView2.Rows.Clear();
            foreach (string table in tables)
            {
                object[] obs = table.Split(new char[] { ':' });
                dataGridView2.Rows.Add(obs);
            }

            query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host + "' AND type = " + (int)DB2MessageType.UserAuth + " AND userName = '" + comboBox1.SelectedItem.ToString() + "'";
            tables = session.getStrings(query);
            dataGridView1.Rows.Clear();
            foreach (string table in tables)
            {
                object[] obs = table.Split(new char[] { ':' });
                dataGridView1.Rows.Add(obs);
            }

            query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host + "' AND type = " + (int)DB2MessageType.SecPolicy + " AND userName = '" + comboBox1.SelectedItem.ToString() + "'";
            tables = session.getStrings(query);
            listBox5.Items.Clear();
            foreach (string table in tables)
            {
                string name = table.Split(new char[] { ':' })[0];
                listBox5.Items.Add(name);
            }
            listBox4.Items.Clear();
            query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host + "' AND type = " + (int)DB2MessageType.AuditPolicy + " AND userName = '" + comboBox1.SelectedItem.ToString() + "'";
            tables = session.getStrings(query);
            listBox4.Items.Clear();
            foreach (string table in tables)
            {
                string name = table.Split(new char[] { ':' })[0];
                listBox4.Items.Add(name);
            }
        }

        private void listBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            string query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host + "' AND type = " + (int)DB2MessageType.AuditPolicy + " AND userName = '" + comboBox1.SelectedItem.ToString() + "' AND Message like '"+listBox4.SelectedItem.ToString()+"%'";
            List<string> info = session.getStrings(query);
            foreach (string mes in info)
            {
                //auditpolicyname, auditstatus, contextstatus,objmaintstatus,sysadminstatus,executestatus,errortype ; 19, 23-27
                string[] names = mes.Split(new char[] { ':' });
                textBox19.Text = names[1];
                textBox23.Text = names[2];
                textBox24.Text = names[3];
                textBox25.Text = names[4];
                textBox26.Text = names[5];
                textBox27.Text = names[6];
            }
        }

        private void listBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            string query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host + "' AND type = " + (int)DB2MessageType.SecPolicy + " AND userName = '" + comboBox1.SelectedItem.ToString() + "' AND Message like '" + listBox5.SelectedItem.ToString() + "%'";
            List<string> info = session.getStrings(query);
            
            foreach (string mes in info)
            {
                //rwseclabelrel, notauthwriteseclabel, userauths, groupauths, roleauths, packed_desc
                string[] names = mes.Split(new char[] { ':' });
                textBox14.Text = names[1];
                textBox12.Text = names[2];
                textBox13.Text = names[3];
                textBox20.Text = names[4];
                textBox21.Text = names[5];
                textBox22.Text = names[6];
            }
            
        }
    }
}
