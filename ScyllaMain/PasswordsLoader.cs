using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;

namespace Scylla
{
    class PasswordsLoader
    {
        public enum PermutationOptions
        {
            AsIs = 0,
            UpperCase = 1,
            LowerCase = 2,
            Double = 4,
            CasePerm = 8,
            Reverse = 16,
            TwoNumAppend = 32,
            PreAPEndDate = 64,
            H4x0rPerm = 128,
            H4x0r = 256
        };
        public enum PassTypeOptions
        {
            Password = 0,
            UserSlashPass = 1,
            UserSpacePass = 2,
            UserMinusPass = 3
        };
        public static readonly char[] H4x0r = { 'a', 'b','t', 'e','i','l','o','s' };
        public static readonly char[] H4x0rUP = { 'A', 'B', 'T', 'E', 'I', 'L', 'O', 'S' };
        public static readonly char[] H4x0rChanges = { '4','8', '7','3','1','1','0','5' };

        public static List<String> loadFile(String path)
        {
            List<String> dict = new List<String>();
            if (!File.Exists(path))
                return dict;
            String[] words = File.ReadAllLines(path);
            dict.AddRange(words);
            return dict;
        }

        public static List<String> loadFile(String path, int inicio, PassTypeOptions words)
        {
            List<String> dict = new List<String>();
            if (!File.Exists(path))
                return dict;
            StreamReader sr = new StreamReader(File.OpenRead(path));
            string str = string.Empty;
            for (int i = 0; i < inicio && (sr.ReadLine() != null); i++) ;
            for (int i = inicio; (str = sr.ReadLine()) != null; i++)
            {
                if (!String.IsNullOrWhiteSpace(str))
                {

                    try
                    {
                        dict.Add(words == PassTypeOptions.Password ? str : words == PassTypeOptions.UserMinusPass ? str.Split(new char[] { '-' })[1] : words == PassTypeOptions.UserSlashPass ? str.Split(new char[] { '/' })[1] : str.Split(new char[] { ' ' })[1]);
                    }
                    catch
                    {
                        throw new Exception("Password file not in the correct format. Exception at line "+i);
                    }
                }
            }
            return dict;
        }
        public static void generarPermutacioens(List<string> lista, PermutationOptions[] perm, ref List<string> advList/*StreamWriter sw*/)
        {
            try
            {
                foreach (PermutationOptions per in perm)
                    PasswordsLoader.generarPermutacioens(lista, per, /*sw*/ ref advList);
            }
            catch (OutOfMemoryException)
            {
                int count = advList.Count;
                advList.Clear();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                throw new OutOfMemoryException("Dude u know many passwords have u generated to get to this exception? - Out of Memory!");
            }
        }
        private static void generarPermutacioens(List<string> lista,  PermutationOptions perm, ref List<string> ls2/*StreamWriter sw*/)
        {
            //List<string> ls2 = new List<string>();
            if (perm == PermutationOptions.AsIs)
                return;
            switch (perm)
            {
                case PermutationOptions.Double:
                    foreach (string s in lista)
                        ls2.Add(s + s);
                    break;
                case PermutationOptions.LowerCase:
                    foreach (string s in lista)
                    {
                        string x = s.ToLower();
                        //if (!x.Equals(s))
                            ls2.Add(x);
                    }
                    break;
                case PermutationOptions.UpperCase:
                    foreach (string s in lista)
                    {
                        string x = s.ToUpper();
                        //if (!x.Equals(s))
                            ls2.Add(x);
                    }
                    break;
                case PermutationOptions.CasePerm:
                    foreach (string s in lista)
                    {
                        string ss = s.ToLower();
                        List<string> newList = new List<string>();
                        getCasePerm(ref newList, ss, 0);
                        ls2.AddRange(newList);
                    }
                    break;
                case PermutationOptions.H4x0rPerm:
                    foreach (string s in lista)
                    {
                        
                        List<string> newList = new List<string>();
                        getH4x0rPerm(ref newList, s, 0);
                        ls2.AddRange(newList);
                        for (int i = 0; i < ls2.Count; i++)
                            for (int j = i + 1; j < ls2.Count; j++)
                            {
                                if (ls2[i] == ls2[j])
                                    ls2.RemoveAt(i);//Console.WriteLine("repetición: " + lista[j]);
                            }
                    }
                    break;
                case PermutationOptions.Reverse:
                    foreach (string s in lista)
                    {
                        string ns = "";
                        for (int i = s.Length - 1; i >= 0; i--)
                            ns += s[i];
                        ls2.Add(ns);
                    }
                    break;
                case PermutationOptions.TwoNumAppend:
                    foreach (string s in lista)
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            ls2.Add(s + i.ToString());
                        }
                        for (int i = 0; i < 10; i++)
                            ls2.Add(s + "0" + i.ToString());
                    }
                    break;
                case PermutationOptions.PreAPEndDate:
                    foreach (string s in lista)
                    {
                        ls2.Add(s + DateTime.Now.Year);
                        ls2.Add(DateTime.Now.Year + s);
                        for (int i = 1; i <= 25; i++)
                        {
                            ls2.Add(s + (DateTime.Now.Year + i));
                            ls2.Add(s + (DateTime.Now.Year - i));
                            ls2.Add((DateTime.Now.Year + i) + s);
                            ls2.Add((DateTime.Now.Year - i) + s);
                        }
                    }
                    break;
                case PermutationOptions.H4x0r:
                    foreach (string ss in lista)
                    {
                        string s = (string)ss.Clone();
                        for (int i = 0; i < s.Length; i++)
                        {
                            for (int j = 0; j < H4x0r.Length; j++)
                                if (s[i] == H4x0r[j] || s[i] == H4x0rUP[j])
                                {
                                    s = s.Replace(s[i], H4x0rChanges[j]);
                                    break;
                                }
                        }
                        ls2.Add(s);
                    }
                    break;
            }
            /*foreach (string strs in ls2)
            {
                sw.WriteLine(strs);
            }
            sw.Flush();*/
        }
        static bool replace = false;
        private static void getH4x0rPerm(ref List<string> ret, string s, int index)
        {
            if (index + 1 > s.Length)
            {
                return;
            }
            replace = false;
            getH4x0rPerm(ref ret, s, index + 1);

            for (int i = 0; i < H4x0r.Length;i++)
                if (s[index] == H4x0r[i] || s[index] == H4x0rUP[i])
                {
                    s = s.Replace(s[index], H4x0rChanges[i]);
                    ret.Add(s);
                    replace = true;
                    break;
                }
            if (replace)
            {
                replace = false;
                getH4x0rPerm(ref ret, s, index + 1);
            }
        }
        private static void getCasePerm(ref List<string> ret , string s, int index)
        {
            //2^#s
            if (index + 1 > s.Length)
            {
                ret.Add(s);
                return;
            }
            getCasePerm(ref ret, s, index + 1);
            s = s.Replace(s[index], Char.ToUpper(s[index]));
            getCasePerm(ref ret, s, index + 1);

        }
        public static List<String> generarPermutaciones(int longitudInicio, int longitudFin)
        {
            return null;
        }

        public static List<String> generarPermutaciones(int longitudInicio, int longitudFin, int palabras)
        {
            return null;
        }

        public static List<String> loadFile(String path, int start)
        {
            List<String> dict = new List<String>();
            if (!File.Exists(path))
                return dict;
            StreamReader r = new StreamReader(File.OpenRead(path));
            string s = String.Empty;
            int i = 0;
            while ((s = r.ReadLine()) != null)
            {
                if (i >= start)
                    dict.Add(s);
                i++;
            }
            //dict.AddRange(words);
            return dict;
        }
    }
}
