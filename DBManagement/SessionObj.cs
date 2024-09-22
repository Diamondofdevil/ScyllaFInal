using System;
using System.Collections.Generic;
using System.Text;

namespace DBManagement
{
    public class SessionObj
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private int actualMod;

        public int ActualMod
        {
            get { return actualMod; }
            set { actualMod = value; }
        }
        private string comments;
        public string Comments
        {
            get { return comments; }
            set { comments = value; }
        }
        public SessionObj()
        {
        }
        public SessionObj(string name, int id)
        {
            Name = name;
            Id = id;
        }
        public override string ToString()
        {
            return Id + "-" + Name + "-" + ActualMod;
        }
    }
}
