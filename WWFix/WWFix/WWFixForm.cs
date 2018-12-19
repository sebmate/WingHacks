using System;
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


namespace WWFix
{




    public partial class WWFixForm : Form
    {



        private const int SysExBufferSize = 128;

        private InputDevice inDevice1 = null; // Unprocessed button presses from user (From Hardware to WWFix)
        private OutputDevice outDevice1 = null; // Processed button presses from user (from WWFix to WERSI OAS)
        private InputDevice inDevice2 = null; // Unprocessed LED instructions (from WERSI OAS to WWFix)
        private OutputDevice outDevice2 = null;  // Processed LED instructions (from WWFix to LEDs)

        private int inDev1Index = 0;
        private int inDev2Index = 0;
        private int outDev1Index = 0;
        private int outDev2Index = 0;

        private int masterVolume = 0;
        Boolean accPlaying = false;
        Boolean userPlaying = false;
        Boolean blockVibratoFix = false;
        Boolean syncStartActive = false;

        Color orange = Color.FromArgb(255, 128, 0);
        Color gray = Color.FromArgb(224, 224, 224);

        private SynchronizationContext context;

        String lastStyleLoaded = "";
        int accLastPlayed;
        int userLastPlayed;
        int vibratoBlockTime;

        private bool volumeSet = false;
        private bool pleaseFixVibrato = true;
        private int polyphony = 0;

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
            vibratoBlockTime = getUnixTime();

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
            watcher.Changed += new System.IO.FileSystemEventHandler(OnChanged);
            watcher.Created += new System.IO.FileSystemEventHandler(OnChanged);
            watcher.Deleted += new System.IO.FileSystemEventHandler(OnChanged);

            watcher.EnableRaisingEvents = true;


            // Configure MIDI

            logText.AppendText(InputDevice.DeviceCount + " input devices found:\n");

            for (int a = 0; a < InputDevice.DeviceCount; a++)
            {
                logText.Text += "   " + a + ": " + InputDevice.GetDeviceCapabilities(a).name + "\n";

                if (InputDevice.GetDeviceCapabilities(a).name.Equals("Wersi MIDI"))
                {
                    inDev1Index = a;
                    logText.AppendText("      => Unprocessed button presses from user (From Hardware to WWFix)\n");
                }
                if (InputDevice.GetDeviceCapabilities(a).name.Equals("WWFix Input"))
                {
                    inDev2Index = a;
                    logText.AppendText("      => Unprocessed LED instructions (from WERSI OAS to WWFix)\n");
                }
            }

            if (InputDevice.DeviceCount == 0)
            {
                logText.AppendText("Error: No MIDI input devices available!\n");
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
                }
                catch (Exception ex)
                {
                    logText.AppendText("Error: " + ex.Message + "\n");
                }
            }


            logText.AppendText(OutputDevice.DeviceCount + " output devices found:" + "\n");

            for (int a = 0; a < OutputDevice.DeviceCount; a++)
            {
                logText.Text += "   " + a + ": " + OutputDevice.GetDeviceCapabilities(a).name + "\n";

                if (OutputDevice.GetDeviceCapabilities(a).name.Equals("WWFix Output"))
                {
                    outDev1Index = a;
                    logText.AppendText("      => Processed button presses from user (from WWFix to WERSI OAS)\n");
                }
                if (OutputDevice.GetDeviceCapabilities(a).name.Equals("Wersi MIDI"))
                {
                    outDev2Index = a;
                    logText.AppendText("      => Processed LED instructions (from WWFix to LEDs)\n");
                }

            }

            if (OutputDevice.DeviceCount == 0)
            {
                logText.AppendText("Error: No MIDI output devices available!\n");
            }
            else
            {

                try
                {
                    outDevice1 = new OutputDevice(outDev1Index);
                    outDevice2 = new OutputDevice(outDev2Index);
                }
                catch (Exception ex)
                {
                    logText.AppendText("Error: " + ex.Message + "\n");

                }
            }

