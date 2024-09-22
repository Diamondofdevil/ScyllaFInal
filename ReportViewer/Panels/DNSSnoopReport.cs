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
    public partial class DNSSnoopReport : IReportPanel//UserControl//
    {
        public enum DNSMessageType
        {
            //Defaults
            NoMessage = -1,
            USER_PASSWORD_FOUND = 0,
            ERROR_MESSAGE = 1,
            //FTP
            //FEAT command
            IsAuthoirtative = 2,
            //Basic answers
            A = 4,
            //NameServer Answers
            NS = 5,
            //Aditional records
            AR = 6,
        }
        private Session session;
        private int id = -1;
        int actualModule = -1;
        string host = "";

        public DNSSnoopReport()
        {
            session = Session.getInstance();
            InitializeComponent();
        }
        internal override void setData(int id, int module, string host__)
        {
            this.id = id;
            this.actualModule = module;
            this.host = host__;
            string query = "SELECT username FROM USER_PASS WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host__ + "'";
            List<string> hosts = session.getStrings(query);
            listBox1.Items.Clear();
            foreach (string host in hosts)
            {
                listBox1.Items.Add(host);
            }
            if (listBox1.Items.Count > 0)
            {
                listBox1.SelectedIndex = 0;
                listBox1_SelectedIndexChanged(null, new EventArgs());
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
                return;
            string query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username = '"+listBox1.SelectedItem.ToString()+"'";
            List<Messages> mes = session.getMessages(query);
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();
            checkBoxIsAuth.Checked = false;
            foreach (Messages message in mes)
            {
                if (message.Type == (int)DNSMessageType.IsAuthoirtative)
                {
                    checkBoxIsAuth.Checked = true;
                }
                string[] answers = message.Message.Split(new char[]{':'}, StringSplitOptions.RemoveEmptyEntries);
                
                if (message.Type == (int)DNSMessageType.A)
                {
                    foreach (string answer in answers)
                    {
                        string[] data = answer.Split(',');
                        DataGridViewRow dgvr = new DataGridViewRow();
                        DataGridViewTextBoxCell cel = new DataGridViewTextBoxCell();
                        DataGridViewTextBoxCell cel2 = new DataGridViewTextBoxCell();
                        cel.Value = data[0];
                        cel2.Value = data[1];
                        dgvr.Cells.Add(cel);
                        dgvr.Cells.Add(cel2);
                        dataGridView1.Rows.Add(dgvr);
                    }
                    continue;
                }
                else if (message.Type == (int)DNSMessageType.NS)
                {
                    foreach (string answer in answers)
                    {
                        string[] data = answer.Split(',');
                        DataGridViewRow dgvr = new DataGridViewRow();
                        DataGridViewTextBoxCell cel = new DataGridViewTextBoxCell();
                        DataGridViewTextBoxCell cel2 = new DataGridViewTextBoxCell();
                        cel.Value = data[0];
                        cel2.Value = data[1];
                        dgvr.Cells.Add(cel);
                        dgvr.Cells.Add(cel2);
                        dataGridView2.Rows.Add(dgvr);
                    }
                }
                else if (message.Type == (int)DNSMessageType.AR)
                {
                    foreach (string answer in answers)
                    {
                        string[] data = answer.Split(',');
                        if (data.Length != 3)
                            continue;
                        DataGridViewRow dgvr = new DataGridViewRow();
                        DataGridViewTextBoxCell cel = new DataGridViewTextBoxCell();
                        DataGridViewTextBoxCell cel2 = new DataGridViewTextBoxCell();
                        DataGridViewTextBoxCell cel3 = new DataGridViewTextBoxCell();
                        cel.Value = data[0];
                        cel2.Value = data[1];
                        cel3.Value = data[2];
                        dgvr.Cells.Add(cel);
                        dgvr.Cells.Add(cel2);
                        dgvr.Cells.Add(cel3);
                        dataGridView3.Rows.Add(dgvr);
                    }
                }
            }
        }
    }
}
