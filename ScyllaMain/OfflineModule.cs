using System;
using System.Collections.Generic;
using System.Text;

namespace Scylla
{
    abstract class OfflineModule : Module
    {
        protected String hash;
        
        public abstract void compare(String palabra);
        public abstract String cipher(String palabra);

        public OfflineModule(int threads, List<string> passDict, String hash, ScyllaGUI gui) : base(threads,passDict, gui, null)
        {
            this.hash = hash;
        }
        
        public void parse(String palabra)
        {
            compare(palabra);
        }

        

    }
}
