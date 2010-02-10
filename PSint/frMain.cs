using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Drawing;

namespace PSint
{
    
    public partial class frMain : Form
    {
        frRun frRun1;

        private bool bReadyToCloseRun = false;

        private string path = "";

        private bool bTextChanged = false;

        public bool bStartBreaking = false;

        public frMain()
        {
            InitializeComponent();
            dialOpen.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            dialSave.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool bWorkOn = true;

            if (bTextChanged)
            {
                switch (
                    MessageBox.Show("This source file was not saved!\n Do you want to save it?", "Unsaved Changes",
                                    MessageBoxButtons.YesNoCancel))
                {
                    case DialogResult.Yes:
                        saveAsToolStripMenuItem_Click(null, EventArgs.Empty);
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
                        bWorkOn = false;
                        break;
                }
            }

            if (bWorkOn) Application.Exit();
        }

        private void dialOpen_FileOk(object sender, CancelEventArgs e)
        {
            LoadFile(dialOpen.FileName);
            bTextChanged = false;
            dialSave.InitialDirectory = Path.GetDirectoryName(dialOpen.FileName);
            dialOpen.InitialDirectory = dialSave.InitialDirectory;
        }

        /// <summary>
        /// Loads file with source to the textbox1. 
        /// </summary>
        /// <param name="sParam">Path to the file</param>
        private void LoadFile(string sParam)
        {
            string s = File.ReadAllText(sParam, Encoding.UTF8);
            textBox1.Clear();
            textBox1.Text = s;
            path = sParam;
            SetCaption();
            reparseAll();
        }

        private void reparseAll()
        {
            int n = 0;
            foreach (string s in textBox1.Lines)
            {
                ParseLine(s, textBox1.GetFirstCharIndexFromLine(n));
                n++;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool bWorkOn = true;

            if (bTextChanged)
            {
                switch (
                    MessageBox.Show("This source file was not saved!\n Do you want to save it?", "Unsaved Changes",
                                    MessageBoxButtons.YesNoCancel))
                {
                    case DialogResult.Yes:
                        saveAsToolStripMenuItem_Click(null, EventArgs.Empty);
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
                        bWorkOn = false;
                        break;
                }
            }

            if (bWorkOn)
            {
                dialOpen.ShowDialog();
                bTextChanged = false;
            }
        }
        
        private void dialSave_FileOk(object sender, CancelEventArgs e)
        {
            SaveFile(dialSave.FileName);
            dialOpen.InitialDirectory = Path.GetDirectoryName(dialSave.FileName);
            dialSave.InitialDirectory = dialOpen.InitialDirectory;
        }

        /// <summary>
        /// Saves Source File, that is opened in textbox1.
        /// </summary>
        /// <param name="sParam">Path to store this file.</param>
        private void SaveFile(string sParam)
        {
            FileStream FS = new FileStream(sParam, FileMode.Create);

            StreamWriter SW = new StreamWriter(FS);
            SW.Write(textBox1.Text);
            SW.Close();

            FS.Close();

            path = sParam;

            SetCaption();
        }

        /// <summary>
        /// This method updates Caption of the Main form.
        /// </summary>
        private void SetCaption()
        {

            if (path != "")
            {
                Text = "PS interpretator - [" + Path.GetFileName(path) + "]";
            }
            else
            {
                Text = "PS interpretator - [Unknown]";
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dialSave.ShowDialog();
            bTextChanged = false;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (path != "")
            {
                SaveFile(path);
            }
            else
                dialSave.ShowDialog();
            bTextChanged = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            bTextChanged = true;
        }

        private void newProjectToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            bool bWorkOn = true;

            if (bTextChanged)
            {

                switch (
                    MessageBox.Show("This source file was not saved!\n Do you want to save it?", "Unsaved Changes",
                                    MessageBoxButtons.YesNoCancel))
                {
                    case DialogResult.Yes:
                        saveAsToolStripMenuItem_Click(null, EventArgs.Empty);
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
                        bWorkOn = false;
                        break;
                }
            }
            if (bWorkOn)
            {
                path = "";
                bTextChanged = false;
                textBox1.Text = "";
                SetCaption();
            }

        }

        /// <summary>
        /// This method runs main source file.
        /// </summary>
        private void runToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("IT'S RUNNING!");
            bStartBreaking = false;
            ShowRun();
            Func main = new Func(textBox1.Text);
            main.Run(this);
            frRun1.textBox2.Text += "\r\nPress ANY KEY...";
            bReadyToCloseRun = true;
            //HideRun();
        }

