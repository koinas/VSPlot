// VsPkg.cs : Implementation of vsplot
//

using System;

using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using System.Text;
using System.Windows.Forms;

using global;
using core;

namespace bukachacha.vsplot
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the registration utility (regpkg.exe) that this class needs
    // to be registered as package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // A Visual Studio component can be registered under different regitry roots; for instance
    // when you debug your package you want to register it in the experimental hive. This
    // attribute specifies the registry root to use if no one is provided to regpkg.exe with
    // the /root switch.
    [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\8.0")]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
    // In order be loaded inside Visual Studio in a machine that has not the VS SDK installed, 
    // package needs to have a valid load key (it can be requested at 
    // http://msdn.microsoft.com/vstudio/extend/). This attributes tells the shell that this 
    // package has a load key embedded in its resources.
    [ProvideLoadKey("Standard", "1.0", "vsplot", "private", 1)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource(1000, 1)]
    // This attribute registers a tool window exposed by this package.
    [ProvideToolWindow(typeof(MyToolWindow))]
    [Guid(GuidList.guidvsplotPkgString)]
    public sealed class vsplot : Package
    {
        const int bitmapResourceID = 300;

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public vsplot()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
            GLOBAL.package_obj = this;
        }

        /// <summary>
        /// This function is called when the user clicks the menu item that shows the 
        /// tool window. See the Initialize method to see how the menu item is associated to 
        /// this function using the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void ShowToolWindow(object sender, EventArgs e)
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            ToolWindowPane window = this.FindToolWindow(typeof(MyToolWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new COMException(Resources.CanNotCreateWindow);
            }
            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        const string pge_path = "D:\\dev\\dlab\\pge\\debug\\pge.dll";
        [DllImport(pge_path)]
        public static extern int test_dbg(IntPtr handle);

        protected override void Initialize()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // Create the command for the tool window
                CommandID toolwndCommandID = new CommandID(GuidList.guidvsplotCmdSet, (int)PkgCmdIDList.cmdidMyTool);
                MenuCommand menuToolWin = new MenuCommand(new EventHandler(ShowToolWindow), toolwndCommandID);
                mcs.AddCommand(menuToolWin);
                //Creating callback for NARISUJ command
                CommandID DrawCommandID = new CommandID(GuidList.guidvsplotCmdSet, (int)PkgCmdIDList.cmdidWatch);
                MenuCommand menuRisujWin = new MenuCommand(new EventHandler(RisujCallback), DrawCommandID);
                mcs.AddCommand(menuRisujWin);
            }

            // Add our command handlers for menu (commands must exist in the .ctc file)
         
        }

        /*public void SetPointNum(int point_num)
        {
            GLOBAL.jadro.SetPointNum(point_num);
        }*/

        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members


        #endregion

        private void RisujCallback(object sender, EventArgs e)
        {
            // Show a Message Box to prove we were here
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            Clipboard.Clear();
            SendKeys.Send("^{c}");
            Application.DoEvents();
            String result = Clipboard.GetText();
            //String result = "		size	-858993460	const int";
            //StringBuilder name = new StringBuilder();
           // StringBuilder value = new StringBuilder();
           // StringBuilder type = new StringBuilder();

            //okno pokazivajem zdes 4tob ne teriat fokus pri CTR C
            ToolWindowPane window = this.FindToolWindow(typeof(MyToolWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new COMException(Resources.CanNotCreateWindow);
            }
            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());


            if (GLOBAL.jadro.AddData(result))
            {
                ToolWindowPane tool_wnd = GLOBAL.package_obj.FindToolWindow(typeof(MyToolWindow), 0, false);
                tool_wnd.Caption = GLOBAL.jadro.GetType() + " " + GLOBAL.jadro.GetVariableName().ToString() + " " + "[" + GLOBAL.pge_io.GetPointNum() + "]";
            }
            else
            {
                ToolWindowPane tool_wnd = GLOBAL.package_obj.FindToolWindow(typeof(MyToolWindow), 0, false);
                tool_wnd.Caption = "Failed to get variable";
            }
            
            ToolWindowPane tool_window = FindToolWindow(typeof(MyToolWindow), 0, true);
            ((MyToolWindow)tool_window).plot_control.Invalidate();
        }

    }
}