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
    public partial class FTPReport : IReportPanel//UserControl//
    {
        public enum FTPMessageType
        {
            //Defaults
            NoMessage = -1,
            USER_PASSWORD_FOUND = 0,
            ERROR_MESSAGE = 1,
            //FTP
            //FEAT command
            FEAT = 2,
            //SYST command
            SYST = 4,
            PWD = 5,
            //Permissions
            LIST = 6,
            STOR = 7,
            DELE = 8,
            MKD = 9,
            RMD = 10,
            DirTrans = 11

        }
        private Session session;
        private int id = -1;
        int actualModule = -1;
        string host = "";

        public FTPReport()
        {
            session = Session.getInstance();
            InitializeComponent();
        }
        internal override void setData(int id, int module, string host__)
        {
            this.id = id;
            this.actualModule = module;
            this.host = host__;
            string query = "SELECT username, pass FROM USER_PASS WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host__ + "'";
            List<DoubleString> strs = session.get2Strings(query);
            clearData();
            foreach (DoubleString s in strs)
            {
                DataGridViewRow dgvr = new DataGridViewRow();
                DataGridViewTextBoxCell cel = new DataGridViewTextBoxCell();
                DataGridViewTextBoxCell cel2 = new DataGridViewTextBoxCell();
                cel.Value = s.s1;
                cel2.Value = s.s2;
                dgvr.Cells.Add(cel);
                dgvr.Cells.Add(cel2);
                dataGridView1.Rows.Add(dgvr);
            }
        }

        private void clearData()
        {
            dataGridView1.Rows.Clear();
            checkBox1.Checked = false;
            checkBox2.Checked = false;
            checkBox3.Checked = false;
            checkBox4.Checked = false;
            checkBox5.Checked = false;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";

        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            label11.Visible = textBox4.Visible = false;
            if (e.RowIndex < 0)
                return;
            string name = (string) dataGridView1.Rows[e.RowIndex].Cells[0].Value;
            string query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username = '" + name + "'";
            List<Messages> mes = session.getMessages(query);
            foreach (Messages message in mes)
            {
                if (message.Type == (int)FTPMessageType.DELE)
                {
                    checkBox4.Checked = true;
                    continue;
                }
                if (message.Type == (int)FTPMessageType.FEAT)
                {
                    textBox2.Text = message.Message;
                    continue;
                }
                if (message.Type == (int)FTPMessageType.LIST)
                {
                    checkBox3.Checked = true;
                    continue;
                }
                if (message.Type == (int)FTPMessageType.MKD)
                {
                    checkBox5.Checked = true;
                    continue;
                }
                if (message.Type == (int)FTPMessageType.PWD)
                {
                    textBox1.Text = message.Message;
                    continue;
                }
                if (message.Type == (int)FTPMessageType.RMD)
                {
                    checkBox1.Checked = true;
                    continue;
                }
                if (message.Type == (int)FTPMessageType.STOR)
                {
                    checkBox2.Checked = true;
                    continue;
                }
                if (message.Type == (int)FTPMessageType.SYST)
                {
                    textBox3.Text = message.Message;
                    continue;
                }
                if (message.Type == (int)FTPMessageType.DirTrans)
                {
                    label11.Visible = textBox4.Visible = true;
                    textBox4.Text = message.Message;
                    continue;
                }
            }
        }
    }
}
