using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DBManagement
{
    public partial class SetSessionManaget : Form
    {
        private SessionObj selectedSession;

        public SessionObj getSelectedSession
        {
            get { return selectedSession; }
            set { selectedSession = value; }
        }
        public SetSessionManaget()
        {
            InitializeComponent();
        }
        public SetSessionManaget(List<SessionObj> sessions)
        {
            InitializeComponent();
            foreach (SessionObj obj in sessions)
            {
                listBox1.Items.Add(obj);
            }
            if (listBox1.Items.Count < 1)
                listBox1.Items.Add(setSession());
            listBox1.SelectedIndex = 0;
        }
        private SessionObj setSession()
        {
            CreateNewSession ns = new CreateNewSession();
            if (ns.ShowDialog() == DialogResult.OK)
            {
                return ns.SessionID;
            }
            else
            {
                setSession();
                ns.Dispose();
            }
            return null;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                selectedSession = (SessionObj) listBox1.Items[listBox1.SelectedIndex];
                DialogResult = DialogResult.OK;
                this.Close();
                return;
            }
            MessageBox.Show("Please select a session", "Select a session", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CreateNewSession ns = new CreateNewSession();
            ns.ShowDialog();
            if (ns.DialogResult == DialogResult.OK)
            {
                listBox1.Items.Add(ns.SessionID);
                listBox1.SelectedItem = ns.SessionID;
                return;
            }
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                selectedSession = (SessionObj)listBox1.Items[listBox1.SelectedIndex];
                DialogResult = DialogResult.OK;
                this.Close();
                return;
            }
            MessageBox.Show("Please select a session", "Select a session", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
