using System;
using System.Collections.Generic;
using System.Text;

namespace Scylla
{
    class ThreadInformation
    {
        private int threadIndex;

        public int ThreadIndex
        {
            get { return threadIndex; }
            set { threadIndex = value; }
        }

        public ThreadInformation(int index)
        {
            this.threadIndex = index;
        }
    }
}
