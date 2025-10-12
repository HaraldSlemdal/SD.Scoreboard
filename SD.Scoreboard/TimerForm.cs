using System;
using System.IO;
using System.Speech.Synthesis;
using System.Windows.Forms;
using System.Media;
using System.Text.RegularExpressions;
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
        private readonly string getReadySoundPath;
        private readonly string wellDoneSoundPath;
        private readonly string halfwaySoundPath;
        private readonly string tenSecondsSoundPath;
        
        // Preloaded audio
        private AudioFileReader beepReader;
        private WaveOutEvent beepPlayer;


        private AudioFileReader whistleReader;
        private WaveOutEvent whistlePlayer;
        
        private AudioFileReader getReadyReader;
        private WaveOutEvent getReadyPlayer;
        
        private AudioFileReader wellDoneReader;
        private WaveOutEvent wellDonePlayer;
        
        private AudioFileReader halfwayReader;
        private WaveOutEvent halfwayPlayer;
        
        private AudioFileReader tenSecondsReader;
        private WaveOutEvent tenSecondsPlayer;

        public TimerForm(int activeSeconds, int pauseSeconds)
        {
            InitializeComponent();
            this.activeSeconds = activeSeconds;
            this.pauseSeconds = pauseSeconds;

            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            synth.Rate = 2;
            
            //Lyder fra https://elevenlabs.io/
            //beepSoundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds", "beep.wav");
            beepSoundPath = @"C:\Users\hs.SKOGDATA\Downloads\beep-329314.mp3";
            whistleSoundPath = @"C:\Users\hs.SKOGDATA\Downloads\calling-whistle-41861.mp3";
            getReadySoundPath = @"C:\Users\hs.SKOGDATA\Downloads\ElevenLabs_Text_to_Speech_audio.mp3";
            //wellDoneSoundPath = @"C:\Users\hs.SKOGDATA\Downloads\well_done.mp3";
            wellDoneSoundPath = @"C:\Users\hs.SKOGDATA\Downloads\well_done2.mp3";
            //halfwaySoundPath = @"C:\Users\hs.SKOGDATA\Downloads\halfway.mp3";
            halfwaySoundPath = @"C:\Users\hs.SKOGDATA\Downloads\Halfway_there.mp3";
            tenSecondsSoundPath = @"C:\Users\hs.SKOGDATA\Downloads\10seconds.mp3";
            
            

            _ = ReadTodaysLog();
            PreloadSounds();
            //ScaleControls();

            //remainingSeconds = 20;
            //StartPausePeriod(false);
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

        private void StartPausePeriod(bool setRemainingSeconds = true)
        {
            isActivePeriod = false;
            if (setRemainingSeconds)
            {
                remainingSeconds = pauseSeconds;
            }
            
            lblStatus.Text = "Pause";
            UpdateTimeLabel();
        }
        
        private void PreloadSounds()
        {
            try
            {
                if (File.Exists(beepSoundPath))
                {
                    beepReader = new AudioFileReader(beepSoundPath);
                    beepPlayer = new WaveOutEvent();
                    beepPlayer.Init(beepReader);
                }


                if (File.Exists(whistleSoundPath))
                {
                    whistleReader = new AudioFileReader(whistleSoundPath);
                    whistlePlayer = new WaveOutEvent();
                    whistlePlayer.Init(whistleReader);
                }
                
                if (File.Exists(getReadySoundPath))
                {
                    getReadyReader = new AudioFileReader(getReadySoundPath);
                    getReadyPlayer = new WaveOutEvent();
                    getReadyPlayer.Init(getReadyReader);
                }
                
                if (File.Exists(wellDoneSoundPath))
                {
                    wellDoneReader = new AudioFileReader(wellDoneSoundPath);
                    wellDonePlayer = new WaveOutEvent();
                    wellDonePlayer.Init(wellDoneReader);
                }
                
                if (File.Exists(halfwaySoundPath))
                {
                    halfwayReader = new AudioFileReader(halfwaySoundPath);
                    halfwayPlayer = new WaveOutEvent();
                    halfwayPlayer.Init(halfwayReader);
                }
                
                if (File.Exists(tenSecondsSoundPath))
                {
                    tenSecondsReader = new AudioFileReader(tenSecondsSoundPath);
                    tenSecondsPlayer = new WaveOutEvent();
                    tenSecondsPlayer.Init(tenSecondsReader);
                }
            }
            catch { }
        }


        private void PlayBeep()
        {
            if (beepPlayer != null && beepReader != null)
            {
                beepReader.Position = 0;
                beepPlayer.Play();
            }
        }
        private void PlayWhistle()
        {
            if (whistlePlayer != null && whistleReader != null)
            {
                whistleReader.Position = 0;
                whistlePlayer.Play();
            }
        }
        
        private void PlayGetReady()
        {
            if (getReadyPlayer != null && getReadyReader != null)
            {
                getReadyReader.Position = 0;
                getReadyPlayer.Play();
            }
        }
        
        private void PlayWellDone()
        {
            if (wellDonePlayer != null && wellDoneReader != null)
            {
                wellDoneReader.Position = 0;
                wellDonePlayer.Play();
            }
        }
        
        private void PlayHalfway()
        {
            if (halfwayPlayer != null && halfwayReader != null)
            {
                halfwayReader.Position = 0;
                halfwayPlayer.Play();
            }
        }
        
        private void PlayTenSeconds()
        {
            if (tenSecondsPlayer != null && tenSecondsReader != null)
            {
                tenSecondsReader.Position = 0;
                tenSecondsPlayer.Play();
            }
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
                    PlayHalfway();
                }

                if (!isActivePeriod && remainingSeconds == 20)
                {
                    PlayGetReady();
                }
                
                if (isActivePeriod && remainingSeconds == 10)
                {
                    PlayTenSeconds();
                }

                if (remainingSeconds is <= 5 and > 0)
                {
                    PlayBeep();
                }

                if (remainingSeconds == 0)
                {
                    // switch state
                    if (isActivePeriod)
                    {
                        StartPausePeriod();
                        PlayWellDone();
                    }
                    else
                    {
                        PlayWhistle();
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
                //string line = DateTime.Now.ToString("HH:mm:ss") + " - Ny aktiv periode startet - Score: Team Yellow " + homeScore + " - Team Red " + awayScore;
                string line = DateTime.Now.ToString("HH:mm:ss") + " - Score: Team Yellow " + homeScore + " - Team Red " + awayScore;
                await File.AppendAllTextAsync(file, line + Environment.NewLine);
            }
            catch { /* ignore logging errors */ }
        }
        
        private async Task ReadTodaysLog()
        {
            try
            {
                string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                Directory.CreateDirectory(dir);
                string file = Path.Combine(dir, DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                //string line = DateTime.Now.ToString("HH:mm:ss") + " - Ny aktiv periode startet - Score: Team Yellow " + homeScore + " - Team Red " + awayScore;
                //string line = DateTime.Now.ToString("HH:mm:ss") + " - Score: Team Yellow " + homeScore + " - Team Red " + awayScore;
                
                string[] lines = await File.ReadAllLinesAsync(file);
                var regex = new Regex(@"Team Yellow (\\d+) - Team Red (\\d+)");
                
                foreach (string line in lines)
                {
                    var match = regex.Match(line);
                    if (match.Success)
                    {
                        int home = int.Parse(match.Groups[1].Value);
                        int away = int.Parse(match.Groups[2].Value);
                        
                        if (home > away)
                        {
                            homeWin++;
                        }
                        else if (away > home)
                        {
                            awayWin++;
                        }
                        else
                        {
                            draw++;
                        }
                    }
                }
                
                lblTotal.Text = $"{homeWin} - {draw} - {awayWin}";
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

        private void TimerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isActivePeriod)
            {
                LogPeriodStart().GetAwaiter();    
            }
            
            // Stopp timere
            tickTimer?.Stop();
            yHoldTimer?.Stop();
            rHoldTimer?.Stop();

            // Stopp og rydd opp i lyd
            beepPlayer?.Stop();
            beepPlayer?.Dispose();
            beepReader?.Dispose();

            whistlePlayer?.Stop();
            whistlePlayer?.Dispose();
            whistleReader?.Dispose();

            // Stopp tale og frigjør
            synth?.Dispose();

            // Avregistrere eventhandlers (valgfritt)
            this.KeyDown -= TimerForm_KeyDown;
            this.KeyUp -= TimerForm_KeyUp;
        }
    }