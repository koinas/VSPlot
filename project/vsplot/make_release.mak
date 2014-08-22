
#define macros
EXECUTABLE_NAME = vsplot.dll
DIR_SRC = vsplot

SRC_FILES= \
  $(DIR_SRC)\About.cs \
  $(DIR_SRC)\About.Designer.cs \
  $(DIR_SRC)\core.cs \
  $(DIR_SRC)\MyControl.cs \
  $(DIR_SRC)\MyControl.Designer.cs \
  $(DIR_SRC)\MyToolWindow.cs \
  $(DIR_SRC)\Guids.cs \
  $(DIR_SRC)\plot.cs \
  $(DIR_SRC)\plot.Designer.cs \
  $(DIR_SRC)\PointNumDlg.cs \
  $(DIR_SRC)\PointNumDlg.designer.cs \
  $(DIR_SRC)\Resources.Designer.cs \
  $(DIR_SRC)\SymbolProperties.cs \
  $(DIR_SRC)\SymbolProperties.Designer.cs \
  $(DIR_SRC)\VsPkg.cs \
  $(DIR_SRC)\Properties\AssemblyInfo.cs \
  $(DIR_SRC)\PkgCmdID.cs \

REF_FILES = \
  "C:\Program Files\Microsoft Visual Studio 8\Common7\IDE\PublicAssemblies\EnvDTE.dll" \
  "C:\Program Files\Visual Studio 2005 SDK\2007.02\VisualStudioIntegration\Common\Assemblies\Microsoft.VisualStudio.OLE.Interop.dll" \
  "C:\Program Files\Visual Studio 2005 SDK\2007.02\VisualStudioIntegration\Common\Assemblies\Microsoft.VisualStudio.Shell.dll" \
  "C:\Program Files\Visual Studio 2005 SDK\2007.02\VisualStudioIntegration\Common\Assemblies\Microsoft.VisualStudio.Shell.Interop.8.0.dll" \
  "C:\Program Files\Visual Studio 2005 SDK\2007.02\VisualStudioIntegration\Common\Assemblies\Microsoft.VisualStudio.Shell.Interop.dll" \
  "C:\Program Files\Visual Studio 2005 SDK\2007.02\VisualStudioIntegration\Common\Assemblies\Microsoft.VisualStudio.TextManager.Interop.dll" \
  "C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\System.Data.dll" \
  "C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\System.Design.dll" \
  "C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\System.dll" \
  "C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\System.Drawing.dll" \
  "C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\System.Windows.Forms.dll" \
  "C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\System.Xml.dll" 
  
REF1 = "C:\Program Files\Microsoft Visual Studio 8\Common7\IDE\PublicAssemblies\EnvDTE.dll"
REF2 = "C:\Program Files\Visual Studio 2005 SDK\2007.02\VisualStudioIntegration\Common\Assemblies\Microsoft.VisualStudio.OLE.Interop.dll" 
REF3 = "C:\Program Files\Visual Studio 2005 SDK\2007.02\VisualStudioIntegration\Common\Assemblies\Microsoft.VisualStudio.Shell.dll" 
REF4 = "C:\Program Files\Visual Studio 2005 SDK\2007.02\VisualStudioIntegration\Common\Assemblies\Microsoft.VisualStudio.Shell.Interop.8.0.dll" 
REF5 = "C:\Program Files\Visual Studio 2005 SDK\2007.02\VisualStudioIntegration\Common\Assemblies\Microsoft.VisualStudio.Shell.Interop.dll"
REF6 = "C:\Program Files\Visual Studio 2005 SDK\2007.02\VisualStudioIntegration\Common\Assemblies\Microsoft.VisualStudio.TextManager.Interop.dll"
REF7 = "C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\System.Data.dll"
REF8 = "C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\System.Design.dll"
REF9 = "C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\System.dll"
REF10 = "C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\System.Drawing.dll"
REF11 = "C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\System.Windows.Forms.dll"
REF12 = "C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\System.Xml.dll"

RES1 = "vsplot\obj\Debug\bukachacha.vsplot.About.resources"
RES2 = "vsplot\obj\Debug\bukachacha.vsplot.plot.resources"
RES3 = "vsplot\obj\Debug\bukachacha.vsplot.PointNumDlg.resources"
RES4 = "vsplot\obj\Debug\bukachacha.vsplot.Resources.resources"
RES5 = "vsplot\obj\Debug\bukachacha.vsplot.SymbolProperties.resources"
RES6 = "vsplot\obj\Debug\bukachacha.vsplot.VSPackage.resources"

  
# description block
$(EXECUTABLE_NAME) : $(SRC_FILES)
  csc  /noconfig /unsafe+ /nowarn:1701,1702 /errorreport:prompt /warn:4 /define:TRACE \
  /reference:$(REF1) /reference:$(REF2) /reference:$(REF3) /reference:$(REF4) /reference:$(REF5) /reference:$(REF6) /reference:$(REF7) /reference:$(REF8) /reference:$(REF9) /reference:$(REF10) /reference:$(REF11) /reference:$(REF12) \
  /resource:$(RES1) /resource:$(RES2) /resource:$(RES3) /resource:$(RES4) /resource:$(RES5) /resource:$(RES6) \
  /target:library \
  /debug:pdbonly /keyfile:vsplot\Key.snk /optimize+ /out:vsplot\obj\Release\vsplot.dll \
  $(SRC_FILES)
  
# copy to bin directory
 copy vsplot\obj\Release\vsplot.dll vsplot\bin\Release
 copy vsplot\obj\Release\vsplot.pdb vsplot\bin\Release

# copy to setup directory
 copy vsplot\obj\Release\vsplot.dll ..\..\setup\src\

# building the setup
 "C:\Program Files\Inno Setup 5\iscc.exe" ..\..\setup\vsplot_setup.iss /O"..\..\setup\Output" /F"vsplot_v1"