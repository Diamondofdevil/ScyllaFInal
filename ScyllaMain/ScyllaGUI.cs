using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenSSL_Wrapper;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using DBManagement;
using System.IO;

namespace Scylla
{
    public partial class ScyllaGUI : Form
    {

        #region Static vars
        public const int GUI_MAX_HEIGHT = 465;
        public const int GUI_CLOSED_WIDTH = 384;
        public const int GUI_OPEN_WIDTH = 757;
        public const int GUI_PANEL_WIDTH = GUI_OPEN_WIDTH - GUI_CLOSED_WIDTH-30;
        private static string LOG_PATH = "Scylla Log-";
        #endregion

        private Session session;
        private SessionObj sessionObj;
        private string host;
        private StreamWriter errLogWritter;
        private bool stopH;
        private List<string> hostList;
        public static Dictionary<string, string> ServiceDictionary;
        private IModuleOptions mod;
        private IOnlineModule module;
        //module ID should replace moduleString
        private int module_ID;
        private string moduleString = string.Empty;

        private int port;
        //Error messages should not be displayed, just last one, for everything else there's a log
        private List<string> allMessages = new List<string>();
        private List<string> nonErrorMessages = new List<string>();
        private List<HostStruct> info;

        public ScyllaGUI()
        {
            ServiceDictionary = new Dictionary<string, string>();
            ServiceDictionary.Add("ftp", "FTP"); ServiceDictionary.Add("telnet", "Terminal"); ServiceDictionary.Add("telnets", "Terminal"); ServiceDictionary.Add("ssh", "Terminal"); ServiceDictionary.Add("pop3", "POP3");
            ServiceDictionary.Add("pop3s", "POP3"); ServiceDictionary.Add("smtp", "SMTP"); ServiceDictionary.Add("smtps", "SMTP"); ServiceDictionary.Add("smb", "SMB"); ServiceDictionary.Add("ms-sql", "MSSQL");
            ServiceDictionary.Add("dns", "DNSCacheSnooping"); ServiceDictionary.Add("http", "HTTP"); ServiceDictionary.Add("https", "HTTP"); ServiceDictionary.Add("imap", "IMAP"); ServiceDictionary.Add("imaps", "IMAPS");
            ServiceDictionary.Add("ldap", "LDAP"); ServiceDictionary.Add("ldaps", "LDAP"); ServiceDictionary.Add("mysql", "MYSQL"); ServiceDictionary.Add("oracle", "ORACLE"); ServiceDictionary.Add("postgres", "PGSQL");
            ServiceDictionary.Add("ibm-db2", "DB2");
            InitializeComponent();
            hostList = new List<string>();
            stopH = false;

            openFileDialog2.CheckFileExists = openFileDialog1.CheckFileExists = true;
            openFileDialog2.CheckPathExists = openFileDialog1.CheckPathExists = true;
            openFileDialog2.Title = openFileDialog1.Title = "Dicctionary file";
            openFileDialog2.Filter = openFileDialog1.Filter = "Dictionary files(*.txt;*.dic)|*.txt;*.dic|All files (*.*)|*.*"; 

            splitContainer1.Panel2Collapsed = true;
            initComboBox(Enum.GetValues(typeof(Scylla.OnlineModule.ModuleList)),"");

            session = Session.getInstance();
            
            List<SessionObj> sessions = session.ActualSessions();
            
            SetSessionManaget ssm = new SetSessionManaget(sessions);

            if (ssm.ShowDialog() == DialogResult.OK)
            {
                sessionObj = ssm.getSelectedSession;
                toolStripLabelSessionName.Text = sessionObj.Name;
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private void initComboBox(Array vals, string text)
        {
            comboBox1.Items.Clear();
            int index = 0;
            for (int i = 0; i< vals.Length; i++){
                var var = vals.GetValue(i);
                if (var.ToString().Equals(text))
                    index = i;
                comboBox1.Items.Add(var.ToString());
            }
            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = index;
        }
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            textBoxArchivo.Text = openFileDialog1.FileName;
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            textBoxUser.Text = openFileDialog2.FileName;
        }

        private void buttonBuscar_Click(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
                openFileDialog1.ShowDialog();
            else
            {
                ListAdvancedOptions lao = new ListAdvancedOptions();
                lao.ShowDialog();
                radioButton2.Checked = true;
                textBoxArchivo.Text = ListAdvancedOptions.FILE_NAME;
            }

        }

        private void buttonBuscarUsers_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        private void botonInicio_Click(object sender, EventArgs e)
        {
            if (botonInicio.Text.StartsWith("Stop"))
            {
                stop();
                return;
            }
            module_ID = comboBox1.SelectedIndex;
            moduleString = comboBox1.Text.ToString();
            richTextBox1.Text = string.Empty;
            nonErrorMessages.Clear();
            allMessages.Clear();
            if (errLogWritter == null)
            {
                errLogWritter = new StreamWriter(File.Open(LOG_PATH + module_ID + session.getLastSession().Name + ".log", FileMode.Append));
                errLogWritter.AutoFlush = true;
                errLogWritter.WriteLine("Aplication LOG for session " + session.getLastSession().Name);
                errLogWritter.WriteLine("This log is automatically generated by Scylla");
                errLogWritter.WriteLine("================================================");
                errLogWritter.WriteLine("Module ID: " + module_ID);
            }
            int threads = mod.getThreadNumber();
            port = mod.getPort();
            SecurityProviderProtocol sslProt = mod.getCipher();
            List<string> users = new List<string>();
            List<string> passwords = new List<string>();
            try
            {
                loadLists(ref users, ref passwords);
            }
            catch
            {
                return;
            }
            object otherOptions = mod.getOtherOptions();
            setHosts();
            List<object> parameters = new List<object>();
            parameters.Add(users); parameters.Add(passwords); parameters.Add(threads); parameters.Add(sslProt); parameters.Add(otherOptions);

            botonInicio.Text = "Stop!";
            Thread t = new Thread(new ParameterizedThreadStart(init));
            t.Start(parameters);
        }
        private void moduleInit(int threads, List<string> users, List<string> passwords,SecurityProviderProtocol sslProt, object otherOptions)
        {

            switch (moduleString)
            {
                case "FTP":

                    module = new ModuleFTP(threads, users, passwords, host, this, sslProt, port, otherOptions);
                    break;
                case "Terminal":
                    if (sslProt != SecurityProviderProtocol.PROT_OTHER)
                        module = new ModuleTELNET(threads, users, passwords, host, this, sslProt, port, otherOptions);
                    else
                        module = new ModuleSSH(threads, users, passwords, host, this, sslProt, port, otherOptions);
                    break;
                case "POP3":
                    module = new ModulePOP3(threads, users, passwords, host, this, sslProt, port, otherOptions);
                    break;
                case "SMTP":
                    module = new ModuleSMTP(threads, users, passwords, host, this, sslProt, port, otherOptions);
                    break;
                case "SMB":
                    module = new FastSMB(threads, users, passwords, host, this, sslProt, port, otherOptions);
                    break;
                case "HTTP":
                    if (((HTTPOptions)otherOptions).attackType == 0L)
                    {
                        module = new ModuleHTTPBasic(threads, users, passwords, host, this, sslProt, port, otherOptions);
                        break;
                    }
                    if (((HTTPOptions)otherOptions).attackType == 2L)
                    {
                        module = new ModuleHTTPDir(threads, users, passwords, host, this, sslProt, port, otherOptions);
                        break;
                    }
                    module = new ModuleHTTPForm(threads, users, passwords, host, this, sslProt, port, otherOptions);
                    break;
                case "MSSQL":
                    if (((MSSQLOptions)otherOptions).fastMSSQL)
                        module = new FastMSSQL(threads, users, passwords, host, this, sslProt, port, otherOptions);
                    else
                        module = new ModuleMSSQL(threads, users, passwords, host, this, sslProt, port, otherOptions);
                    break;
                case "IMAP":
                    module = new ModuleIMAP(threads, users, passwords, host, this, sslProt, port, otherOptions);
                    break;
                case "MYSQL":
                    module = new ModuleMySQUAL(threads, users, passwords, host, this, sslProt, port, otherOptions);
                    break;
                case "ORACLE":
                    module = new ModuleOracle(threads, users, passwords, host, this, sslProt, port, otherOptions);
                    break;
                case "DB2":
                    module = new ModuleDB2(threads, users, passwords, host, this, SecurityProviderProtocol.PROT_NO_SSL, port, otherOptions);
                    break;
                case "LDAP":
                    module = new ModuleLDAP(threads, users, passwords, host, this, sslProt, port, otherOptions);
                    break;
                case "PGSQL":
                    module = new ModulePostgres(threads, users, passwords, host, this, sslProt, port, otherOptions);
                    break;
                case "DNSCacheSnooping":
                    module = new ModuleDNSCacheSnoop(threads, users, passwords, host, this, sslProt, port, otherOptions);
                    break;
                default:
                    throw new NotImplementedException();
            }
            session.addModule(sessionObj.Id, module_ID);
        }
        
        public void init(object parameters)
        {
            //TODO: multithread
            System.Threading.Tasks.ParallelOptions opts = new System.Threading.Tasks.ParallelOptions();
            opts.MaxDegreeOfParallelism = hostList.Count > (int)numericUpDown1.Value ? (int)numericUpDown1.Value : hostList.Count;
            System.Threading.Tasks.Parallel.ForEach<string>(hostList,opts, h =>
            {
                if (!stopH)
                {
                    host = h;
                    //lol
                    moduleInit((int)((List<object>)parameters)[2], (List<string>)((List<object>)parameters)[0], (List<string>)((List<object>)parameters)[1], (SecurityProviderProtocol)((List<object>)parameters)[3], ((List<object>)parameters)[4]);
                    module.init();
                }
            });
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            errLogWritter.Close();
            setButtonText("Start");
            setTimers(0, 0, 0, 0);
            stopH = false;
        }
        delegate void SetTextCallback(string text);
        public void setButtonText(string txt)
        {
            if (botonInicio.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(setButtonText);
                this.Invoke(d, new object[] { txt });
            }
            else
            {
                botonInicio.Text = txt;
            }
        }
        public void stop()
        {
            stopH = true;
            module.stop();
        }
        private void loadLists(ref List<string> users, ref List<string> passwords)
        {
            if (checkBoxUserList.Checked)
            {
                users = PasswordsLoader.loadFile(textBoxUser.Text);
            }
            else
            {
                if (!String.IsNullOrEmpty(textBoxUser.Text))
                    users.Add(textBoxUser.Text);
            }
           
            if ( radioButton2.Checked)
            {
                passwords = PasswordsLoader.loadFile(textBoxArchivo.Text);
                if (passwords.Count == 0)
                {
                    MessageBox.Show("Password list cannot be opened or is empty", "File load fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new Exception();
                }
            }
            else if (radioButton1.Checked)
            {
                if (!String.IsNullOrEmpty(textBoxArchivo.Text))
                    passwords.Add(textBoxArchivo.Text);
            }
            else { }
                mod.onListsLoad(ref users, ref passwords);
                //PasswordsLoader.generarPermutacioens(ref users, PasswordsLoader.PermutationOptions.H4x0r);
        }
        private void checkBoxUserList_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUserList.Checked == true)
            {
                label5.Text = "WordList User";
                buttonBuscarUsers.Visible = true;
            }
            else
            {
                
                label5.Text = "User";
                buttonBuscarUsers.Visible = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBoxArchivo.Visible = true;
            buttonBuscar.Visible = true;
            if (radioButton2.Checked)
            {
                buttonBuscar.Text = "search";
                buttonBuscar.Size = new Size(51, 20);
                buttonBuscar.Location = new Point(263, 68);
                return;
            }
            if (radioButton3.Checked)
            {
                textBoxArchivo.Visible = false;
                buttonBuscar.Text = "Set Advanced Options";
                buttonBuscar.Size = new Size(150, 20);
                buttonBuscar.Location = new Point(128, 68);
                buttonBuscar.ResumeLayout();
                
                return;
            }
            if (radioButton1.Checked)
            {
                buttonBuscar.Location = new Point(263, 68);
                buttonBuscar.Size = new Size(51, 20);
                textBoxArchivo.Text = "";
                buttonBuscar.Visible = false;
            }
        }
        
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            splitContainer1.Panel2.Controls.Clear();
            switch (comboBox1.Text.ToString())
            {
                case "FTP":
                    splitContainer1.Panel2.Controls.Add((mod = FTPOptionPanel.getPanelSingleton()));
                    break;
                case "Terminal":
                    splitContainer1.Panel2.Controls.Add((mod = TerminalOptionPanel.getPanelSingleton()));
                    break;
                case "POP3":
                    splitContainer1.Panel2.Controls.Add((mod = POP3OptionPanel.getPanelSingleton()));
                    break;
                case "SMTP":
                    splitContainer1.Panel2.Controls.Add((mod = SMTPOptionPanel.getPanelSingleton()));
                    break;
                case "SMB":
                    splitContainer1.Panel2.Controls.Add((mod = SMBOptionPanel.getPanelSingleton()));
                    break;
                case "HTTP":
                    splitContainer1.Panel2.Controls.Add((mod = HTTPOptionPanel.getPanelSingleton()));
                    break;
                case "MSSQL":
                    splitContainer1.Panel2.Controls.Add((mod = MSSQLOptionPanel.getPanelSingleton()));
                    break;
                case "IMAP":
                    splitContainer1.Panel2.Controls.Add((mod = IMAPOptionPanel.getPanelSingleton()));
                    break;
                case "ORACLE":
                    splitContainer1.Panel2.Controls.Add((mod = OracleOptionPanel.getPanelSingleton()));
                    break;
                case "DB2":
                    splitContainer1.Panel2.Controls.Add((mod = DB2OptionPanel.getPanelSingleton()));
                    break;
                case "MYSQL":
                    splitContainer1.Panel2.Controls.Add((mod = MySQLOptionPanel.getPanelSingleton()));
                    break;
                case "PGSQL":
                    splitContainer1.Panel2.Controls.Add((mod = PostgresOptionPanel.getPanelSingleton()));
                    break;
                case "LDAP":
                    splitContainer1.Panel2.Controls.Add((mod = LDAPOptionPanel.getPanelSingleton()));
                    break;
                case "DNSCacheSnooping":
                    splitContainer1.Panel2.Controls.Add((mod = DNSCacheSnoopPanel.getPanelSingleton()));
                    break;
            }
            if (mod != null)
            {
                mod.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;
            if(splitContainer1.Panel2Collapsed){
                this.Size = new Size(GUI_CLOSED_WIDTH, GUI_MAX_HEIGHT+72);
                this.splitContainer1.Size = new Size(GUI_CLOSED_WIDTH-20, GUI_MAX_HEIGHT);
                this.buttonCollapse.Text = ">";
                this.splitContainer1.SplitterDistance = GUI_CLOSED_WIDTH;
                return;
            }
            this.Size = new Size(GUI_OPEN_WIDTH, GUI_MAX_HEIGHT+72);
            this.splitContainer1.Size = new Size(GUI_OPEN_WIDTH-20, GUI_MAX_HEIGHT);
            this.buttonCollapse.Text = "<";
            this.splitContainer1.SplitterDistance = GUI_CLOSED_WIDTH - 20;
        }
        delegate void SetText2Callback(string text, bool error);
        
        internal void addMessage(string message, bool error)
        {
            if (this.richTextBox1.InvokeRequired)
            {
                SetText2Callback d = new SetText2Callback(addMessage);
                this.Invoke(d, new object[] { message, error });
            }
            else
            {
                try
                {
                    //sometimes there's a message to the GUI before constructed, httpDir case
                    errLogWritter.WriteLine(message);
                }
                catch { }
                if (!error)
                    nonErrorMessages.Add(message);
                allMessages.Add(message);
                if (error && !checkBox1.Checked)
                    return;
                this.richTextBox1.Text += message + Environment.NewLine;
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.SelectionLength = 0;
                richTextBox1.SelectionColor = richTextBox1.ForeColor;
                richTextBox1.ScrollToCaret();
                //this.richTextBox1.ScrollToCaret();
            }
        }

        delegate void SetTimerCallback(int pmin, int toGo, int threads, int average);
        internal void setTimers(int pmin, int toGo, int threads, int average)
        {
            if (this.labelTS.InvokeRequired)
            {
                SetTimerCallback d = new SetTimerCallback(setTimers);
                try
                {
                    this.Invoke(d, pmin, toGo, threads, average);
                }
                catch { }
            }
            else
            {
                this.labelTS.Text = "tries/Sec: ("+ pmin.ToString() + ") "+average+" - Left:  "+toGo.ToString();
                this.labelThreads.Text = threads.ToString();
                //this.labelTot.Text = toGo.ToString();
            }
        }
        private void setHosts()
        {
            hostList.Clear();

            if (radioButton5.Checked)
            {
                hostList.Add(textBoxHostname.Text);
                return;
            }
            
            
            textBoxHostname.Text = host;
            StreamReader sr = new StreamReader(File.OpenRead(host));
            string nh = string.Empty;
            while ((nh = sr.ReadLine()) != null)
            {
                if (nh.Trim().StartsWith("#") || string.IsNullOrWhiteSpace(nh.Trim()))
                    continue;
                hostList.Add(nh);
            }
            sr.Close();
        }
        internal void setActualHost(string nHost)
        {
            if (textBoxHostname.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(setActualHost);
                this.Invoke(d, nHost);
                
            }else
                textBoxHostname.Text = host = nHost;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            AboutScylla about = new AboutScylla();
            about.Show();
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ReportViewer.Form1 f = new ReportViewer.Form1(sessionObj.Id);
            f.Show();
        }

        internal void addToDBFound(int type, string username, string message)
        {
            session.addUserPass(sessionObj.Id, host, type, username, message, module_ID);
        }

        internal void addToDB(int type, string username, string message, bool truncated)
        {
            if(truncated)
                session.addMessagePossibleTruncation(message, sessionObj.Id, host, module_ID, type, username);
            else
                session.addMessage(message, sessionObj.Id, host, module_ID, type, username);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Title = "Host list";
            ofd.Filter = "Dictionary files(*.txt;*.dic)|*.txt;*.dic|All files (*.*)|*.*"; 
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxHostname.Text = host = ofd.FileName;
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            hostList.Clear();
            label4.Visible = numericUpDown1.Visible = button1.Visible = !radioButton5.Checked;
            //radioButton5.Checked;
        }

        private void textBoxHostname_Leave(object sender, EventArgs e)
        {
            string actualText = comboBox1.Text;
            if (info != null && info.Count > 0)
            {
                foreach (HostStruct hs in info)
                {
                    if (this.textBoxHostname.Text.Trim().Equals(hs.Ip) || this.textBoxHostname.Text.Trim().Equals(hs.Host))
                    {
                        initComboBox(hs.Services.ToArray(),actualText);
                        return;
                    }
                }
            }
            initComboBox(Enum.GetValues(typeof(Scylla.OnlineModule.ModuleList)), actualText);
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            richTextBox1.Text = string.Empty;
            if (checkBox1.Checked)
            {
                foreach (string str in allMessages)
                    richTextBox1.Text += str + Environment.NewLine;
                return;
            }
            foreach (string str in nonErrorMessages)
                richTextBox1.Text += str + Environment.NewLine;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            AboutScylla about = new AboutScylla();
            about.Show();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            DatabaseBrowser.ScyllaDBBrowser dbb = new DatabaseBrowser.ScyllaDBBrowser();
            dbb.Show();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            ReportViewer.Form1 f = new ReportViewer.Form1(sessionObj.Id);
            f.Show();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            List<SessionObj> sessions = session.ActualSessions();

            SetSessionManaget ssm = new SetSessionManaget(sessions);
            if (ssm.ShowDialog() == DialogResult.OK)
            {
                sessionObj = ssm.getSelectedSession;
                toolStripLabelSessionName.Text = sessionObj.Name;
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Sorry, no option panel yet, but it would be done soon :)", "Sorry", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            string actualText = comboBox1.Text;
            NmapWrapper nmap = new NmapWrapper();
            if (nmap.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;
            info = nmap.Info;
            if (info != null && info.Count > 0)
            {
                foreach (HostStruct hs in info)
                {
                    if (this.textBoxHostname.Text.Contains(hs.Ip) || (hs.Host != null && this.textBoxHostname.Text.Contains(hs.Host)))
                    {
                        initComboBox(hs.Services.ToArray(), actualText);
                        return;
                    }
                }
            }
            initComboBox(Enum.GetValues(typeof(Scylla.OnlineModule.ModuleList)), actualText);
        }
    }
}
