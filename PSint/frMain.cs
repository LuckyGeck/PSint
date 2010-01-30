using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

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
            textBox1.Text = File.ReadAllText(sParam, Encoding.UTF8);
            path = sParam;
            SetCaption();
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
        /// This method is processing vars.
        /// </summary>
        /// <param name="sParam">String with @vars.</param>
        /// <param name="fFunc">The instance of Func class, which is currently running.</param>
        /// <returns>String, where all @vars are changed to their values.</returns>
        private string processVars(string sParam, Func fFunc)
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

            string[] logicalMarks = new string[6] {"<",">","<=",">=","==","!="};
            string[] sParams = sParam.Split(logicalMarks, StringSplitOptions.None);
            if (sParams.Length != 1) 
            {
                string usedLogic = sParam.Substring(sParams[0].Length, sParam.Length - sParams[0].Length - sParams[1].Length).Trim();
                sParams[0] = processComplicatedSeq(sParams[0].Trim());
                sParams[1] = processComplicatedSeq(sParams[1].Trim());
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
                
            return answer;
        }

        /// <summary>
        /// This method processes Complicated seq.
        /// </summary>
        /// <param name="sParam">Complicated seq.</param>
        /// <returns>Result of calculating this seq.</returns>
        private string processComplicatedSeq(string sParam)
        {
            sParam = setBrackets(sParam);
            ///Here will be your code
            return sParam;
        }

        /// <summary>
        /// This method sets all brackets needed in this seq.
        /// </summary>
        /// <param name="sParam">Unparsed seq.</param>
        /// <returns>Seq. with brackets.</returns>
        private string setBrackets(string sParam)
        {
            ///Here will be your code
            return sParam;
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
                        if (File.Exists(Path.GetDirectoryName(path) + "\\" + funcPath + ".ps"))
                        {
                            string fullPath = Path.GetDirectoryName(path) + "\\" + funcPath + ".ps";
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

                    case "#return":
                        if (isCmd(param))
                        {
                            string[] sCmdPar = extractCmdParam(param);
                            param = sCmdPar[0] + execCmd(sCmdPar[1], sCmdPar[2], fFunc);
                        }
                        return param;

                    default:

                        throw new Exception("Err");
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

    }
}