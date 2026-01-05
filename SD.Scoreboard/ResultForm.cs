using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
using NAudio.Wave;
using NAudio.Wave.SampleProviders; 


namespace SD.Scoreboard
{
    public partial class ResultForm : Form
    {
        private int activeSeconds;
        private int pauseSeconds;
        private int remainingSeconds;
        private bool isActivePeriod = true;
        private Timer tickTimer;

        private int homeScore = 0;
        private int awayScore = 0;

        // Statistikk for 3 lag
        private class TeamStats
        {
            public string Name { get; set; }
            public int Played { get; set; }
            public int Won { get; set; }
            public int Draw { get; set; }
            public int Lost { get; set; }
            public int GoalsFor { get; set; }
            public int GoalsAgainst { get; set; }
            public int GoalDiff => GoalsFor - GoalsAgainst;
            public int Points => (Won * 3) + (Draw * 1);
        }

        private TeamStats red = new TeamStats { Name = "Rød" };
        private TeamStats yellow = new TeamStats { Name = "Gul" };
        private TeamStats blue = new TeamStats { Name = "Blå" };

        private int currentMatchIndex = 0; // 0: Rød-Gul, 1: Blå-Rød, 2: Gul-Blå

        private bool yDown = false;
        private bool rDown = false;

        private Timer yHoldTimer;
        private Timer rHoldTimer;
        private bool yHoldTriggered = false;
        private bool rHoldTriggered = false;

        // Sound files
        private readonly string beepSoundPath;
        private readonly string whistleSoundPath;
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

        public ResultForm(int activeSeconds, int pauseSeconds)
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
            //getReadySoundPath = Path.Combine(baseDir, "ElevenLabs_Text_to_Speech_audio.mp3");
            wellDoneSoundPath = Path.Combine(baseDir, "well_done2.mp3");
            halfwaySoundPath = Path.Combine(baseDir, "Halfway_there.mp3");
            tenSecondsSoundPath = Path.Combine(baseDir, "10seconds.mp3");

            InitAudio();
            PreloadSounds();
            
            ScaleControls();
            UpdateMatchLabels(); // Sett riktige navn med en gang
            StartActivePeriod(first: true);

            tickTimer = new Timer();
            tickTimer.Interval = 1000;
            tickTimer.Tick += TickTimer_Tick;
            tickTimer.Start();

            yHoldTimer = new Timer();
            yHoldTimer.Interval = 1000; // 1s hold
            yHoldTimer.Tick += (s, e) => { 
                yHoldTriggered = true; 
                yHoldTimer.Stop(); 
                ResetScoreForColor("Gul"); 
            };

            rHoldTimer = new Timer();
            rHoldTimer.Interval = 1000; // 1s hold
            rHoldTimer.Tick += (s, e) => { 
                rHoldTriggered = true; 
                rHoldTimer.Stop(); 
                ResetScoreForColor("Rød"); 
            };

            this.KeyPreview = true;
            this.KeyDown += ResultForm_KeyDown;
            this.KeyUp += ResultForm_KeyUp;
            this.FormClosing += ResultForm_FormClosing;
        }

