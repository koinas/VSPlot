
using System;

using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using System.Windows.Forms;
using EnvDTE;
using System.Text;
using bukachacha.vsplot;
using global;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Reflection;

namespace global
{
    using core;
    unsafe class GLOBAL
    {
        //public static uint MAX_POINT_NUM = 1024*100;
        //public static uint MAX_PAGE_SIZE = (MAX_POINT_NUM + 1) * sizeof(float);
        public static int point_num = 0;
        //public static double[] data_array = new double[MAX_POINT_NUM];
        //public static bool is_update = false;
        public static plot plot_global;
        public static core.Core jadro;
        public static char* data_pointer = (char*)0;
        public static PGE_IO pge_io;
        public static int plot_id;
        public static vsplot package_obj;
    }
}

namespace core
{

     public struct MEMORY_BASIC_INFORMATION {
            int  BaseAddress;
            int  AllocationBase;
            int  AllocationProtect;
            public uint RegionSize;
            public int  State;
            int  Protect;
            int  Type;
            };



    public struct SYSTEM_INFO
    {
        public ushort processorArchitecture;
        ushort reserved;
        public uint pageSize;
        public IntPtr minimumApplicationAddress;  // minimum address
        public IntPtr maximumApplicationAddress;  // maximum address
        public IntPtr activeProcessorMask;
        public uint numberOfProcessors;
        public uint processorType;
        public uint allocationGranularity;
        public ushort processorLevel;
        public ushort processorRevision;
    }


    public class Core
    {
        public const uint TYPE_INT	= 0x00;
        public const uint TYPE_FLOAT	= 0x01;
        public const uint TYPE_SIGNED	= 0x02;
        public const uint TYPE_UNSIGNED  = 0x03;

        public const UInt32 INVALID_HANDLE_VALUE = 0xffffffff;
        public const UInt32 PAGE_EXECUTE_READWRITE = 0x00000040;
        public const UInt32 FILE_MAP_READ = 0x04;

        private uint bytes_per_point;
        private uint int_float;
        private uint signed_unsigned;

        [DllImport("Kernel32.dll")]public static extern IntPtr OpenFileMapping(uint access, bool inherit, string name);
        [DllImport("Kernel32.dll")]public static extern IntPtr MapViewOfFile(IntPtr h, uint desired_access, uint high, uint low, uint size);
        [DllImport("Kernel32.dll")]public static extern void CloseHandle(IntPtr handle);
        [DllImport("Kernel32.dll")]public static extern void UnmapViewOfFile(IntPtr pointer);
        [DllImport("Kernel32.dll")]public static extern uint VirtualQueryEx(IntPtr handle, IntPtr address, ref MEMORY_BASIC_INFORMATION info, int size);
        [DllImport("Kernel32.dll")]public static extern void GetSystemInfo(ref SYSTEM_INFO info);
        [DllImport("Kernel32.dll")]public static extern IntPtr OpenProcess(uint desired_access, bool inherit_handle, int process_id);

        [System.Runtime.InteropServices.DllImportAttribute("kernel32.dll", EntryPoint = "ReadProcessMemory")]
        [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool ReadProcessMemory([System.Runtime.InteropServices.InAttribute()] System.IntPtr hProcess, [System.Runtime.InteropServices.InAttribute()] System.IntPtr lpBaseAddress, System.IntPtr lpBuffer, uint nSize, System.IntPtr lpNumberOfBytesRead);

        /// Return Type: DWORD->unsigned int
        [System.Runtime.InteropServices.DllImportAttribute("kernel32.dll", EntryPoint = "GetLastError")]public static extern uint GetLastError();

        static System.Threading.EventWaitHandle data_processed_event;
        static System.Threading.EventWaitHandle data_received_event;

        public const uint PROCESS_VM_READ = (0x0010);
        public const uint PROCESS_QUERY_INFORMATION = 0x0400;
        public const uint MEM_COMMIT = 0x1000;

        private uint point_num = 1024;
        private uint addr = 0x00;

        private uint PAGE_SIZE = 0;

        StringBuilder name;
        String data_type_string;
        

        public DataTypes data_types;
        public class DataTypes
        {
            [XmlElement("DataType")]
            public List<DataType> data_type = new List<DataType>();
        };

        public class DataType
        {
            public string name;
            public uint bytes_per_point;
            public uint float_int;
            public uint signed_unsigned;
        };

        public  Core()
        {
            SYSTEM_INFO info = new SYSTEM_INFO();
            GetSystemInfo(ref info);
            PAGE_SIZE = info.pageSize;
            //dbg //mbd
            //CreateDataTypesXml();
            
        }
        
        void OutputMessage(string message)
        {
            DTE dte = (DTE)Package.GetGlobalService(typeof(DTE));
            Window window = dte.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);
            OutputWindow outputWindow = (OutputWindow)window.Object;
            if (outputWindow == null) return;
            if (outputWindow.ActivePane == null)
            {
                outputWindow.OutputWindowPanes.Add("VSPlot");
            }
            else
                outputWindow.ActivePane.Activate();
            
            outputWindow.ActivePane.OutputString(message+"\n");
        }

