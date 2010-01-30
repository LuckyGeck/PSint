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

        /// <summary>
        /// Constructor of clear object.
        /// </summary>
        public Base()
        {
            Clear();
        }
        
        /// <summary>
        /// Constructor of an empty var with a particular name.
        /// </summary>
        /// <param name="sName">Name of the var.</param>
        public Base(string sName)
        {
            Clear();
            name = sName;
            use = "Empty";
        }
        
        /// <summary>
        /// Constructor of a longint-typed var.
        /// </summary>
        /// <param name="sName">Name of the var.</param>
        /// <param name="lParam">Value of the var (longint)</param>
        public Base(string sName,long lParam)
        {
            Clear();
            lg = lParam;
            name = sName;
            use = "Long";           
        }

        /// <summary>
        /// Constructor of a double-typed var.
        /// </summary>
        /// <param name="sName">Name of the var.</param>
        /// <param name="dParam">Value of the var (double)</param>
        public Base(string sName,double dParam)
        {
            Clear();
            db = dParam;
            name = sName;
            use = "Double";
        }

        /// <summary>
        /// Constructor of a string-typed var.
        /// </summary>
        /// <param name="sName">Name of the var.</param>
        /// <param name="sParam">Value of the var (string)</param>
        public Base(string sName,String sParam)
        {
            Clear();
            str = sParam;
            name = sName;
            use = "String";
        }

        /// <summary>
        /// Constructor, which creates a clone of a particular var.
        /// </summary>
        /// <param name="bParam">Var to clone.</param>
        public Base(Base bParam)
        {
            Clear();
            name = bParam.name;
            use = bParam.use;
            lg = bParam.lg;
            db = bParam.db;
            str = bParam.str;
        }

        /// <summary>
        /// Sets a string value.
        /// </summary>
        /// <param name="sParam">Value of the var.</param>
        public void Set(String sParam) { str = sParam; use = "String"; }

        /// <summary>
        /// Sets longint value to the var.
        /// </summary>
        /// <param name="lParam">Longint value.</param>
        public void Set(long lParam) { lg = lParam; use = "Long"; }

        /// <summary>
        /// Sets a double value to the var
        /// </summary>
        /// <param name="dParam">Double value of the var.</param>
        public void Set(double dParam) { db = dParam; use = "Double"; }

        /// <summary>
        /// Sets a value to the var by copying it form another var.
        /// </summary>
        /// <param name="bParam">Base var to get value from.</param>
        public void Set(Base bParam) 
        { 
            str = bParam.str;
            lg = bParam.lg;
            db = bParam.db;
            use = bParam.use;        
        }

        /// <summary>
        /// Sets a var value of unknown type (type of var is automaticly checked)
        /// Unnamed var.
        /// </summary>
        /// <param name="sParam">Var value</param>
        public void SetUntyped(string sParam) 
        {
            SetUntyped("Unnamed", sParam);
        }

        /// <summary>
        /// Sets a var value of unknown type (type of var is automaticly checked)
        /// </summary>
        /// <param name="sName">Var name</param>
        /// <param name="sParam">Var value</param>
        public void SetUntyped(string sName, string sParam)
        {
            name = sName;

            long lg;
            double db;

            if (long.TryParse(sParam, out lg))
            {
                Set(lg);
            }
            else
                if (double.TryParse(sParam, out db))
                {
                    Set(db);
                }
                else
                {
                    Set(sParam);
                } 
        }
        
        /// <summary>
        /// Gets a value of the var.
        /// </summary>
        /// <returns>Value of the var.</returns>
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

        /// <summary>
        /// Change var's value to null. Name doesn't change
        /// </summary>
        public void Clear() 
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
               c.use = a.use;
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
                c.use = a.use;
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
                c.use = a.use;
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
                c.use = a.use;
            }
            return c;
        }
    }

    public class Func
    {
        static public List<Base> globalVrb = new List<Base>();
        private List<Base> vrb; // Variable
        public string sInput;
        public string sOutput;
        //  private string sFuncName;
        private string[] code;
        //  private int nPos;
        public string sReturn = "";
        private string[] sParams;
        private List<int> nIfStack;

        /// <summary>
        /// Constructor for Func class. 
        /// (Without init params)
        /// </summary>
        /// <param name="sCode">Source of the function.</param>
        public Func(String sCode)
        {
            addConsts();
            nIfStack = new List<int>();
            sInput = "Console";
            sOutput = "Console";
            string[] c = new string[1];
            c[0]="\r\n";
            code = sCode.Split(c,System.StringSplitOptions.None);
        }

        /// <summary>
        /// This method inits Vars list and adds consts to it.
        /// </summary>
        private void addConsts()
        {
            vrb = new List<Base>();
            vrb.Add(new Base("@pi", Math.PI));
            vrb.Add(new Base("@avagadra", 6.02 * Math.Pow(10, 23)));
            vrb.Add(new Base("@g", 10));
            vrb.Add(new Base("@authors", "Pavel Sychev and Semen Mikheynok"));
        }

        /// <summary>
        /// Constructor for Func class. 
        /// </summary>
        /// <param name="sCode">Source of the function.</param>
        /// <param name="sParam">Init params for this function.</param>
        public Func(String sCode, String sParam)
        {
            addConsts();
            nIfStack = new List<int>();
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
            string[] c = new string[1];
            c[0] = "\r\n";
            code = sCode.Split(c, System.StringSplitOptions.None);
        }

        /// <summary>
        /// Checks if a var with sName already exists.
        /// </summary>
        /// <param name="sName">Name of var to search.</param>
        /// <returns>It such var doesn't exist, it returns -1.
        /// If such var exists - it returns Zero-Based number of this var in List.
        /// </returns>
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
        
        /// <summary>
        /// Makes a copy of param var and adds it to the List. 
        /// </summary>
        /// <param name="param">Base-type variable.</param>
        private void addVar(Base param)
        {

            if (varExists(param.name) == -1)
            {
                vrb.Add(new Base(param));
            }
        }

        /// <summary>
        /// Sets the value of param to var from the list, which name is equal to param's name.
        /// If such var in list doesn't exists - it is added. 
        /// </summary>
        /// <param name="param">Base-type variable</param>
        public void setVar(Base param)
        {
            int nNum = varExists(param.name);
            if (nNum == -1) { addVar(param); }
            else vrb[nNum].Set(param);
        }

        /// <summary>
        /// Gets the value of var from the list with the name equal to sName
        /// </summary>
        /// <param name="sName">The name of the param to get.</param>
        /// <returns>Value of var</returns>
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

        /// <summary>
        /// Checks if a global var with sName already exists.
        /// </summary>
        /// <param name="sName">Name of var to search.</param>
        /// <returns>It such var doesn't exist, it returns -1.
        /// If such var exists - it returns Zero-Based number of this var in List.
        /// </returns>
        private int varGlExists(string sName)
        {
            int retVal = -1;
            int counter = 0;
            foreach (Base b in globalVrb)
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

        /// <summary>
        /// Makes a copy of param global var and adds it to the List. 
        /// </summary>
        /// <param name="param">Base-type variable.</param>
        private void addGlVar(Base param)
        {

            if (varGlExists(param.name) == -1)
            {
                globalVrb.Add(new Base(param));
            }
        }

        /// <summary>
        /// Sets the value of param to global var from the list, which name is equal to param's name.
        /// If such var in list doesn't exists - it is added. 
        /// </summary>
        /// <param name="param">Base-type variable</param>
        public void setGlVar(Base param)
        {
            int nNum = varGlExists(param.name);
            if (nNum == -1) { addGlVar(param); }
            else globalVrb[nNum].Set(param);
        }

        /// <summary>
        /// Gets the value of global var from the list with the name equal to sName
        /// </summary>
        /// <param name="sName">The name of the param to get.</param>
        /// <returns>Value of var</returns>
        public string getGlVar(string sName)
        {
            int nNum = varGlExists(sName);
            if (nNum == -1)
            {
                addGlVar(new Base("", sName));
                return "";
            }
            return globalVrb[nNum].Get();
        }

        /// <summary>
        /// This method Runs the function.
        /// </summary>
        /// <param name="frmain1">Link to the main form.</param>
        /// <returns>If this function has return value, it returns this value.</returns>
        public string Run(frMain frmain1)
        {
            bool bBreak = false;
            bool bEndIfSearch = false;
            for (int n = 0; n < code.Length; n++)
            {
                Application.DoEvents();

                if (frmain1.bStartBreaking) 
                {
                    break; 
                }

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

                        switch (cmd.ToLower())
                        {
                            case "#if":
                                nIfStack.Add(n);

                                if (!bEndIfSearch) //if we must process this if
                                {
                                    if (!frmain1.processLogicalSeq(param, this)) // param=>false
                                    { 
                                        bEndIfSearch = true; 
                                    }
                                }
                                break;
                            case "#endif":
                                if (bEndIfSearch)
                                {
                                    nIfStack.Remove(nIfStack.Max());
                                    if (nIfStack.Count == 0) 
                                    {
                                        bEndIfSearch = false;
                                    }
                                }
                                else 
                                {
                                    n = nIfStack.Max() - 1; 
                                }
                                break;
                            case "#return":
                                if (bEndIfSearch) break;
                                sReturn = frmain1.execCmd(cmd, param, this);
                                break;
                            case "#goto":
                                if (bEndIfSearch) break;
                                int nLine = Convert.ToInt32(param.Trim().Split(' ')[0]);
                                if (nLine > code.Length) 
                                { 
                                    frmain1.Error("Unreachable line - " + nLine.ToString(), n + 1); 
                                    bBreak = true; 
                                }
                                else 
                                {
                                    n = nLine - 2;
                                }
                                break;
                            default:
                                if (bEndIfSearch) break;
                                frmain1.execCmd(cmd, param, this);
                                break;
                        }
                        if (bBreak) break;

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
}
