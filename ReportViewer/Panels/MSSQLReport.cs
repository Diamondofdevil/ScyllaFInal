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
    //listBox1_SelectedIndexChanged
    public partial class MSSQLReport : IReportPanel//UserControl//
    {
        Session session;
        public enum MSSQLMessageType
        {
            FoundNoMessage = -2,
            NoMessage = -1,
            USER_PASSWORD_FOUND = 0,
            ERROR_MESSAGE = 1,

            CommandLine = 2,
            SSPI = 3,
            DB = 4,
            Users = 5,
            OneClickOwnage = 50
        }
        public MSSQLReport()
        {
            InitializeComponent();
            session = Session.getInstance();
        }
        int actualModule;
        int id;
        string host;

        internal override void setData(int id, int module, string host)
        {
            listBox2.Items.Clear();
            this.id = id; this.actualModule = module; this.host = host;

            string query = "SELECT username FROM USER_PASS WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host + "'";
            List<String> strs = session.getStrings(query);
            foreach (String user in strs)
            {
                if (!listBox2.Items.Contains(user))
                    listBox2.Items.Add(user);
            }

            query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host + "' AND type = " + (int)MSSQLMessageType.Users;
            List<Messages> mes = session.getMessages(query);
            foreach (Messages message in mes)
            {
                if (!listBox2.Items.Contains(message.Message.Split(new char[] { ':' })[0]))
                    listBox2.Items.Add(message.Message.Split(new char[] { ':' })[0]);
            }
            query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host + "' AND type = " + (int)MSSQLMessageType.SSPI;
            mes = session.getMessages(query);
            if (mes.Count > 0)
                checkBox1.Checked = true;  
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            listBox1.Items.Clear();
            
            //textBox1.Text = listBox1.SelectedItem.ToString();
            string query = "SELECT pass FROM USER_PASS WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username LIKE '%" + listBox2.SelectedItem.ToString() + "%'";
            textBox3.Text = session.getString(query);

            query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND type = " + (int)MSSQLMessageType.DB + " AND username LIKE '%" + listBox2.SelectedItem.ToString() + "%'"; ;
            List<Messages> mes = session.getMessages(query);
            foreach (Messages m in mes)
            {
                listBox1.Items.Add(m.Message);
            }
            query = "SELECT Message FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND type = " + (int)MSSQLMessageType.OneClickOwnage + " AND username = '" + listBox2.SelectedItem.ToString() + "' order by message"; ;
            List<string> strs = session.getStrings(query);

            if (strs.Count > 1000)
                richTextBoxFSH.Text = "Message history is too long, please see the session log to see the history";
            else
            {
                richTextBoxFSH.Text = string.Empty;
                foreach (string txt in strs)
                {
                    richTextBoxFSH.Text += txt.Substring(4);
                }
            }

            query = "SELECT Message FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND type = " + (int)MSSQLMessageType.Users + " AND username = '" + listBox2.SelectedItem.ToString() + "'"; ;
            string mess = session.getString(query);

            //name:sid:is_disabled:create_date:default_database_name:password_hash
            if (string.IsNullOrWhiteSpace(mess.Trim()))
                return;
            string[] messages = mess.Split(new char[] { ':' });
            textBox4.Text = messages[1];
            checkBox2.Checked = messages[2].ToLower() == "false" || messages[2] == "0" ? true : false;
            textBox5.Text = messages[3] + ":" + messages[4]+":" + messages[5];
            textBox6.Text = messages[6];
            textBox2.Text = messages[7];
            
        }

    }
}
