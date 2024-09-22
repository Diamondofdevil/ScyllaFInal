using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DBManagement
{
    public partial class ListBoxForm : Form
    {
        public ListBoxForm()
        {
            InitializeComponent();
        }
        public ListBoxForm(string title, string text)
        {
            InitializeComponent();
            this.Text = title;
            label1.Text = text;
        }
        public void addItem(string name)
        {
            richTextBox1.Text += name + Environment.NewLine;
        }
        public void addItem(string[] names)
        {
            foreach (object name in names)
            {
                richTextBox1.Text += name.ToString() + Environment.NewLine;
            }
        }
        public void addItem(Messages[] messages)
        {
            foreach (Messages message in messages)
            {
                richTextBox1.Text += message.Message + Environment.NewLine;
            }
        }

        public void addItemTruncated(Messages[] messages)
        {
            foreach (Messages name in messages)
            {
                richTextBox1.Text += name.Message.Substring(4) + Environment.NewLine;
            }
        }
        public void addItemTruncated(string[] messages)
        {
            foreach (string name in messages)
            {
                richTextBox1.Text += name.Substring(4) + Environment.NewLine;
            }
        }
    }
}
