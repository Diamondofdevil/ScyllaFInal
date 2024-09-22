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
    public partial class PasswordSelectionDialog : Form
    {
        private List<List<string>> strs;
        private string connectionString;

        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }
        private DBManager.DB_TYPE type;

        public DBManager.DB_TYPE Type { 
            get { return type; } 
            set { type = value; } 
        }

        public PasswordSelectionDialog()
        {
            InitializeComponent();
        }

        public PasswordSelectionDialog(List<List<string>> strs)
        {
            // TODO: Complete member initialization
            this.strs = strs;
            InitializeComponent();
        }
    }
}
