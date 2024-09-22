using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.DataAccess.Client;

namespace DBUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CreateFunction()
        {

            string permissions = "DECLARE KEYNUM NUMBER;"+
                "\nBEGIN"+
                "\nSYS.DBMS_JAVA.GRANT_PERMISSION(grantee => 'DBADMIN',"+
                "permission_type => 'java.io.FilePermission',permission_name => '<<ALL FILES>>',permission_action => "+
                "'execute',key => KEYNUM);\nEND;";
            CreateCode();
            CreateProcedures();
            OracleConnection con = new OracleConnection("User Id=sys;Password=uniandes; DBA Privilege = SYSDBA; Data Source=localhost/ORCL");
            try
            {
                con.Open();
            }
            catch (Exception e)
            {
            }
            
            OracleCommand com = new OracleCommand(permissions, con);
            try
            {
                com.ExecuteNonQuery();
                //com = new OracleCommand("grant execute on java source \"OS_HELPER\" to public", con);
                //com.ExecuteNonQuery();
                //com = new OracleCommand("create or replace public synonym \"ExternalCall\" for \"ExternalCall\"",con);
                //com.ExecuteNonQuery();
                //com = new OracleCommand("grant execute on \"ExternalCall\" to public",con);
                //com.ExecuteNonQuery();
                //com = new OracleCommand("grant execute on OS_COMMAND to public",con);
                //com.ExecuteNonQuery();
                //com = new OracleCommand("create or replace public synonym OS_COMMAND for OS_COMMAND", con);
            }
            catch (Exception e)
            {
                //1031 priviledges
                Debug.WriteLine(e.Message);
            }

        }

        public void CreateProcedures()
        {
            string codeDir = "c:\\FunctionsWrapper.sql";
            StreamReader sr = new StreamReader(codeDir);
            string wrapper = sr.ReadToEnd();
            sr.Close();
            OracleConnection con = new OracleConnection("User Id=dbsnmp; Password=uniandes2011; Data Source=localhost/ORCL");
            con.Open();

            OracleCommand com = new OracleCommand(wrapper, con);
            try
            {
                com.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            con.Close();
        }
        [TestMethod]
        public void CreateCode()
        {
            string codeDir = "c:\\OraJava.sql";
            StreamReader sr = new StreamReader(codeDir);
            string code = sr.ReadToEnd();
            sr.Close();
            OracleConnection con = new OracleConnection("User Id=dbsnmp; Password=uniandes2011; Data Source=localhost/ORCL");
            con.Open();
            OracleCommand com = new OracleCommand(code, con);
            try
            {
                com.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            com = new OracleCommand("alter java source \"OS_HELPER\" compile", con);
            com.ExecuteNonQuery();
            
            con.Close();
        }
    }
}

