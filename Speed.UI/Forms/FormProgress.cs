using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Speed.UI.Forms
{

    public partial class FormProgress : Form
    {

        DateTime d1;
        DateTime d2;
        int index = 0;


        public FormProgress()
        {
            InitializeComponent();
        }

        private void FormProgress_Load(object sender, EventArgs e)
        {
            d1 = DateTime.Now;
            label1.Text = label2.Text = "";
            //timer1.Interval = 1000;
            //timer2.Interval = 1000;
            //timer1.Enabled = true;
            //timer2.Enabled = true;
            //timer1.Start();
        }

        public void SetMessage(string message)
        {
            lblMessage.Text = message;
            lblMessage.Invalidate();
            lblMessage.Refresh();
            Application.DoEvents();
        }

        public void StartProgress(int minimum, int maximum)
        {
            progressBar.Value = minimum;
            progressBar.Minimum = minimum;
            progressBar.Maximum = maximum;
            progressBar.Refresh();
            d2 = DateTime.Now;
            if (!timer2.Enabled)
                timer2.Start();
            index = 0;
        }
        public void SetProgress(int value, string message = null)
        {
            progressBar.Value = value;
            progressBar.Refresh();
            if (message != null)
                SetMessage(message);
            timer1_Tick(null, null);
        }

        public void SetProgress(string message = null)
        {
            progressBar.Value = ++index;
            progressBar.Refresh();
            if (message != null)
                SetMessage(message);
            timer1_Tick(null, null);
        }

        public void SetProgress()
        {
            SetProgress(null);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = DateTime.Now.Subtract(d1).ToString();
            label1.Update();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            label2.Text = DateTime.Now.Subtract(d2).ToString();
            label2.Update();
        }

    }

}
