﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Sanford.Multimedia;
using Sanford.Multimedia.Midi;
using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;


// Requires: https://www.codeproject.com/Articles/6228/%2fArticles%2f6228%2fC-MIDI-Toolkit

namespace WWFix
{

    public partial class WWFixForm : Form
    {
        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy,
                               int dwData, int dwExtraInfo);

        public enum MouseActionAdresses
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }


        private const int SysExBufferSize = 128;
        private InputDevice inDevice1 = null;   // Unprocessed upper manual / control buttons (from Hardware to WWFix)
        private InputDevice inDevice2 = null;   // Unprocessed LED instructions (from WERSI OAS to WWFix)
        private InputDevice inDevice3 = null;   // Unprocessed lower manual / pedal (from Hardware to WWFix)

        private OutputDevice outDevice1 = null; // Processed upper manual / control buttons (from WWFix to WERSI OAS)
        private OutputDevice outDevice2 = null; // Processed LED instructions (from WWFix to LEDs)
        private OutputDevice outDevice3 = null; // Processed lower manual / pedal (from WWFix to WERSI OAS)

        private int inDev1Index = -1;
        private int inDev2Index = -1;
        private int inDev3Index = -1;
        private int outDev1Index = -1;
        private int outDev2Index = -1;
        private int outDev3Index = -1;
        
        private int masterVolume = 0;
        Boolean accPlaying = false;
        Boolean userPlaying = false;
        Boolean blockVibratoFix = false;
        Boolean syncStartActive = false;
        Boolean ignoreStartStopLED = false;
        Boolean volumePotiIgnore = false;
        Boolean setLowerDrawbars = false;
        Boolean ignoreLower2OnEvent = false;
        private int organTypButtonsPressed = 0;
        private int endingButtonsPressed = 0;


        private int lastIntroEndButtonPressed = 0;
        
        Color orange = Color.FromArgb(255-15, 128-15, 0);
        Color orangeBright = Color.FromArgb(255, 128+15, 0+15);
        Color gray = Color.FromArgb(224, 224, 224);

        private SynchronizationContext context;

        String lastStyleLoaded = "";
        long accLastPlayed;
        long userLastPlayed;
        long vibratoLastFixed;

        private bool volumeSet = false;
        private bool pleaseFixVibrato = true;
        private int polyphony = 0;
        private int numberOfStyleLoadRequests = 0;

        public bool firstLogParse { get; private set; }


        // From: https://stackoverflow.com/a/52906286
        public void wait(int milliseconds)
        {
            System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
            if (milliseconds == 0 || milliseconds < 0) return;
            //Console.WriteLine("start wait timer");
            timer1.Interval = milliseconds;
            timer1.Enabled = true;
            timer1.Start();
            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
                //Console.WriteLine("stop wait timer");
            };
            while (timer1.Enabled)
            {
                Application.DoEvents();
            }
        }


        public WWFixForm()
        {
            // Truncate the WERSI OAS log file
            System.IO.File.WriteAllText("C:\\Wersi\\System\\logfiles\\wersi.log", "");

            wait(2000);
            InitializeComponent();
        }

        private void WWFixForm_Load(object sender, EventArgs e)
        {

        }


        protected override void OnLoad(EventArgs e)
        {
            accLastPlayed = getUnixTime();
            userLastPlayed = getUnixTime();
         
            var th = new Thread(ExecuteInForeground);
            th.Start();

            // Load volume:

            try
            {   // Open the text file using a stream reader.
                using (System.IO.StreamReader sr = new System.IO.StreamReader("volume.ini"))
                {
                    masterVolume = int.Parse(sr.ReadToEnd());
                    volumeLabel.Text = "Master Volume: " + masterVolume;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(ex.Message);
            }
            
            // --- Monitor for style load ---
            System.IO.FileSystemWatcher watcher = new System.IO.FileSystemWatcher();
            watcher.SynchronizingObject = this;
            watcher.Path = "C:\\Wersi\\System\\logfiles\\";
            watcher.NotifyFilter = System.IO.NotifyFilters.LastAccess | System.IO.NotifyFilters.LastWrite
               | System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.DirectoryName;
            watcher.Filter = "*.log";
            watcher.Changed += new System.IO.FileSystemEventHandler(OASLogChanged);
            watcher.Created += new System.IO.FileSystemEventHandler(OASLogChanged);
            watcher.Deleted += new System.IO.FileSystemEventHandler(OASLogChanged);

            watcher.EnableRaisingEvents = true;
            
            // Configure MIDI

            doLog(1, InputDevice.DeviceCount + " input devices found:\n");

            for (int a = 0; a < InputDevice.DeviceCount; a++)
            {
                logText.Text += "   " + a + ": " + InputDevice.GetDeviceCapabilities(a).name + "\n";

                if (InputDevice.GetDeviceCapabilities(a).name.Equals("Wersi MIDI"))
                {
                    inDev1Index = a;
                    doLog(1, "      => Unprocessed upper manual / control buttons (from Hardware to WWFix)\n");
                }
                if (InputDevice.GetDeviceCapabilities(a).name.Equals("WWFix Input"))
                {
                    inDev2Index = a;
                    doLog(1, "      => Unprocessed LED instructions (from WERSI OAS to WWFix)\n");
                }
                if (InputDevice.GetDeviceCapabilities(a).name.Equals("MIDIIN3 (Wersi MIDI)"))
                {
                    inDev3Index = a;
                    doLog(1, "      => Unprocessed lower manual / pedal (from Hardware to WWFix)\n");
                }
            }

            if (inDev1Index == -1) MessageBox.Show("WWFix Error: Could not connect to the MIDI interface \"Wersi MIDI\".\nMake sure that the Wersi MIDI driver is loaded before starting WWFix.");
            if (inDev2Index == -1) MessageBox.Show("WWFix Error: Could not connect to the MIDI interface \"WWFix Input\".\nMake sure that loopMIDI is configured properly.");
            if (inDev3Index == -1) MessageBox.Show("WWFix Error: Could not connect to the MIDI interface \"MIDIIN3 (Wersi MIDI)\".\nMake sure that the Wersi MIDI driver is loaded before starting WWFix.");


            if (InputDevice.DeviceCount == 0)
            {
                doLog(1, "Error: No MIDI input devices available!\n");
            }
            else
            {
                try
                {
                    context = SynchronizationContext.Current;
                    inDevice1 = new InputDevice(inDev1Index);
                    inDevice1.ChannelMessageReceived += HandleChannelMessageReceived1;
                    inDevice1.SysCommonMessageReceived += HandleSysCommonMessageReceived1;
                    inDevice1.SysExMessageReceived += HandleSysExMessageReceived1;
                    inDevice1.SysRealtimeMessageReceived += HandleSysRealtimeMessageReceived1;
                    inDevice1.Error += new EventHandler<ErrorEventArgs>(inDevice1_Error);
                    inDevice1.StartRecording();

                    context = SynchronizationContext.Current;
                    inDevice2 = new InputDevice(inDev2Index);
                    inDevice2.ChannelMessageReceived += HandleChannelMessageReceived2;
                    inDevice2.SysCommonMessageReceived += HandleSysCommonMessageReceived2;
                    inDevice2.SysExMessageReceived += HandleSysExMessageReceived2;
                    inDevice2.SysRealtimeMessageReceived += HandleSysRealtimeMessageReceived2;
                    inDevice2.Error += new EventHandler<ErrorEventArgs>(inDevice2_Error);
                    inDevice2.StartRecording();

                    context = SynchronizationContext.Current;
                    inDevice3 = new InputDevice(inDev3Index);
                    inDevice3.ChannelMessageReceived += HandleChannelMessageReceived3;
                    inDevice3.SysCommonMessageReceived += HandleSysCommonMessageReceived3;
                    inDevice3.SysExMessageReceived += HandleSysExMessageReceived3;
                    inDevice3.SysRealtimeMessageReceived += HandleSysRealtimeMessageReceived3;
                    inDevice3.Error += new EventHandler<ErrorEventArgs>(inDevice3_Error);
                    inDevice3.StartRecording();
                }
                catch (Exception ex)
                {
                    doLog(1, "Error: " + ex.Message + "\n");
                }
            }

            doLog(1, OutputDevice.DeviceCount + " output devices found:" + "\n");

            for (int a = 0; a < OutputDevice.DeviceCount; a++)
            {
                logText.Text += "   " + a + ": " + OutputDevice.GetDeviceCapabilities(a).name + "\n";

                if (OutputDevice.GetDeviceCapabilities(a).name.Equals("WWFix Output"))
                {
                    outDev1Index = a;
                    doLog(1, "      => Processed upper manual / control buttons (from WWFix to WERSI OAS)\n");
                }
                if (OutputDevice.GetDeviceCapabilities(a).name.Equals("Wersi MIDI"))
                {
                    outDev2Index = a;
                    doLog(1, "      => Processed LED instructions (from WWFix to LEDs)\n");
                }
                if (OutputDevice.GetDeviceCapabilities(a).name.Equals("WWFix Output2"))
                {
                    outDev3Index = a;
                    doLog(1, "      => Processed lower manual / pedal (from WWFix to WERSI OAS)\n");
                }
            }

            if (outDev1Index == -1) MessageBox.Show("WWFix Error: Could not connect to the MIDI interface \"WWFix Output\".\nMake sure that loopMIDI is configured properly.");
            if (outDev2Index == -1) MessageBox.Show("WWFix Error: Could not connect to the MIDI interface \"Wersi MIDI\".\nMake sure that the Wersi MIDI driver is loaded before starting WWFix.");
            if (outDev3Index == -1) MessageBox.Show("WWFix Error: Could not connect to the MIDI interface \"WWFix Output2\".\nMake sure that loopMIDI is configured properly.");

            if (OutputDevice.DeviceCount == 0)
            {
                doLog(1, "Error: No MIDI output devices available!\n");
            }
            else
            {
                try
                {
                    outDevice1 = new OutputDevice(outDev1Index);
                    outDevice2 = new OutputDevice(outDev2Index);
                    outDevice3 = new OutputDevice(outDev3Index);
                }
                catch (Exception ex)
                {
                    doLog(1, "Error: " + ex.Message + "\n");
                }
            }

            base.OnLoad(e);
        }

        private void doLog(int type, string message)
        {
            if (type == 1 && logImportant.Checked)
            {
                logText.AppendText(message);
            }
            if (type == 2 && logMIDI.Checked)
            {
                logText.AppendText(message);
            }
        }

        private void OASLogChanged(object source, System.IO.FileSystemEventArgs e)
        {

            if (accPlaying) return;

            String styleLoaded = "";
            //logText.Text += "File: " + e.FullPath + " " + e.ChangeType + "\n";

            var filestream = new System.IO.FileStream(e.FullPath,
                                          System.IO.FileMode.Open,
                                          System.IO.FileAccess.Read,
                                          System.IO.FileShare.ReadWrite);
            var file = new System.IO.StreamReader(filestream, System.Text.Encoding.UTF8, true, 128);
            String line = "";
            while ((line = file.ReadLine()) != null)
            {
                if (line.ToLower().Contains(".stw") || line.ToLower().Contains(".sty"))
                {
                    line = line.Replace('(', ' ');
                    string[] entries = line.Split(' ');
                    for (int a = 0; a < entries.Length; a++)
                    {
                        if (entries[a].ToLower().Contains(".stw") || entries[a].ToLower().Contains(".sty"))
                        {
                            styleLoaded = entries[a];
                        }
                    }
                }
            }

            if (!firstLogParse && !styleLoaded.Equals(lastStyleLoaded))
            {
                
                numberOfStyleLoadRequests++;

                doLog(1, "Style loaded: " + styleLoaded + "\n");
                lastStyleLoaded = styleLoaded;
                firstLogParse = false;

                if (numberOfStyleLoadRequests > 3 && /*!userPlaying &&*/ !accPlaying)
                {

                    wait(100);

                    ignoreStartStopLED = true;
                    volumePotiIgnore = true;

                    doLog(1, "Silently pre-loading style:\n");

                    int delayTime = 10;

                    ChannelMessage cm;

                    // Switch the Sync Start button through one to ensure it's in the correct state:

                    cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 88, 0);
                    outDevice1.Send(cm);
                    cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 88, 127);
                    outDevice1.Send(cm);
                    wait(delayTime);

                    cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 88, 0);
                    outDevice1.Send(cm);
                    cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 88, 127);
                    outDevice1.Send(cm);
                    wait(delayTime);

                    Boolean actSync = syncStartActive;

                    if (actSync) // Sync Start is active, deactivate it
                    {
                        doLog(1, "Temporarily disabling 'Sync Start' ...\n");

                        cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 88, 0);
                        outDevice1.Send(cm);
                        cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 88, 127);
                        outDevice1.Send(cm);
                        wait(delayTime);
                    }

                    // Set volume to 0:
                    doLog(1, "Muting audio output ...\n");
                    cm = new ChannelMessage(ChannelCommand.Controller, 0, 2, 0);
                    outDevice1.Send(cm);
                    
                    if (numberOfStyleLoadRequests > 4)
                    {
                        // Start Acc:
                        doLog(1, "Starting Intro 1 ...\n");
                        cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 80, 127);
                        outDevice1.Send(cm);
                        cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 80, 0);
                        outDevice1.Send(cm);

                        wait(delayTime);

                        // Stop Acc:
                        doLog(1, "Stopping Intro 1 ...\n");
                        cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 89, 127);
                        outDevice1.Send(cm);
                        cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 89, 0);
                        outDevice1.Send(cm);

                        wait(delayTime);

                        // Start Acc:
                        doLog(1, "Starting Intro 2 ...\n");
                        cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 86, 127);
                        outDevice1.Send(cm);
                        cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 86, 0);
                        outDevice1.Send(cm);

                        wait(delayTime);

                        // Stop Acc:
                        doLog(1, "Stopping Intro 2 ...\n");
                        cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 89, 127);
                        outDevice1.Send(cm);
                        cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 89, 0);
                        outDevice1.Send(cm);
                    }

                    wait(delayTime);

                    // Start Acc:
                    doLog(1, "Starting style ...\n");
                    cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 89, 127);
                    outDevice1.Send(cm);
                    cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 89, 0);
                    outDevice1.Send(cm);

                    wait(delayTime);

                    // Stop Acc:
                    doLog(1, "Stopping style ...\n");
                    cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 89, 127);
                    outDevice1.Send(cm);
                    cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 89, 0);
                    outDevice1.Send(cm);

                    // Set volume back:
                    /*
                    doLog(1, "Turning audio output on again ...\n");
                    cm = new ChannelMessage(ChannelCommand.Controller, 0, 2, masterVolume);
                    outDevice1.Send(cm);
                    */

                    volumeSet = false;
                    
                    if (actSync) // Sync Start was active, re-activate it
                    {
                        wait(delayTime);
                        doLog(1, "Re-activating 'Sync Start'.\n");
                        cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 88, 0);
                        outDevice1.Send(cm);
                        cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 88, 127);
                        outDevice1.Send(cm);
                    }

                    ignoreStartStopLED = false;

                }
                else
                {
                    doLog(1, "WARNING: Not pre-loading style because the accompaniment is active or the WERSI OAS app was still initializing.\n");
                }
            }
        }


        // =======================================================================================================================
        // Handles for: Unprocessed button presses from user (  Upper/Ctrl:  to WWFix)

        private void HandleChannelMessageReceived1(object sender, ChannelMessageEventArgs e)
        {
            context.Post(delegate (object dummy)
            {

                lastIntroEndButtonPressed = 0;

                // Set Master Volume (if necessary and if the user performs an action)

                if (!volumeSet && e.Message.Command.ToString().Equals("NoteOn"))
                {
                    ChannelMessage volChange = new ChannelMessage(ChannelCommand.Controller, 0, 2, masterVolume);
                    outDevice1.Send(volChange);
                    volumeSet = true;
                    volumePotiIgnore = false;
                    doLog(1, "Fixing Master Volume ...\n");
                }


                // User has pressed the Vibrato button, block fix:
                if (e.Message.Command.ToString().Equals("NoteOn") && e.Message.MidiChannel == 8 && e.Message.Data1 == 61 && e.Message.Data2 == 127)
                {
                    blockVibratoFix = true;

                }
                // User has released the Vibrato button, allow fix:
                if (e.Message.Command.ToString().Equals("NoteOn") && e.Message.MidiChannel == 8 && e.Message.Data1 == 61 && e.Message.Data2 == 0)
                {
                    blockVibratoFix = false;

                }

                // --------------- Fade Out Shortcut ---------------
                if (e.Message.Command.ToString().Equals("NoteOn") && e.Message.MidiChannel == 8 && e.Message.Data1 == 91 && e.Message.Data2 == 127)
                {
                    
                    doLog(2, "Upper/Ctrl:  " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Ending 1 Pressed)\n");
                    endingButtonsPressed++;
                    if (endingButtonsPressed == 2)
                    {
                        endingButtonsPressed = 0;
                        fadeOut();
                    }
                }
                else if (e.Message.Command.ToString().Equals("NoteOn") && e.Message.MidiChannel == 8 && e.Message.Data1 == 91 && e.Message.Data2 == 0)
                {
                   
                    doLog(2, "Upper/Ctrl:  " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Ending 1 Released)\n");
                    endingButtonsPressed--;
                    if (endingButtonsPressed == 0)
                    {
                        outDevice1.Send(new ChannelMessage(ChannelCommand.NoteOn, 8, 91, 127));
                        outDevice1.Send(new ChannelMessage(ChannelCommand.NoteOn, 8, 91, 0));

                        lastIntroEndButtonPressed = 3;
                        doLog(2, "Upper/Ctrl:  " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Ending 1)\n");
                        outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 80, 0));
                        outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 86, 0));
                        outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 91, 0));
                        outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 90, 0));
                        if (!accPlaying || !syncStartActive)
                        {
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 91, 1));
                        }
                        else
                        {
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 91, 2));
                        }
                    }
                    if (endingButtonsPressed < 0) endingButtonsPressed = 0;

                }
                else if (e.Message.Command.ToString().Equals("NoteOn") && e.Message.MidiChannel == 8 && e.Message.Data1 == 90 && e.Message.Data2 == 127)
                {
                    doLog(2, "Upper/Ctrl:  " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Ending 2 Pressed)\n");
                    endingButtonsPressed++;
                    if (endingButtonsPressed == 2)
                    {
                        endingButtonsPressed = 0;
                        fadeOut();
                    }
                }
                else if (e.Message.Command.ToString().Equals("NoteOn") && e.Message.MidiChannel == 8 && e.Message.Data1 == 90 && e.Message.Data2 == 0)
                {
               
                    doLog(2, "Upper/Ctrl:  " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Ending 2 Released)\n");
                    endingButtonsPressed--;
                    if (endingButtonsPressed == 0)
                    {
                        outDevice1.Send(new ChannelMessage(ChannelCommand.NoteOn, 8, 90, 127));
                        outDevice1.Send(new ChannelMessage(ChannelCommand.NoteOn, 8, 90, 0));

                        lastIntroEndButtonPressed = 4;
                        doLog(2, "Upper/Ctrl:  " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Ending 2)\n");
                        outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 80, 0));
                        outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 86, 0));
                        outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 91, 0));
                        outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 90, 0));
                        if (!accPlaying || !syncStartActive)
                        {
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 90, 1));
                        }
                        else
                        {
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 90, 2));
                        }
                    }
                    if (endingButtonsPressed < 0) endingButtonsPressed = 0;

                }
                // --------------- Open VB3 Shortcut ---------------
                else if (e.Message.Command.ToString().Equals("NoteOn") && e.Message.MidiChannel == 8 && e.Message.Data1 == 63 && e.Message.Data2 == 127)
                {
                    doLog(2, "Upper/Ctrl:  " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Organ Typ A B Pressed)\n");
                    organTypButtonsPressed++;
                    if (organTypButtonsPressed == 2)
                    {
                        organTypButtonsPressed = 0;
                        openVB3();
                    }
                }
                else if (e.Message.Command.ToString().Equals("NoteOn") && e.Message.MidiChannel == 8 && e.Message.Data1 == 63 && e.Message.Data2 == 0)
                {
                    doLog(2, "Upper/Ctrl:  " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Organ Typ A B Released)\n");
                    organTypButtonsPressed--;
                    if (organTypButtonsPressed == 0)
                    {
                        outDevice1.Send(new ChannelMessage(ChannelCommand.NoteOn, 8, 63, 127));
                        outDevice1.Send(new ChannelMessage(ChannelCommand.NoteOn, 8, 63, 0));
                    }
                    if (organTypButtonsPressed < 0) organTypButtonsPressed = 0;
                    
                }
                else if (e.Message.Command.ToString().Equals("NoteOn") && e.Message.MidiChannel == 8 && e.Message.Data1 == 62 && e.Message.Data2 == 127)
                {
                    doLog(2, "Upper/Ctrl:  " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Organ Typ C D Pressed)\n");
                    organTypButtonsPressed++;
                    if (organTypButtonsPressed == 2)
                    {
                        organTypButtonsPressed = 0;
                        openVB3();
                    }
                }
                else if (e.Message.Command.ToString().Equals("NoteOn") && e.Message.MidiChannel == 8 && e.Message.Data1 == 62 && e.Message.Data2 == 0)
                {
                    doLog(2, "Upper/Ctrl:  " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Organ Typ C D Released)\n");
                    organTypButtonsPressed--;
                    if (organTypButtonsPressed == 0)
                    {
                        outDevice1.Send(new ChannelMessage(ChannelCommand.NoteOn, 8, 62, 127));
                        outDevice1.Send(new ChannelMessage(ChannelCommand.NoteOn, 8, 62, 0));
                    }
                    if (organTypButtonsPressed < 0) organTypButtonsPressed = 0;

                }
                // --------------- Lower Drawbar Shortcut (16' and 8') ---------------
                else if (e.Message.Command.ToString().Equals("NoteOn") && e.Message.MidiChannel == 8 && e.Message.Data1 == 55 && e.Message.Data2 == 127)
                {
                    doLog(2, "Upper/Ctrl:  " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Lower 2 On Pressed)\n");
                    setLowerDrawbars = true;
                    ignoreLower2OnEvent = false;
                }
                else if (e.Message.Command.ToString().Equals("NoteOn") && e.Message.MidiChannel == 8 && e.Message.Data1 == 55 && e.Message.Data2 == 0)
                {
                    doLog(2, "Upper/Ctrl:  " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Lower 2 On Released)\n");
                    if (ignoreLower2OnEvent == false)
                    {
                        outDevice1.Send(new ChannelMessage(ChannelCommand.NoteOn, 8, 55, 127));
                        outDevice1.Send(new ChannelMessage(ChannelCommand.NoteOn, 8, 55, 0));
                    }
                    ignoreLower2OnEvent = false;
                    setLowerDrawbars = false;
                }
                else if (e.Message.Command.ToString().Equals("PitchWheel") && e.Message.MidiChannel == 10 && e.Message.Data1 == 1)
                {
                    doLog(2, "Upper/Ctrl:  " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Lower 8\" Drawbar)\n");
                    if (setLowerDrawbars == true)
                    {
                        outDevice1.Send(new ChannelMessage(ChannelCommand.PitchWheel, 10, 0, e.Message.Data2));
                        drawBar16.Value = -e.Message.Data2 + 127;
                        ignoreLower2OnEvent = true;
                    } else
                    {
                        outDevice1.Send(e.Message);
                    }

                }
                else if (e.Message.Command.ToString().Equals("PitchWheel") && e.Message.MidiChannel == 10 && e.Message.Data1 == 3)
                {
                    doLog(2, "Upper/Ctrl:  " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Lower 4\" Drawbar)\n");
                    if (setLowerDrawbars == true)
                    {
                        outDevice1.Send(new ChannelMessage(ChannelCommand.PitchWheel, 10, 2, e.Message.Data2));
                        drawBar5.Value = -e.Message.Data2 + 127;
                        ignoreLower2OnEvent = true;
                    }
                    else
                    {
                        outDevice1.Send(e.Message);
                    }

                }
                else if (e.Message.Command.ToString().Equals("PitchWheel") && e.Message.MidiChannel == 0 && e.Message.Data1 == 0)
                {

                    doLog(2, "Upper/Ctrl:  " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Pitch Wheel)\n");

                    outDevice1.Send(e.Message);

                    // Some tests, trying if I can detune a specific channel. Not working so far.

                    /*
                    ChannelMessage cm = null;
                    cm = new ChannelMessage(ChannelCommand.PitchWheel, int.Parse(channel.Text), 0, e.Message.Data2);
                    outDevice1.Send(cm);
                    */

                    /*
                    ChannelMessage volChange = null;
                    volChange = new ChannelMessage(ChannelCommand.Controller, int.Parse(channel.Text), 101, 0);
                    extDevice.Send(volChange);
                    volChange = new ChannelMessage(ChannelCommand.Controller, int.Parse(channel.Text), 100, 0);
                    extDevice.Send(volChange);
                    volChange = new ChannelMessage(ChannelCommand.Controller, int.Parse(channel.Text), e.Message.Data1, 64);
                    extDevice.Send(volChange);
                    volChange = new ChannelMessage(ChannelCommand.Controller, int.Parse(channel.Text), 101, 127);
                    extDevice.Send(volChange);
                    volChange = new ChannelMessage(ChannelCommand.Controller, int.Parse(channel.Text), 100, 127);
                    extDevice.Send(volChange);
                    */

                }
                else if (e.Message.Command.ToString().Equals("Controller") && e.Message.MidiChannel == 0 && e.Message.Data1 == 2)
                {
                    masterVolume = e.Message.Data2;
                    volumeLabel.Text = "Master Volume: " + masterVolume;
                    if (volumePotiIgnore == false)
                    {
                        doLog(2, "Upper/Ctrl:  " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (" + "Master Volume: " + masterVolume + ")\n");
                        outDevice1.Send(e.Message);
                    }
                }
                else if ((e.Message.Command.ToString().Equals("NoteOn") || e.Message.Command.ToString().Equals("NoteOff")) && e.Message.MidiChannel == 0)
                {
                    userLastPlayed = getUnixTime();
                    userPlaying = true;
                    userPlayingLabel.Text = "User Playing";
                    userPlayingLabel.BackColor = orange;

                    if (e.Message.Command.ToString().Equals("NoteOn"))
                    {
                        polyphony++;
                    }
                    if (e.Message.Command.ToString().Equals("NoteOff"))
                    {
                        polyphony--;
                    }
                    doLog(2, "Upper/Ctrl:  " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (User Playing, " + polyphony + ")\n");

                    if (pleaseFixVibrato == true)
                    {
                        blockVibratoFix = true;
                        doLog(1, "Fixing Vibrato 1/2/3 ...\n");
                        inDevice2.StopRecording();
                        for (int a = 0; a < 4; a++)
                        {
                            ChannelMessage cm = null;
                            cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 61, 127);
                            outDevice1.Send(cm);
                            cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 61, 0);
                            outDevice1.Send(cm);
                        }
                        pleaseFixVibrato = false;
                        blockVibratoFix = false;
                        doLog(1, "Finished fixing Vibrato 1/2/3 ...\n");
                        inDevice2.StartRecording();
                    }
                    outDevice1.Send(e.Message);

                }
                else if (e.Message.Command.ToString().Equals("NoteOn") && e.Message.MidiChannel == 8 && (e.Message.Data2 == 127 || e.Message.Data2 == 0))
                {
                    outDevice1.Send(e.Message);

                    switch (e.Message.Data1)
                    {
                        case 80:
                            lastIntroEndButtonPressed = 1;
                            doLog(2, "Upper/Ctrl:  " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Intro 1)\n");
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 80, 0));
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 86, 0));
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 91, 0));
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 90, 0));
                            if (!accPlaying || !syncStartActive)
                            {
                                outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 80, 1));
                            }
                            else
                            {
                                outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 80, 2));
                            }
                            break;
                        case 86:
                            lastIntroEndButtonPressed = 2;
                            doLog(2, "Upper/Ctrl:  " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Intro 2)\n");
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 80, 0));
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 86, 0));
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 91, 0));
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 90, 0));
                            if (!accPlaying || !syncStartActive)
                            {
                                outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 86, 1));
                            }
                            else
                            {
                                outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 86, 2));
                            }
                            break;
                        /*
                        case 91:
                            lastIntroEndButtonPressed = 3;
                            doLog(2, "Upper/Ctrl:  " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Ending 1)\n");
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 80, 0));
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 86, 0));
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 91, 0));
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 90, 0));
                            if (!accPlaying || !syncStartActive)
                            {
                                outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 91, 1));
                            }
                            else
                            {
                                outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 91, 2));
                            }
                            break;
                        case 90:
                            lastIntroEndButtonPressed = 4;
                            doLog(2, "Upper/Ctrl:  " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Ending 2)\n");
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 80, 0));
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 86, 0));
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 91, 0));
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 90, 0));
                            if (!accPlaying || !syncStartActive)
                            {
                                outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 90, 1));
                            }
                            else
                            {
                                outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 90, 2));
                            }
                            break;
                            */
                        default:
                            lastIntroEndButtonPressed = 0;
                            doLog(2, "Upper/Ctrl:  " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + "\n");
                            break;
                    }
                }
                else
                {
                    outDevice1.Send(e.Message);
                    doLog(2, "Upper/Ctrl:  " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + "\n");
                }


            }, null);
        }

        private void HandleSysExMessageReceived1(object sender, SysExMessageEventArgs e)
        {
            context.Post(delegate (object dummy)
            {
                string result = "Upper/Ctrl:  ";
                foreach (byte b in e.Message)
                {
                    result += string.Format("{0:X2} ", b);
                }

                outDevice1.Send(e.Message);
                doLog(2, result + "\n");
            }, null);
        }

        private void HandleSysCommonMessageReceived1(object sender, SysCommonMessageEventArgs e)
        {
            context.Post(delegate (object dummy)
            {
                doLog(2, "Upper/Ctrl:  " +
                        e.Message.SysCommonType.ToString() + ' ' +
                        e.Message.Data1.ToString() + ' ' +
                        e.Message.Data2.ToString() + "\n");
                outDevice1.Send(e.Message);

            }, null);
        }

        private void HandleSysRealtimeMessageReceived1(object sender, SysRealtimeMessageEventArgs e)
        {
            context.Post(delegate (object dummy)
            {
                doLog(2, "Upper/Ctrl:  " + e.Message.SysRealtimeType.ToString() + "\n");
                outDevice1.Send(e.Message);
            }, null);
        }

        private void inDevice1_Error(object sender, ErrorEventArgs e)
        {
            doLog(1, "Error (1): " + e.Error.Message + "\n");
        }


        // =======================================================================================================================
        // Handles for: Unprocessed LED instructions (<=    <= From WERSI OAS to WWFix)

        private void HandleChannelMessageReceived2(object sender, ChannelMessageEventArgs e)
        {
            context.Post(delegate (object dummy)
            {

                if (e.Message.Command.ToString().Equals("Controller") && e.Message.MidiChannel == 8 && (e.Message.Data1 == 82 || e.Message.Data1 == 83 || e.Message.Data1 == 84 || e.Message.Data1 == 85) && (e.Message.Data2 == 1) && accPlaying)
                {
                    doLog(2, "Wersi OAS:   " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Variation A/B/C/D solid)\n");
                    outDevice2.Send(e.Message);

                    // Turn Intro/Ending LEDs off:
                    doLog(1, "Turning Intro/Ending LEDs off\n");
                    outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 80, 0));
                    outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 86, 0));
                    outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 91, 0));
                    outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 90, 0));

                }
                else
                if (e.Message.Command.ToString().Equals("Controller") && e.Message.MidiChannel == 8 && (e.Message.Data1 == 80 || e.Message.Data1 == 86 || e.Message.Data1 == 91 || e.Message.Data1 == 90) && (e.Message.Data2 == 0 || e.Message.Data2 == 1 || e.Message.Data2 == 2))
                {
                    doLog(2, "Wersi OAS:   " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Intro/Ending LED)\n");


                    switch (lastIntroEndButtonPressed)
                    {
                        case 1:
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 80, 0)); // Turn all LEDs off first
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 86, 0));
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 91, 0));
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 90, 0));

                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 80, e.Message.Data2));
                            doLog(1, "Activated Intro 1\n");
                            break;
                        case 2:
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 80, 0)); // Turn all LEDs off first
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 86, 0));
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 91, 0));
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 90, 0));

                            if (e.Message.Data2 != 0)
                            {
                                outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 86, e.Message.Data2));
                            }
                            else
                            {
                                outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 86, 1));
                            }
                            doLog(1, "Activated Intro 2\n");
                            break;
                        case 3:
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 80, 0)); // Turn all LEDs off first
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 86, 0));
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 91, 0));
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 90, 0));

                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 91, e.Message.Data2));
                            doLog(1, "Activated Ending 1\n");
                            break;
                        case 4:
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 80, 0)); // Turn all LEDs off first
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 86, 0));
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 91, 0));
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 90, 0));

                            if (e.Message.Data2 != 0)
                            {
                                outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 90, e.Message.Data2));
                            }
                            else
                            {
                                outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 90, 1));
                            }
                            doLog(1, "Activated Ending 2\n");
                            break;
                        default:
                            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, e.Message.Data1, e.Message.Data2));
                            doLog(1, "Nothing changed\n");
                            break;

                    }
                }

                else if (e.Message.Command.ToString().Equals("Controller") && e.Message.MidiChannel == 8 && e.Message.Data1 == 88 && e.Message.Data2 == 0)
                {
                    doLog(2, "Wersi OAS:   " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Sync Start OFF)\n");
                    syncStartActive = false;
                    outDevice2.Send(e.Message);
                }
                else if (e.Message.Command.ToString().Equals("Controller") && e.Message.MidiChannel == 8 && e.Message.Data1 == 88 && e.Message.Data2 == 1)
                {
                    doLog(2, "Wersi OAS:   " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Sync Start ON)\n");
                    syncStartActive = true;
                    outDevice2.Send(e.Message);
                }
                else if (e.Message.Command.ToString().Equals("Controller") && e.Message.MidiChannel == 8 && e.Message.Data1 == 89 && e.Message.Data2 == 1)
                {
                    doLog(2, "Wersi OAS:   " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Acc Beat GREEN)\n");
                    if (ignoreStartStopLED == false)
                    {
                        accLastPlayed = getUnixTime();
                        accPlaying = true;
                        accRunningLabel.Text = "Acc Running";
                        accRunningLabel.BackColor = orangeBright;
                    }
                    outDevice2.Send(e.Message);
                }
                else if (e.Message.Command.ToString().Equals("Controller") && e.Message.MidiChannel == 9 && e.Message.Data1 == 89 && e.Message.Data2 == 1)
                {
                    doLog(2, "Wersi OAS:   " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Acc Beat RED)\n");
                    if (ignoreStartStopLED == false)
                    {
                        accLastPlayed = getUnixTime();
                        accPlaying = true;
                        accRunningLabel.Text = "Acc Running";
                        accRunningLabel.BackColor = orangeBright;
                    }
                    outDevice2.Send(e.Message);
                }
                else if (e.Message.Command.ToString().Equals("Controller") && e.Message.MidiChannel == 8 && e.Message.Data1 == 89 && e.Message.Data2 == 0)
                {
                    doLog(2, "Wersi OAS:   " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + "\n");
                    if (ignoreStartStopLED == false)
                    {
                        accLastPlayed = getUnixTime();
                        accPlaying = true;
                        accRunningLabel.Text = "Acc Running";
                        accRunningLabel.BackColor = orange;
                    }
                    outDevice2.Send(e.Message);
                }
                else if (e.Message.Command.ToString().Equals("Controller") && e.Message.MidiChannel == 9 && e.Message.Data1 == 89 && e.Message.Data2 == 0)
                {
                    doLog(2, "Wersi OAS:   " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + "\n");
                    if (ignoreStartStopLED == false)
                    {
                        accLastPlayed = getUnixTime();
                        accPlaying = true;
                        accRunningLabel.Text = "Acc Running";
                        accRunningLabel.BackColor = orange;
                    }
                    outDevice2.Send(e.Message);
                }
                else if (e.Message.Command.ToString().Equals("Controller") && (e.Message.MidiChannel == 8 || e.Message.MidiChannel == 9) && e.Message.Data1 == 61 && (e.Message.Data2 == 0 || e.Message.Data2 == 1))
                {

                    doLog(2, "Wersi OAS:   " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Vibrato LEDs)\n");

                    if (blockVibratoFix == false && vibratoLastFixed + 1000 < getUnixTime())
                    {
                        doLog(1, "Vibrato fix planned.\n");
                        vibratoLastFixed = getUnixTime();
                        pleaseFixVibrato = true;
                    }
                    outDevice2.Send(e.Message);
                }
                else
                {
                    doLog(2, "Wersi OAS:   " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + "\n");
                    outDevice2.Send(e.Message);
                }


            }, null);
        }

        private void HandleSysExMessageReceived2(object sender, SysExMessageEventArgs e)
        {
            context.Post(delegate (object dummy)
            {
                string result = "Wersi OAS:   ";
                foreach (byte b in e.Message)
                {
                    result += string.Format("{0:X2} ", b);
                }

                outDevice2.Send(e.Message);
                doLog(2, result + "\n");
            }, null);
        }

        private void HandleSysCommonMessageReceived2(object sender, SysCommonMessageEventArgs e)
        {
            context.Post(delegate (object dummy)
            {
                doLog(2, "Wersi OAS:   " +
                        e.Message.SysCommonType.ToString() + ' ' +
                        e.Message.Data1.ToString() + ' ' +
                        e.Message.Data2.ToString() + "\n");
                outDevice2.Send(e.Message);

            }, null);
        }

        private void HandleSysRealtimeMessageReceived2(object sender, SysRealtimeMessageEventArgs e)
        {
            context.Post(delegate (object dummy)
            {
                doLog(2, "Wersi OAS:   " + e.Message.SysRealtimeType.ToString() + "\n");
                outDevice2.Send(e.Message);
            }, null);
        }

        private void inDevice2_Error(object sender, ErrorEventArgs e)
        {
            doLog(1, "Error (2): " + e.Error.Message + "\n");
        }



        // =======================================================================================================================
        // Handles for: Unprocessed pedal/lower manual

        private void HandleChannelMessageReceived3(object sender, ChannelMessageEventArgs e)
        {
            context.Post(delegate (object dummy)
            {

                // Set Master Volume (if necessary and if the user performs an action)

                if (!volumeSet && e.Message.Command.ToString().Equals("NoteOn"))
                {
                    ChannelMessage volChange = new ChannelMessage(ChannelCommand.Controller, 0, 2, masterVolume);
                    outDevice3.Send(volChange);
                    volumeSet = true;
                    volumePotiIgnore = false;
                    doLog(1, "Fixing Master Volume ...\n");
                }

                if ((e.Message.Command.ToString().Equals("NoteOn") || e.Message.Command.ToString().Equals("NoteOff")) && e.Message.MidiChannel == 0)
                {
                    userLastPlayed = getUnixTime();
                    userPlaying = true;
                    userPlayingLabel.Text = "User Playing";
                    userPlayingLabel.BackColor = orange;

                    if (e.Message.Command.ToString().Equals("NoteOn"))
                    {
                        polyphony++;
                    }
                    if (e.Message.Command.ToString().Equals("NoteOff"))
                    {
                        polyphony--;
                    }
                    doLog(2, "Lower/Pedal: " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (User Playing, " + polyphony + ")\n");

                    if (pleaseFixVibrato == true)
                    {
                        blockVibratoFix = true;
                        doLog(1, "Fixing Vibrato 1/2/3 ...\n");
                        inDevice2.StopRecording();
                        for (int a = 0; a < 4; a++)
                        {
                            ChannelMessage cm = null;
                            cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 61, 127);
                            outDevice1.Send(cm);
                            cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 61, 0);
                            outDevice1.Send(cm);
                        }
                        pleaseFixVibrato = false;
                        blockVibratoFix = false;
                        doLog(1, "Finished fixing Vibrato 1/2/3 ...\n");
                        inDevice2.StartRecording();
                    }
                    outDevice3.Send(e.Message);

                }                
                else
                {
                    outDevice3.Send(e.Message);
                    doLog(2, "Lower/Pedal: " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + "\n");
                }

            }, null);
        }

        private void HandleSysExMessageReceived3(object sender, SysExMessageEventArgs e)
        {
            context.Post(delegate (object dummy)
            {
                string result = "Lower/Pedal: ";
                foreach (byte b in e.Message)
                {
                    result += string.Format("{0:X2} ", b);
                }

                outDevice3.Send(e.Message);
                doLog(2, result + "\n");
            }, null);
        }

        private void HandleSysCommonMessageReceived3(object sender, SysCommonMessageEventArgs e)
        {
            context.Post(delegate (object dummy)
            {
                doLog(2, "Lower/Pedal: " +
                        e.Message.SysCommonType.ToString() + ' ' +
                        e.Message.Data1.ToString() + ' ' +
                        e.Message.Data2.ToString() + "\n");
                outDevice3.Send(e.Message);

            }, null);
        }

        private void HandleSysRealtimeMessageReceived3(object sender, SysRealtimeMessageEventArgs e)
        {
            context.Post(delegate (object dummy)
            {
                doLog(2, "Lower/Pedal: " + e.Message.SysRealtimeType.ToString() + "\n");
                outDevice3.Send(e.Message);
            }, null);
        }

        private void inDevice3_Error(object sender, ErrorEventArgs e)
        {
            doLog(1, "Error (1): " + e.Error.Message + "\n");
        }

        // =======================================================================================================================


        private long getUnixTime()
        {
            return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
        }


        private void ExecuteInForeground()
        {

            while (true)
            {
                if (accPlaying && (accLastPlayed + 500) < getUnixTime())
                {
                    accPlaying = false;
                    accRunningLabel.Text = "Acc Not Running";
                    accRunningLabel.BackColor = gray;
                }
                if (userPlaying && (userLastPlayed + 500) < getUnixTime() && polyphony <= 0)
                {
                    userPlaying = false;
                    userPlayingLabel.Text = "User Not Playing";
                    userPlayingLabel.BackColor = gray;
                }

                Thread.Sleep(100);
            }
        }

        protected override void OnClosed(EventArgs e)
        {

            System.IO.File.WriteAllText("volume.ini", "" + masterVolume);

            if (inDevice1 != null)
            {
                inDevice1.Close();
            }
            if (inDevice2 != null)
            {
                inDevice2.Close();
            }
            if (outDevice1 != null)
            {
                outDevice1.Close();
            }
            if (outDevice2 != null)
            {
                outDevice2.Close();
            }

            base.OnClosed(e);
        }


        private void WWFixForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(1);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void drawBar16_Scroll(object sender, EventArgs e)
        {
            int value = -drawBar16.Value + 127;
            //doLog(1, "Drawbar 16 value = " + value + "\n");
            ChannelMessage volChange = new ChannelMessage(ChannelCommand.PitchWheel, 10, 0, value);
            outDevice1.Send(volChange);
        }

        private void drawBar5_Scroll(object sender, EventArgs e)
        {
            int value = -drawBar5.Value + 127;
            //doLog(1, "Drawbar 5 1/3 value = " + value + "\n");
            ChannelMessage volChange = new ChannelMessage(ChannelCommand.PitchWheel, 10, 2, value);
            outDevice1.Send(volChange);
        }

        private void Full16_Click(object sender, EventArgs e)
        {
            ChannelMessage volChange = new ChannelMessage(ChannelCommand.PitchWheel, 10, 0, 127);
            outDevice1.Send(volChange);
            drawBar16.Value = 0;
            focusLog();
        }

        private void Full5_Click(object sender, EventArgs e)
        {
            ChannelMessage volChange = new ChannelMessage(ChannelCommand.PitchWheel, 10, 2, 127);
            outDevice1.Send(volChange);
            drawBar5.Value = 0;
            focusLog();
        }

        private void Off16_Click(object sender, EventArgs e)
        {
            ChannelMessage volChange = new ChannelMessage(ChannelCommand.PitchWheel, 10, 0, 0);
            outDevice1.Send(volChange);
            drawBar16.Value = 127;
            focusLog();
        }

        private void Off5_Click(object sender, EventArgs e)
        {
            ChannelMessage volChange = new ChannelMessage(ChannelCommand.PitchWheel, 10, 2, 0);
            outDevice1.Send(volChange);
            drawBar5.Value = 127;
            focusLog();
        }

        private void drawBar16_MouseUp(object sender, MouseEventArgs e)
        {
            focusLog();
        }

        private void drawBar5_MouseUp(object sender, MouseEventArgs e)
        {
            focusLog();
        }

        private void drawBar16_ValueChanged(object sender, EventArgs e)
        {
            focusLog();
        }

        private void drawBar5_ValueChanged(object sender, EventArgs e)
        {
            focusLog();
        }

        private void focusLog()
        {
            logText.SelectionStart = logText.Text.Length;
            logText.ScrollToCaret();
            this.ActiveControl = logText;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void logMIDI_Click(object sender, EventArgs e)
        {
            focusLog();
        } 

        private void logImportant_Click(object sender, EventArgs e)
        {
            focusLog();
        }


        public void LeftClick(int x, int y)
        {
            //Cursor.Position = new Point((int)x, (int)y);
            Cursor.Position = new System.Drawing.Point(x, y);
            mouse_event((int)(MouseActionAdresses.LEFTDOWN), 0, 0, 0, 0);
            Thread.Sleep(100);
            mouse_event((int)(MouseActionAdresses.LEFTUP), 0, 0, 0, 0);
            Thread.Sleep(100);
        }

        public void openVB3()
        {
            doLog(1, "Opening VB3 ...\n");
            LeftClick(540, 30);
            LeftClick(100, 560);
            LeftClick(650, 250);
        }

        public void fadeOut()
        {
            logText.SelectionStart = logText.Text.Length;
            logText.ScrollToCaret();
            this.ActiveControl = logText;

            doLog(1, "Fading out ...\n");
            volumePotiIgnore = true;

            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 90, 2));
            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 91, 2));

            ChannelMessage cm = null;
            for (int v = masterVolume; v >= 0; v--)
            {
                cm = new ChannelMessage(ChannelCommand.Controller, 0, 2, v);
                outDevice1.Send(cm);
                volumeLabel.Text = "Master Volume: " + v;
                wait(100);
            }

            doLog(1, "Stopping style ...\n");
            cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 89, 127);
            outDevice1.Send(cm);
            cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 89, 0);
            outDevice1.Send(cm);

            wait(100);
            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 90, 1));
            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 91, 1));

            wait(2900);
            cm = new ChannelMessage(ChannelCommand.Controller, 0, 2, masterVolume);
            outDevice1.Send(cm);
            volumeLabel.Text = "Master Volume: " + masterVolume;

            volumePotiIgnore = false;

            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 90, 0));
            outDevice2.Send(new ChannelMessage(ChannelCommand.Controller, 8, 91, 0));
        }


        private void button1_Click(object sender, EventArgs e)
        {

           
        }
    }
}
