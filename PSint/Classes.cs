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
        public Base(Base bParam)
        {
            Clear();
            name = bParam.name;
            use = bParam.use;
            lg = bParam.lg;
            db = bParam.db;
            str = bParam.str;
        }

        public void Set(String sParam) { str = sParam; use = "String"; }

        public void Set(long lParam) { lg = lParam; use = "Long"; }

        public void Set(double dParam) { db = dParam; use = "Double"; }

        public void Set(Base bParam) 
        { 
            str = bParam.str;
            lg = bParam.lg;
            db = bParam.db;
            use = bParam.use;        
        }
        
        public string Get()
        {
            switch (use)
            {
                case "String":
                    return str;
                case "Long":
                    return lg.ToString();
                case "Double":
                    return db.ToString();
                default: return "";
            }
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
        static public List<Base> globalVrb;
        private List<Base> vrb; // Variable
        public string sInput;
        public string sOutput;
        //  private string sFuncName;
        private string[] code;
        //  private int nPos;
        public string sReturn = "";
        private string[] sParams;
        public Func(String s)
        {
            addConsts();
            char[] c = "\r\n".ToCharArray();
            code = s.Split(c);
            sInput = "Console";
            sOutput = "Console";
        }

        private void addConsts()
        {
            vrb = new List<Base>();
            vrb.Add(new Base("@pi", Math.PI));
            vrb.Add(new Base("@avagadra", 6.02 * Math.Pow(10, 23)));
            vrb.Add(new Base("@g", 10));
            vrb.Add(new Base("@authors", "Pavel Sychev and Semen Mikheynok"));
        }

        public Func(String s, String sParam)
        {
            addConsts();
            sInput = "Console";
            sOutput = "Console";
            if (sParam != "")
            {
                sParams = sParam.Split(' ');
                for (int i = 0; i < sParams.Count(); i++)
                {
                    sParams[i] = sParams[i].Trim();
                    if (sParams[i] != "")
                    {
                        long lg;
                        double db;

                        if (long.TryParse(sParams[i], out lg))
                        {
                            vrb.Add(new Base("@param_" + i.ToString(), lg));
                        }
                        else
                            if (double.TryParse(sParams[i], out db))
                            {
                                vrb.Add(new Base("@param_" + i.ToString(), db));
                            }
                            else
                            {
                                vrb.Add(new Base("@param_" + i.ToString(), sParams[i]));
                            }
                    }

                }

            }
            char[] c = "\r\n".ToCharArray();
            code = s.Split(c);
        }

        private int varExists(string sName)
        {
            int retVal = -1;
            int counter = 0;
            foreach (Base b in vrb)
            {
                if (b.name == sName)
                {
                    retVal = counter;
                    return retVal;
                }
                counter++;
            }
            return retVal;
        }

        private void addVar(Base param)
        {

            if (varExists(param.name) == -1)
            {
                vrb.Add(new Base(param));
            }
        }

        public void setVar(Base param)
        {
            int nNum = varExists(param.name);
            if (nNum == -1) { addVar(param); }
            else vrb[nNum].Set(param);
        }

        public string getVar(string sName)
        {
            int nNum = varExists(sName);
            if (nNum == -1)
            {
                addVar(new Base("", sName));
                return "";
            }
            return vrb[nNum].Get();
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
                        if (cmd == "#return")
                        {
                            sReturn = frmain1.execCmd(cmd, param, this);
                        }
                        else
                        {
                            frmain1.execCmd(cmd, param, this);
                        }

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
