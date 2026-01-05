using Timer = System.Windows.Forms.Timer;
using NAudio.Wave;
using NAudio.Wave.SampleProviders; 


namespace SD.Scoreboard
{
    public partial class TimerForm : Form
    {
        private int activeSeconds;
        private int pauseSeconds;
        private int remainingSeconds;
        private bool isActivePeriod = true;
        private Timer tickTimer;

        private int homeScore = 0;
        private int awayScore = 0;

        private int homeWin = 0;
        private int awayWin = 0;
        private int draw = 0;
        
        private bool yDown = false;
        private bool rDown = false;

        private Timer yHoldTimer;
        private Timer rHoldTimer;
        private bool yHoldTriggered = false;
        private bool rHoldTriggered = false;

        // Sound files
        private readonly string beepPath;
        private readonly string beepSoundPath;
        private readonly string whistleSoundPath;
        private readonly string getReadySoundPath;
        private readonly string wellDoneSoundPath;
        private readonly string halfwaySoundPath;
        private readonly string tenSecondsSoundPath;
        private CachedSound beepSound;
        private CachedSound whistleSound;
        private CachedSound halfwaySound;
        private CachedSound tenSecondsSound;
        private CachedSound periodOverSound;

        // One shared output + mixer kept alive for the app lifetime
        private IWavePlayer outputDevice;
        private MixingSampleProvider mixer; // float 32-bit

        public TimerForm(int activeSeconds, int pauseSeconds)
        {
            InitializeComponent();
            this.activeSeconds = activeSeconds;
            this.pauseSeconds = pauseSeconds;

            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;

            var baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds");
            //Lyder fra https://elevenlabs.io/
            
            beepSoundPath = Path.Combine(baseDir, "beep.mp3");
            whistleSoundPath = Path.Combine(baseDir, "calling-whistle.mp3");
            getReadySoundPath = Path.Combine(baseDir, "ElevenLabs_Text_to_Speech_audio.mp3");
            wellDoneSoundPath = Path.Combine(baseDir, "well_done2.mp3");
            halfwaySoundPath = Path.Combine(baseDir, "Halfway_there.mp3");
            tenSecondsSoundPath = Path.Combine(baseDir, "10seconds.mp3");

            InitAudio();
            PreloadSounds();
            ScaleControls();
            StartActivePeriod(first: true);

            tickTimer = new Timer();
            tickTimer.Interval = 1000;
            tickTimer.Tick += TickTimer_Tick;
            tickTimer.Start();

            yHoldTimer = new Timer();
            yHoldTimer.Interval = 1000; // 1s hold
            yHoldTimer.Tick += (s, e) => { yHoldTriggered = true; yHoldTimer.Stop(); homeScore = 0; UpdateScores(); };

            rHoldTimer = new Timer();
            rHoldTimer.Interval = 1000; // 1s hold
            rHoldTimer.Tick += (s, e) => { rHoldTriggered = true; rHoldTimer.Stop(); awayScore = 0; UpdateScores(); };

            this.KeyPreview = true;
            this.KeyDown += TimerForm_KeyDown;
            this.KeyUp += TimerForm_KeyUp;
            this.FormClosing += TimerForm_FormClosing;
        }

