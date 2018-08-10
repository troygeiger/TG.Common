using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TG.Common;

namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WaitForm.ShowForm("M", this);
            timer1.Start();
            //Thread.Sleep(3000);
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WaitForm.CloseForm();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            WaitForm.CloseForm();
        }
    }
}
