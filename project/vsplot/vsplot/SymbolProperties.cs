using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

using global;

namespace bukachacha.vsplot
{
    public partial class SymbolProperties : Form
    {
        public SymbolProperties()
        {
            InitializeComponent();
            textBox1.Text = Convert.ToString(GLOBAL.pge_io.GetSymbolSize());
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
                int number = Convert.ToInt32(textBox1.Text);
                //GLOBAL.jadro.SetPointNum(number);
                GLOBAL.pge_io.SetSymbolSize(number);
                Close();
                
                ToolWindowPane tool_window = GLOBAL.package_obj.FindToolWindow(typeof(MyToolWindow), 0, true);
                ((MyToolWindow)tool_window).plot_control.Invalidate();

            }

            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

    }
}