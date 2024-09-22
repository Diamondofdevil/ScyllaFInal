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
    public partial class LDAPReport : IReportPanel//UserControl//
    {
        public enum LDAPMessageType
        {
            //Defaults
            NoMessage = -1,
            USER_PASSWORD_FOUND = 0,
            ERROR_MESSAGE = 1,
            //LDAP
            USERS = 2,
            USER_GROUP = 3,
            COMPUTERS = 4,
            ANONYMOUS = 5
        }
        private Session session;
        private int id = -1;
        int actualModule = -1;
        string host = "";

        public LDAPReport()
        {
            session = Session.getInstance();
            InitializeComponent();
        }
        internal override void setData(int id, int module, string host__)
        {
            this.id = id;
            this.actualModule = module;
            this.host = host__;
            label3.Text = "139";
            string query = "SELECT username FROM USER_PASS WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host__ + "'";
            List<string> strs = session.getStrings(query);
            clearData();
            comboBox1.Items.AddRange(strs.ToArray());
            if (strs.Count > 0)
                comboBox1.SelectedIndex = 0;
            comboBox1_SelectedIndexChanged(null, new EventArgs());
        }
       
        private void clearData()
        {
            comboBox1.Items.Clear();
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0)
                return;
            string query = "SELECT pass FROM USER_PASS WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username = '" + comboBox1.Text + "'";
            textBox4.Text = session.getString(query);
            query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username = '" + comboBox1.Text + "' ORDER BY Message";
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            List<Messages> mes = session.getMessages(query);
            string actual = string.Empty;
            foreach (Messages m in mes)
            {
                string name = m.Message;
                if(m.Message.Contains(":"))
                    name = m.Message.Substring(0, m.Message.IndexOf(':'));
                if (actual.Equals(name))
                {
                    continue;
                }
                actual = name;
                if (m.Type == (int)LDAPMessageType.USERS)
                {
                    listBox1.Items.Add(name);
                    //dataGridView1.Rows.Add(dgvr);
                    continue;
                }
                if (m.Type == (int)LDAPMessageType.COMPUTERS)
                {
                    listBox2.Items.Add(name);
                    //dataGridView2.Rows.Add(dgvr);
                    continue;
                }
                if (m.Type == (int)LDAPMessageType.ANONYMOUS)
                    checkBox1.Checked = true;
            }
            if (listBox1.Items.Count > 0)
                listBox1.SelectedIndex = 0;
            if (listBox2.Items.Count > 0)
                listBox2.SelectedIndex = 0;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            dataGridView1.Rows.Clear();
            string query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username = '" + comboBox1.Text + "' and message like '"+listBox1.Text+"%' and type = "+(int)LDAPMessageType.USERS+" ORDER BY Message";

            List<Messages> mes = session.getMessages(query);
            
            foreach (Messages m in mes)
            {
                string att = m.Message.Substring(m.Message.IndexOf(':') + 1, m.Message.Substring(m.Message.IndexOf(':') + 1).IndexOf(':'));
                string fm = m.Message.Substring(m.Message.IndexOf(':') + 1);
                string val = fm.Substring(fm.IndexOf(':')+1);
                val = val.Substring(0, val.Length - 1);

                DataGridViewRow dgvr = new DataGridViewRow();
                //DataGridViewTextBoxCell cel = new DataGridViewTextBoxCell();
                DataGridViewTextBoxCell cel2 = new DataGridViewTextBoxCell();
                DataGridViewTextBoxCell cel3 = new DataGridViewTextBoxCell();
                
                cel2.Value = att;
                cel3.Value = val;
                //dgvr.Cells.Add(cel);
                dgvr.Cells.Add(cel2);
                dgvr.Cells.Add(cel3);
                //if (m.Type == (int)LDAPMessageType.USERS)
                //{
                    dataGridView1.Rows.Add(dgvr);
                    continue;
                //}
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView2.Rows.Clear();
            string query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username = '" + comboBox1.Text + "' and message like '" + listBox2.Text + "%' and type = " + (int)LDAPMessageType.COMPUTERS + " ORDER BY Message";

            List<Messages> mes = session.getMessages(query);

            foreach (Messages m in mes)
            {
                string att = m.Message.Substring(m.Message.IndexOf(':') + 1, m.Message.Substring(m.Message.IndexOf(':') + 1).IndexOf(':'));
                string fm = m.Message.Substring(m.Message.IndexOf(':') + 1);
                string val = fm.Substring(fm.IndexOf(':') + 1);
                val = val.Substring(0, val.Length - 1);

                DataGridViewRow dgvr = new DataGridViewRow();
                //DataGridViewTextBoxCell cel = new DataGridViewTextBoxCell();
                DataGridViewTextBoxCell cel2 = new DataGridViewTextBoxCell();
                DataGridViewTextBoxCell cel3 = new DataGridViewTextBoxCell();

                cel2.Value = att;
                cel3.Value = val;
                //dgvr.Cells.Add(cel);
                dgvr.Cells.Add(cel2);
                dgvr.Cells.Add(cel3);
                dataGridView2.Rows.Add(dgvr);
                continue;
            }
        }
    }
}
