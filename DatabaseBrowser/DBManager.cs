using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DatabaseBrowser
{

    public class DBManager
    {
        public enum DB_TYPE
        {
            ORACLE = 0,
            MSSQL = 1,
            DB2 = 2,
            MYSQL = 3,
            POSTGRES = 4,
            SQLServerCE35 = 5
        }
        private static readonly string[] QUERY_TABLES = new string[]
        {
            "select table_name from all_tables order by table_name",
            "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES",
            "select tabschema CONCAT '.' CONCAT TABNAME from syscat.tables",
            "SHOW TABLES",
            "SELECT table_name FROM information_schema.tables ORDER BY table_name",
            "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES",
        };
        private static readonly string[] QUERY_DB = new string[]
        {
            //select * from user_tablespaces;    
            "select name from v$database",
            "SELECT name FROM sys.databases",
            "select * from SYSIBM.SYSDATABASE",
            "SHOW DATABASES",
            "SELECT datname FROM pg_database",
            "SELECT name FROM sys.databases",
        };
        private static readonly string[] ACTUAL_DB_NAME = new string[]
        {
            "SELECT ora_database_name FROM dual",
            "SELECT DB_NAME()",
            "SELECT CURRENT SERVER FROM SYSIBM.SYSDUMMY1",
            "SELECT DATABASE()",
            "select current_database()",
            "SELECT DB_NAME()",
        };
        /// <summary>
        /// You gotta add the table name: select * from "APPEND_TABLE"
        /// </summary>
        private static readonly string[] SELECT_TABLE1 = new string[]
        {
            "SELECT * FROM ",
            "SELECT * FROM ",
            "SELECT * FROM ",
            "SELECT * FROM ",
            "SELECT * FROM ",
            "SELECT * FROM ",
        };
        /// <summary>
        /// Use a DB
        /// </summary>
        private static readonly string[] USE_DB = new string[]
        {
            "USE ",
            "USE ",
            "",
            "USE ",
            "USE ",
            "USE ",
        };
        /*
         * Yup, this is taken from:
         * Copyright (C) 2011-2012 Iván Costales Suárez
            This file is part of CompactView.
            You should have received a copy of the GNU General Public License
            along with CompactView.  If not, see <http://www.gnu.org/licenses/>.
            CompactView web site <http://sourceforge.net/p/compactview/>.
         * */
        private Regex regexSemicolon = new Regex("(?:[^;']|'[^']*')+", RegexOptions.Compiled | RegexOptions.Multiline);
        private Regex regexCreateAlterDrop = new Regex("create|alter|drop", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private Regex regexInsertUpdateDelete = new Regex("insert|update|delete", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private Regex regexTables = new Regex(@"\bjoin\s+(?<Retrieve>[a-zA-Z\._\d]+)\b|\bfrom\s+(?<Retrieve>[a-zA-Z\._\d]+)\b|\bupdate\s+(?<Update>[a-zA-Z\._\d]+)\b|\binsert\s+(?:\binto\b)?\s+(?<Insert>[a-zA-Z\._\d]+)\b|\btruncate\s+table\s+(?<Delete>[a-zA-Z\._\d]+)\b|\bdelete\s+(?:\bfrom\b)?\s+(?<Delete>[a-zA-Z\._\d]+)\b");

        private string lastError;

        public string LastError
        {
            get { return lastError; }
            set { lastError = value; }
        }
        private DbCommand comm;
        private DbConnection con;
        private DB_TYPE type;
        private List<string> sqlHistory;

        public List<string> SqlHistory
        {
            get { return sqlHistory; }
            set { sqlHistory = value; }
        }

        private static DBManager singleton;
        private DBManager(DbCommand comm, DbConnection con, DB_TYPE type)
        {
            LastError = string.Empty;
            this.comm = comm;
            this.con = con;
            this.type = type;
            sqlHistory = new List<string>();
        }
        public static DBManager getInstance()
        {
            //if null, no one create it, so fuck off
            return singleton;
        }
        //DBA Privilege = SYSDBA
        public static DBManager createInstance(DbConnection con, DB_TYPE type)
        {
            singleton = null;
            singleton = new DBManager(con.CreateCommand(), con, type);
            return singleton;
        }
        public void clearHistory()
        {
            sqlHistory.Clear();
        }
        public void executeNonQuery(string sql)
        {
            if (con.State == System.Data.ConnectionState.Closed)
                con.Open();
            comm = con.CreateCommand();
            comm.CommandText = sql;
            comm.ExecuteNonQuery();
        }
        private DbDataReader executeDR(string sql)
        {
            if (con.State == System.Data.ConnectionState.Closed)
                con.Open();
            comm = con.CreateCommand();
            comm.CommandText = sql;
            return comm.ExecuteReader();
        }
        public List<string> getTableNames()
        {
            List<string> tableNames = new List<string>();
            DbDataReader dr = null;
            try
            {
                dr = executeDR(QUERY_TABLES[(int)type]);
                while (dr.Read()) tableNames.Add(dr.GetString(0));
                dr.Close();
            }
            catch (Exception e)
            {
                try
                {
                    dr.Close();
                }
                catch { }
                LastError = e.Message;
                return null;
            }
            return tableNames;
        }
        public string getActualDB()
        {
            string name = string.Empty;
            DbDataReader dr = null;
            try{
                if (type == DB_TYPE.SQLServerCE35)
                    return con.Database;
                dr = executeDR(ACTUAL_DB_NAME[(int)type]);
            if (dr.Read())
            {
                if (!dr.IsDBNull(0))
                    name = dr.GetString(0);
                else
                    name = "";
            }
            dr.Close();
            }
            catch (Exception e)
            {
                try
                {
                    dr.Close();
                }
                catch { }
                LastError = e.Message;
                return null;
            }
            return name;
        }

        internal DbDataReader getTableData(string sql)
        {
            return execute(SELECT_TABLE1[(int)type] + sql);
        }

        private List<string> getTables(string sql)
        {
            List<string> tableNames = new List<string>();
            DbDataReader dr = null;
            try{
                dr = executeDR(QUERY_TABLES[(int)type]);
            while (dr.Read()) tableNames.Add(dr.GetString(0));
            dr.Close();
            }
            catch (Exception e)
            {
                try
                {
                    dr.Close();
                }
                catch { }
                LastError = e.Message;
                return null;
            }
            return tableNames;
        }
        public DbDataReader execute(string sql)
        {
            LastError = string.Empty;
            if (con.State == ConnectionState.Closed)
                try
                {
                    con.Open();
                }
                catch (Exception e)
                {
                    LastError = "Unable to connect to server: " + (e.InnerException == null ? e.Message : e.InnerException.Message);
                }
            int QueryCount = 0;
            DbDataReader dr = null;
            for (Match m = regexSemicolon.Match(sql); m.Success; m = m.NextMatch()) if (!string.IsNullOrEmpty(m.Value))
            {
                if (dr != null && !dr.IsClosed)
                    dr.Close();
                try
                {
                    QueryCount++;
                    sqlHistory.Add(m.Value.Trim());
                    comm.CommandText = m.Value.Trim();
                    dr = comm.ExecuteReader();
                }
                catch (Exception e)
                {
                    LastError = "Query" + " " + QueryCount + ": " + (e.InnerException == null ? e.Message : e.InnerException.Message);
                    return null;
                }

            }
            return dr;
        }
        public bool isTableUpdate(string sql)
        {
            string reg = regexSemicolon.Replace(sql, "");
            reg = string.IsNullOrEmpty(reg) ? sql : reg;
            return regexCreateAlterDrop.IsMatch(reg);
        }
        
        public bool isDataUpdate(string sql)
        {
            string reg = regexSemicolon.Replace(sql, "");
            reg = string.IsNullOrEmpty(reg) ? sql : reg;
            return regexInsertUpdateDelete.IsMatch(reg);
        }
        private DataSet convertDataSet(DbDataReader dr)
        {
            DataSet dataSet = new DataSet();
            DataTable schemaTable = dr.GetSchemaTable();
            DataTable dataTable = new DataTable();

            for (int i = 0; i < schemaTable.Rows.Count; i++)
            {
                DataRow dataRow = schemaTable.Rows[i];
                string columnName = dataRow["ColumnName"].ToString();
                
                DataColumn column = new DataColumn(columnName, Type.GetType(dataRow["DataType"].ToString()));
                dataTable.Columns.Add(column);
            }
            dataSet.Tables.Add(dataTable);
            while (dr.Read())
            {
                DataRow dataRow = dataTable.NewRow();
                for (int i = 0; i < dr.FieldCount; i++)
                    dataRow[i] = dr.GetValue(i);
                dataTable.Rows.Add(dataRow);
            }
            return dataSet;
        }

        internal List<string> getDatabases()
        {
            List<string> dbNames = new List<string>();
            DbDataReader dr = null;
            try
            {
                dr = executeDR(QUERY_DB[(int)type]);
                while (dr.Read()) dbNames.Add(dr.GetString(0));
                dr.Close();
            }
            catch (Exception e)
            {
                try
                {
                    dr.Close();
                }
                catch { }
                LastError = e.Message;
                return null;
            }
            return dbNames;
        }

        internal bool useDB(string dbName)
        {
            try
            {
                executeNonQuery(USE_DB[(int)type] + dbName);
                return true;
            }
            catch (Exception e)
            {
                LastError = e.Message;
                return false;
            }
        }
    }
}
