using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PSint
{
    public partial class frMain : Form
    {
        string path = "";
        
        bool bTextChanged = false;
        
        public frMain()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool bWorkOn = true;
            
            if (bTextChanged)
            {
                switch (MessageBox.Show("This source file was not saved!\n Do you want to save it?", "Unsaved Changes", MessageBoxButtons.YesNoCancel))
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
                switch (MessageBox.Show("This source file was not saved!\n Do you want to save it?", "Unsaved Changes", MessageBoxButtons.YesNoCancel))
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

            FileStream FS = new FileStream(p, FileMode.OpenOrCreate);

            StreamWriter SW = new StreamWriter(FS);
            SW.Write(textBox1.Text);
            SW.Close();
            
            FS.Close();

            path = p;

            SetCaption();
        }

        private void SetCaption()
        {
            if (path!="")
            {

                this.Text = "PS interpretator - [" + Path.GetFileName(path) + "]";
                            }
            else
            {
                this.Text = "PS interpretator - [Unknown]";
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

        private void textBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyValue)
                {
                    case 'A': textBox1.SelectAll(); break;
                    case 'C': textBox1.Copy(); break;
                    case 'V': textBox1.Paste(); break;
                }
            }
        }

        private void newProjectToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            bool bWorkOn = true;
            
            if (bTextChanged)
            {
                switch (MessageBox.Show("This source file was not saved!\n Do you want to save it?", "Unsaved Changes", MessageBoxButtons.YesNoCancel))
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
            MessageBox.Show("IT'S RUNNING!");
        }
    }
}