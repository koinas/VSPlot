using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

using global;
using EnvDTE;

namespace bukachacha.vsplot
{
    public partial class PointNumDlg : Form
    {
        public PointNumDlg()
        {
            InitializeComponent();
            textBox1.Text = Convert.ToString(GLOBAL.pge_io.GetPointNum());
        }



        private void OnEditClick(object sender, EventArgs e)
        {
           // GLOBAL.jadro.SetPointNum(100);
            //Close();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                uint number = Convert.ToUInt32(textBox1.Text);
                //GLOBAL.jadro.SetPointNum(number);
                GLOBAL.pge_io.SetPointNum(number);
                Close();
     
            }

            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

    }
}