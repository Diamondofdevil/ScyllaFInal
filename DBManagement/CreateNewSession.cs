using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DBManagement
{
    public partial class CreateNewSession : Form
    {
        public CreateNewSession()
        {
            InitializeComponent();
        }
        private SessionObj sessionID;

        public SessionObj SessionID
        {
            get { return sessionID; }
            set { sessionID = value; }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Session s = Session.getInstance();
            if (!String.IsNullOrEmpty(textBox1.Text))
            {
                s.createSession(textBox1.Text, textBox2.Text);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            SessionID = s.getLastSession();
            return;
        }
    }
}