            base.OnLoad(e);
        }



        private void OnChanged(object source, System.IO.FileSystemEventArgs e)
        {
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
                logText.AppendText("Style loaded: " + styleLoaded + "\n");
                lastStyleLoaded = styleLoaded;
                firstLogParse = false;

                if (/*!userPlaying &&*/ !accPlaying) {

                    logText.AppendText("Silently pre-loading style:\n");

                    Boolean actSync = syncStartActive;
                    ChannelMessage cm;

                    if (actSync) // Sync Start active, deactivate it
                    {
                        logText.AppendText("Temporarily disabling 'Sync Start' ...\n");
                        cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 88, 0);
                        outDevice1.Send(cm);
                        cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 88, 127);
                        outDevice1.Send(cm);
                        wait(100);
                    }

                    // Set volume to 0:
                    logText.AppendText("Muting audio output ...\n");
                    cm = new ChannelMessage(ChannelCommand.Controller, 0, 2, 0);
                    outDevice1.Send(cm);

                    // Start Acc:
                    logText.AppendText("Starting style ...\n");
                    cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 89, 127);
                    outDevice1.Send(cm);
                    cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 89, 0);
                    outDevice1.Send(cm);

                    // Stop Acc:
                    logText.AppendText("Stopping style ...\n");
                    cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 89, 127);
                    outDevice1.Send(cm);
                    cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 89, 0);
                    outDevice1.Send(cm);

                    // Set volume back:
                    logText.AppendText("Turning audio output on again ...\n");
                    cm = new ChannelMessage(ChannelCommand.Controller, 0, 2, masterVolume);
                    outDevice1.Send(cm);

                    if (actSync) // Sync Start was active, re-activate it
                    {
                        wait(100);
                        logText.AppendText("Re-activating 'Sync Start'.\n");
                        cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 88, 0);
                        outDevice1.Send(cm);
                        cm = new ChannelMessage(ChannelCommand.NoteOn, 8, 88, 127);
                        outDevice1.Send(cm);
                    }
                } else {
                    logText.AppendText("WARNING: Not pre-loading style because the accompaniment is active.\n");
                }
            }
        }

        
        // =======================================================================================================================
        // Handles for: Unprocessed button presses from user (From Hardware :  to WWFix)

        private void HandleChannelMessageReceived1(object sender, ChannelMessageEventArgs e)
        {
            context.Post(delegate (object dummy)
            {

                // Set Master Volume (if necessary and if the user performs an action)

                if (!volumeSet && e.Message.Command.ToString().Equals("NoteOn"))
                {
                    ChannelMessage volChange = new ChannelMessage(ChannelCommand.Controller, 0, 2, masterVolume);
                    outDevice1.Send(volChange);
                    volumeSet = true;
                    logText.AppendText("Fixing Master Volume ...\n");
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


                if (e.Message.Command.ToString().Equals("Controller") && e.Message.MidiChannel == 0 && e.Message.Data1 == 2)
                {
                    masterVolume = e.Message.Data2;
                    volumeLabel.Text = "Master Volume: " + masterVolume;
                    logText.AppendText("From Hardware : " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (" + "Master Volume: " + masterVolume + ")\n");
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
                    logText.AppendText("From Hardware : " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (User Playing, " + polyphony + ")\n");
                                        
                    if (pleaseFixVibrato == true)
                    {
                        blockVibratoFix = true;
                        logText.AppendText("Fixing Vibrato 1/2/3 ...\n");
                        inDevice2.StopRecording();
                        ChannelMessage volChange = null;
                        for (int a = 0; a < 4; a++)
                        {
                            volChange = new ChannelMessage(ChannelCommand.NoteOn, 8, 61, 127);
                            outDevice1.Send(volChange);
                            volChange = new ChannelMessage(ChannelCommand.NoteOn, 8, 61, 0);
                            outDevice1.Send(volChange);
                        }
                        pleaseFixVibrato = false;
                        blockVibratoFix = false;
                        logText.AppendText("Finished fixing Vibrato 1/2/3 ...\n");
                        inDevice2.StartRecording();
                    }
                    
                
                }
                else
                {
                    logText.AppendText("From Hardware : " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + "\n");
                }

                outDevice1.Send(e.Message);
            }, null);
        }

        private void HandleSysExMessageReceived1(object sender, SysExMessageEventArgs e)
        {
            context.Post(delegate (object dummy)
            {
                string result = "From Hardware : ";
                foreach (byte b in e.Message)
                {
                    result += string.Format("{0:X2} ", b);
                }

                outDevice1.Send(e.Message);
                logText.AppendText(result + "\n");
            }, null);
        }

        private void HandleSysCommonMessageReceived1(object sender, SysCommonMessageEventArgs e)
        {
            context.Post(delegate (object dummy)
            {
                logText.AppendText("From Hardware : " +
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
                logText.AppendText("From Hardware : " + e.Message.SysRealtimeType.ToString() + "\n");
                outDevice1.Send(e.Message);
            }, null);
        }

        private void inDevice1_Error(object sender, ErrorEventArgs e)
        {
            logText.AppendText("Error (1): " + e.Error.Message + "\n");
        }


        // =======================================================================================================================
        // Handles for: Unprocessed LED instructions (from WERSI OAS to WWFix)

        private void HandleChannelMessageReceived2(object sender, ChannelMessageEventArgs e)
        {
            context.Post(delegate (object dummy)
            {

                if (e.Message.Command.ToString().Equals("Controller") && e.Message.MidiChannel == 8 && e.Message.Data1 == 88 && e.Message.Data2 == 0)
                {
                    logText.AppendText("From WERSI OAS: " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Sync Start OFF)\n");
                    syncStartActive = false;
                }
                else if (e.Message.Command.ToString().Equals("Controller") && e.Message.MidiChannel == 8 && e.Message.Data1 == 88 && e.Message.Data2 == 1)
                {
                    logText.AppendText("From WERSI OAS: " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Sync Start ON)\n");
                    syncStartActive = true;
                }
                else if (e.Message.Command.ToString().Equals("Controller") && e.Message.MidiChannel == 8 && e.Message.Data1 == 89 && e.Message.Data2 == 1)
                {
                    logText.AppendText("From WERSI OAS: " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Acc Beat GREEN)\n");
                    accLastPlayed = getUnixTime();
                    accPlaying = true;
                    accRunningLabel.Text = "Acc Running";
                    accRunningLabel.BackColor = orange;
                }
                else if (e.Message.Command.ToString().Equals("Controller") && e.Message.MidiChannel == 9 && e.Message.Data1 == 89 && e.Message.Data2 == 1)
                {
                    logText.AppendText("From WERSI OAS: " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Acc Beat RED)\n");
                    accLastPlayed = getUnixTime();
                    accPlaying = true;
                    accRunningLabel.Text = "Acc Running";
                    accRunningLabel.BackColor = orange;
                }
                else if (e.Message.Command.ToString().Equals("Controller") && (e.Message.MidiChannel == 8 || e.Message.MidiChannel == 9) && e.Message.Data1 == 61 && (e.Message.Data2 == 0 || e.Message.Data2 == 1))
                {

                    logText.AppendText("From WERSI OAS: " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + " (Vibrato LEDs)\n");

                    if (blockVibratoFix == false)
                    {
                        logText.AppendText("Vibrato fix planned.\n");
                        pleaseFixVibrato = true;
                    }

                }
                else
                {
                    logText.AppendText("From WERSI OAS: " + e.Message.Command.ToString() + ' ' + e.Message.MidiChannel.ToString() + ' ' + e.Message.Data1.ToString() + ' ' + e.Message.Data2.ToString() + "\n");

                }

                outDevice2.Send(e.Message);
            }, null);
        }

        private void HandleSysExMessageReceived2(object sender, SysExMessageEventArgs e)
        {
            context.Post(delegate (object dummy)
            {
                string result = "From WERSI OAS: ";
                foreach (byte b in e.Message)
                {
                    result += string.Format("{0:X2} ", b);
                }

                outDevice2.Send(e.Message);
                logText.AppendText(result + "\n");
            }, null);
        }

        private void HandleSysCommonMessageReceived2(object sender, SysCommonMessageEventArgs e)
        {
            context.Post(delegate (object dummy)
            {
                logText.AppendText("From WERSI OAS: " +
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
                logText.AppendText("From WERSI OAS: " + e.Message.SysRealtimeType.ToString() + "\n");
                outDevice2.Send(e.Message);
            }, null);
        }

        private void inDevice2_Error(object sender, ErrorEventArgs e)
        {
            logText.AppendText("Error (2): " + e.Error.Message + "\n");
        }

        private Int32 getUnixTime()
        {
            return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }


        private void ExecuteInForeground()
        {

            while (true)
            {
                if (accPlaying && (accLastPlayed + 1) < getUnixTime())
                {
                    accPlaying = false;
                    accRunningLabel.Text = "Acc Not Running";
                    accRunningLabel.BackColor = gray;
                }
                if (userPlaying && (userLastPlayed + 1) < getUnixTime() && polyphony <= 0)
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
            //logText.AppendText("Drawbar 16 value = " + value + "\n");
            ChannelMessage volChange = new ChannelMessage(ChannelCommand.PitchWheel, 10, 0, value);
            outDevice1.Send(volChange);
        }

        private void drawBar5_Scroll(object sender, EventArgs e)
        {
            int value = -drawBar5.Value + 127;
            //logText.AppendText("Drawbar 5 1/3 value = " + value + "\n");
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
    }
}
