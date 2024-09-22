using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using OpenSSL_Wrapper;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace Scylla
{
    public abstract class OnlineModule : Module
    {
        private static readonly int RECEIVE_TIMEOUT = 40000;
        /// <summary>
        /// passwordSpinLock to get each password
        /// </summary>
        SpinLock passwordLock = new SpinLock(false);
        /// <summary>
        /// sslSpinLock to init sockets
        /// </summary>
        SpinLock sslLock = new SpinLock(false);
        /// <summary>
        /// Socket spinLock to get sockets :P
        /// </summary>
        SpinLock socketLock = new SpinLock(false);
        //better to init now
        MemoryStream ms = new MemoryStream();
        /// <summary>
        /// Just in case the socket need to send some info -- not used
        /// </summary>
        protected Object misc = null;
        
        /// <summary>
        /// EndPoint direction to connect
        /// </summary>
        protected IPEndPoint ep;
        /// <summary>
        /// Queue for implementing the socket Pool
        /// </summary>
        protected Queue<Socket> sockPool;
        /// <summary>
        /// if is using ssl ...
        /// </summary>
        protected SecurityProviderProtocol usingSSL;
        /// <summary>
        /// reference to an OpenSSL objecto, have it loaded just in case...
        /// </summary>
        private OpenSSL ossl;
        /// <summary>
        /// Determines if use gzip compresion
        /// </summary>
        private bool gzip;

        protected int MULTIPLIER = 2;

        protected static readonly int ERROR = 1;
        protected static readonly int FOUND = 0;
        protected static readonly int NO_DB_MESSAGE = -1;
        protected static readonly int FOUND_NO_NEXT = -2;
        protected static readonly int TRUNCATED_MESSAGES = 50;
        
        public enum ModuleList
        {
            FTP = 0,
            Terminal = 1,
            POP3 = 2,
            SMTP = 3,
            SMB = 4,
            HTTP = 5,
            IMAP = 6,
            LDAP = 7,
            MSSQL = 8,
            MYSQL = 9,
            ORACLE = 10,
            DB2 = 11,
            PGSQL = 12,
            DNSCacheSnooping = 13,
        };
        
        protected abstract bool connect(ref Socket conn);
        public abstract bool testPassword(String user, String password, ref object misc);
        protected abstract void tryHacks(String user, String password, Object misc = null);
        public abstract override bool preProcessing();
        #region CONTRUCTORES
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        /// <param name="threads">numero de threads a usar</param>
        /// <param name="users">lista de user names</param>
        /// <param name="passes">lista de passwords</param>
        /// <param name="gui">referencia a la GUI</param>
        /// <param name="sslProto">usa SSL?</param>
        public OnlineModule(int threads, List<string> users, List<string> passes, ScyllaGUI gui, SecurityProviderProtocol sslProto, bool gzip) : this(threads,users,passes,gui,sslProto)
        {
            this.gzip = gzip;
        }
        public OnlineModule(int threads, List<string> users, List<string> passes, ScyllaGUI gui, SecurityProviderProtocol sslProto) : base(threads, passes,gui,users)
        {
            
            gzip = false;
            usingSSL = sslProto;
            die = false;
            sockPool = new Queue<Socket>();
            /*for (int i = 0; i < threads + 5; i++)
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.TypeOfService, 0x10);
                socket.LingerState = new LingerOption(false, 0);
                socket.NoDelay = true;
                socket.ReceiveTimeout = RECEIVE_TIMEOUT;
                sockPool.Enqueue(socket);
            }*/
        }

        #endregion
        #region GET_ADDRESS
        /// <summary>
        /// gets the IPEndPoint from a Host host in the Port port
        /// </summary>
        /// <pre>if host is not valid, throws an exception</pre>
        /// <post>returns an end point resolving the host address</post>
        /// <param name="host">target host</param>
        /// <param name="port">target port</param>
        /// <returns>an IPEndPoint from a Host host in the Port port</returns>
        public IPEndPoint getEndPoint(string host, int port)
        {
            return getEndPoint(host + ":" + port.ToString());
        }
        /// <summary>
        /// gets the IPEndPoint from a Host host in the Port port
        /// </summary>
        /// <pre>host MUST BE in the format host:port if host is not valid, throws an exception</pre>
        /// <post>returns an end point resolving the host address</post>
        /// <param name="host">target host</param>
        /// <returns>an IPEndPoint from a Host host in the Port port</returns>
        public IPEndPoint getEndPoint(string host)
        {
            int port = 1;
            if (!host.Contains(":"))
            {
                throw new ArgumentException("Unable to parse host: " + host);
            }
            string[] split = host.Split(new char[] { ':' });
            if (!Int32.TryParse(split[1], out port))
            {
                throw new ArgumentException("Unable to parse host: " + host);
            }
            host = split[0];

            if (host == "localhost")
                return new IPEndPoint(new IPAddress(IPAddress.Parse("127.0.0.1").GetAddressBytes()), port);

            IPAddress address;
            if (!IPAddress.TryParse(host, out address))
            {
                try
                {
                    IPHostEntry host_ = Dns.GetHostEntry(host);
                    //try to get ipv4 address first, to avoid bugs in some apps like fgdump
                    foreach (IPAddress address_ in host_.AddressList)
                    {
                        if (address_.AddressFamily == AddressFamily.InterNetwork)
                        {
                            address = address_;
                            break;
                        }
                    }
                    //if no ipv4, try for any (ipv6?)
                    if(address == null)
                        address = Dns.GetHostEntry(host).AddressList[0];
                }
                catch
                {
                    gui.addMessage("Unable to resolve host name: " + host,true);
                    die = true;
                    return null;
                    //MessageBox.Show("Unable to resolve host name: " + host, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //throw new Exception("Unable to resolve host name: " + host);
                    
                }

            }
            return new IPEndPoint(address, port);

        }
        #endregion
        /// <summary>
        /// TODO: if needed, parse the password for uppercase_lowercase permutations; ( Exist a permutation p |p != null && #p =#password : permutationOfLowerCase-UpperCaseOf(password) )
        /// </summary>
        /// <param name="user">valid user name</param>
        /// <param name="password">valid password</param>
        /// <param name="misc">-- optional -- not used</param>
        public void parse(string user, string password, object misc)
        {
            try
            {
                if (testPassword(user, password, ref misc))
                {
                    Debug.WriteLine("Found pass " + password);
                    notify(password, user, FOUND);
                    tryHacks(user, password, misc);
                }
                //reset the error counter, if no exception, at least one thread could connect
                errCount = 0;
            }
            catch (Exception ex)
            {
                notify(password + ": " + ex.Message, user, ERROR);
            }
        }
        /// <summary>
        /// notifies an error, a success or a message
        /// </summary>
        /// <post>If error, manage the error, else prompt the password or message and add it to the data base</post>
        /// <param name="message">The message to prompt and to add to the database. If type == ERROR it MUST be password:message or password</param>
        /// <param name="username">the username who prompts the message</param>
        /// <param name="type">Type of the message. As an exaple, Oracle module message types are provided (note that the first 3 are obligatory</param>
        /// public enum OracleMessageType
        ///        {
        ///            //Defaults
        ///            //NO MESSAGE is stored in the DataBase
        ///            NoMessage = -1,
        ///            //Used when a user-password is found, the message MUST be the password
        ///            USER_PASSWORD_FOUND = 0,
        ///            //When an error happend
        ///            ERROR_MESSAGE = 1,
        ///           //Oracle
        ///           //username:created_time
        ///           USER = 2,
        ///           //version
        ///           VERSION = 3,
        ///           //sids
        ///           SID = 4,
        ///           //just role name
        ///           ROLE = 5,
        ///           //Profile:pwdPolicy
        ///           PASSWORD_POLICY = 6,
        ///           //username:status:profile
        ///           STATUS = 7,
        ///           //User:role
        ///           USER_ROLE = 8,
        ///           //Tables user can see
        ///           TABLES = 9,
        ///           //username:hash
        ///           HASHES = 10,
        ///           LINKS = 11,
        ///           //username:schemaname:osuser:machine:terminal:program:module:logon_time     
        ///           AUDIT_INFO = 12
        ///        }
        public void notify(String message, string username, int type)
        {
            if (type == NO_DB_MESSAGE)
            {
                gui.addMessage(username + " - " + message, false);
                return;
            }
            if (type == FOUND)
            {
                tryNextUser();
                //MESSAGE COULD BE: password or password:message
                gui.addToDBFound(type, username, message);
                gui.addMessage("Found user: " +username + " Password: " + message,false);
                return;
            }
            if (type == ERROR)
            {
                //MESSAGE MUST BE: password:message or password
                string pass_ = message.Split(new char[] { ':' })[0];
                gui.addMessage(message, true);
                manageError(ref pass_, ref username);
                return;
            }
            //used for null users
            if (type == FOUND_NO_NEXT)
            {
                gui.addToDBFound(type, username, message);
                gui.addMessage("Found user: " + username + " Password: " + message,false);
                return;
            }
            if (type >= TRUNCATED_MESSAGES)
            {
                gui.addToDB(type, username, message, true);
                gui.addMessage("With User " + username + " - " + message, false);
                return;
            }
            if (type > ERROR)
            {
                gui.addToDB(type, username, message,false);
                gui.addMessage("With User " + username + " - " + message,false);
                return;
            }
        }

        /// <summary>
        /// manage an error occured
        /// </summary>
        /// <pre>pass must not be null</pre>
        /// <post>if there are more than numThreads*2 errors, quit the program (host must be dead)</post>
        /// <param name="pass">password where the error occured</param>
        private void manageError(ref string pass, ref string user)
        {
            errCount++;
            if (errCount < numThreads*MULTIPLIER && !die)
                parse(user, pass, misc);
            else
                die = true;
            Debug.WriteLine("ERROR COUNT: " + errCount + "MAX ERR: " + numThreads * 2 + "INDEX = " + index);
        }
        /// <summary>
        /// Thread start process, get the next password and next user
        /// </summary>
        /// <post>if (indexOfPass > dictPasses.Count && indexOfUsers lessThan dictUsers.Count) change user, if index lessThan dictPasses.Count nextPassword, else die. Also if the attempt is successfull to connect, restart the error counter to 0</post>
        /// <param name="stateInfo">not used</param>
        public override void processWord(Object threadInfo)
        {
            ThreadInformation info = (ThreadInformation)(threadInfo);
            string2 userPass = new string2(true);
            bool refer = false;
            while (!die)
            {
                //prove to be faster than mutex :) and i only get a pass so it's fine to use spinLocks :P
                try
                {
                    refer = false;
                    passwordLock.Enter(ref refer);
                    getPass(ref userPass);
                }
                finally
                {
                    if(refer)
                        passwordLock.Exit(true);
                }
                if (userPass.IsNull)
                    break;
                parse(userPass.User, userPass.Pass, (this is ModuleSSH) ? info.ThreadIndex : misc);
                index++;
            }
        }
        public void addUserToList(string user)
        {
            m.WaitOne();
            
            if (!users.Contains(user))
            {
                users.Enqueue(user);              
            }
            m.ReleaseMutex();
        }
        public void addPasswordToList(string pass)
        {
            m.WaitOne();
            if (!dictPasses.Contains(pass))
            {
                dictPasses.Add(pass);
            }
            m.ReleaseMutex();
        }
        public void addFastUserToList(string user)
        {
            users.Enqueue(user);
        }
        #region SOCKET_PROCEDURES
        /// <summary>
        /// gets a socket from the socket pool
        /// </summary>
        /// <returns>a socket ready to use</returns>
        public Socket getSocket()
        {
            Socket socket = null;
            bool refer = false;
                try
                {
                    refer = false;
                    socketLock.Enter(ref refer);
                    while (sockPool.Count > 0)
                    {
                        socket = sockPool.Dequeue();
                        if (socket != null && socket.Connected)
                        {
                            return socket;
                        }
                    }
                }
                finally
                {
                    if (refer)
                        socketLock.Exit(true);
                }
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.TypeOfService, 0x10);
            socket.LingerState = new LingerOption(false, 0);
            socket.NoDelay = true;
            socket.ReceiveTimeout = OnlineModule.RECEIVE_TIMEOUT;
            socket.SendTimeout = OnlineModule.RECEIVE_TIMEOUT;
            return socket;
        }
        /// <summary>
        /// send data through the socket conn, manages the use of ssl
        /// </summary>
        /// <param name="data">data to send</param>
        /// <param name="conn">socket used to send data</param>
        protected void send(String data, ref Socket conn)
        {
            //mmm unsafe functions work better...
            byte[] cmdBytes = Encoding.UTF8.GetBytes(data);
            if (usingSSL != SecurityProviderProtocol.PROT_NO_SSL)
            {
                    ossl.send(cmdBytes, cmdBytes.Length, conn);
                    return;
            }
            conn.Send(cmdBytes, cmdBytes.Length, 0);
        }
        /// <summary>
        /// send data through the socket conn, manages the use of ssl
        /// </summary>
        /// <param name="cmdBytes">bytes to send</param>
        /// <param name="conn">socket used to send bytes</param>
        protected void send(Byte[] cmdBytes, ref Socket conn)
        {
            if (usingSSL != SecurityProviderProtocol.PROT_NO_SSL && usingSSL != SecurityProviderProtocol.PROT_OTHER)
            {
                ossl.send(cmdBytes, cmdBytes.Length, conn);
                return;
            }
            conn.Send(cmdBytes, cmdBytes.Length, 0);
        }
        
        /// <summary>
        /// receives data through the socket conn and stores it in the buffer, manages the use of ssl if needed
        /// </summary>
        /// <param name="buffer">buffer to receive data</param>
        /// <param name="conn">socket from wich data is received</param>
        /// <returns>amount of data readed</returns>
        protected int receive(ref Byte[] buffer, ref Socket conn)
        {
            if (usingSSL != SecurityProviderProtocol.PROT_NO_SSL && usingSSL != SecurityProviderProtocol.PROT_OTHER)
            {
                    return ossl.receive(buffer, buffer.Length, conn);

            }
            return conn.Receive(buffer, SocketFlags.None);
        }
        /// <summary>
        /// conects the socket conn, manages SSL if needed
        /// </summary>
        /// <param name="conn">socket to connect</param>
        /// 
        protected void connectSocket(ref Socket conn)
        {
            if (usingSSL != SecurityProviderProtocol.PROT_NO_SSL && usingSSL != SecurityProviderProtocol.PROT_OTHER)
            {
                bool refer = false;
                try
                {
                    sslLock.Enter(ref refer);
                    ossl = new OpenSSL();
                }
                finally
                {
                    if (refer)
                        sslLock.Exit(true);
                }
                ossl.OpenSSLConnect(usingSSL, conn, ep);
                return;
            }
            conn.Connect(ep);
        }
        /// <summary>
        /// disconects a socket
        /// </summary>
        /// <param name="socket"></param>
        protected void disconnectSocket(Socket socket)
        {

            if (usingSSL != SecurityProviderProtocol.PROT_NO_SSL && usingSSL != SecurityProviderProtocol.PROT_OTHER)
            {
                ossl.disconnect(socket);
            }
            else if (socket != null)// || sockPool.Count >= numThreads + 5)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close(0);
                return;
            }
        }
        protected void AddReuseSocket(Socket socket)
        {
            m.WaitOne();
            if (socket.Connected)
                sockPool.Enqueue(socket);
            else
                disconnectSocket(socket);
            m.ReleaseMutex();
        }

        #endregion
        protected string Zip(string text)
        {

            return Zip(Encoding.UTF8.GetBytes(text));
        }

        protected string Zip(byte[] buffer)
        {
            GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true);
            zip.Write(buffer, 0, buffer.Length);
            ms.Position = 0;

            MemoryStream outStream = new MemoryStream();
            byte[] compressed = new byte[ms.Length];

            ms.Read(compressed, 0, compressed.Length);
            byte[] gzBuffer = new byte[compressed.Length + 4];

            System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);

            System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);

            return Convert.ToBase64String(gzBuffer);
        }
        protected byte[] UnZip(string compressedText)
        {
            return UnZip(Convert.FromBase64String(compressedText), false);
        }
        protected byte[] UnZip(byte[] gzBuffer, bool b64)
        {
            if (b64){
                char[] cb = new char[gzBuffer.Length];
                Array.Copy(gzBuffer, cb, gzBuffer.Length);
                gzBuffer = Convert.FromBase64CharArray(cb, 0, gzBuffer.Length);
            }
            int msgLength = BitConverter.ToInt32(gzBuffer, 0);

            ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

            byte[] buffer = new byte[msgLength];

            ms.Position = 0;
            GZipStream zip = new GZipStream(ms, CompressionMode.Decompress);

            zip.Read(buffer, 0, buffer.Length);

            return buffer;
        }
    }
}
