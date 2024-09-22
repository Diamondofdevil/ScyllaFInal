using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ReportViewer.Panels;
using DBManagement;

namespace ReportViewer
{
    
    public partial class Form1 : Form
    {
        private enum ModuleList
        {
            FTP = 0,
            Telnet = 1,
            POP3 = 2,
            SMTP = 3,
            SMB = 4,
            HTTP = 5,
            IMAP = 6,
            LDAP = 7,
            MSSQL = 8,
            MYSQL = 9,
            ORACLE = 10,
            DB2 = 11,
            PGSQL = 12,
            DNS,
        };
        private Session session;
        private int id;
        IReportPanel rep;
        public Form1()
        {
            session = Session.getInstance();
            InitializeComponent();
            panel1.Controls.Add(new OracleReport());
            id = 1;
        }
        public Form1(int id)
        {
            session = Session.getInstance();
            InitializeComponent();
            panel1.Controls.Add((rep = new OracleReport()));
            this.id = id;
            setSessionInfo();
        }

        private void setSessionInfo()
        {
            SessionObj sob = session.getSession(this.id);
            label4.Text = sob.Name;
            textBox1.Text = sob.Comments;
        }


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            if (listBox1.SelectedIndex >= 0)
            {
                panel1.Controls.Clear();
                listBox2.Items.AddRange(session.getHosts(id, listBox1.SelectedIndex).ToArray());
                switch (listBox1.SelectedIndex)
                {
                    case (int)ModuleList.FTP:
                        panel1.Controls.Add(rep = new FTPReport());
                        break;
                    case (int)ModuleList.Telnet:
                        panel1.Controls.Add((rep = new TelnetReport()));
                        break;
                    case (int)ModuleList.POP3:
                        panel1.Controls.Add((rep = new POP3Report()));
                        break;
                    case (int)ModuleList.SMTP:
                        panel1.Controls.Add((rep = new SMTPReport()));
                        break;
                    case (int)ModuleList.SMB:
                        panel1.Controls.Add(rep = new SMB());
                        break;
                    case (int)ModuleList.HTTP:
                        panel1.Controls.Add(rep = new HTTPReport());
                        break;
                    case (int)ModuleList.IMAP:
                        panel1.Controls.Add(rep = new IMAPReport());
                        break;
                    case (int)ModuleList.MSSQL:
                        panel1.Controls.Add(rep = new MSSQLReport());
                        break;
                    case (int)ModuleList.MYSQL:
                        panel1.Controls.Add(rep = new MySQLReport());
                        break;
                    case (int)ModuleList.ORACLE:
                        panel1.Controls.Add(rep = new OracleReport());
                        break;
                    case (int)ModuleList.DB2:
                        panel1.Controls.Add(rep = new DB2Report());
                        break;
                    case (int)ModuleList.LDAP:
                        panel1.Controls.Add(rep = new LDAPReport());
                        break;
                    case(int)ModuleList.PGSQL:
                        panel1.Controls.Add(rep = new PostgresReport());
                        break;
                    case (int)ModuleList.DNS:
                        panel1.Controls.Add(rep = new DNSSnoopReport());
                        break;
                }
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex < 0)
                return;
            fillInfo(listBox2.SelectedItem.ToString());
        }

        private void fillInfo(string p)
        {
            rep.setData(id, listBox1.SelectedIndex, listBox2.SelectedItem.ToString());
        }
    }
}
