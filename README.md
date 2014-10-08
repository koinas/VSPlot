VSPlot is a package for Visual Studio to plot arrays in a separate window.

The setup filer is in the "VSPlot/setup/Output/" directory.

Make sure no Visual Studio is running.
Run the setup and install choosing the appropriate Visual Studio version.

Select View->Other Windows and check that Plot menu item exists.

Open any project and enter debug mode.
Make sure you have any variable (pointer or array) in Autos or Watch window.
Right click on the variable and select Plot command.

The data will be plotted in the separate window.
You can place it anywhere you want using Visual Studio interface.

VSPlot plugin cannot detect how many points array have, you must enter number of points you want to plot.
Right click on the window to open context menu.
Double click on plot to see the value.
Left click to zoom.

There is data_types.xml in the setup directory which contains all suppoted types.
Read for details.

VSplot was tested under 2005,2008,2010,2012,2013 visual studios.
C# runtime is needed to run this plugin.

Contact me at alex.koinas@gmail.com for any problems,questions.