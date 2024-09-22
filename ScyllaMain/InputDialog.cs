using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Scylla
{
    public partial class InputDialog : Form
    {
        private string text;

        public string GetText
        {
            get { return text; }
            set { text = value; }
        }
        public InputDialog()
        {
            InitializeComponent();
        }
        public InputDialog(string title, string label, string txtBox, string butCancel, string butAccept)
        {
            InitializeComponent();
            this.Text = title;
            label1.Text = label;
            textBox1.Text = txtBox;
            button1.Text = butAccept;
            button2.Text = butCancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            GetText = textBox1.Text;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
