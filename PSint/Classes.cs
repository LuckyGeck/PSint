using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PSint
{
    public class Base
    {
        public string name;
        private long lg;
        private double db;
        private string str;
        private string use; // Show using part of class: Long - lg, Double - db, String - str, Empty - nothing, Error - error;

        public Base()
        {
            Clear();
        }
        

        public Base(string Rname)
        {
            Clear();
            name = Rname;
            use = "Empty";
        }
        
        public Base(string sName,long lParam)
        {
            Clear();
            lg = lParam;
            name = sName;
            use = "Long";           
        }
        
        public Base(string sName,double dParam)
        {
            Clear();
            db = dParam;
            name = sName;
            use = "Double";
        }
        
        public Base(string sName,String sParam)
        {
            Clear();
            str = sParam;
            name = sName;
            use = "String";
        }
        
        public void Clear() // Change variable value to null. Name doesn't change
        {
            use = "Empty";
            lg = 0;
            db = 0;
            str = "";
        }
        
        public static Base operator +(Base a, Base b)
        {
            Base c = new Base();
            if (a.use == b.use)
            {
               c.lg = a.lg + b.lg;
               c.db = a.db + b.db;
               c.str= a.str + b.str;
            }
            return c;
        }
        public static Base operator -(Base a, Base b)
        {
            Base c = new Base();
            if (a.use == b.use)
            {
                c.lg = a.lg - b.lg;
                c.db = a.db - b.db;
                /*while (a.str.IndexOf(b.str) != -1)
                {
                    a.str.Remove(a.str.IndexOf(b.str), a.str.IndexOf(b.str) + b.str.Length);
                }*/
            }
            return c;
        }
        public static Base operator *(Base a, Base b)
        {
        Base c = new Base();
            if (a.use == b.use)
            {
                if(a.use == "Long")
                    c.lg = a.lg * b.lg;
                if(a.use == "Double")
                    c.db = a.db * b.db;
                //a.str *= b.str;
            }
            return c;
        }
        public static Base operator /(Base a, Base b)
        {
            Base c = new Base();
            if (a.use == b.use)
            {
                if (a.use == "Long")
                    c.lg = a.lg + b.lg;
                if (a.use == "Double")
                    c.db = a.db + b.db;
                //a.str /= b.str;
            }
            return c;
        }
    }
    public class Func
    {
        private List<Base> vrb; // Variable
        private string sInput;
        private string sOutput;
        private string sFuncName;
        private string[] code;
        private int nPos;
        public string sReturn = "";
        
        public Func(String s)
        {
            char[] c = "\r\n".ToCharArray();
            code = s.Split(c);
        }

        public string Run(frMain frmain1)
        {
            for (int n = 0; n < code.Length; n++)
            {
                String s = code[n];
                s.Trim();

                if (s != "")
                {
                    if ((s[0] == '#') || (s[0] == '='))
                    {
                        string cmd;
                        string param = "";
                        if (s.IndexOf(' ') >= 0)
                        {
                            cmd = s.Substring(0, s.IndexOf(' '));
                            param = s.Substring(s.IndexOf(' ') + 1);
                        }
                        else
                            cmd = s;

                       frmain1.execCmd(cmd, param);
                    }
                    else
                    {
                        frmain1.Error("Unknown operator", n + 1);
                        // stop running
                        break;
                    }
                }


            }
            return sReturn;
        }
    }

    public class Methods
    {
    }
}
