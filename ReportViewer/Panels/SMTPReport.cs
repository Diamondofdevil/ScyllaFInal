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
    public enum SMTPMessageType
    {
        //Defaults
        //NO MESSAGE is stored in the DataBase
        NoMessage = -1,
        //Used when a user-password is found, the message MUST be the password
        USER_PASSWORD_FOUND = 0,
        //When an error happend
        ERROR_MESSAGE = 1,

        VRFY_ALLOWED = 2,
        ROOT_MAIL = 3,
        ANON_ANON = 4,
        ANON_USER = 5,
        USER_ANON = 6,
        USER_USER = 7,
        AUTH_OPTIONS = 8,
        HELP = 9
    }
    public partial class SMTPReport : IReportPanel//UserControl//
    {
        private Session session;
        private int id = -1;
        int actualModule = -1;
        string host = "";

        public SMTPReport()
        {
            session = Session.getInstance();
            InitializeComponent();
        }

        internal override void setData(int id, int module, string host)
        {
            this.id = id;
            this.actualModule = module;
            this.host = host;
            //label3.Text = "139";
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
            query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' ORDER BY Message";// AND username = '" + listBox1.Text + "'
            
            List<Messages> mes = session.getMessages(query);
            richTextBox1.Text = "";
            richTextBox2.Text = "";
            string actual = string.Empty;
            checkBox1.Checked = false;
            checkBox2.Checked = false;
            checkBox3.Checked = false;
            checkBox4.Checked = false;
            foreach (Messages m in mes)
            {
                if(m.Type == (int)SMTPMessageType.AUTH_OPTIONS)
                    richTextBox1.Text += m.Message+Environment.NewLine;
                if(m.Type == (int)SMTPMessageType.HELP)
                    richTextBox2.Text += m.Message + Environment.NewLine;
                if (m.Type == (int)SMTPMessageType.ANON_ANON)
                    checkBox4.Checked = true;
                if (m.Type == (int)SMTPMessageType.USER_USER)
                    checkBox1.Checked = true;
                if (m.Type == (int)SMTPMessageType.USER_ANON)
                    checkBox2.Checked = true;
                if (m.Type == (int)SMTPMessageType.ANON_USER)
                    checkBox3.Checked = true;
            }
        }
    }
}
