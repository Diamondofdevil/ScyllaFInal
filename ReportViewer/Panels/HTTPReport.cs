using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DBManagement;
using System.Diagnostics;

namespace ReportViewer.Panels
{
    public partial class HTTPReport : IReportPanel//UserControl//
    {
        private int id;
        private int actualModule;
        private string host;
        private Session session;
        public HTTPReport()
        {
            session = Session.getInstance();
            InitializeComponent();
        }

        internal override void setData(int id, int module, string host)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            treeView1.Nodes.Clear();
            this.id = id; this.actualModule = module; this.host = host;
            string query = "SELECT DISTINCT message FROM Messages WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host + "' AND (type = " + 9+" OR type ="+8+")";
            List<string> urls = session.getStrings(query);
            foreach (string url in urls)
            {
                listBox1.Items.Add(url);
            }

            TreeNode tn = new TreeNode("/");
            treeView1.Nodes.Add(tn);
            addToTV(ref tn);
            query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host + "' AND type = " + 10 + " AND message LIKE '../%'";
            List<Messages> mes = session.getMessages(query);
            if (mes.Count == 0)
            {
                treeView1.Update();
                return;
            }

            tn = new TreeNode("../");
            treeView1.Nodes.Add(tn);
            addToTV(ref tn);
            treeView1.Update();
        }
        internal void addToTV(ref TreeNode tn)
        {
            string query = "";
            if (tn.Text == "/")
                query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND type = " + 10 + "AND (message = '" + tn.Text + "' OR message = '')";
            else
            {
                string path = "";
                TreeNode parent = tn.Parent;
                while (parent != null && parent.Text != "/")
                {
                    path = parent.Text + path;
                    parent = parent.Parent;
                }
                path += tn.Text;
                query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND type = " + 10 + " AND (message = '" + path + "' OR message = '" + "/"+path + "')";
            }
            List<Messages> mes = session.getMessages(query);

            foreach (Messages message in mes)
            {
                if (String.IsNullOrWhiteSpace(message.User) || message.User.StartsWith("../"))
                    continue;
                TreeNode node = new TreeNode(message.User);
                tn.Nodes.Add(node);
                if (message.User.EndsWith("/"))
                {
                    addToTV(ref node);
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            if (listBox1.SelectedIndex < 0)
                return;
            string query = "SELECT * FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND (type = " + 9 + " OR type =" + 8 + ") AND message = '"+listBox1.Text+"'";
            List<Messages> users = session.getMessages(query);
            if (users.Count == 0)
                return;
            if (users[0].Type == 9)
                textBox1.Text = "FORM";
            else
                textBox1.Text = "Basic";
            foreach (Messages user in users)
            {
                listBox2.Items.Add(user.User.Split(new char[] {':'})[0]);
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex < 0)
                return;
            string query = "SELECT username FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND (type = " + 9 + " OR type =" + 8 + ") AND message = '" + listBox1.Text + "' AND username LIKE '"+listBox2.Text+"%'";
            List<string> users = session.getStrings(query);
            if (users.Count == 0)
                return;
            textBox2.Text = users[0].Substring(users[0].IndexOf(':')+1);
        }

        private void openURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;
            string path = "";
            TreeNode parent = treeView1.SelectedNode.Parent;
            while (parent != null)
            {
                path = parent.Text + path;
                parent = parent.Parent;
            }
            path += treeView1.SelectedNode.Text;
            path = "http://"+host + path;
            Process p = new Process();
            ProcessStartInfo psi = new ProcessStartInfo(path);
            p.StartInfo = psi;
            p.Start();
            
        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            treeView1.SelectedNode = treeView1.GetNodeAt(e.X, e.Y);
        }
    }
}
