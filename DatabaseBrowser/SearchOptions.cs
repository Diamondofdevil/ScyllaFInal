using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DatabaseBrowser
{
    public partial class SearchOptions : Form
    {
        private static readonly int TEXT_DATA_WORDS_DB_SEARCH_ID = 1;

        private bool searchTableNames;

        public bool SearchTableNames
        {
            get { return searchTableNames; }
            set { searchTableNames = value; }
        }

        private bool searchTableColumns;

        public bool SearchTableColumns
        {
            get { return searchTableColumns; }
            set { searchTableColumns = value; }
        }

        private List<string> words;

        public List<string> Words
        {
            get { return words; }
            set { words = value; }
        }

        public SearchOptions()
        {
            words = new List<string>();
            InitializeComponent();
            loadWords();
        }

        private void loadWords()
        {
           checkedListBox1.Items.AddRange(DBManagement.Session.getInstance().getStrings("SELECT data from textData where id = " + TEXT_DATA_WORDS_DB_SEARCH_ID).ToArray());
           for (int i = 0; i < checkedListBox1.Items.Count; i++ )
               checkedListBox1.SetItemChecked(i, true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            words.Clear();
            SearchTableNames = checkBox1.Checked;
            SearchTableColumns = checkBox2.Checked;
            foreach (object word in checkedListBox1.CheckedItems)
            {
                words.Add(word.ToString());
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