        public vsplot package_obj;

        public bool ReadDataTypes()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataTypes));
            try
            {

                string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                dir += "\\data_types.xml";
                using (TextReader reader = new StreamReader(dir))
                {
                    data_types = new DataTypes();
                    data_types = (DataTypes)(serializer.Deserialize(reader));
                }
            }
            catch (Exception e)
            {
                 MessageBox.Show(e.Message);
                 return false;
            }
            return true;
        }

        public void CreateDataTypesXml()
        {
            //this is for initial XML creation
            DataTypes dts = new DataTypes();
            DataType dt = new DataType();
            dt.name = "float";
            dt.bytes_per_point = 4;
            dt.float_int = 1;
            dt.signed_unsigned = 0;
            dts.data_type.Add(dt);

            dt = new DataType();
            dt.name = "double";
            dt.bytes_per_point = 8;
            dt.float_int = 1;
            dt.signed_unsigned = 0;
            dts.data_type.Add(dt);

            dt = new DataType();
            dt.name = "unsigned char";
            dt.bytes_per_point = 1;
            dt.float_int = 0;
            dt.signed_unsigned = 1;
            dts.data_type.Add(dt);

            XmlSerializer serializer = new XmlSerializer(typeof(DataTypes));
            try
            {
                string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                dir += "\\data_types.xml";
                using (TextWriter writer = new StreamWriter(dir))
                {
                    serializer.Serialize(writer, dts);
                }
            }
            catch (Exception e)
            {
            }
        }
        

        public void InitCore(vsplot obj)
        {
            ReadDataTypes();
            package_obj = obj;


        }

        unsafe  void thread_proc(vsplot obj)
        {
          
        }
        unsafe public bool LoadCfg()
        {
 
            return false;
        }

        unsafe public bool ReadProcessM(uint address, uint point_num, uint bytes_per_point,IntPtr result,ref uint point_num_read)
        {
            //detecting how many points we can read
            //detecting how many pages age commitied 
            //reading them 
            //truncating point_num if necessary
            point_num_read = 0;
            // here we must check if the process is available for debugging
            DTE dte = (DTE)Package.GetGlobalService(typeof(DTE));
            EnvDTE.Debugger dbg = dte.Debugger;
            EnvDTE.Process current_process = dbg.CurrentProcess;
            if (current_process == null) return false;

            int process_id = ((DTE)Package.GetGlobalService(typeof(DTE))).Debugger.CurrentProcess.ProcessID;
            IntPtr handle = OpenProcess(PROCESS_VM_READ | PROCESS_QUERY_INFORMATION, false, process_id);
            if (handle.ToInt32() == 0) return false;

            uint variable_address = address;
            uint tmp = address / PAGE_SIZE; //avoiding optimisation
            uint base_address = tmp * PAGE_SIZE;
            IntPtr base_address_param = new IntPtr(base_address);
            MEMORY_BASIC_INFORMATION info = new MEMORY_BASIC_INFORMATION();
            uint err = VirtualQueryEx(handle, base_address_param, ref info, sizeof(MEMORY_BASIC_INFORMATION));
            if (err == 0) return false;
            if (info.State != MEM_COMMIT) return false;
            //calculating number of points commited
            uint region_size = info.RegionSize;
            uint variable_address_offset = variable_address - base_address;
            uint bytes_available = info.RegionSize - variable_address_offset;
            uint point_num_available = bytes_available / bytes_per_point;
            if (point_num > point_num_available) point_num = point_num_available;
            //reading available data
            uint bytes_requested = point_num * bytes_per_point;
            int result_size = 0x00;
            IntPtr result_size_param = new IntPtr(&result_size);
            IntPtr variable_address_param = new IntPtr(variable_address);
            bool r = ReadProcessMemory(handle, variable_address_param, result, bytes_requested, result_size_param);
            if (r == false) return false;
            if (result_size != bytes_requested) return false;
            point_num_read = point_num;
            CloseHandle(handle);
            return true;
        }

         private void parse_watch(String input, ref StringBuilder name, ref StringBuilder value, ref StringBuilder type)
        {
            //This fuction extracts variable name and type from the Clipboard on CtrC command 
            //on a selected item in the Watch window
            //input looks like this
            //            size	-858993460	const int\r
            //\t as delimiters
            //sometimes there is "" for char pointers so " is delimieter also
            //space is also delimiter
            //const int is a type
            //size is variable name

            OutputMessage("Input String: " + input);

            int words = 0;
            char delimiter = (char)(0x09);  // \t
            char delimiter1 = (char)(0x22); //"
            char delimiter2 = (char)(0x00); // space is not delimiter
            char endline = '\r';
            for (int i = 0; i < input.Length; ++i)
            {
                if (input[i] == endline) break;
                if (((input[i] == delimiter) || (input[i] == delimiter1) || (input[i] == delimiter2)) && ((input[i + 1] != delimiter) && (input[i + 1] != delimiter1) && (input[i + 1] != delimiter2))) ++words;
                if ((input[i] != delimiter) && (input[i] != delimiter1) && (input[i] != delimiter2) && (words == 1)) name.Append(input[i]);
                if ((input[i] != delimiter) && (input[i] != delimiter1) && (input[i] != delimiter2) && (words == 2)) value.Append(input[i]);
                if ((input[i] != delimiter) && (input[i] != delimiter1) && (input[i] != delimiter2) && (words == 3)) type.Append(input[i]);
            }

            //also there must be numbers 
            //like 0x01020304 {1.7800000}
            //in vs1023
            //removing averything except
            //first 10 symbols
            {
                int value_length = value.Length;

                value.Remove(10, value_length - 1 - 9);
            }
            //the type may contain [] like
            //float[num]
            //in this case we removing num]
            //float [256] -> float [
            int length = type.Length;
            int idx = 0;
            for (idx = 0; idx < length; ++idx)
                if (type[idx] == '[') break;
            if (idx!=length)
            type.Remove(idx + 1, length - idx - 1);

            OutputMessage("Parse Result: " + name.ToString() + " " + value.ToString() + " " + type.ToString());

        }

        // void analyze_type(ref StringBuilder type,ref int is_signed
        public bool GetType(StringBuilder type, ref uint bytes_per_point, ref uint float_int,ref uint signed_unsigned)
        {
            int size = data_types.data_type.Count;
            for (int i = 0; i < size; ++i)
            {                
               if(String.Compare(type.ToString(),data_types.data_type[i].name) == 0)
                //if(type.ToString().Contains(data_types.data_type[i].name))
                {                   
                    bytes_per_point = data_types.data_type[i].bytes_per_point;
                    float_int = data_types.data_type[i].float_int;
                    signed_unsigned = data_types.data_type[i].signed_unsigned;                
                    data_type_string = data_types.data_type[i].name;
                    return true;
                }
            }
            return false;
        }

        unsafe public bool AddData(String unparsed)
        {
            name = new StringBuilder();
            StringBuilder value = new StringBuilder();
            StringBuilder type = new StringBuilder();
            parse_watch(unparsed, ref name, ref value, ref type);


            try
            {
                addr = Convert.ToUInt32(value.ToString(), 16);
            }
            catch (Exception e)
            {
                return false;
            }


            if (GetType(type, ref bytes_per_point, ref int_float, ref signed_unsigned))
            {        
                OutputMessage("Trying to read " + name.ToString() + " of type: " + type.ToString() + " [ " + point_num + " ]" + " points " + "at address: " +"0x"+addr.ToString("x8"));
                DrawData();
                               
            }
            else
            {
                OutputMessage("Failed to get variable");
                return false;
            }

            return true;
        }



        unsafe public void DrawData()
        {
            uint byte_num = point_num * bytes_per_point;
             IntPtr variable_data;
            try
            {
                variable_data = Marshal.AllocHGlobal((IntPtr)byte_num);
            }
            catch (OutOfMemoryException e)
            {
                OutputMessage("Out Of Memory in AllocHGlobal");
                return ;
            }
            
            uint point_num_read = 0;

            OutputMessage("Reading memory: " + "0x"+addr.ToString("x8") + " point num: " + point_num);

            bool result = ReadProcessM(addr, point_num, bytes_per_point, variable_data, ref point_num_read);
            
            

            if (result == false)
            {
                Marshal.FreeHGlobal(variable_data);
                OutputMessage("Failed to read memory");
                return;
            }
            
            point_num = (uint)point_num_read;
            GLOBAL.point_num = (int)point_num_read;



            ARRAY_DESCRIPTOR descriptor = new ARRAY_DESCRIPTOR();
            descriptor.point_num = GLOBAL.point_num;
            descriptor.bytes_per_point = bytes_per_point;
            descriptor.type_int_float = int_float;
            descriptor.type_signed_unsigned = signed_unsigned;

            descriptor.y = (char*)variable_data;
            descriptor.ignore_x = true;


            
            PGE_IO.set_array(GLOBAL.plot_id, ref descriptor);

            Marshal.FreeHGlobal(variable_data);

        }

        public uint SetPointNum(uint pn)
        {
            point_num = pn;
            DrawData();
            return point_num;
        }

        public uint GetPointNum()
        {
            return (uint)point_num;
        }
        public string GetVariableName()
        {
            return name.ToString();
        }
        public string GetType()
        {
            return data_type_string;
        }
    };

    public class PGE_IO
    {
        //const string pge_path = "D:\\dev\\dlab\\pge\\debug\\pge.dll";
        const string pge_path = "pge.dll";
        [DllImport(pge_path)]
        public static extern int create_array_render(IntPtr handle);
        [DllImport(pge_path)]
        public static extern int create_window_render(IntPtr handle);
        [DllImport(pge_path)]
        public static extern int set_array(int plot_id, ref ARRAY_DESCRIPTOR desc);
        [DllImport(pge_path)]
        public static extern int draw_id(int render_id);
        [DllImport(pge_path)]
        public static extern int begin_draw(IntPtr handle);
        [DllImport(pge_path)]
        public static extern int resize(IntPtr handle, int width, int height);
        [DllImport(pge_path)]
        private static extern int dbg_set_struct(ref ARRAY_DESCRIPTOR desc);
        [DllImport(pge_path)]
        private static extern int set_plot_type_raw(IntPtr handle, int plot_type);
        [DllImport(pge_path)]
        private static extern int set_frame(int id, bool is_enabled);
        [DllImport(pge_path)]
        public static extern int draw_zoom(IntPtr handle, int x,int y,int widht,int height);
        [DllImport(pge_path)]
        public static extern int apply_zoom(IntPtr handle);
        [DllImport(pge_path)]
        public static extern int autoscale_all(IntPtr handle);
        [DllImport(pge_path)]
        public static extern int set_data_type(int plot_id, int type);
        [DllImport(pge_path)]
        public static extern int set_array_point_color(int plot_id, float red,float green,float blue);
        [DllImport(pge_path)]
        public static extern int set_array_line_color(int plot_id, float red, float green, float blue);
        [DllImport(pge_path)]
        public static extern int set_array_figure_color(int plot_id, float red, float green, float blue);
        [DllImport(pge_path)]
        public unsafe static extern int get_array_value(int plot_id, int x,int y,double* x_val,double* y_val);
        [DllImport(pge_path)]
        public unsafe static extern int set_array_figure_size(int plot_id, int size);
        [DllImport(pge_path)]
        public unsafe static extern int set_bkg_color(int plot_id, float red, float green, float blue);        
        [DllImport(pge_path)]
        public static extern int test_dbg(IntPtr handle);


        private bool is_alive = false;

        [Serializable()]
        public class PlotParameters
        {
            public float line_color_red;
            public float line_color_green;
            public float line_color_blue;

            public float point_color_red;
            public float point_color_green;
            public float point_color_blue;

            public float cross_color_red;
            public float cross_color_green;
            public float cross_color_blue;

            public float background_color_red;
            public float background_color_green;
            public float background_color_blue;

            public int plot_type;
            public int figure_size;
            public uint point_num;
        };


        private bool check_dll()
        {
            try
            {
                Marshal.PrelinkAll(this.GetType());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
            return true;
        }
        public void SavePlotParameters()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PlotParameters));
                string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                dir += "\\config.xml";
                using (TextWriter writer = new StreamWriter(dir))
                {
                    serializer.Serialize(writer, plot_param);
                }
            }
            catch (Exception e)
            {
            }
        }

        public void RestorePlotParameters()
        {
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(PlotParameters));
                string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                dir += "\\config.xml";
                using (TextReader reader = new StreamReader(dir))
                {
                    plot_param = (PlotParameters)(deserializer.Deserialize(reader));
                }
            }
            catch (Exception e)
            {
                //specifying default parameters
                plot_param.point_color_red = 1;
                plot_param.point_color_green = 0;
                plot_param.point_color_blue = 0;

                plot_param.line_color_red = 1;
                plot_param.line_color_green = 0;
                plot_param.line_color_blue = 0;

                plot_param.cross_color_red = 0;
                plot_param.cross_color_green = 0;
                plot_param.cross_color_blue = 1;

                plot_param.background_color_red = 1;
                plot_param.background_color_green = 1;
                plot_param.background_color_blue = 1;

                plot_param.plot_type = 0x02;
                plot_param.figure_size = 12;
                plot_param.point_num = 1024;
                SavePlotParameters();
            }

        }

        int wnd_id = 0;
        IntPtr wnd_handle;
        public PlotParameters plot_param;
        unsafe public bool init(IntPtr handle)
        {

            if (check_dll() == false)
            {
                is_alive = false;
                return false;
            }
            is_alive = true;

            GLOBAL.jadro = new Core();
            GLOBAL.jadro.InitCore(GLOBAL.package_obj);
          

            
            wnd_id = create_window_render(handle);
            GLOBAL.plot_id = create_array_render(handle);
            set_frame(GLOBAL.plot_id, true);
            wnd_handle = handle;
            plot_param = new PlotParameters();
            RestorePlotParameters();

            SetPlotType(plot_param.plot_type);
            
            SetPointColor(plot_param.point_color_red, plot_param.point_color_green, plot_param.point_color_blue);
            SetLineColor(plot_param.line_color_red, plot_param.line_color_green, plot_param.line_color_blue);
            SetSymbolColor(plot_param.cross_color_red, plot_param.cross_color_green, plot_param.cross_color_blue);
            SetPointNum(plot_param.point_num);
            SetSymbolSize(plot_param.figure_size);
            SetBkgColor(plot_param.background_color_red, plot_param.background_color_green, plot_param.background_color_blue);
            
            
            return true;
        }

        unsafe public void process_paint()
        {
            if(is_alive)
            begin_draw(wnd_handle);
        }
        
        unsafe public void process_resize(IntPtr handle,int width,int height)
        {
            if (is_alive)
            {
                resize(handle, width, height);
            }
        }


        public void SetPlotType(int type)
        {
            plot_param.plot_type = type;
            SavePlotParameters();
            if (is_alive)
            set_data_type(GLOBAL.plot_id, plot_param.plot_type);
           
           

        }

        public void SetPointColor(float red, float green,float blue)
        {
            plot_param.point_color_red = red;
            plot_param.point_color_green = green;
            plot_param.point_color_blue = blue;
            SavePlotParameters();
            if (is_alive)
            set_array_point_color(GLOBAL.plot_id, plot_param.point_color_red, plot_param.point_color_green, plot_param.point_color_blue);

        }

        public void SetLineColor(float red, float green, float blue)
        {
            plot_param.line_color_red = red;
            plot_param.line_color_green = green;
            plot_param.line_color_blue = blue;
            SavePlotParameters();
            if (is_alive)
            set_array_line_color(GLOBAL.plot_id, plot_param.line_color_red, plot_param.line_color_green, plot_param.line_color_blue);

            
        }

        public void SetSymbolColor(float red, float green, float blue)
        {
            plot_param.cross_color_red = red;
            plot_param.cross_color_green = green;
            plot_param.cross_color_blue = blue;
            SavePlotParameters();
            if (is_alive)
            set_array_figure_color(GLOBAL.plot_id, plot_param.cross_color_red, plot_param.cross_color_green, plot_param.cross_color_blue);

           
        }

        public void SetSymbolSize(int size)
        {
            plot_param.figure_size = size;
            SavePlotParameters();
            if (is_alive)
            set_array_figure_size(GLOBAL.plot_id, plot_param.figure_size);

           
        }

        public void SetPointNum(uint point_num)
        {
           plot_param.point_num = GLOBAL.jadro.SetPointNum(point_num);
           SavePlotParameters();

           ToolWindowPane tool_wnd = GLOBAL.package_obj.FindToolWindow(typeof(MyToolWindow), 0, false);
           if (tool_wnd != null)
           {
               tool_wnd.Caption = GLOBAL.jadro.GetType() + " " + GLOBAL.jadro.GetVariableName().ToString() + " " + "[" + GLOBAL.pge_io.GetPointNum() + "]";
               ((MyToolWindow)tool_wnd).plot_control.Invalidate();
           }

        }

        public uint GetPointNum()
        {
            return plot_param.point_num;
        }

        public int GetSymbolSize()
        {
            return plot_param.figure_size;
        }

        public void SetBkgColor(float red, float green, float blue)
        {
            plot_param.background_color_red = red;
            plot_param.background_color_green = green;
            plot_param.background_color_blue = blue;
            SavePlotParameters();
            if (is_alive)
            set_bkg_color(GLOBAL.plot_id, plot_param.background_color_red, plot_param.background_color_green, plot_param.background_color_blue);

            
        }

    };


}