using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Data.SqlServerCe;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlTypes;

namespace DBManagement
{
    public struct DoubleString
    {
        public string s1;
        public string s2;
        
    }
    public class Session : IDisposable
    {
        public static readonly string DATABASE_FILE = "Sessions.sdf";
        public static readonly string DB_CREATE_SCRIPT_PATH = "dbTemplate.sql";
        /// <summary>
        /// Conexión a la base de datos
        /// </summary>
        private SqlCeConnection dbConnection;
        /// <summary>
        /// Comando usado para realizar comandos de bases de datos
        /// </summary>
        private SqlCeCommand dbCommand;

        private static Session thisObj;
        private Session()
        {
            
            if (!File.Exists(DATABASE_FILE))
            {
            
                crateDB();
            
            }

            dbConnection = new SqlCeConnection(@"DataSource=" + DATABASE_FILE);

            try
            {
            
                dbConnection.Open();
            
            }
            catch
            {
                MessageBox.Show("Can't connecto to the database, make sure any other app is using it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(-1);
            }
        }
        /// <summary>
        /// Gets an instance of this singleton
        /// </summary>
        /// <returns>an instance of Session</returns>
        public static Session getInstance()
        {
            if (thisObj == null)
                thisObj = new Session();
            return thisObj;
        }
        /// <summary>
        /// Gets the last session created
        /// </summary>
        /// <returns>a SessionObj that represents the las session created</returns>
        public List<SessionObj> ActualSessions()
        {
            dbCommand = new SqlCeCommand(@"SELECT * FROM session", dbConnection);
            SqlCeDataReader reader = dbCommand.ExecuteReader();
            List<SessionObj> ret = new List<SessionObj>();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string name = reader.GetString(2);
                SessionObj obj = new SessionObj(name, id);
                if (reader.IsDBNull(3))
                    obj.ActualMod = 0;
                else
                    obj.ActualMod = reader.GetInt32(3);
                if (!reader.IsDBNull(1))
                    obj.Comments = reader.GetString(1);
                ret.Add(obj);
            }
            return ret;
        }

