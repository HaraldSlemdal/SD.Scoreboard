using System;
using System.IO;
using System.Speech.Synthesis;
using System.Windows.Forms;
using System.Media;
using Timer = System.Windows.Forms.Timer;
using NAudio.Wave;


namespace SD.Scoreboard;

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

        private SpeechSynthesizer synth = new SpeechSynthesizer();

        private readonly string beepSoundPath;
        private readonly string whistleSoundPath;

        public TimerForm(int activeSeconds, int pauseSeconds)
        {
            InitializeComponent();
            this.activeSeconds = activeSeconds;
            this.pauseSeconds = pauseSeconds;

            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            synth.Rate = 2;
            
            //beepSoundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds", "beep.wav");
            beepSoundPath = @"C:\Users\hs.SKOGDATA\Downloads\beep-329314.mp3";
            whistleSoundPath = @"C:\Users\hs.SKOGDATA\Downloads\calling-whistle-41861.mp3";

            ScaleControls();
            
            _ = StartActivePeriod();

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
        }

        private async Task StartActivePeriod()
        {
            isActivePeriod = true;
            remainingSeconds = activeSeconds;
            lblStatus.Text = "Aktiv periode";
            _ = LogPeriodStart();
            UpdateTimeLabel();
            homeScore = 0;
            awayScore = 0;
            UpdateScores();
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

                // Halfway announcement
                if (isActivePeriod && remainingSeconds == activeSeconds / 2)
                {
                    synth.SpeakAsync("Half way there");
                }

                if (!isActivePeriod && remainingSeconds == 20)
                {
                    synth.SpeakAsync("20 seconds. Get ready");
                }
                
                if (isActivePeriod && remainingSeconds == 10)
                {
                    synth.SpeakAsync("10 seconds");
                }

                if (remainingSeconds is <= 5 and > 0)
                {
                    PlaySound(beepSoundPath);
                }

                if (remainingSeconds == 0)
                {
                    PlaySound(whistleSoundPath);
                    
                    // switch state
                    if (isActivePeriod)
                    {
                        StartPausePeriod();
                        synth.SpeakAsync("Rest");
                    }
                    else
                    {
                        UpdateTotal();
                        _ = StartActivePeriod();
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
            if (e.KeyCode == Keys.Y && !yDown)
            {
                yDown = true;
                yHoldTriggered = false;
                yHoldTimer.Start();
                // check simultaneous
                if (rDown)
                {
                    // both pressed simultaneously -> toggle pause/start
                    TogglePause();
                }
            }
            else if (e.KeyCode == Keys.R && !rDown)
            {
                rDown = true;
                rHoldTriggered = false;
                rHoldTimer.Start();
                if (yDown)
                {
                    TogglePause();
                }
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
                    // short press -> increment
                    homeScore++;
                    UpdateScores();
                }
                else
                {
                    // hold already triggered, we already set to 0
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
                else
                {
                    // hold reset already
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
            // If both holdTriggered are true at same time, reset everything
            if (yHoldTriggered && rHoldTriggered)
            {
                // reset timer and scores
                homeScore = 0;
                awayScore = 0;
                isActivePeriod = true;
                remainingSeconds = activeSeconds;
                UpdateScores();
                UpdateTimeLabel();
            }
        }

        private async Task LogPeriodStart()
        {
            try
            {
                string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                Directory.CreateDirectory(dir);
                string file = Path.Combine(dir, DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                string line = DateTime.Now.ToString("HH:mm:ss") + " - Ny aktiv periode startet - Score: Team Yellow " + homeScore + " - Team Red " + awayScore;
                await File.AppendAllTextAsync(file, line + Environment.NewLine);
            }
            catch { /* ignore logging errors */ }
        }
        
        private void ScaleControls()
        {
            float scaleFactor = Math.Min((float)Screen.PrimaryScreen.Bounds.Width / 688f,
                (float)Screen.PrimaryScreen.Bounds.Height / 240f);

            foreach (Control c in this.Controls)
            {
                c.Font = new System.Drawing.Font(c.Font.FontFamily, c.Font.Size * scaleFactor, c.Font.Style);
                c.Width = (int)(c.Width * scaleFactor);
                c.Height = (int)(c.Height * scaleFactor);
                c.Left = (int)(c.Left * scaleFactor);
                c.Top = (int)(c.Top * scaleFactor);
            }
        }

        private void PlaySound(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    using var audioFile = new AudioFileReader(path);
                    using var outputDevice = new WaveOutEvent();
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                        
                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        Application.DoEvents();
                    }
                }
                catch
                {
                }
            }
        }
    }