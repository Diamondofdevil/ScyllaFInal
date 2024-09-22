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
    public enum POP3MessageType
    {
        //Defaults
        //NO MESSAGE is stored in the DataBase
        NoMessage = -1,
        //Used when a user-password is found, the message MUST be the password
        USER_PASSWORD_FOUND = 0,
        //When an error happend
        ERROR_MESSAGE = 1,

        //username:created_time
        MAIL = 2,
        ADDRESS = 3,
        HEADER = 50
    }
    public partial class POP3Report : IReportPanel//UserControl//
    {
        private Session session;
        private int id = -1;
        int actualModule = -1;
        string host = "";

        public POP3Report()
        {
            session = Session.getInstance();
            InitializeComponent();
        }

        internal override void setData(int id, int module, string host)
        {
            this.id = id;
            this.actualModule = module;
            this.host = host;
            string query = "SELECT username FROM USER_PASS WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host + "'";
            List<string> strs = session.getStrings(query);
            clearData();
            listBox1.Items.AddRange(strs.ToArray());
            if (strs.Count > 0)
                listBox1.SelectedIndex = 0;
            listBox1_SelectedIndexChanged(null, new EventArgs());
        }

        private void clearData()
        {
            listBox1.Items.Clear();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
                return;
            string query = "SELECT pass FROM USER_PASS WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username = '" + listBox1.Text + "'";
            textBox1.Text = session.getString(query);
            query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username = '" + listBox1.Text + "' AND (type = "+ (int)POP3MessageType.MAIL +" or type = "+(int) POP3MessageType.ADDRESS+" ) ORDER BY Message";
            
            List<Messages> mes = session.getMessages(query);
            string actual = string.Empty;
            listBox2.Items.Clear();
            richTextBox1.Text = "";
            foreach (Messages m in mes)
            {
                if (m.Type == (int)POP3MessageType.MAIL)
                    listBox2.Items.Add(m.Message);
                if (m.Type == (int)POP3MessageType.ADDRESS)
                    richTextBox1.Text += m.Message + Environment.NewLine;
                //string name = m.Message.Substring(0, m.Message.IndexOf(':'));
            }
            textBox2.Text = mes.Count.ToString();
            if (listBox2.Items.Count > 0)
            {
                listBox2.SelectedIndex = 0;
                listBox2_SelectedIndexChanged(null, new EventArgs());
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex < 0)
                return;
            string query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username = '" + listBox2.Text + "' AND type = " + (int)POP3MessageType.HEADER+ " ORDER BY Message";
            List<String> mes = session.getStrings(query);
            richTextBox2.Text = "";
            foreach (String m in mes)
            {
                richTextBox2.Text += m.Substring(4);
            }
        }
    }
}