        private void crateDB()
        {
            string cbs = @"DataSource=" + DATABASE_FILE;
            SqlCeEngine en = new SqlCeEngine(cbs);
            try
            {
                // create a new database
                en.CreateDatabase();
            }
            catch
            {
                MessageBox.Show("Unable to create de database, verify u have enough permisions on DB directory", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            createDBTable(DB_CREATE_SCRIPT_PATH, cbs);
        }
        /// <summary>
        /// Same as createDBTable, but no connection string is passed, it is suposed that the connection is already open
        /// </summary>
        /// <param name="s">Path to SQL file</param>
        private void createDBTable(string s)
        {
            createDBTable(s, "");
        }
        /// <summary>
        /// Creates a new SQL Table in the data base
        /// </summary>
        /// <param name="s">Path to the SQL script file that creates the table</param>
        /// <param name="csb">connection string of the DB</param>
        private void createDBTable(string s, string csb)
        {
            try
            {
                FileInfo file = new FileInfo(s);
                StreamReader script = file.OpenText();
                string r = string.Empty;
                if (!String.IsNullOrEmpty(csb))
                    dbConnection = new SqlCeConnection(csb);

                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.Open();
                }

                while ((r = script.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(r) || r.StartsWith("#") || r.StartsWith("--"))
                        continue;
                    dbCommand = new SqlCeCommand(r, dbConnection);
                    dbCommand.ExecuteNonQuery();
                }
            }
            catch (SqlCeException e)
            {
                MessageBox.Show("Imposible to create a new DB, verify it doesn't exists or you have the script \n" + e.Message + "\n" + s, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (dbConnection.State != System.Data.ConnectionState.Closed)
                    dbConnection.Close();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            dbCommand.Dispose();
            dbConnection.Close();
        }

        #endregion
        /// <summary>
        /// Creates a new session
        /// </summary>
        /// <param name="p">Name of the session</param>
        /// <param name="p_2">Comments for this session</param>
        public void createSession(string p, string p_2)
        {
            dbCommand = new SqlCeCommand(@"INSERT INTO session (comments, name, actualModule) VALUES (@comments,@name, @actualModule);", dbConnection);
            
            dbCommand.Parameters.Clear();
            this.dbCommand.Parameters.Add("@comments", p_2);
            this.dbCommand.Parameters.Add("@name", p);
            this.dbCommand.Parameters.Add("@actualModule", SqlInt32.Null);
            dbCommand.ExecuteNonQuery();
        }
        /// <summary>
        /// Adds a new module to the id session, this is only added when a user tests a specific module
        /// </summary>
        /// <param name="id">id of the session</param>
        /// <param name="module">id of the module</param>
        public void addModule(int id, int module)
        {
            dbCommand = new SqlCeCommand(@"INSERT INTO moduleFound (ID, Module) VALUES (@ID,@Module);", dbConnection);

            dbCommand.Parameters.Clear();
            this.dbCommand.Parameters.Add("@ID", id);
            this.dbCommand.Parameters.Add("@Module", module);
            try
            {
                dbCommand.ExecuteNonQuery();
            }
            catch { }
        }

        public SessionObj getLastSession()
        {
            dbCommand = new SqlCeCommand(@"SELECT * FROM session", dbConnection);
            SqlCeDataReader reader = dbCommand.ExecuteReader();
            List<SessionObj> ret = new List<SessionObj>();
            string name = String.Empty;
            SessionObj obj = new SessionObj();
            while (reader.Read())
            {
                obj.Name = reader.GetString(2);
                obj.Id = reader.GetInt32(0);
                if (reader.IsDBNull(3))
                    obj.ActualMod = 0;
                else
                    obj.ActualMod = reader.GetInt32(3);
                if (!reader.IsDBNull(1))
                    obj.Comments = reader.GetString(1);
            }
            return obj;
        }
        /// <summary>
        /// Adds a message to the database (table Message)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="id"></param>
        /// <param name="host"></param>
        /// <param name="module"></param>
        /// <param name="type"></param>
        /// <param name="username"></param>
        public void addMessage(string message, int id, string host, int module, int type, string username)
        {

            dbCommand = new SqlCeCommand(@"INSERT INTO Messages (ID, host_, Module, type, Message, UserName) VALUES (@ID, @host_, @Module, @type, @Message, @UserName);", dbConnection);
            try
            {
                dbCommand.Parameters.Clear();
                this.dbCommand.Parameters.Add("@ID", id);
                this.dbCommand.Parameters.Add("@host_", host);
                this.dbCommand.Parameters.Add("@Module", module);
                this.dbCommand.Parameters.Add("@type", type);
                this.dbCommand.Parameters.Add("@Message", message);
                this.dbCommand.Parameters.Add("@UserName", username);
                dbCommand.ExecuteNonQuery();
            }
            catch (Exception e) { System.Diagnostics.Debug.WriteLine(e.Message); }

        }
        public void addMessagePossibleTruncation(string message, int id, string host, int module, int type, string username)
        {
            //string truncation error control
            int index = 0; //used to find the message
            while (message.Length > 1)
            {
                int len = message.Length;
                string s = index.ToString("D4") + message.Substring(0, len > 2044 ? 2044 : len);
                if (len < 2047)
                {
                    s = index.ToString("D4") + message;
                    message = string.Empty;
                }
                else
                    message = message.Substring(2044 + 1);

                index++;
                dbCommand = new SqlCeCommand(@"INSERT INTO Messages (ID, host_, Module, type, Message, UserName) VALUES (@ID, @host_, @Module, @type, @Message, @UserName);", dbConnection);
                try
                {
                    dbCommand.Parameters.Clear();
                    this.dbCommand.Parameters.Add("@ID", id);
                    this.dbCommand.Parameters.Add("@host_", host);
                    this.dbCommand.Parameters.Add("@Module", module);
                    this.dbCommand.Parameters.Add("@type", type);
                    this.dbCommand.Parameters.Add("@Message", s);
                    this.dbCommand.Parameters.Add("@UserName", username);
                    dbCommand.ExecuteNonQuery();
                }
                catch (Exception e) { System.Diagnostics.Debug.WriteLine(e.Message); }
            }
        }
        /// <summary>
        /// Gets Messages objects from de specified query. This query must be made to the Message Table. E.g:
        /// "SELECT * FROM Messages WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username = '" + name + "'";
        /// </summary>
        /// <pre> The query MUST be correct. Thie method would ONLY execute it and converte de parameters to a "Message" Object</pre>
        /// <param name="query">the SQL query to execute</param>
        /// <returns>A list of Messages</returns>
        public List<Messages> getMessages(string query)
        {
            List<Messages> ret = new List<Messages>();
            try
            {
                dbCommand = new SqlCeCommand(query, dbConnection);
                SqlCeDataReader reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {
                    Messages mes = new Messages();
                    mes.Id = reader.GetInt32(0);
                    mes.Module = reader.GetInt32(1);
                    mes.Host = reader.GetString(2);
                    mes.Type = reader.GetInt32(3);
                    mes.Message = reader.GetString(4);
                    mes.User = reader.GetString(5);
                    ret.Add(mes);
                }
                return ret;
            }
            catch
            {
                return ret;
            }
        }

        public List<int> getModules(int id)
        {
            string com = "SELECT DISTINCT module FROM moduleFound";
            List<int> ret = new List<int>();
            try
            {
                dbCommand = new SqlCeCommand(com, dbConnection);
                SqlCeDataReader reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {
                    ret.Add((int)reader.GetSqlInt32(0));
                }
                return ret;
            }
            catch
            {
                return ret;
            }
        }

        public List<string> getHosts(int id, int p)
        {

            string com = "SELECT DISTINCT NATURAL.host_ FROM USER_PASS AS NATURAL INNER JOIN Messages ON NATURAL.ID = NATURAL.ID AND NATURAL.ID = NATURAL.ID WHERE (NATURAL.Module = " + p + ") AND (NATURAL.ID = " + id + ")";
            List<string> ret = new List<string>();
            try
            {
                dbCommand = new SqlCeCommand(com, dbConnection);
                SqlCeDataReader reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {
                    if(!reader.IsDBNull(0))
                        ret.Add(reader.GetString(0));
                }
                return ret;
            }
            catch
            {
                return ret;
            }
        }

        public void addUserPass(int id,string host_, int type, string username, string pass, int module)
        {
             //ID INT NOT NULL, Module INT NOT NULL, UserName NVARCHAR(70), Pass NVARCHAR(70)
            
            dbCommand = new SqlCeCommand(@"INSERT INTO USER_PASS (ID, host_, Module, pass, UserName) VALUES (@ID, @host_, @Module, @pass, @UserName);", dbConnection);
            try
            {
                dbCommand.Parameters.Clear();
                this.dbCommand.Parameters.Add("@ID", id);
                this.dbCommand.Parameters.Add("@host_", host_);
                this.dbCommand.Parameters.Add("@Module", module);
                //this.dbCommand.Parameters.Add("@type", type);
                this.dbCommand.Parameters.Add("@pass", pass);
                this.dbCommand.Parameters.Add("@UserName", username);
                dbCommand.ExecuteNonQuery();
            }
            catch { }
        }
        /// <summary>
        /// Gets only one string (the first) from the specified query. E.g:
        /// "SELECT pass FROM USER_PASS WHERE id = " + id + " AND module = " + actualModule + " AND host_ = '" + host + "' AND username = '" + listBox1.SelectedItem.ToString() + "'";
        /// </summary>
        /// <pre> The query MUST be correct. Thie method would ONLY execute it and converte de parameters to a "Message" Object</pre>
        /// <param name="query">The query to execute</param>
        /// <returns>The first item of the query (it sould be an string)</returns>
        public string getString(string query)
        {
            try
            {
                dbCommand = new SqlCeCommand(query, dbConnection);
                SqlCeDataReader reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {

                    //object o = reader.GetValue(0);
                    return reader.GetString(0);
                }
                return "";
            }
            catch { return ""; }
        }
        /// <summary>
        /// gets a list of strings from the specified query. The query SHOULD only select one item of the table and the item must be an string. E.g:
        /// "SELECT usename FROM USER_PASS WHERE id = " + id + " AND module = " + p + " AND host_ = '" + p_3 + "'";
        /// </summary>
        /// <pre> The query MUST be correct. Thie method would ONLY execute it and converte de parameters to a "Message" Object</pre>
        /// <param name="query">the query to execute</param>
        /// <returns>a list of strings from the specified query</returns>
        public List<string> getStrings(string query)
        {
            List<string> strs = new List<string>();
            try
            {
                dbCommand = new SqlCeCommand(query, dbConnection);
                SqlCeDataReader reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {
                    strs.Add(reader.GetString(0));
                }
                return strs;
            }
            catch { return strs; }
        }
        public List<List<string>> getStringsList(string query)
        {
            List<List<string>> strs = new List<List<string>>();
            try
            {
                dbCommand = new SqlCeCommand(query, dbConnection);
                SqlCeDataReader reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {
                    List<string> lstrs = new List<string>();
                    for(int i = 0; i < reader.FieldCount; i++)
                        lstrs.Add(reader.GetString(i));
                }
                return strs;
            }
            catch { return strs; }
        }
        public SessionObj getSession(int id_)
        {
            
            dbCommand = new SqlCeCommand(@"SELECT * FROM session where ID = "+id_, dbConnection);
            SqlCeDataReader reader = dbCommand.ExecuteReader();
            List<SessionObj> ret = new List<SessionObj>();
            string name = String.Empty;
            SessionObj obj = new SessionObj();
            while (reader.Read())
            {
                obj.Name = reader.GetString(2);
                obj.Id = reader.GetInt32(0);
                if (reader.IsDBNull(3))
                    obj.ActualMod = 0;
                else
                    obj.ActualMod = reader.GetInt32(3);
                if (!reader.IsDBNull(1))
                    obj.Comments = reader.GetString(1);
            }
            return obj;
        }
        /// <summary>
        /// gets a list of 2 strings from the specified query. The query SHOULD only select two items of the table and the items must be an string. E.g:
        /// "SELECT username, pass FROM USER_PASS WHERE id = " + id + " AND module = " + module + " AND host_ = '" + host__ + "'";
        /// </summary>
        /// <pre> The query MUST be correct. Thie method would ONLY execute it and converte de parameters to a "Message" Object</pre>
        /// <param name="query">the query to execute</param>
        /// <returns>a list of DoubleString objects</returns>
        public List<DoubleString> get2Strings(string query)
        {
            List<DoubleString> strs = new List<DoubleString>();
            try
            {
                dbCommand = new SqlCeCommand(query, dbConnection);
                SqlCeDataReader reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {
                    DoubleString tmp = new DoubleString();
                    tmp.s1 = reader.GetString(0);
                    tmp.s2 = reader.GetString(1);
                    strs.Add(tmp);
                }
                return strs;
            }
            catch { return strs; }
        }

        public void addTextData(int p, string fileName)
        {
            dbCommand = new SqlCeCommand(@"INSERT INTO textData (ID, DATA) VALUES (@ID,@DATA);", dbConnection);

            dbCommand.Parameters.Clear();
            this.dbCommand.Parameters.Add("@ID", p);
            this.dbCommand.Parameters.Add("@DATA", fileName);
            try
            {
                dbCommand.ExecuteNonQuery();
            }
            catch { }
        }

        public void delTextData(int TEXT_DATA_LIST_ID, string fileName)
        {
            dbCommand = new SqlCeCommand(@"DELETE FROM textData WHERE ID = @ID AND DATA = @DATA;", dbConnection);

            dbCommand.Parameters.Clear();
            this.dbCommand.Parameters.Add("@ID", TEXT_DATA_LIST_ID);
            this.dbCommand.Parameters.Add("@DATA", fileName);
            try
            {
                dbCommand.ExecuteNonQuery();
            }
            catch { }
        }
    }
}
