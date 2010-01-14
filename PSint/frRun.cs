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

        public frRun(frMain form)
        {
            form1 = form;
            InitializeComponent();
        }

        private void frRun_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string s = textBox1.Text;
                textBox1.Text = "";
                form1.inKbrd(s);
            }
        }
    }
}