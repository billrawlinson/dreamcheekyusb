**Project Description**

This is just a fork of the original DreamCheeky library for controlling the big red button.  I have made the following changes:


1.  I got rid of all the stuff I don't care about such as the LED and some other Iron Man button references.
2. I've also updated the HID library to the latest version available as of this writing that is in nuget.  So we don't have that whole libraries source in this fork either.
3. If you don't pass an action to this particular version it will automatically send the Media Pause/Play button.  I use it to start the Google Music Desktop App.  
4. this version does not use a timer to determine if the button is pushed but uses the normal HID way of polling using the ReadRecord method.


Mostly this was just a proof of concept to see if I could get it working.



What follows is most of the original readme:



[DreamCheekyUSB](https://github.com/gbrayut/dreamcheekyusb) provides a Console App and .NET drivers for the Dream Cheeky Webmail Notifier and the Dream Cheeky Iron Man USB Stress Button.

DreamCheekyUSB was created using the [https://github.com/mikeobrien/HidLibrary/](https://github.com/mikeobrien/HidLibrary/) and is released under the Apache License V2.0

You can control the LED using either DreamCheekyLED.exe with command line arguments or via C#/VB/Powershell using the DreamCheekyUSB.DreamCheekyLED object. The code supports multiple devices and has options for blinking and fading. See [DreamCheekyLED](DreamCheekyLED) for command line examples or the DreamCheekyLED*.ps1 files for powershell examples.

The DreamCheekyBTN.exe has command line arguments that will let you run a program whenever the button is pressed or convert the button press events into a keyboard macro that can perform an action or be picked up by other programs like AutoHotKey. The code should also support multiple devices, and can be run multiple times for additional triggers. See [DreamCheekyBTN](DreamCheekyBTN) for command line examples or the AutoHotKey.ahk file for a sample AutoHotKey script.


