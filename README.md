# Autodrive

##What Is This?
Autodrive is an open source, .NET framework component for automating medical physics beam measurements. It includes APIs for interacting with standard equipment including electrometers, scanning systems and even a linac (currently limited to Varian C Series linacs).

###Electrometers Currently Supported
* PTW Unidose
* Standard Imaging Max 4000 and Max 4000 Plus
* PTW TBA Electrometer (with MP3 scanning system)

###Scanning Systems Supported
* PTW Tandem TBA Controller
* Standard Imaging DoseView1D

###Linac Supported
* Varian Medical Systems C-Series Linacs

####How is the linac controlled?
Linac control is perfomed using a [keyboard emulator](http://www.vetra.com/335text.html) through the service mode console. The API has a map of the Varian Service Mode console and executes as if a user was entering keys. It can be thought of as a very sophisticated macro with fine grained control. The API has the method that looks like:
```cs
MachineState ms = MachineState.InitNew();
ms.X1 = 10;
ms.MU = 200;
ms.GantryAngle = 270;

Linac l = new CSeriesLinac();
l.Initalize("COM3"); //Uses RS232 to keyboard P/S2 interface

l.SetMachineState(ms); // Modes up whatever you put in
l.BeamOn();
```

> **Warning:** Controlling a radiation device and large mechanical components via a robotic software system should be done with extreme care. Please use with caution and at your own risk. 
