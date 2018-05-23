# WingHacks
Small tools for making the great Wersi Pegasus Wing even greater!

The tools are implemented in .NET C#.

## WWSwitcher

This is a small task switcher for the Wersi Pegasus Wing keyboard. It allows you to switch between the Wersi OAS application and other Windows programs running in the background. This is very useful when you want to run VSTs and don't want to use a PC keyboard to switch between the Wersi application and the VSTs. How to run VSTs is described here: https://www.oas-forum.de/viewtopic.php?f=26&t=2598 (in German language).

When started, it creates a semi-transparent red button (which is actually a window) in the upper right corner, above the Wersi logo (it's barely visible). When clicking on it, it displays a window with all running tasks:

![WWSwitcher](WWSwitcher.png)

You can also open the Windows start menu (upper left button), switch to the OAS application (blue button) or exit WWSwitcher (upper right button.

It is recommended to put the EXE file (which can be found here: WWSwitcher/WWSwitcher/bin/Debug/WWSwitcher.exe) into the autostart menu of Windows.

### Limitations:

* It can only deal with six other programs at the moment. But this should be enough.

### Changelog:

* 0.01 to 0.02: The tool now brings itself correctly to the foreground, it now behaves as a user would expect!