        private void InitAudio()
        {
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 1)) { ReadFully = true };
            outputDevice = new WaveOutEvent { DesiredLatency = 60 }; 
            outputDevice.Init(mixer);
            outputDevice.Play(); 
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
            }
        }

        private void PlayCached(CachedSound sound)
        {
            if (sound != null)
            {
                var provider = new CachedSoundSampleProvider(sound);
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
            int topOffset = (int)(20 * scaleFactor);
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
            
            if (!first)
            {
                UpdateStats(); // Lagre resultater fra forrige kamp
                currentMatchIndex = (currentMatchIndex + 1) % 3; // Roter til neste kamp
                UpdateMatchLabels();
            }
            
            homeScore = 0;
            awayScore = 0;
            UpdateScores();
            UpdateTimeLabel();
            UpdateTotal(); // Oppdater tabell-visningen
        }

        private void UpdateMatchLabels()
        {
            // Kamp 1: Rød (Home) vs Gul (Away)
            // Kamp 2: Blå (Home) vs Rød (Away)
            // Kamp 3: Gul (Home) vs Blå (Away)
            switch (currentMatchIndex)
            {
                case 0:
                    lblHomeScoreDescription.Text = red.Name;
                    lblAwayScoreDescription.Text = yellow.Name;
                    break;
                case 1:
                    lblHomeScoreDescription.Text = blue.Name;
                    lblAwayScoreDescription.Text = red.Name;
                    break;
                case 2:
                    lblHomeScoreDescription.Text = yellow.Name;
                    lblAwayScoreDescription.Text = blue.Name;
                    break;
            }
        }

        private void UpdateStats()
        {
            TeamStats home = GetTeamByIndex(currentMatchIndex, true);
            TeamStats away = GetTeamByIndex(currentMatchIndex, false);

            home.Played++;
            away.Played++;
            home.GoalsFor += homeScore;
            home.GoalsAgainst += awayScore;
            away.GoalsFor += awayScore;
            away.GoalsAgainst += homeScore;

            if (homeScore > awayScore)
            {
                home.Won++;
                away.Lost++;
            }
            else if (awayScore > homeScore)
            {
                away.Won++;
                home.Lost++;
            }
            else
            {
                home.Draw++;
                away.Draw++;
            }
        }

        private TeamStats GetTeamByIndex(int index, bool isHome)
        {
            if (index == 0) return isHome ? red : yellow;
            if (index == 1) return isHome ? blue : red;
            return isHome ? yellow : blue;
        }

        private void UpdateTotal()
        {
            // Legg lagene i en liste for sortering
            var teams = new System.Collections.Generic.List<TeamStats> { red, yellow, blue };

            // Sorterer etter poeng (synkende), deretter målforskjell (synkende)
            teams.Sort((a, b) => {
                int res = b.Points.CompareTo(a.Points);
                if (res == 0) res = b.GoalDiff.CompareTo(a.GoalDiff);
                return res;
            });

            // Formatert tabell-visning
            string format = "{0}: {1}k  {2}s-{3}u-{4}t  ({5}{6})  {7}p";
            var lines = new System.Collections.Generic.List<string>();

            foreach (var team in teams)
            {
                lines.Add(string.Format(format, 
                    team.Name.PadRight(5), 
                    team.Played, 
                    team.Won, 
                    team.Draw, 
                    team.Lost, 
                    (team.GoalDiff >= 0 ? "+" : ""), 
                    team.GoalDiff, 
                    team.Points));
            }

            lblTotal.Text = string.Join("\n", lines);
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

        private void ResultForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                TogglePause();
            }
        
            // Logikk for hvem som får mål basert på kamp-index
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

        private void ResultForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Y)
            {
                yDown = false;
                yHoldTimer.Stop();
                if (!yHoldTriggered)
                {
                    // Y gir mål til Gul (hvis de spiller) eller Blå (hvis de spiller mot Rød)
                    if (lblHomeScoreDescription.Text == "Gul" || lblAwayScoreDescription.Text == "Gul")
                    {
                        if (lblHomeScoreDescription.Text == "Gul") homeScore++; else awayScore++;
                    }
                    else if (lblHomeScoreDescription.Text == "Blå") // Kamp 2: Blå vs Rød
                    {
                        homeScore++;
                    }
                    UpdateScores();
                }
            }
            else if (e.KeyCode == Keys.R)
            {
                rDown = false;
                rHoldTimer.Stop();
                if (!rHoldTriggered)
                {
                    // R gir mål til Rød (hvis de spiller) eller Blå (hvis de spiller mot Gul)
                    if (lblHomeScoreDescription.Text == "Rød" || lblAwayScoreDescription.Text == "Rød")
                    {
                        if (lblHomeScoreDescription.Text == "Rød") homeScore++; else awayScore++;
                    }
                    else if (lblAwayScoreDescription.Text == "Blå") // Kamp 3: Gul vs Blå
                    {
                        awayScore++;
                    }
                    UpdateScores();
                }
            }
        }

        private void ResetScoreForColor(string colorName)
        {
            if (lblHomeScoreDescription.Text == colorName) 
            {
                homeScore = 0;
            }
            else if (lblAwayScoreDescription.Text == colorName) 
            {
                awayScore = 0;
            }
            // Blå-logikk for reset
            else if (colorName == "Gul" && lblHomeScoreDescription.Text == "Blå") 
            {
                homeScore = 0; // Blå bruker "Gul"-tast (Y) i Kamp 2
            }
            else if (colorName == "Rød" && lblAwayScoreDescription.Text == "Blå")
            {
                awayScore = 0; // Blå bruker "Rød"-tast (R) i Kamp 3
            }
            
            UpdateScores();
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

        private void LogPeriodStart()
        {
            try
            {
                string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                Directory.CreateDirectory(dir);
                string file = Path.Combine(dir, DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                string line = DateTime.Now.ToString("HH:mm:ss") + $" - Ny aktiv periode startet - Score: Home {homeScore} - Away {awayScore}";
                File.AppendAllText(file, line + Environment.NewLine);
            }
            catch
            {
            }
        }

        private void ResultForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            tickTimer?.Stop();
            yHoldTimer?.Stop();
            rHoldTimer?.Stop();

            outputDevice?.Stop();
            outputDevice?.Dispose();
        }
    }
}