using System;
using System.Collections.Generic;
using System.Text;

namespace Scylla
{
    public interface IOnlineModule
    {
        /// <summary>
        /// Tests a username and password in the defined module
        /// </summary>
        /// <param name="user">Username to test</param>
        /// <param name="pass">Password to test</param>
        /// <param name="misc">unused - for further use if needed</param>
        /// <returns>the attempt was succesful?</returns>
        bool testPassword(String user, String pass, ref object misc);
        /// <summary>
        /// Initiates the app, Please do not use, extend from the class OnlineModule
        /// </summary>
        void init();
        /// <summary>
        /// Stop the app, Please do not use, extend from the class OnlineModule
        /// </summary>
        void stop();

        /// <summary>
        /// Makes pre-processing functions possibly needed befor making brute force
        /// </summary>
        /// <returns>returns false if module needs to stop (do not make bf attack)</returns>
        bool preProcessing();
        /// <summary>
        /// Sets a new host name to test
        /// </summary>
        /// <param name="hostNmae">host name in any notation (JUST ONE! not CIDR or anything else)</param>
        void setHost(string hostNmae, int port);
    }
}
