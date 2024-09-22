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
    public enum IMAPMessageType
    {
        //Defaults
        //NO MESSAGE is stored in the DataBase
        NoMessage = -1,
        //Used when a user-password is found, the message MUST be the password
        USER_PASSWORD_FOUND = 0,
        //When an error happend
        ERROR_MESSAGE = 1,

        MAIL = 2,
        ADDRESS = 3,
        FOLDER = 4,
        FOLDER_OPTION = 51,
        MAIL_HEADERS = 50
    }

    public partial class IMAPReport : IReportPanel//UserControl//
    {
        private Session session;
        private int id = -1;
        int actualModule = -1;
        string host = "";

        public IMAPReport()
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
            query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username = '" + listBox1.Text + "' AND (type = "+(int)IMAPMessageType.ADDRESS+" OR type = "+(int)IMAPMessageType.FOLDER+" ) ORDER BY Message";
            List<Messages> mes = session.getMessages(query);
            string actual = string.Empty;
            listBox3.Items.Clear();
            listBox2.Items.Clear();
            int totMessages = 0;
            foreach (Messages m in mes)
            {
                if (m.Type == (int)IMAPMessageType.ADDRESS)
                    listBox2.Items.Add(m.Message);
                if (m.Type == (int)IMAPMessageType.FOLDER)
                {
                    listBox3.Items.Add(m.Message);
                    totMessages += getCountMessages(m.Message);
                }
            }
            textBox2.Text = totMessages.ToString();
        }

        private int getCountMessages(string p)
        {
            string query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username = '" + p + "' AND type = " + (int)IMAPMessageType.FOLDER_OPTION + " ORDER BY Message";
            int totM = 0;
            foreach (string mes in session.getStrings(query))
            {
                foreach (string ss in mes.Split(new string[] { "\r\n" }, StringSplitOptions.None))
                {
                    if (ss.Contains(" EXISTS"))
                    {
                        totM += int.Parse(ss.Split(' ')[1]);
                        break;
                    }
                }
            }
            return totM;
        }

        private void listBox3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username = '" + listBox3.Text + "' AND type = " + (int)IMAPMessageType.FOLDER_OPTION + " ORDER BY Message";
            
            ListBoxForm lbf = new ListBoxForm("Folder " + listBox3.Text, listBox3.Text);
            lbf.addItemTruncated(session.getStrings(query).ToArray());
            lbf.ShowDialog();
        }

        private void listBox4_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username = '" + listBox3.Text.Replace("\"","")+":"+listBox4.Text + "' AND type = " + (int)IMAPMessageType.MAIL_HEADERS + " ORDER BY Message";

            ListBoxForm lbf = new ListBoxForm("Message " + listBox4.Text, listBox4.Text);
            lbf.addItemTruncated(session.getStrings(query).ToArray());
            lbf.ShowDialog();
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox3.SelectedIndex < 0)
                return;
            string query = "SELECT message FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username = '" + listBox1.Text.Replace("\"", "") + "' AND type = " + (int)IMAPMessageType.MAIL + " AND message LIKE '" + listBox3.Text.Replace("\"", "") + "%' ORDER BY Message";
            List<string> mes = session.getStrings(query);
            listBox4.Items.Clear();
            foreach(string m in mes)
                listBox4.Items.Add(m.Substring(listBox3.Text.Length-1));
        }
    }
}
