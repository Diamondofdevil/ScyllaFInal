using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Xml;

namespace Scylla
{
    public partial class NmapWrapper : Form
    {
        public static readonly string outputPath = Environment.CurrentDirectory + "\\nmapRes.xml";
        private string fullPath;
        List<HostStruct> info;

        public List<HostStruct> Info
        {
            get { return info; }
            set { info = value; }
        }
        public NmapWrapper()
        {
            InitializeComponent();
            fullPath = string.Empty;
            info = new List<HostStruct>();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            panel1.Enabled = !checkBox4.Checked;
            textBox3.Enabled = checkBox4.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            button1.Enabled = textBox1.Enabled = checkBox1.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Filter = "Executables|*.exe|All Files |*.*";
            ofd.Title = @"Open Nmap executable";
            if (ofd.ShowDialog() == DialogResult.OK)
                textBox1.Text = ofd.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string parameters = string.Empty;
            if (checkBox4.Checked)
                parameters = textBox3.Text;
            else
            {
                if (radioButton1.Checked)
                    parameters += "-sS";
                else
                    parameters += "-sT";
                if (checkBox2.Checked)
                    parameters += " -sU";
                if (checkBox3.Checked)
                    parameters += " -A";
                parameters += " -p" + ((int)numericUpDown2.Value).ToString() + "-" + ((int)numericUpDown1.Value).ToString();
                parameters += " " + textBox2.Text;
            }
            parameters += " -oX "+ (fullPath = outputPath);

            Process p = new Process();
            ProcessStartInfo ps = new ProcessStartInfo();

            if (checkBox1.Checked)
            {
                ps.FileName = textBox1.Text;
            }
            else
                ps.FileName = "nmap.exe";
            outputRtb.Text = ps.FileName + " " +parameters;
            
            ps.Arguments = parameters;
            ps.CreateNoWindow = true;
            ps.UseShellExecute = false;
            ps.RedirectStandardError = true;
            ps.RedirectStandardOutput = true;
            p.StartInfo = ps;
            p.ErrorDataReceived += build_ErrorDataReceived;
            p.OutputDataReceived += build_ErrorDataReceived;
            p.EnableRaisingEvents = true;
            try
            {
                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                p.Exited += p_Exited;
            }
            catch { MessageBox.Show("Nmap cannot be initialized", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            groupBox3.Enabled = groupBox2.Enabled = false;
            //p.WaitForExit();
            //readScann();
            //this.Close();
        }
        void p_Exited(object sender, EventArgs e)
        {
            readScann();
            if (info.Count == 0 || info[0].Services == null || info[0].Services.Count == 0)
            {
                info = new List<HostStruct>();
                MessageBox.Show("Nmap completed with errors, can't get services. Please review the output and try again","Nmap Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                reEnableGroupBoxes();
                return;
            }
            setDialogResults();
        }
        delegate void CloseCallback();
        private void setDialogResults()
        {
            if (this.InvokeRequired)
            {
                CloseCallback d = new CloseCallback(setDialogResults);
                this.Invoke(d);
            }
            else
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }
        private void reEnableGroupBoxes()
        {
            if (groupBox3.InvokeRequired)
            {
                CloseCallback d = new CloseCallback(reEnableGroupBoxes);
                groupBox3.Invoke(d);
            }
            else
            {
                groupBox3.Enabled = groupBox2.Enabled = true;
            }
        }
        delegate void SetTextCallback(string text);
        void build_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                appendOutput(e.Data);
            }
                //does it matter to print what's next when the object is disposed? :P
            catch { }
        }
        private void appendOutput(string strMessage)
        {
            if (!String.IsNullOrWhiteSpace(strMessage))
            {
                if (outputRtb.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(appendOutput);
                    this.Invoke(d, strMessage );
                }
                else
                {
                    outputRtb.Text += strMessage + Environment.NewLine;
                }
            }
        }
        private void readScann()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fullPath);
            XmlNodeList run = doc.SelectNodes("nmaprun");//GetElementById("nmap").GetElementsByTagName("");
            if (run.Count == 0)
                return;
            XmlNodeList hosts = run[0].SelectNodes("host");
            foreach (XmlNode host in hosts)
            {
                HostStruct tmp = new HostStruct();
                tmp.Services = new List<serviceStruct>();

                if (!host.HasChildNodes)
                    return;
                XmlNode hostsInfo = host.SelectSingleNode("hostnames");
                XmlNodeList hostNames = hostsInfo.SelectNodes("hostname");
                if (hostNames.Count > 0)
                    tmp.Host = hostNames[0].Attributes["name"].Value;
                /*foreach (XmlNode hostname in hostNames)
                {
                    Debug.WriteLine(hostname.Attributes["name"].Value);
                }*/
                
                XmlNodeList addresses = host.SelectNodes("address");
                foreach (XmlNode address in addresses)
                {
                    if (address.Attributes["addrtype"].Value == "ipv4")
                    {
                        tmp.Ip = address.Attributes["addr"].Value;
                        break;
                    }
                }

                XmlNode portInfo = host.SelectSingleNode("ports");
                if (portInfo == null)
                    continue;
                XmlNodeList ports = portInfo.SelectNodes("port");
                foreach (XmlNode port in ports)
                {
                    //port->service:name
                    XmlNode service = port.SelectSingleNode("service");
                    if (service.Attributes.Count == 0)
                        continue;
                    foreach (var value in Enum.GetValues(typeof(serviceType)))
                    {
                        string serviceName = service.Attributes["name"].Value.Replace("-", "");
                        if(ScyllaGUI.ServiceDictionary.ContainsKey(serviceName))
                        //if (serviceName == ((serviceType)value).ToString())
                        {
                            //ftp/*any*/, telnet, telnets, ssh, pop3, pop3s, smtp, smtps, smb/*445*/, mssqls/*remember replace "-" for ""*/, http, https, imap, imaps, ldaps, mysql, oracle, potgres, dns, ibmdb2
                            serviceStruct ss = new serviceStruct();
                            int.TryParse(port.Attributes["portid"].Value, out ss.port);
                            ss.portType = ScyllaGUI.ServiceDictionary[serviceName];
                            tmp.Services.Add(ss);
                            break;
                        }
                    }
                }
                info.Add(tmp);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Title = "Nmap XML file";
            ofd.Filter = "Nmap xml files(*.xml)|*.xml|All files (*.*)|*.*"; 
            if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            textBox5.Text = ofd.FileName;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(fullPath = textBox5.Text))
                return;
            p_Exited(null,new EventArgs());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            info.Clear();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
    public enum serviceType
    {
        imap, imaps, ldaps, mysql, oracle, potgres, dns, ibmdb2
    }
    public struct serviceStruct
    {
        public string portType;
        public int port;
        public override string ToString()
        {
            return portType;
        }
    }
    public class HostStruct
    {
        private string host;

        public string Host
        {
            get { return host; }
            set { host = value; }
        }
        private string ip;

        public string Ip
        {
            get { return ip; }
            set { ip = value; }
        }
        private List<serviceStruct> services;

        public List<serviceStruct> Services
        {
            get { return services; }
            set { services = value; }
        }
    }
}
