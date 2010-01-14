using System;
using System.ComponentModel;
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

        public frMain()
        {
            InitializeComponent();
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
        }

        private void LoadFile(string p)
        {
            textBox1.Text = File.ReadAllText(p, Encoding.UTF8);
            path = p;
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
        }

        private void SaveFile(string p)
        {
            FileStream FS = new FileStream(p, FileMode.Create);

            StreamWriter SW = new StreamWriter(FS);
            SW.Write(textBox1.Text);
            SW.Close();

            FS.Close();

            path = p;

            SetCaption();
        }

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

        private void runToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("IT'S RUNNING!");
            ShowRun();
            for (int n = 0; n < textBox1.Lines.Length; n++)
            {
                string s = textBox1.Lines[n];
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

                        execCmd(cmd, param);
                  }
                    else
                    {
                        Error("Unknown operator", n + 1);
                        break;
                    }
                }

            }
            frRun1.textBox2.Text += "\nPress ANY KEY...";
            bReadyToCloseRun = true;
            //HideRun();
        }

        private void HideRun()
        {
            frRun1.Hide();
            frRun1.Close();
            bReadyToCloseRun = false;
            this.Show();
        }

        private void ShowRun()
        {
            bReadyToCloseRun = false;
            frRun1 = new frRun(this);
            frRun1.Show();
            frRun1.textBox2.Clear();
        }

        private string execCmd(string cmd, string param)
        {
            //Here will be (switch), which will run functons for stndart cmd signatures

            //MessageBox.Show("Command=" + cmd + "\nParams='" + param + "'");
            switch (cmd)
            {
                case "#out":
                    frRun1.textBox2.Text += param;
                    /*MessageBox.Show(param); */
                    return "OK";
                case "#Time":
                    frRun1.textBox2.Text += DateTime.Now.TimeOfDay;
                    return "OK";
                case "#Sleep":
                    System.Threading.Thread.Sleep(Int32.Parse(param));
                    return "OK";
                default: return "Err";
            }
        }

        private void Error(string p, int n)
        {
            MessageBox.Show(p + "\nLine №" + n, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        }
    }
}