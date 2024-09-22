using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DatabaseBrowser
{
    public partial class ScyllaDBBrowser : Form
    {
        string dbType = "";
        string dbIp = "";
        string dbName = "";
        private DBManager dbManager;
        public ScyllaDBBrowser()
        {
            InitializeComponent();
        }
        public ScyllaDBBrowser(int sessionId)
        {
            InitializeComponent();
            /*string query = "SELECT username, pass, host_ FROM USER_PASS WHERE id = " + sessionId;
            List<List<string>> strs = DBManagement.Session.getInstance().getStringsList(query);
            if (strs.Count > 0)
            {
                PasswordSelectionDialog pwdS = new PasswordSelectionDialog(strs);
                if (pwdS.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;
                SqlConnection con = new SqlConnection(pwdS.ConnectionString);
                dbManager = DBManager.createInstance(con, pwdS.Type);
                loadDB();
            }*/
        }
        public ScyllaDBBrowser(DbConnection con, DBManager.DB_TYPE type)
        {
            InitializeComponent();
            dbManager = DBManager.createInstance(con, type);
            
            loadDB();
        }
        private void loadDB()
        {
            List<string> dbs = dbManager.getDatabases();
            dbName = dbManager.getActualDB();
            if (dbName == null)
            {
                infoStripLabel.Text = dbManager.LastError;
                return;
            }
            if (string.IsNullOrEmpty(dbName))
            {
                if(dbs.Count != 0)
                    dbName = dbs[0];
            }
            this.Text = string.Concat("Scylla Database Browser", " - ", dbType,dbIp,dbName);
            // Fill tree with database name and table names
            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();

            if (dbs != null)
                foreach (string db in dbs)
                {
                    TreeNode node = treeView1.Nodes.Add(db, db, 0, 0);
                    //you need to use a db first
                    List<string> tableNames = null;
                    if (dbManager.useDB(db) || db == dbName)
                        tableNames = dbManager.getTableNames();
                    else continue;
                    if (tableNames == null)
                    {
                        infoStripLabel.Text = dbManager.LastError;
                        continue;
                    }

                    foreach (string tableName in tableNames)
                        node.Nodes.Add(tableName, tableName, 1, 1);
                }
            else if(!string.IsNullOrEmpty(dbName))
            {
                TreeNode node = treeView1.Nodes.Add(dbName, dbName, 0, 0);
                List<string> tableNames = dbManager.getTableNames();
                foreach (string tableName in tableNames)
                    node.Nodes.Add(tableName, tableName, 1, 1);
            }
            TreeNode main = treeView1.Nodes[0];//.Add(dbName, dbName,0,0);
            main.Expand();
            treeView1.EndUpdate();
            treeView1.SelectedNode = treeView1.Nodes[0];
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            dataGrid.SuspendLayout();
            if (e.Node.Level == 0)
            {
                //TODO: Give some info of the db
            }
            else
            {
                dbManager.useDB(e.Node.Parent.Text);
                setResultSet(dbManager.getTableData(e.Node.Text),dbManager.isDataUpdate(e.Node.Text), dbManager.isTableUpdate(e.Node.Text));
            }
            dataGrid.ResumeLayout();
        }

        private void setResultSet(DbDataReader dr, bool data, bool tables)
        {
            dataGrid.Columns.Clear();
            if (dr == null)
            {
                return;
            }
            if (dr.FieldCount == 0)
            {
                dr.Close();
                if(tables)
                    loadDB();
                if (data)
                {
                    TreeNode node = treeView1.SelectedNode;  // Update data view
                    treeView1.SelectedNode = null;
                    treeView1.SelectedNode = node;
                }
                return;
            }
            DataTable schemaTable = null;
            try
            {
                schemaTable = dr.GetSchemaTable();
            }
            catch { dr.Close(); return; }
            DataTable dataTable = new DataTable();

            for (int i = 0; i < schemaTable.Rows.Count; i++)
            {

                DataGridViewTextBoxColumn tbc = new DataGridViewTextBoxColumn();
                DataRow dataRow = schemaTable.Rows[i];
                string columnName = dataRow["ColumnName"].ToString();
                tbc.HeaderText = columnName;
                dataGrid.Columns.Add(tbc);
            }
            while (dr.Read())
            {
                DataGridViewRow row = new DataGridViewRow();
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    //
                    DataGridViewTextBoxCell tbc = new DataGridViewTextBoxCell();
                    if (dr.IsDBNull(i))
                    {
                        tbc.Value = "NULL";
                    }
                    else
                    {
                        object ob = dr.GetValue(i);
                        if (ob is Array)
                        {
                            string s = string.Empty;
                            if (((Array)ob).Length > 500)
                                s = "Array to long to show here, sorry";
                            else
                            {
                                foreach (object o in (Array)ob)
                                    s += o.ToString();
                            }
                            tbc.Value = s;
                        }else
                            tbc.Value = ob;
                    }
                    row.Cells.Add(tbc);
                }
                dataGrid.Rows.Add(row);
            }
            dr.Close();
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            DateTime initTime = DateTime.Now;

            setResultSet(dbManager.execute(richTextBox1.Text.Trim()),dbManager.isDataUpdate(richTextBox1.Text.Trim()), dbManager.isTableUpdate(richTextBox1.Text.Trim()));
            listBox1.Items.Clear();
            listBox1.Items.AddRange(dbManager.SqlHistory.ToArray());
            infoStripLabel.Text = string.IsNullOrEmpty(dbManager.LastError) ? "Total time taken: " + ((long)(DateTime.Now - initTime).TotalMilliseconds).ToString() : dbManager.LastError;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save result set";
            sfd.Filter = "Text files(*.txt)|*.txt|All files (*.*)|*.*";
            sfd.AddExtension = true;
            sfd.DefaultExt = "txt";
            if (STAShowDialog(sfd) != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            StreamWriter sw = new StreamWriter(sfd.FileName);
            sw.WriteLine("#Generated by Scylla");
            sw.WriteLine(richTextBox1.Text);
            StringBuilder line = new StringBuilder();
            foreach (DataGridViewColumn col in dataGrid.Columns)
            {
                line.Append(col.HeaderText + ",");
            }
            line.Length -= 1;
            sw.WriteLine(line);
            line.Length = 0;
            foreach (DataGridViewRow row in dataGrid.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    line.Append(cell.Value.ToString() + ",");
                }

                line.Length -= 1;
                sw.WriteLine(line);
                line.Length = 0;
            }
            sw.Close();
            infoStripLabel.Text = "CSV export done";
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyData == Keys.F5 && richTextBox1.Focused) || (e.Control && (e.KeyData == Keys.Enter || e.KeyData == Keys.Return)))
                toolStripButton1.PerformClick();
        }
        public static DialogResult STAShowDialog(FileDialog dialog)
        {
            DialogState state = new DialogState();
            state.dialog = dialog;
            System.Threading.Thread t = new System.Threading.Thread(state.ThreadProcShowDialog);
            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.Start();
            t.Join();
            return state.result;
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveResultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton2.PerformClick();
        }

        private void openDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DBOpen dbo = new DBOpen();
            if (dbo.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            dbManager = DBManager.createInstance(dbo.Connection, (DatabaseBrowser.DBManager.DB_TYPE) dbo.Type);
            loadDB();
        }

        private void executeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton1.PerformClick();
        }


        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
                return;
            tabControl1.SelectedIndex = 0;
            richTextBox1.Text = listBox1.SelectedItem.ToString();
        }
        private struct FindItems
        {
            public List<string> words;
            public bool searchInColums;
            public bool searchInTables;
        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (dbManager == null)
            {
                infoStripLabel.Text = "Please connect first to a database";
                return;
            }
            SearchOptions so = new SearchOptions();
            if (so.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            if (so.SearchTableColumns || so.SearchTableNames)
            {
                
                dataGrid.Columns.Clear();
                treeView1.Enabled = false;
                DataGridViewTextBoxColumn tbC = new DataGridViewTextBoxColumn();
                tbC.HeaderText = "Table";
                DataGridViewTextBoxColumn tbC1 = new DataGridViewTextBoxColumn();
                tbC1.HeaderText = "Column";
                dataGrid.Columns.Add(tbC);
                dataGrid.Columns.Add(tbC1);

                FindItems fi = new FindItems();
                fi.words = so.Words;
                fi.searchInTables = so.SearchTableNames; fi.searchInColums = so.SearchTableColumns;
                Thread t = new Thread(new ParameterizedThreadStart(startFind));
                t.Start(fi);
                
            }
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Index == 0)
                loadDB();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TODO: block treeview
            if (dataGrid.SelectedCells.Count < 0)
                return;
            int row = dataGrid.SelectedCells[0].RowIndex;
            StringBuilder sb = new StringBuilder();
            dataGrid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            Clipboard.SetDataObject(this.dataGrid.GetClipboardContent());
            //Clipboard.SetText(sb.ToString());
        }
        private delegate void updateTreeDelegate(DataGridViewRow results);
        private delegate void SetTextCallback(string s);
        private delegate void reEnableTreeView();
        private void startFind(object o)
        {
            FindItems so = (FindItems)o;
            List<string> results = findInDB(so.words, so.searchInTables, so.searchInColums);
            foreach (string str in results)
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell tbc = new DataGridViewTextBoxCell();
                string[] vals = str.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                tbc.Value = vals[0];
                row.Cells.Add(tbc);
                if (vals.Length > 1)
                {
                    DataGridViewTextBoxCell tbc1 = new DataGridViewTextBoxCell();
                    tbc1.Value = vals[1];
                    row.Cells.Add(tbc1);
                }
                updateTree(row);
            }
            reEnableTV();
        }

        private void updateTree(DataGridViewRow results)
        {

            if (dataGrid.InvokeRequired)
            {
                updateTreeDelegate d = new updateTreeDelegate(updateTree);
                this.Invoke(d, results);
            }
            else
            {
                dataGrid.Rows.Add(results);
            }
        }
        private void reEnableTV()
        {
            if (treeView1.InvokeRequired)
            {
                reEnableTreeView d = new reEnableTreeView(reEnableTV);
                this.Invoke(d);
            }
            else
            {
                treeView1.Enabled = true;
            }
        }
        private List<string> findInDB(List<string> words, bool searchInTables, bool searchInCols)
        {
            List<string> dbs = dbManager.getDatabases();
            List<string> results = new List<string>();
            int pos = 0;
            if (dbs != null)
                foreach (string db in dbs)
                {
                    List<string> tableNames = null;
                    if (dbManager.useDB(db) || db == dbName)
                        tableNames = dbManager.getTableNames();
                    else continue;
                    if (tableNames == null)
                    {
                        continue;
                    }
                    foreach (string tableName in tableNames)
                    {
                        pos++;
                        setLabelText("Processing " + pos + " Items");
                        if (searchInTables)
                            foreach (string w in words)
                            {
                                if (tableName.ToLower().Contains(w.ToLower()))
                                    results.Add(tableName);
                            }
                        if (searchInCols)
                            results.AddRange(findInTable(words, tableName));
                    }
                }
            else if (!string.IsNullOrEmpty(dbName))
            {
                
                List<string> tableNames = dbManager.getTableNames();
                foreach (string tableName in tableNames)
                {
                    if (searchInTables)
                        foreach (string w in words)
                        {
                            if (tableName.ToLower().Contains(w.ToLower()))
                                results.Add(tableName);
                        }
                    if (searchInCols)
                        results.AddRange(findInTable(words, tableName));
                }
            }
            return results;
        }
        
        private void setLabelText(string p)
        {
            if (this.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(setLabelText);
                this.Invoke(d, p);
            }
            else
            {
                infoStripLabel.Text = p;

            }
        }
        private List<string> findInTable(List<string> words, string tableName)
        {
            List<string> results = new List<string>();
            
            DbDataReader dr = dbManager.getTableData(tableName);// + " LIMIT 1"); yes, there should be a limit but...

            if (dr == null)
                return results;
            if (dr.FieldCount == 0)
            {
                dr.Close();
                return results;
            }
            DataTable schemaTable = null;
            try
            {
                schemaTable = dr.GetSchemaTable();
            }
            catch { dr.Close(); return results; }

            for (int i = 0; i < schemaTable.Rows.Count; i++)
            {
                DataRow dataRow = schemaTable.Rows[i];
                foreach (string w in words)
                {
                    if (dataRow["ColumnName"].ToString().ToLower().Contains(w.ToLower()))
                    {
                        results.Add(tableName + ":" + dataRow["ColumnName"].ToString());
                    }
                }
            }
            dr.Close();
            return results;
        }
    }
    public class DialogState
    {

        public DialogResult result;
        public FileDialog dialog;
        public void ThreadProcShowDialog()
        {
            result = dialog.ShowDialog();
        }
    }
}
