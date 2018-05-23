# WingHacks
Small tools for making the great Wersi Pegasus Wing even greater!

The tools are implemented in .NET C#.

## WWSwitcher

This is a small task switcher for the Wersi Pegasus Wing keyboard. It allows you to switch between the Wersi OAS application and other Windows programs running in the background. This is very useful when you want to run VSTs and don't want to use a PC keyboard to switch between the Wersi application and the VSTs. How to run VSTs is described here: https://www.oas-forum.de/viewtopic.php?f=26&t=2598 (in German language).

When started, it creates a semi-transparent red window in the upper right corner, above the Wersi logo (it's barely visible). When clicking on it, it displays a window with all running tasks:

![WWSwitcher](WWSwitcher.png)

You can also open the Windows start menu (upper left button), switch to the OAS application (blue button) or exit WWSwitcher (upper right button.

It is recommended to put the EXE file (which can be found here: WWSwitcher/WWSwitcher/bin/Debug/WWSwitcher.exe) into the autostart menu of Windows.

### Limitations / known issues:

* It can only deal with six other programs at the moment. But this should be enough.
* Certain task switching operations cause WWSwitcher to loose its focus (the form's TopMost property). I've tried various things to circumvent this, but had no success so far. It works fine if you only switch between applications via WWSwitch, and do not attempt to switch from OAS to OAS. If you loose focus, you can re-activate it with Alt+Tab on a physical PC keyboard.



