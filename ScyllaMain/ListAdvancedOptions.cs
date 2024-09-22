using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DBManagement;

namespace Scylla
{
    public partial class ListAdvancedOptions : Form
    {
        private List<string> userList;
        private List<string> passwordList;
        private static readonly string OPTION = "<Pick an option>";
        private static readonly string USER = "User";
        private static readonly string PASS = "Password";
        private static readonly string USERPASS_SLASH = "User/Password ListType";
        private static readonly string USERPASS_SPACE = "User Password ListType";
        private static readonly string USERPASS_LINE = "User-Password ListType";
        public static readonly string FILE_NAME = "AdvancePassList.txt";

        public static readonly int TEXT_DATA_LIST_ID = 2;

        public ListAdvancedOptions()
        {
            InitializeComponent();
            userList = new List<string>();
            passwordList = new List<string>();
            loadLists();
        }

        private void loadLists()
        {
            addLists(Session.getInstance().getStrings("SELECT data from textData where id = "+TEXT_DATA_LIST_ID).ToArray());
        }

        private void aToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Title = "Dictionary lists";
            ofd.Filter = "Dictionary files(*.txt;*.dic)|*.txt;*.dic|All files (*.*)|*.*"; 
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                addLists(ofd.FileNames);
            }
        }
        private void addLists(string[] fileNames)
        {
            foreach (string fileName in fileNames)
            {
                DataGridViewComboBoxCell combo = new DataGridViewComboBoxCell();
                DataGridViewTextBoxCell text = new DataGridViewTextBoxCell();
                DataGridViewTextBoxCell textInd = new DataGridViewTextBoxCell();
                DataGridViewRow row = new DataGridViewRow();

                combo.ValueType = System.Type.GetType("System.String");
                text.Value = fileName;
                DBManagement.Session.getInstance().addTextData(TEXT_DATA_LIST_ID, fileName);
                textInd.Value = 0;

                combo.Items.Add(OPTION);
                combo.Items.Add(PASS);
                combo.Items.Add(USERPASS_SLASH);
                combo.Items.Add(USERPASS_LINE);
                combo.Items.Add(USERPASS_SPACE);
                combo.Value = PASS;

                row.Cells.Add(text);
                row.Cells.Add(textInd);
                row.Cells.Add(combo);

                dataGridView1.Rows.Add(row);
            }
        }
        public List<string> UserList
        {
            get { return userList; }
        }
        public List<string> PasswordList
        {
            get { return passwordList; }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<PasswordsLoader.PermutationOptions> perms = new List<PasswordsLoader.PermutationOptions>();
            if (checkBox1.Checked)
                perms.Add(PasswordsLoader.PermutationOptions.AsIs);
            if (checkBox2.Checked)
                perms.Add(PasswordsLoader.PermutationOptions.Reverse);
            if (checkBox3.Checked)
                perms.Add(PasswordsLoader.PermutationOptions.Double);
            if (checkBox4.Checked)
                perms.Add(PasswordsLoader.PermutationOptions.LowerCase);
            if (checkBox5.Checked)
                perms.Add(PasswordsLoader.PermutationOptions.H4x0rPerm);
            if (checkBox6.Checked)
                perms.Add(PasswordsLoader.PermutationOptions.UpperCase);
            if (checkBox7.Checked)
                perms.Add(PasswordsLoader.PermutationOptions.CasePerm);
            if (checkBox8.Checked)
                perms.Add(PasswordsLoader.PermutationOptions.TwoNumAppend);
            if (checkBox9.Checked)
                perms.Add(PasswordsLoader.PermutationOptions.PreAPEndDate);
            if (checkBox10.Checked)
                perms.Add(PasswordsLoader.PermutationOptions.H4x0r);
            //StreamWriter sw = new StreamWriter(File.Create(FILE_NAME));
            List<string> sw = new List<string>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            { 

                try
                {
                    DataGridViewComboBoxCell dgvc = (DataGridViewComboBoxCell) row.Cells[2];
                    PasswordsLoader.PassTypeOptions passOpt = (PasswordsLoader.PassTypeOptions) (dgvc.Items.IndexOf(dgvc.Value))-1;
                    if ((int)passOpt <0 || (int)passOpt > 4)
                        throw new Exception("Please select a data type");
                    PasswordsLoader.generarPermutacioens(PasswordsLoader.loadFile((string)row.Cells[0].Value, (int)row.Cells[1].Value, passOpt), perms.ToArray(), ref sw);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
            }
            for (int i = 0; i < sw.Count; i++)
                for (int j = i + 1; j < sw.Count; j++)
                {
                    if (sw[i] == sw[j])
                        sw.RemoveAt(i);
                }
            //i know i know, but is for u to have the list if you wanna use it again :)
            File.WriteAllLines(FILE_NAME, sw.ToArray());
            
            this.Close();
        }

        private void ddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count < 0)
                return;
            List<DataGridViewRow> selectedRows = new List<DataGridViewRow>();

            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                if (!selectedRows.Contains(cell.OwningRow))
                    selectedRows.Add(cell.OwningRow);
            }
            foreach (DataGridViewRow row in selectedRows)
            {
                string fileName = (string)row.Cells[0].Value;
                dataGridView1.Rows.Remove(row);
                DBManagement.Session.getInstance().delTextData(TEXT_DATA_LIST_ID, fileName);
            }
        }
    }
}