        /// <summary>
        /// THis method hides Run window.
        /// </summary>
        private void HideRun()
        {
            frRun1.Hide();
            frRun1.Close();
            bReadyToCloseRun = false;
            this.Show();
        }
        /// <summary>
        /// This method shows the Run window.
        /// </summary>
        private void ShowRun()
        {
            bReadyToCloseRun = false;
            frRun1 = new frRun(this);
            frRun1.Show();
            frRun1.textBox2.Clear();
        }     
        /// <summary>
        /// This method shows the Error message.
        /// </summary>
        /// <param name="sParam">Error text.</param>
        /// <param name="nLine">The number of the line, where the exception occured.</param>
        public void Error(string sParam, int nLine)
        {
            MessageBox.Show(sParam + "\nLine №" + nLine, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// This method shows the Error message.
        /// </summary>
        /// <param name="sParam">Error text.</param>
        public void Error(string sParam)
        {
            MessageBox.Show(sParam, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// This method is processing vars.
        /// </summary>
        /// <param name="sParam">String with @vars.</param>
        /// <param name="fFunc">The instance of Func class, which is currently running.</param>
        /// <returns>String, where all @vars are changed to their values.</returns>
        public string processVars(string sParam, Func fFunc)
        {
            string sRet = "";
            string[] splitParam = sParam.Split(' ');
            foreach (string s in splitParam)
            {
                if (s.IndexOf('@') == 0) // if local var
                {
                    sRet += fFunc.getVar(s) + " ";
                }
                else
                    if (s.IndexOf('&') == 0) //if global var
                    {
                        sRet += fFunc.getGlVar(s) + " ";
                    }
                    else sRet += s + " ";
            }
            sRet = sRet.Trim();
            return sRet;
        }

           /// <summary>
           /// Method, processing simple sequence - A <+,-,*,/> B
           /// </summary>
           /// <param name="sParam">Sequence</param>
           /// <param name="fFunc">Func class instance</param>
           /// <returns>Result of the sequence.</returns>
        private string processSimpleSeq(string sParam, Func fFunc)
        {
            if (sParam.IndexOf('+') > -1)
            {
                string[] sSeqParams = sParam.Split('+');
                Base a = new Base();
                a.SetUntyped(sSeqParams[0].Trim());
                Base b = new Base();
                b.SetUntyped(sSeqParams[1].Trim());
                Base c = a + b;
                return c.Get();
            }
            else
                if (sParam.IndexOf('-') > -1)
                {
                    string[] sSeqParams = sParam.Split('-');
                    Base a = new Base();
                    a.SetUntyped(sSeqParams[0].Trim());
                    Base b = new Base();
                    b.SetUntyped(sSeqParams[1].Trim());
                    Base c = a - b;
                    return c.Get();
                }
                else
                    if (sParam.IndexOf('*') > -1)
                    {
                        string[] sSeqParams = sParam.Split('*');
                        Base a = new Base();
                        a.SetUntyped(sSeqParams[0].Trim());
                        Base b = new Base();
                        b.SetUntyped(sSeqParams[1].Trim());
                        Base c = a * b;
                        return c.Get();
                    }
                    else
                        if (sParam.IndexOf('/') > -1)
                        {
                            string[] sSeqParams = sParam.Split('/');
                            Base a = new Base();
                            a.SetUntyped(sSeqParams[0].Trim());
                            Base b = new Base();
                            b.SetUntyped(sSeqParams[1].Trim());
                            Base c = a / b;
                            return c.Get();
                        }
                        else return sParam;
        }

        /// <summary>
        /// This method process some logical sequence and returns result - is it true or false.
        /// </summary>
        /// <param name="sParam">Some sequence</param>
        /// <param name="fFunc">Func class instance</param>
        /// <returns>Is this seq. true or false.</returns>
        public bool processLogicalSeq(string sParam, Func fFunc) 
        {
            bool answer = false;

            string[] logicalMarks = new string[6] { "<=", ">=", "==", "!=", "<", ">"};
            string[] sParams = sParam.Split(logicalMarks, StringSplitOptions.None);
            if (sParams.Length != 1)
            {
                string usedLogic = sParam.Substring(sParams[0].Length, sParam.Length - sParams[0].Length - sParams[1].Length).Trim();
                sParams[0] = processComplicatedSeq(processVars(sParams[0].Trim(), fFunc));
                sParams[1] = processComplicatedSeq(processVars(sParams[1].Trim(), fFunc));
                if (!bStartBreaking)
                {
                    Base a = new Base();
                    a.SetUntyped(sParams[0]);
                    Base b = new Base();
                    b.SetUntyped(sParams[1]);

                    switch (usedLogic)
                    {
                        case "<":
                            answer = (a < b);
                            break;
                        case ">":
                            answer = (a > b);
                            break;
                        case "<=":
                            answer = (a <= b);
                            break;
                        case ">=":
                            answer = (a >= b);
                            break;
                        case "!=":
                            answer = (a != b);
                            break;
                        case "==":
                            answer = (a == b);
                            break;
                    }
                }
                else
                    switch (sParams[0][0])
                    {
                        case '@':
                            try
                            {
                                answer = Convert.ToBoolean(fFunc.getVar(sParams[0].Trim()));
                            }
                            catch
                            {
                                answer = false;
                            }
                            break;
                        case '&':
                            try
                            {
                                answer = Convert.ToBoolean(fFunc.getGlVar(sParams[0].Trim()));
                            }
                            catch
                            {
                                answer = false;
                            }
                            break;
                        default:
                            break;
                    }
            }
            return answer;
        }

        /// <summary>
        /// This method processes Complicated seq.
        /// </summary>
        /// <param name="sParam">Complicated seq.</param>
        /// <returns>Result of calculating this seq.</returns>
        private string processComplicatedSeq(string sParam)
        {
            //sParam = setBrackets(sParam);
            /////Here will be your code
            int iFirst = 0;
            int iLast;
            if (!pairBrackets(sParam))
            {
                if (!bStartBreaking)
                {
                    Error("You have unpaired brackets!");
                    bStartBreaking = true;
                }
                return "false";
            }
            else
            {
                string sRewrite;
                sParam = sParam.Trim();
                while (sParam.IndexOf('(') != -1)
                {
                    for (int i = 0; i < sParam.Length; i++)
                    {
                        if (sParam[i] == '(')
                            iFirst = i; // Trying to find the deepiest bracket pair
                        if (sParam[i] == ')')
                        {
                            iLast = i; // Trying to find the deepiest bracket pair
                            sRewrite = CountExpression(sParam.Substring(iFirst + 1, iLast - iFirst - 1)); // Recount expression in this bracket
                            sParam = sParam.Remove(iFirst, iLast - iFirst + 1); // Delete expression
                            sParam = sParam.Insert(iFirst, sRewrite); // insert answer of this expression
                        }
                    }
                }
                return sParam;
            }
        }

        /// <summary>
        /// This method checks, if brackets are paired. 
        /// </summary>
        /// <param name="sParam">Some expression</param>
        /// <returns>True - brackets are paired
        /// False - not paired.</returns>
        private bool pairBrackets(string sParam)
        {
            int oBrack = 0;
            int cBrack = 0;
            foreach (Char c in sParam)
            {
                switch (c)
                {
                    case '(':
                        oBrack++;
                        break;
                    case ')':
                        cBrack++;
                        break;
                    default:
                        break;
                }
            }
            if (oBrack != cBrack)
            {
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// This method sets all brackets needed in this seq.
        /// </summary>
        /// <param name="sParam">Unparsed seq.</param>
        /// <returns>Seq. with brackets.</returns>
        private string CountExpression(string sParam)
        {
            ///Here will be your code
            int i1, i2;
            bool flag = true;
            List<string> sParams = new List<string>(); // Use List , not string[], because we need to delete some elements and insert some new
            for (i1 = 0 , i2 = 0; i2 < sParam.Length; i2++)
            {
                if (sParam[i2] == '*' || sParam[i2] == '/' || sParam[i2] == '+' || sParam[i2] == '-')
                {
                    sParams.Add(sParam.Substring(i1,i2-i1));
                    i1 = i2 + 1;
                    sParams.Add(sParam[i2].ToString());
                }
            }
            sParams.Add(sParam.Substring(i1));
            Base a = new Base();
            Base b = new Base();
            Base c;
            while (flag)
            {
                flag = false;
                for (int i = 0; i < sParams.Count; i++)
                {
                    if (sParams[i] == "*" || sParams[i] == "/") // In First we find all operations * ans / and count them
                    {
                        a.SetUntyped(sParams[i - 1]);
                        b.SetUntyped(sParams[i + 1]);
                        switch (sParams[i])
                        {
                            case "*":
                                c = a * b;
                                break;
                            case "/":
                                c = a / b;
                                break;
                            default:
                                c = new Base();
                                break;
                        }
                        sParams.RemoveAt(i - 1);
                        sParams.RemoveAt(i);
                        sParams[i - 1] = c.Get();
                        flag = true;
                        break;
                    }
                }
            }
            while (sParams.Count != 1)
            {
                for (int i = 0; i < sParams.Count; i++) // After we work with + and -
                {
                    if (sParams[i] == "+" || sParams[i] == "-")
                    {
                        a.SetUntyped(sParams[i - 1]);
                        b.SetUntyped(sParams[i + 1]);
                        switch (sParams[i])
                        {
                            case "+":
                                c = a + b;
                                break;
                            case "-":
                                c = a - b;
                                break;
                            default:
                                c = new Base();
                                break;
                        }
                        sParams.RemoveAt(i - 1);
                        sParams.RemoveAt(i);
                        sParams[i - 1] = c.Get();
                        break;
                    }
                }
            }
            return sParams[0];
        }
        
        /// <summary>
        /// This method executes one line of code.
        /// </summary>
        /// <param name="cmd">The name of command to execute.</param>
        /// <param name="param">Parameters for this command</param>
        /// <param name="fFunc">The instance of Func class, which is currently running.</param>
        /// <returns>String, which is the result of executing this cmd. </returns>
        public string execCmd(string cmd, string param, Func fFunc)
        {
            ///Here will be (switch), which will run functions for standart cmd signatures
            try
            {
                if (bStartBreaking) { return ""; }
                if ((cmd!="=")&&(cmd!="#in")) ///if we have cmd, that is not changing vars - then process them = change to thier values
                {
                    param=processVars(param,fFunc);
                }

                switch (cmd.ToLower())
                {
                    case "#outstream":
                        param = processVars(param, fFunc);
                        fFunc.sOutput = param.Trim();
                        return "";
                    case "#instream":
                        param = processVars(param, fFunc);
                        fFunc.sInput = param.Trim();
                        return "";
                    case "#temp": // this cmd is used only for testing some features, i.e. processing simple seq.
                        param = processSimpleSeq(param, fFunc); 
                        frRun1.textBox2.Text += param;
                        frRun1.textBox2.Refresh();
                        return "";
                    case "#in":
                        string ret = "";
                        string varName = param.Split(' ')[0];
                        if (fFunc.sInput.ToLower() == "console")
                        {
                            ret = frRun1.gettext();
                        }
                        else 
                        {
                            FileStream FS = new FileStream(fFunc.sInput, FileMode.OpenOrCreate);
                            StreamReader SR = new StreamReader(FS);
                            ret = SR.ReadToEnd();
                            SR.Close();
                            FS.Close();
                        }

                        if (varName.IndexOf('@') == 0) //local var
                        {
                            fFunc.setVar(new Base(varName, ret));
                        }
                        else
                            if (varName.IndexOf('&') == 0) //global var
                            {
                                fFunc.setGlVar(new Base(varName, ret));
                            }

                        return "";

                    case "=":
                        string var1Name = param.Split(' ')[0];
                        param = param.Substring(var1Name.Length + 1);
                        param = processVars(param, fFunc);
                        if (isCmd(param))
                        {
                            string[] sCmdPar = extractCmdParam(param);
                            param = sCmdPar[0] + execCmd(sCmdPar[1], sCmdPar[2], fFunc);
                        }

                        param = param.Trim();

                        if (param[0] == '(' && param[param.Length - 1] == ')')
                        {
                            param = processBlanks(param, fFunc);
                            param = processVars(param, fFunc);
                            param = processComplicatedSeq(param);
                        }
                        long lg;
                        double db;
                        Base b;
                        if (long.TryParse(param,out lg))
                            {
                                b = new Base(var1Name, lg);
                            }
                        else
                            if (double.TryParse(param, out db))
                            {
                                b = new Base(var1Name, db);
                            }
                            else
                            {
                                b = new Base(var1Name, param);
                            }
                       
                            if (var1Name.IndexOf('@') == 0)
                        {
                            fFunc.setVar(b);
                        }
                        else
                            if (var1Name.IndexOf('&') == 0)
                            {
                                fFunc.setGlVar(b);
                            }

                        return "";

                    case "#function":
                        if (isCmd(param))
                        {
                            string[] sCmdPar = extractCmdParam(param);
                            param = sCmdPar[0] + execCmd(sCmdPar[1], sCmdPar[2],fFunc);
                        }
                        string funcPath = param.Split(' ')[0];
                        param = param.Substring(param.IndexOf(' ') + 1);
                        string fullPath="";

                        if (path.Trim() != "")
                        {
                            if (File.Exists(Path.GetDirectoryName(path) + "\\" + funcPath + ".ps"))
                            {
                                fullPath = Path.GetDirectoryName(path) + "\\" + funcPath + ".ps";
                            }
                        }
                        if (fullPath=="")
                            if (File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "\\" + funcPath + ".ps"))
                            {
                                fullPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\" + funcPath + ".ps";
                            }
                        if (fullPath!="")
                        {
                            FileStream FS = new FileStream(fullPath, FileMode.OpenOrCreate);

                            StreamReader SR = new StreamReader(FS);
                            string funcSource = SR.ReadToEnd();
                            SR.Close();
                            FS.Close();

                            Func fnAdditional = new Func(funcSource, param);
                            string sReturn = fnAdditional.Run(this);
                            return sReturn;
                        }
                        else
                            return "Error while launching function " + funcPath;
                    case "#random":///random a b
                        if (isCmd(param))
                        {
                            string[] sCmdPar = extractCmdParam(param);
                            param = sCmdPar[0] + execCmd(sCmdPar[1], sCmdPar[2], fFunc);
                        }
                        Random rnd = new Random(DateTime.Now.Millisecond);
                        string[] par = param.Split(' ');
                        string ans = rnd.Next(Int32.Parse(par[0]), Int32.Parse(par[1])).ToString();
                        param = param.Substring(par[0].Length + 1 + par[1].Length);
                        param.Trim();

                        return ans + param;

                    case "#out":
                        if (isCmd(param))
                        {
                            string[] sCmdPar = extractCmdParam(param);
                            param = sCmdPar[0] + execCmd(sCmdPar[1], sCmdPar[2],fFunc);
                        }
                        ///DELETE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        if (param[0] == '(' && param[param.Length - 1] == ')')
                        {
                            param = processBlanks(param, fFunc);
                            param = processVars(param, fFunc);
                            param = processComplicatedSeq(param);
                        }
                        param = param.Replace("\\n", "\r\n");
                        param = param.Replace("==", "=");
                        if (fFunc.sOutput.ToLower() == "console")
                        {
                            frRun1.textBox2.Text += param;
                            frRun1.textBox2.Refresh();
                        }
                        else
                        {
                            FileStream FS = new FileStream(fFunc.sOutput.Trim(), FileMode.OpenOrCreate);
                            StreamWriter SW = new StreamWriter(FS);
                            SW.Write(param);
                            SW.Close();
                            FS.Close();
                        }
                        return "";

                    case "#time":

                        if (isCmd(param))
                        {
                            string[] sCmdPar = extractCmdParam(param);
                            param = sCmdPar[0] + execCmd(sCmdPar[1], sCmdPar[2], fFunc);
                        }
                        return DateTime.Now.TimeOfDay + " " + param;

                    case "#sleep":
                        System.Threading.Thread.Sleep(Int32.Parse(param));
                        return "";

                    case "#clear": // this command use for clear program - window
                        frRun1.textBox2.Clear();
                        return "";

                    case "#return":
                        if (isCmd(param))
                        {
                            string[] sCmdPar = extractCmdParam(param);
                            param = sCmdPar[0] + execCmd(sCmdPar[1], sCmdPar[2], fFunc);
                        }
                        return param;

                    default:
                        return execCmd("#function", cmd.Remove(0,1) + " " + param, fFunc);
                    //  return "Err";
                }
            }
            catch
            {
                return "Parse_Error! Unknown command!";
            }
        }

        /// <summary>
        /// This method extracts Params from one string to the array.
        /// </summary>
        /// <param name="sParams"></param>
        /// <returns>The array, where: 
        /// [0] = text, before cmd
        /// [1] = cmd
        /// [2] = text, after cmd
        /// </returns>
        public String[] extractCmdParam(String sParams)
        {
            sParams = " " + sParams;
            String[] output = new string[3];
            if (sParams.IndexOf(' ') >= 0)
            {
                output[0] = sParams.Substring(1, sParams.IndexOf(" #"));
                sParams = sParams.Remove(0, sParams.IndexOf(" #") + 1);
                output[1] = sParams.Substring(0, sParams.IndexOf(' '));
                output[2] = sParams.Substring(sParams.IndexOf(' ') + 1);
            }
            else
            {
                output[0] = "";
                output[1] = sParams;
                output[2] = "";
            }
            return output;
        }

        /// <summary>
        /// Checks, if string contains commands at all.
        /// </summary>
        /// <param name="param">String which if looked up for cmd's</param>
        /// <returns>True - there are cmd's.
        /// False - there are no cmd's.
        /// </returns>
        private bool isCmd(string param)
        {
            param = " " + param;
            if (param.Length != 0)
            {
                if (param.IndexOf(" #") > -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
                return false;
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.SelectAll();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Copy();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Cut();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Paste();
        }

        internal void inKbrd(string s)
        {
            if (bReadyToCloseRun)
            {
                HideRun();
            }
            else
                if (frRun1.bGotText==false)
                {
                    frRun1.sEnteredText = s;
                    frRun1.bGotText = true;
                }
        }

        private void onlineWikiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://wiki.github.com/LuckyGeck/PSint/");
        }


        public string processBlanks(string sParam, Func fFunc)
        {
            bool flag;
            char[] Marks = new char[] { '*', '/', '+', '-' /*, ' '*/ , '(', ')' };
            // now space button - is the now onlyist way to split string
            for (int i = sParam.Length - 1; i >= 0; i--)
            {
                flag = false;
                foreach (char cNow in Marks)
                {
                    if (cNow == sParam[i])
                        flag = true;
                }
                if (flag)
                {
                    sParam = sParam.Insert(i + 1, " ");
                    sParam = sParam.Insert(i, " ");
                }
            }
            return sParam;
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
           // textBox1.SuspendLayout();
            int nSelSt = textBox1.SelectionStart;
            int nLine = textBox1.GetLineFromCharIndex(textBox1.GetFirstCharIndexOfCurrentLine());
            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Tab) || (e.KeyCode == Keys.Space) || (e.KeyCode==Keys.Back) || (e.KeyCode==Keys.Delete))
            {
                if (textBox1.Text != "")
                    ParseLine(textBox1.Lines[nLine]);
            }
            if ((e.KeyCode == Keys.Enter) || (e.KeyCode==Keys.Back) || (e.KeyCode==Keys.Delete))
            {
                if (nLine > 1)
                    ParseLine(textBox1.Lines[nLine - 1], textBox1.GetFirstCharIndexFromLine(nLine - 1));
            }
            if (textBox1.SelectionStart!=nSelSt)
            textBox1.SelectionStart = nSelSt;
            //textBox1.ResumeLayout();
        }

        /// <summary>
        /// This method Highlights the current line in textbox
        /// </summary>
        /// <param name="line">Text of the line</param>
        public void ParseLine(string line)
        {
            ParseLine(line, textBox1.GetFirstCharIndexOfCurrentLine());
        }

        //ololo Onotole otake
        /// <summary>
        /// This method highlights a line, that strts from particular symbol.
        /// </summary>
        /// <param name="line">The text of the line.</param>
        /// <param name="nParam">The number of the first symbol in our line.</param>
        public void ParseLine(string line, int nParam)
        {

            int nPos = nParam;
            Regex r = new Regex("([ \\t{}():;])");
            String[] tokens = r.Split(line);

            String[] blueKeywords = { "#in", "#out", "#random", "#time", "#clear", "#sleep", "=" };
            String[] redKeywords = { "#function", "#return", "#instream", "#outstream" };
            String[] greenKeywords = { "#while", "#if", "#endwhile", "#endif" };

            foreach (string token in tokens)
            {
                Color cl = Color.Black;
                bool clSelected = false;

                ///Blue highlighting
                for (int i = 0; i < blueKeywords.Length; i++)
                {
                    if (blueKeywords[i] == token.ToLower())
                    {
                        cl = Color.Blue;
                        clSelected = true;
                        break;
                    }
                }

                ///Red Highlighting
                if (!clSelected)
                    for (int i = 0; i < redKeywords.Length; i++)
                    {
                        if (redKeywords[i] == token.ToLower())
                        {
                            cl = Color.Red;
                            clSelected = true;
                            break;
                        }
                    }

                ///Green highlighting
                if (!clSelected)
                    for (int i = 0; i < greenKeywords.Length; i++)
                    {
                        if (greenKeywords[i] == token.ToLower())
                        {
                            cl = Color.Green;
                            clSelected = true;
                            break;
                        }
                    }

                //Painting!)))
                textBox1.Select(nPos, token.Length);
                textBox1.SelectionColor = cl;

                if (cl != Color.Black)
                    textBox1.SelectionFont = new Font("Verdana", 10, FontStyle.Bold);
                else
                    textBox1.SelectionFont = new Font("Verdana", 10, FontStyle.Regular);

                textBox1.DeselectAll();

                nPos += token.Length; // position in line
            }

            textBox1.SelectionColor = Color.Black;
            textBox1.SelectionFont = new Font("Verdana", 10, FontStyle.Regular);
        }

    }
}
