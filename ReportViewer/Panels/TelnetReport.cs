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
    /// <summary>
    /// notifies an error, a success or a message
    /// </summary>
    /// <post>If error, manage the error, else prompt the password or message and add it to the data base</post>
    /// <param name="message">The message to prompt and to add to the database. If type == ERROR it MUST be password:message or password</param>
    /// <param name="username">the username who prompts the message</param>
    /// <param name="type">Type of the message. As an exaple, Oracle module message types are provided (note that the first 3 are obligatory</param>
    public enum TelnetMessageType
    {
        //Defaults
        //NO MESSAGE is stored in the DataBase
        NoMessage = -1,
        //Used when a user-password is found, the message MUST be the password
        USER_PASSWORD_FOUND = 0,
        //When an error happend
        ERROR_MESSAGE = 1,
        //Oracle
        //username:created_time
        CD = 2,
        AUTH_TYPE = 3,
        SHADOW = 50,
        PASSWORD = 51,
        SUDO_COMMANDS = 52
    }

    public partial class TelnetReport : IReportPanel//UserControl//
    {
        private Session session;
        private int id = -1;
        int actualModule = -1;
        string host = "";

        public TelnetReport()
        {
            session = Session.getInstance();
            InitializeComponent();
        }

        internal override void setData(int id, int module, string host)
        {
            this.id = id;
            this.actualModule = module;
            this.host = host;
            string query = "SELECT username, pass FROM USER_PASS WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host + "'";
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
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            string name = (string) dataGridView1.Rows[e.RowIndex].Cells[0].Value;
            string query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username = '" + name + "'";
            List<Messages> mes = session.getMessages(query);
            textBox1.Text = textBox2.Text = textBox3.Text = textBox4.Text = string.Empty;
            foreach (Messages message in mes)
            {
                if (message.Type == (int)TelnetMessageType.CD)
                {
                    textBox1.Text += message.Message.Trim();
                    continue;
                }
                if (message.Type == (int)TelnetMessageType.PASSWORD)
                {
                    textBox2.Text += message.Message.Substring(4).Trim(); ;
                    continue; 
                }
                if (message.Type == (int)TelnetMessageType.SHADOW)
                {
                    textBox3.Text += message.Message.Substring(4).Trim();
                    continue;
                }
                if (message.Type == (int) TelnetMessageType.SUDO_COMMANDS)
                {
                    textBox4.Text += message.Message.Substring(4).Trim();
                }
                if (message.Type == (int)TelnetMessageType.AUTH_TYPE)
                {
                    textBox5.Text += message.Message;
                }
            }
        }
    }
}
