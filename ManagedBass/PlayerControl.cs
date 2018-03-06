using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ManagedBass.Misc;

namespace ManagedBass
{
    /// <summary>
    /// float[] samples = new float[44100];
    /// </summary>

    public partial class PlayerControl : UserControl
    {
        // private readonly int _defaultSoundDevice = -1;
        // visuals class instance
        private readonly Visuals _vis = new Visuals();
        private double _Length;
        private int _handle;
        private bool _Loop;

        // Timer
        private int _TickCounter;
        private int _UpdateInterval = 50; // 50ms
        private BASSTimer _UpdateTimer;

        // Spectrum
        private int _SpectrumIdx;

        public PlayerControl()
        {
            // Activates double buffering 
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);

            this.UpdateStyles();

            InitializeComponent();

            // To Form Load
            /*var currentDev = Bass.CurrentDevice;
            if (currentDev == -1 || !Bass.GetDeviceInfo(Bass.CurrentDevice).IsInitialized)
            {
                _defaultSoundDevice = currentDev;
            }*/
        }

        private void OnLoad(object sender, EventArgs e)
        {
            _handle = 0;
            _SpectrumIdx = 2; // Options.MainSettings.PlayerSpectrumIndex;

            labelInfo.Text = "00:00:00.000 / 00:00:00.000";

            _PositionTrack.Maximum = 100;
            _PositionTrack.Enabled = true;

            _CheckLoop.Checked = Loop;
            _VolumeTrack.Value = (int)(Volume * 100);

            // create a secure timer
            _UpdateTimer = new BASSTimer(_UpdateInterval);
            _UpdateTimer.Tick += new EventHandler(timerUpdate_Tick);

            /*if (Bass.CurrentDevice == -1)
            {
                // Using the play function for the first time
                Console.WriteLine("Player: Bass Initialisation");

                Bass.Free();

                if (!Bass.Init(_defaultSoundDevice, 44100, DeviceInitFlags.Default, IntPtr.Zero))
                {
                    Console.WriteLine("Player: Error Init Bass: {0:G}", Bass.LastError);
                    return;
                }
            }*/

            // Bass.UpdatePeriod = 230;

            // Bass.Start(); return false

            if (!Bass.Configure(Configuration.IncludeDefaultDevice, true))
            {
                Console.WriteLine("Player: Error Configure Bass: {0:G}", Bass.LastError);
                return;
            }

            if (!Bass.Init())
            {
                Console.WriteLine("Player: Error Init Bass: {0:G}", Bass.LastError);
                return;
            }
        }

        public void LoadFile(string soundPath)
        {
            if (string.IsNullOrEmpty(soundPath) || !File.Exists(soundPath))
            {
                FreeStream();
                return;
            }

            // Free Stream
            FreeStream();

            // File Stream
            var fileMemory = File.ReadAllBytes(soundPath);

            // _handle = Bass.CreateStream(soundfile, 0, 0, BassFlags.Default | BassFlags.Float | BassFlags.AutoFree);

            // _handle = Bass.CreateStream(soundfile);

            _handle = Bass.CreateStream(fileMemory, 0, fileMemory.Length, BassFlags.Default);

            // Free
            try
            {
                fileMemory = null;
                File.Delete(soundPath);
                GC.Collect();
            }
            catch { }

            if (_handle == 0)
            {
                Console.WriteLine("Player: Error Creating stream for {0} {1:G}", soundPath, Bass.LastError);
                return;
            }

            _Length = Bass.ChannelBytes2Seconds(_handle, Bass.ChannelGetLength(_handle));

            // Bass.FloatingPointDSP = true;

            if (Bass.ChannelSetDevice(_handle, 1))
            {
                Console.WriteLine("Player: Error Set Device {0:G}", Bass.LastError);
                return;
            }

            // Set Volume
            Volume = _VolumeTrack.Value / 100.0;

            // Set Balance
            Balance = _PanTrack.Value / 100.0f;

            // Play
            Play(true);
        }

        public void Play(bool Restart = false)
        {
            if (_handle != 0)
            {
                if (Bass.ChannelIsActive(_handle) == PlaybackState.Stopped || Bass.ChannelIsActive(_handle) == PlaybackState.Paused)
                {
                    if (Loop)
                    {
                        Bass.ChannelAddFlag(_handle, BassFlags.Loop);
                    }
                    else
                    {
                        Bass.ChannelRemoveFlag(_handle, BassFlags.Loop);
                    }

                    if (!Bass.ChannelPlay(_handle, Restart))
                    {
                        Console.WriteLine("Player: Error Playing {0:G}", Bass.LastError);
                        _handle = 0;
                        return;
                    }
                }

                _UpdateTimer.Start();
            }
        }

        public void Pause()
        {
            if (_handle != 0)
            {
                if (Bass.ChannelIsActive(_handle) == PlaybackState.Playing)
                {
                    Bass.ChannelPause(_handle);
                }
            }
        }

        public void Stop()
        {
            if (_handle != 0)
            {
                if (Bass.ChannelIsActive(_handle) == PlaybackState.Playing || Bass.ChannelIsActive(_handle) == PlaybackState.Paused)
                {
                    Bass.ChannelStop(_handle);
                }
            }
        }

        public void Reset()
        {
            if (_handle != 0)
            {
                Stop();

                Bass.ChannelSetPosition(_handle, 0);

                Play();
            }
        }

