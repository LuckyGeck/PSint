using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PSint
{
    public partial class frRun : Form
    {
        private frMain form1;
        public bool bGotText = false;
        public string sEnteredText = "";

        public frRun(frMain form)
        {
            form1 = form;
            InitializeComponent();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string s = textBox1.Text;
                sEnteredText = textBox1.Text;
                textBox1.Text = "";
                bGotText = true;
                form1.inKbrd(s);
            }            
        }

        public string gettext()
        {
            bGotText=false;
            this.Activate();
            this.textBox1.Focus();

            while (!bGotText)
            {
                Application.DoEvents(); /// Processing new Events, for example user insert some text into textbox
                System.Threading.Thread.Sleep(50); /// Waiting 50 mili-seconds
            }
            MessageBox.Show(sEnteredText);
            return sEnteredText;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

    }
}