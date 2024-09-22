using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBManagement
{
    public class Messages
    {
        private string user;

        public string User
        {
            get { return user; }
            set { user = value; }
        }
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        private string host;

        public string Host
        {
            get { return host; }
            set { host = value; }
        }
        private int module;

        public int Module
        {
            get { return module; }
            set { module = value; }
        }
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        private int type;

        public int Type
        {
            get { return type; }
            set { type = value; }
        }
    }
}