        public void FreeStream()
        {
            try
            {
                if (_handle != 0)
                {
                    _Length = 0;
                    // Position = -1;
                    Bass.StreamFree(_handle);
                    Bass.ChannelStop(_handle); // (StreamFree) Stop Playback.
                    Bass.MusicFree(_handle);
                    _handle = 0;
                }
            }
            catch { }
        }

        private bool IsPlaying
        {
            get
            {
                return Bass.ChannelIsActive(_handle) == PlaybackState.Playing ? true : false;
            }
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            if (IsPlaying)
            {
                Pause();
            }
            else
            {
                Play();
            }
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            // here we gather info about the stream, when it is playing...
            if (_handle == 0 || Bass.ChannelIsActive(_handle) != PlaybackState.Playing && Bass.ChannelIsActive(_handle) != PlaybackState.Paused)
            {
                // the stream is NOT playing anymore...
                _UpdateTimer.Stop();
                _PositionTrack.Value = 0;
                picSpectrum.Image = null;
                _vis.ClearPeaks();
                return;
            }

            // from here on, the stream is for sure playing...
            _TickCounter++;

            if (_TickCounter == 2)
            {
                _TickCounter = 0;

                this.labelInfo.Text = string.Format("{0:#0.00} / {1:#0.00}", Utils.TSHMSF(Position), Utils.TSHMSF(_Length));
            }

            _PositionTrack.Value = (int)(Math.Round((Position / _Length), 2) * _PositionTrack.Maximum);

            // update spectrum
            DrawSpectrum();
        }

        #region Loop
        public bool Loop
        {
            get { return _Loop; }
            set { _Loop = value; }
        }

        private void _CheckLoop_CheckedChanged(object sender, EventArgs e)
        {
            Loop = _CheckLoop.Checked;
        }
        #endregion

        #region Position
        private double Position
        {
            get { return Bass.ChannelBytes2Seconds(_handle, Bass.ChannelGetPosition(_handle)); }
            set { Bass.ChannelSetPosition(_handle, Bass.ChannelSeconds2Bytes(_handle, value)); }
        }
        private void OnPositionTrack_Scroll(object sender, EventArgs e)
        {
            if (_handle != 0)
            {
                Position = _Length * _PositionTrack.Value / (_PositionTrack.Maximum * 1.0);
            }
        }
        #endregion

        #region Frequency
        double _freq = 44100;

        /// <summary>
        /// Gets or Sets the Playback Frequency in Hertz.
        /// Default is 44100 Hz.
        /// </summary>
        public double Frequency
        {
            get { return _freq; }
            set
            {
                if (_handle != 0 || !Bass.ChannelSetAttribute(_handle, ChannelAttribute.Frequency, value))
                    return;
                _freq = value;
            }
        }
        #endregion

        #region Balance
        float _pan;

        /// <summary>
        /// Gets or Sets Balance (Panning) (-1 ... 0 ... 1).
        /// -1 Represents Completely Left.
        ///  1 Represents Completely Right.
        /// Default is 0.
        /// </summary>
        private float Balance
        {
            get { return _pan; }
            set
            {
                if (_handle != 0)
                {
                    if (!Bass.ChannelSetAttribute(_handle, ChannelAttribute.Pan, value))
                        return;
                }

                _pan = value;
            }
        }
        private void OnPanTrack_Scroll(object sender, EventArgs e)
        {
            if (_handle != 0)
            {
                Balance = _PanTrack.Value / 100.0f;
            }
        }
        #endregion

        #region Volume
        double _vol = 1.0;

        /// <summary>
        /// Gets or Sets the Playback Volume.
        /// </summary>
        public double Volume
        {
            get { return _vol; }
            set
            {
                if (_handle != 0)
                {
                    if (!Bass.ChannelSetAttribute(_handle, ChannelAttribute.Volume, value))
                        return;
                }
                _vol = value;
            }
        }
        private void OnVolumeTrack_Scroll(object sender, EventArgs e)
        {
            if (_handle != 0)
            {
                Volume = _VolumeTrack.Value / 100.0;
            }
        }

        #endregion

        #region Spectrum

        private void pictureBoxSpectrum_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _SpectrumIdx++;
            else
                _SpectrumIdx--;

            if (_SpectrumIdx > 2)
                _SpectrumIdx = 0;
            if (_SpectrumIdx < 0)
                _SpectrumIdx = 2;

            //Options.MainSettings.PlayerSpectrumIndex = _specIdx;
            picSpectrum.Image = null;
            _vis.ClearPeaks();
        }

        private void DrawSpectrum()
        {
            switch (_SpectrumIdx)
            {
            case 0:
                picSpectrum.Image = _vis.CreateSpectrumLinePeak(_handle, picSpectrum.Width, picSpectrum.Height, Color.SeaGreen, Color.LightGreen, Color.Orange, Color.Black, 2, 1, 2, 10, false, false, false);
                break;
            // WaveForm
            case 1:
                picSpectrum.Image = _vis.CreateWaveForm(_handle, picSpectrum.Width, picSpectrum.Height, Color.Green, Color.Red, Color.Gray, Color.Black, 1, true, false, true);
                break;
            // line spectrum (width = resolution)
            case 2:
                picSpectrum.Image = _vis.CreateSpectrumLinePeak(_handle, picSpectrum.Width, picSpectrum.Height, Color.FromArgb(255, 226, 173), Color.FromArgb(250, 181, 63), Color.FromArgb(150, 150, 150), Color.Black, 3, 1, 1, 10, false, false, false);
                break;
            }
        }

        #endregion

    }
}