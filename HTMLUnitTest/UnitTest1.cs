using System;
using System.Collections.Specialized;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scylla;

namespace HTMLUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void URIParser()
        {
            string uri1 = @"http://www.test.com/folder/file.php";
            string uri2 = @"https://test.com/folder/";
            string uri3 = @"http://www.test.com:8080";
            string uri4 = @"test.html";
            string uri5 = @"/test.html";
            string uri6 = @"/folder/test.html";
            string uri7 = @"/folder/";
            string uri8 = @"/";
            string uri9 = @"folder/";

            URLInfo info = new URLInfo();

            Assert.IsTrue(HTTPHelper.getURLInfo(uri1, ref info));
            Assert.AreEqual("www.test.com", info.host);
            Assert.AreEqual(80, info.port);
            Assert.AreEqual("/folder/", info.path);
            Assert.AreEqual("file.php", info.end);

            //string uri2 = @"https://test.com/folder/";
            info = new URLInfo();
            Assert.IsTrue(HTTPHelper.getURLInfo(uri2, ref info));
            Assert.AreEqual("test.com", info.host);
            Assert.AreEqual(443, info.port);
            Assert.AreEqual("folder/", info.end);
            Assert.AreEqual("/", info.path);

            //string uri3 = @"http://www.test.com:8080";
            info = new URLInfo();
            Assert.IsTrue(HTTPHelper.getURLInfo(uri3, ref info));
            Assert.AreEqual("www.test.com", info.host);
            Assert.AreEqual(8080, info.port);
            Assert.AreEqual("/", info.path);
            Assert.AreEqual("", info.end);

            //string uri4 = @"test.html";
            info = new URLInfo();
            Assert.IsTrue(HTTPHelper.getURLInfo(uri4, ref info));
            Assert.AreEqual("test.html", info.end);
            Assert.AreEqual("/", info.path);
            

            //string uri5 = @"/test.html";
            info = new URLInfo();
            Assert.IsTrue(HTTPHelper.getURLInfo(uri5, ref info));
            Assert.AreEqual("test.html", info.end);
            Assert.AreEqual("/", info.path);

            //string uri6 = @"/folder/test.html";
            info = new URLInfo();
            Assert.IsTrue(HTTPHelper.getURLInfo(uri6, ref info));
            Assert.AreEqual("test.html", info.end);
            Assert.AreEqual("/folder/", info.path);

            //string uri7 = @"/folder/";
            info = new URLInfo();
            Assert.IsTrue(HTTPHelper.getURLInfo(uri7, ref info));
            Assert.AreEqual("folder/", info.end);
            Assert.AreEqual("/", info.path);
            
            //string uri8 = @"/";
            info = new URLInfo();
            Assert.IsTrue(HTTPHelper.getURLInfo(uri8, ref info));
            Assert.AreEqual("", info.end);
            Assert.AreEqual("/", info.path);
            
            //string uri9 = @"folder/";
            info = new URLInfo();
            Assert.IsTrue(HTTPHelper.getURLInfo(uri9, ref info));
            Assert.AreEqual("folder/", info.end);
            Assert.AreEqual("/", info.path);



            StringDictionary sd = new StringDictionary();
            sd.Add("bla","bla");
            string nula = sd["nula"];
            if(sd["nula"] == "bla")
                Debug.WriteLine(nula);
        }
    }
}
