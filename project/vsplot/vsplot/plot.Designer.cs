using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using global;
using System.Drawing;
using System.Diagnostics;
using System.Timers;
using System.Threading;
using core;
using EnvDTE;
using Microsoft.VisualStudio.Shell;


namespace bukachacha.vsplot
{
    [StructLayout(LayoutKind.Sequential,CharSet = CharSet.Unicode)]
    unsafe public  struct ARRAY_DESCRIPTOR
    {
        //public const int MAX_PLOT_NAME_LENGTH = 20;   
        public uint bytes_per_point;
	    public uint type_int_float;
	    public uint type_signed_unsigned;
	    public char* x;
	    public char* y;
        public bool ignore_x;
        public double point_from;
        public double point_to;
        public double* x_values;
        public double* y_values;
	    public int point_num;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=20)]public string name;
        public double xdiv;
        //public double x_start;
        //public double x_end;
        public double ydiv;
        //public double y_start;
        //public double y_end;
        public  double x_from;
	    public  double x_to;
	    public  double y_from;
	    public  double y_to;
    };

	partial class plot
	{         
        private static ToolTip tip = new ToolTip();
        private bool is_zoom_set = false;
        private Point zoom_from;
     
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

        unsafe protected override void OnPaint(PaintEventArgs e)
        {
            GLOBAL.pge_io.process_paint();
        }

        unsafe protected override void OnLoad(System.EventArgs e)
        {
           GLOBAL.pge_io = new PGE_IO();
           GLOBAL.pge_io.init(Handle);
 
           
        }

        protected override void OnResize(EventArgs e)
        {
            if (GLOBAL.pge_io != null)
            {
                GLOBAL.pge_io.process_resize(Handle, Width, Height);               
                GLOBAL.pge_io.process_paint();
            }
        }

        protected override unsafe void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //tracking popup menu
                ContextMenu mnuContextMenu = new ContextMenu();
                
                
                ////////////////////////////////////////////////////////////////////////////////////
                MenuItem mnuItemPoints = new MenuItem();
                mnuItemPoints.Text = "&Points";
                mnuItemPoints.Click += new System.EventHandler(this.menu1KHandler);
                mnuContextMenu.MenuItems.Add(mnuItemPoints);

                MenuItem menuManual = new MenuItem("Manual"); menuManual.Click += new System.EventHandler(this.menuManualHandler);
                MenuItem menu1K = new MenuItem("1K"); menu1K.Click += new System.EventHandler(this.menu1KHandler);
                MenuItem menu2K = new MenuItem("2K"); menu2K.Click += new System.EventHandler(this.menu2KHandler);
                MenuItem menu4K = new MenuItem("4K"); menu4K.Click += new System.EventHandler(this.menu4KHandler);
                MenuItem menu8K = new MenuItem("8K"); menu8K.Click += new System.EventHandler(this.menu8KHandler);
                //MenuItem menu_autoscale = new MenuItem("Autoscale"); menu_autoscale.Click += new System.EventHandler(this.menu_autoscaleHandler);

                mnuItemPoints.MenuItems.Add(menuManual);
                //mnuItemPoints.MenuItems.Add(menu_autoscale);
                mnuItemPoints.MenuItems.Add(menu1K);
                mnuItemPoints.MenuItems.Add(menu2K);
                mnuItemPoints.MenuItems.Add(menu4K);
                mnuItemPoints.MenuItems.Add(menu8K); 
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////

                MenuItem mnuItemAutoscale = new MenuItem();
                mnuItemAutoscale.Text = "Autoscale";
                mnuItemAutoscale.Click += new System.EventHandler(this.menu_autoscaleHandler);
                mnuContextMenu.MenuItems.Add(mnuItemAutoscale);
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                MenuItem MenuItemPlotType = new MenuItem();
                MenuItemPlotType.Text = "Plot Type";
                //MenuItemPlotType.Click += new System.EventHandler(this.menu_plottype);
                mnuContextMenu.MenuItems.Add(MenuItemPlotType);

                MenuItem menu_point = new MenuItem("Point"); menu_point.Click += new System.EventHandler(this.menu_point_Handler);
                MenuItem menu_line = new MenuItem("Line"); menu_line.Click += new System.EventHandler(this.menu_line_Handler);
                MenuItem menu_figure = new MenuItem("Cross"); menu_figure.Click += new System.EventHandler(this.menu_symbol_Handler);
                MenuItem menu_line_figure = new MenuItem("Line+Cross"); menu_line_figure.Click += new System.EventHandler(this.menu_line_symbol_Handler);
                MenuItemPlotType.MenuItems.Add(menu_point);
                MenuItemPlotType.MenuItems.Add(menu_line);
                MenuItemPlotType.MenuItems.Add(menu_figure);
                MenuItemPlotType.MenuItems.Add(menu_line_figure);
                ////////////////////////////////////////////////////////////////////////////////////////
                MenuItem MenuColor = new MenuItem();
                MenuColor.Text = "Plot Color";
                mnuContextMenu.MenuItems.Add(MenuColor);
                MenuItem MenuColorPoint = new MenuItem("Point"); MenuColorPoint.Click += new System.EventHandler(this.menu_color_point_handler);
                MenuItem MenuColorLine = new MenuItem("Line"); MenuColorLine.Click += new System.EventHandler(this.menu_color_line_handler);
                MenuItem MenuColorSymbol = new MenuItem("Symbol"); MenuColorSymbol.Click += new System.EventHandler(this.menu_color_symbol_handler);
                MenuColor.MenuItems.Add(MenuColorPoint);
                MenuColor.MenuItems.Add(MenuColorLine);
                MenuColor.MenuItems.Add(MenuColorSymbol);
                ////////////////////////////////////////////////////////////////////////////////////////////////
                MenuItem MenuBkgColor = new MenuItem();
                MenuBkgColor.Text = "Background Color";
                mnuContextMenu.MenuItems.Add(MenuBkgColor); MenuBkgColor.Click += new System.EventHandler(this.menu_bkgcolor_handler);
                ////////////////////////////////////////////////////////////////////////////////////////////
                MenuItem MenuSymbolProperties = new MenuItem();
                MenuSymbolProperties.Text = "Size";
                MenuSymbolProperties.Click += new System.EventHandler(this.menu_symbol_properties_handler);
                mnuContextMenu.MenuItems.Add(MenuSymbolProperties);
                

                ////////////////////////////////////////////////////////////////////////////////////////////////
                MenuItem MenuAbout = new MenuItem();
                MenuAbout.Text = "About";
                MenuAbout.Click += new System.EventHandler(this.menu_about_handler);
                mnuContextMenu.MenuItems.Add(MenuAbout);
                //this.ContextMenu = mnuContextMenu;
                mnuContextMenu.Show(this, new Point(e.X, e.Y));
            }
            if (e.Button == MouseButtons.Left)
            {
                if (is_zoom_set)
                {
                    is_zoom_set = false;
                    PGE_IO.apply_zoom(Handle);
                }
                
            }
        }
        protected override unsafe void OnMouseDoubleClick(MouseEventArgs e)
        {
          
                  Point point = PointToClient(Cursor.Position);
                  
                    Point ttpoint = point;
                    point.Y = Height - point.Y;
                    double x_value = 0;
                    double y_value = 0;
                    int result = PGE_IO.get_array_value(GLOBAL.plot_id, point.X, point.Y, &x_value, &y_value);
                    if (result == 0) return;

                    ttpoint.X = ttpoint.X + 15;
                    ttpoint.Y = ttpoint.Y + 5;
                    tip.Show("X: " + x_value + "\r\nY: " + y_value,
                          this, ttpoint);
                
        }

        bool showPopup = false;
        protected unsafe override void OnMouseHover(EventArgs e)
        {

        }

        protected override void OnMouseLeave(EventArgs e)
        {
            showPopup = false;
            tip.Hide(this);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                is_zoom_set = true;
                zoom_from = PointToClient(Cursor.Position);
                zoom_from.Y = Height - zoom_from.Y;
            }
        }
       
        protected override void OnMouseMove(MouseEventArgs e)
        {
            //Debug.WriteLine("MoseMove");
            ResetMouseEventArgs();
            //Debug.WriteLine("OnMouseMove");
           // timer.Stop(); timer.Start();
            // if (showPopup) 
            if (is_zoom_set)
            {
                Point point = PointToClient(Cursor.Position);
                point.Y = Height - point.Y;
                PGE_IO.draw_zoom(Handle, zoom_from.X, zoom_from.Y, point.X - zoom_from.X, point.Y - zoom_from.Y);
                
            }
       
            
            base.OnMouseMove(e);
        }

        private void menu_autoscaleHandler(object sender, System.EventArgs e)
        {
            PGE_IO.autoscale_all(Handle);
        }
        
        private void menu1KHandler(object sender, System.EventArgs e)
        {
            GLOBAL.pge_io.SetPointNum(1024);

            Invalidate();
        }

        private void menu2KHandler(object sender, System.EventArgs e)
        {

            GLOBAL.pge_io.SetPointNum(2048);

            Invalidate();
        }

        private void menu4KHandler(object sender, System.EventArgs e)
        {
            GLOBAL.pge_io.SetPointNum(4096);

            Invalidate();
        }

        private void menu8KHandler(object sender, System.EventArgs e)
        {
            GLOBAL.pge_io.SetPointNum(8192);

            Invalidate();
        }

        private void menuManualHandler(object sender, System.EventArgs e)
        {
            PointNumDlg form = new PointNumDlg();
            form.ShowDialog();
            //form.CenterToScreen();

        }

        private void menu_point_Handler(object sender, System.EventArgs e)
        {
            GLOBAL.pge_io.SetPlotType(0x01);
            Invalidate();
        }

        private void menu_line_Handler(object sender, System.EventArgs e)
        {
            GLOBAL.pge_io.SetPlotType(0x02);
            Invalidate();
        }

        private void menu_symbol_Handler(object sender, System.EventArgs e)
        {
            GLOBAL.pge_io.SetPlotType(0x04); 
            Invalidate();
        }
        
        private void menu_line_symbol_Handler(object sender, System.EventArgs e)
        {
            GLOBAL.pge_io.SetPlotType(0x08);
            Invalidate();
        }

        private void menu_color_point_handler(object sender, System.EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.ShowDialog();
            GLOBAL.pge_io.SetPointColor((float)dlg.Color.R / 255, (float)dlg.Color.G / 255, (float)dlg.Color.B / 255);           
            Invalidate();
        }

        private void menu_color_line_handler(object sender, System.EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.ShowDialog();
            GLOBAL.pge_io.SetLineColor((float)dlg.Color.R / 255, (float)dlg.Color.G / 255, (float)dlg.Color.B / 255);           
            Invalidate();
        }

        private void menu_color_symbol_handler(object sender, System.EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.ShowDialog();
            GLOBAL.pge_io.SetSymbolColor((float)dlg.Color.R / 255, (float)dlg.Color.G / 255, (float)dlg.Color.B / 255);           
            Invalidate();
        }
        private void menu_symbol_properties_handler(object sender, System.EventArgs e)
        {
            SymbolProperties form = new SymbolProperties();
            form.ShowDialog();
        }


        private void menu_bkgcolor_handler(object sender, System.EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.ShowDialog();
            GLOBAL.pge_io.SetBkgColor((float)dlg.Color.R / 255, (float)dlg.Color.G / 255, (float)dlg.Color.B / 255);
            Invalidate();
        }

        private void menu_about_handler(object sender, System.EventArgs e)
        {
            About form = new About();
            form.ShowDialog();
        }
        private void NewItemClick(object sender, System.EventArgs e)
        {

        }

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
this.SuspendLayout();
// 
// plot
// 
this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
this.ClientSize = new System.Drawing.Size(292, 273);
this.ControlBox = false;
this.MaximizeBox = false;
this.MinimizeBox = false;
this.Name = "plot";
this.Text = "plot";
this.ResumeLayout(false);

		}

		#endregion
	}
}