        private void InitAudio()
        {
            // Mixer runs continuously so the device is primed (no clipped starts)
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 1)) { ReadFully = true };
            outputDevice = new WaveOutEvent { DesiredLatency = 60 }; // ~60ms to be safe
            outputDevice.Init(mixer);
            outputDevice.Play(); // start with silence so device is ready
        }

        private void PreloadSounds()
        {
            try
            {
                if (File.Exists(beepSoundPath)) { beepSound = new CachedSound(beepSoundPath); }
                if (File.Exists(whistleSoundPath)) { whistleSound = new CachedSound(whistleSoundPath); }
                if (File.Exists(halfwaySoundPath)) { halfwaySound = new CachedSound(halfwaySoundPath); }
                if (File.Exists(tenSecondsSoundPath)) { tenSecondsSound = new CachedSound(tenSecondsSoundPath); }
                if (File.Exists(wellDoneSoundPath)) { periodOverSound = new CachedSound(wellDoneSoundPath); }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // swallow – we simply won't play sounds that fail loading
            }
        }

        private void PlayCached(CachedSound sound)
        {
            if (sound != null)
            {
                // Each play gets a fresh provider; mixer handles parallel playback safely
                var provider = new CachedSoundSampleProvider(sound);
                // If formats differ, convert to mixer's format
                ISampleProvider toAdd = provider;
                if (!provider.WaveFormat.Equals(mixer.WaveFormat))
                {
                    toAdd = new WdlResamplingSampleProvider(provider, mixer.WaveFormat.SampleRate);
                }
                mixer.AddMixerInput(toAdd);
            }
        }

        private void ScaleControls()
        {
            float scaleFactor = Math.Min((float)Screen.PrimaryScreen.Bounds.Width / 688f, (float)Screen.PrimaryScreen.Bounds.Height / 240f);
            int topOffset = (int)(40 * scaleFactor); // Flytter alt litt ned
            foreach (Control c in this.Controls)
            {
                c.Font = new System.Drawing.Font(c.Font.FontFamily, c.Font.Size * scaleFactor, c.Font.Style);
                c.Width = (int)(c.Width * scaleFactor);
                c.Height = (int)(c.Height * scaleFactor);
                c.Left = (int)(c.Left * scaleFactor);
                c.Top = (int)(c.Top * scaleFactor) + topOffset;
            }
        }

        private void StartActivePeriod(bool first = false)
        {
            PlayCached(whistleSound);
            
            isActivePeriod = true;
            remainingSeconds = activeSeconds;
            lblStatus.Text = "Aktiv periode";
            LogPeriodStart();
            if (!first)
            {
                UpdateTotal();
            }
            homeScore = 0;
            awayScore = 0;
            UpdateScores();
            UpdateTimeLabel();
        }

        private void StartPausePeriod()
        {
            isActivePeriod = false;
            remainingSeconds = pauseSeconds;
            lblStatus.Text = "Pause";
            UpdateTimeLabel();
        }

        private void TickTimer_Tick(object sender, EventArgs e)
        {
            if (remainingSeconds > 0)
            {
                remainingSeconds--;
                UpdateTimeLabel();

                if (isActivePeriod && remainingSeconds == activeSeconds / 2)
                {
                    PlayCached(halfwaySound);
                }

                if (remainingSeconds == 10)
                {
                    PlayCached(tenSecondsSound);
                }

                if (remainingSeconds <= 5 && remainingSeconds > 0)
                {
                    PlayCached(beepSound);
                }

                if (remainingSeconds == 0)
                {
                    //PlayCached(whistleSound);
                    if (isActivePeriod)
                    {
                        StartPausePeriod();
                        PlayCached(periodOverSound);
                    }
                    else
                    {
                        StartActivePeriod();
                    }
                }
            }
        }

        private void UpdateTimeLabel()
        {
            int m = remainingSeconds / 60;
            int s = remainingSeconds % 60;
            lblTime.Text = $"{m}:{s:D2}";
        }

        private void UpdateScores()
        {
            lblHomeScore.Text = homeScore.ToString();
            lblAwayScore.Text = awayScore.ToString();
        }
        
        private void UpdateTotal()
        {
            if (homeScore > awayScore)
            {
                homeWin++;
            }
            else if (awayScore > homeScore)
            {
                awayWin++;
            }
            else
            {
                draw++;
            }
            
            lblTotal.Text = $"{homeWin} - {draw} - {awayWin}";
        }

        private void TimerForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                TogglePause();
            }
            
            if (e.KeyCode == Keys.Y && !yDown)
            {
                yDown = true;
                yHoldTriggered = false;
                yHoldTimer.Start();
            }
            else if (e.KeyCode == Keys.R && !rDown)
            {
                rDown = true;
                rHoldTriggered = false;
                rHoldTimer.Start();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void TimerForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Y)
            {
                yDown = false;
                yHoldTimer.Stop();
                if (!yHoldTriggered)
                {
                    homeScore++;
                    UpdateScores();
                }
                CheckBothHeldReset();
            }
            else if (e.KeyCode == Keys.R)
            {
                rDown = false;
                rHoldTimer.Stop();
                if (!rHoldTriggered)
                {
                    awayScore++;
                    UpdateScores();
                }
                CheckBothHeldReset();
            }
        }

        private void TogglePause()
        {
            if (tickTimer.Enabled)
            {
                tickTimer.Stop();
                lblStatus.Text = "Paused";
            }
            else
            {
                tickTimer.Start();
                lblStatus.Text = isActivePeriod ? "Aktiv periode" : "Pause";
            }
        }

        private void CheckBothHeldReset()
        {
            if (yHoldTriggered && rHoldTriggered)
            {
                homeScore = 0;
                awayScore = 0;
                isActivePeriod = true;
                remainingSeconds = activeSeconds;
                UpdateScores();
                UpdateTimeLabel();
            }
        }

        private void LogPeriodStart()
        {
            try
            {
                string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                Directory.CreateDirectory(dir);
                string file = Path.Combine(dir, DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                string line = DateTime.Now.ToString("HH:mm:ss") + $" - Ny aktiv periode startet - Score: Team Yellow {homeScore} - Team Red {awayScore}";
                File.AppendAllText(file, line + Environment.NewLine);
            }
            catch
            {
            }
        }

        private void TimerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            tickTimer?.Stop();
            yHoldTimer?.Stop();
            rHoldTimer?.Stop();

            outputDevice?.Stop();
            outputDevice?.Dispose();
            // mixer is managed object only; let GC collect
        }
    }
}
