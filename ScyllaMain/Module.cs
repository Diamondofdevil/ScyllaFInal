using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Scylla
{
    public struct string2
    {
        private string user;

        public string User
        {
            get { return user; }
            set { user = value; }
        }
        private string pass;

        public string Pass
        {
            get { return pass; }
            set { pass = value; }
        }
        public bool IsNull;

        public string2(bool isNull)
        {
            user = pass = string.Empty;
            IsNull = isNull;
        }

        public string2(string user, string pass)
        {
            IsNull = false;
            this.user = user;
            this.pass = pass;
        }
    }
    public abstract class Module
    {
        protected List<String> dictPasses;
        /// <summary>
        /// reference to the GUI
        /// </summary>
        protected ScyllaGUI gui;
        protected static int numThreads;
        
        protected int index;
        protected int errCount;
        protected bool die;
        protected int tCount;

        private int passIndex = 0;
        private int totTries = 0;
        private int cicle;
        
        protected Queue<string> users;

        private System.Timers.Timer t;
        protected Mutex m;
        private static int workT;

        public Module(int threads, List<string> dict, ScyllaGUI gui, List<string> users)
        {
            this.users = new Queue<string>(users);
            //passIndex = totTries = 0;
            cicle = -4;
            tCount = (dict.Count == 0?1:dict.Count) * (users.Count == 0? 1 : users.Count);
            m = new Mutex();
            this.gui = gui;
            errCount = 0;
            numThreads = threads < 0 ? 10:threads;
            this.dictPasses = dict;
            index = -1;
            
            t = new System.Timers.Timer();
        }
        public void init()
        {
            preProcessing();
            t.Interval = 600;
            t.Elapsed += new System.Timers.ElapsedEventHandler(timerCallBack);
            t.Enabled = true;
            t.Start();
            workT = 0;
            unchecked
            {
                Parallel.For(0, numThreads, i =>
                {

                    ThreadInformation info = new ThreadInformation(i);
                    info.ThreadIndex = workT;
                    workT++;
                    processWord(info);
                    workT--;
                });
            }
            t.Stop();
        }

        public abstract bool preProcessing();
        public abstract void setHost(string host, int port);
        
        //who's calling must use the mutex!
        protected void getPass( ref string2 userPas)
        {
            if (passIndex >= dictPasses.Count || users.Count == 0)
            {
                userPas.IsNull = true;
                return;
            }
            userPas.Pass = dictPasses[passIndex++];
            //the last one
            if (passIndex == dictPasses.Count)
            {
                passIndex = 0;
                userPas.User = users.Dequeue();
            }else
                userPas.User = users.Peek();

            userPas.IsNull = false;
        }
        protected void tryNextUser()
        {
            /*if(users.Count > 0)
                users.Dequeue();*/
            passIndex = dictPasses.Count-1;
        }

        public void timerCallBack(object o, System.Timers.ElapsedEventArgs eea)
        {
            int average = 0;
            //first 2 seconds normally don't give info, prehacks or stuff
            if (cicle > 0)
            {
                totTries += index;
                average = (int) Math.Round(((decimal)totTries/cicle),0, MidpointRounding.ToEven);
            }
            cicle++;
            tCount -= index;
            gui.setTimers(index, tCount, workT, average);
            index = 0;
        }
        public void stop()
        {
            die = true;
        }
        protected int IndexCount
        {
            set {
                tCount += value;
                index += value;
            }
        }
        public virtual void processWord(Object oList) { }
    }
}
