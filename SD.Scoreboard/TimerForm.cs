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

        // Statistikk for 2 lag
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
            //getReadySoundPath = Path.Combine(baseDir, "ElevenLabs_Text_to_Speech_audio.mp3");
            wellDoneSoundPath = Path.Combine(baseDir, "well_done2.mp3");
            halfwaySoundPath = Path.Combine(baseDir, "Halfway_there.mp3");
            tenSecondsSoundPath = Path.Combine(baseDir, "10seconds.mp3");

            InitAudio();
            PreloadSounds();

            LoadStatsFromLog();
            ScaleControls();
            UpdateMatchLabels(); // Sett riktige navn med en gang
            StartActivePeriod(first: true);

            tickTimer = new Timer();
            tickTimer.Interval = 1000;
            tickTimer.Tick += TickTimer_Tick;
            tickTimer.Start();

            yHoldTimer = new Timer();
            yHoldTimer.Interval = 1000; // 1s hold
            yHoldTimer.Tick += (s, e) =>
            {
                yHoldTriggered = true;
                yHoldTimer.Stop();
                ResetScoreForColor("Gul");
            };

            rHoldTimer = new Timer();
            rHoldTimer.Interval = 1000; // 1s hold
            rHoldTimer.Tick += (s, e) =>
            {
                rHoldTriggered = true;
                rHoldTimer.Stop();
                ResetScoreForColor("Rød");
            };

            this.KeyPreview = true;
            this.KeyDown += TimerForm_KeyDown;
            this.KeyUp += TimerForm_KeyUp;
            this.FormClosing += TimerForm_FormClosing;
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
            

            UpdateScores();
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
            float scaleFactor = Math.Min((float)Screen.PrimaryScreen.Bounds.Width / 688f,
                (float)Screen.PrimaryScreen.Bounds.Height / 240f);
            int topOffset = (int)(20 * scaleFactor);
            foreach (Control c in this.Controls)
            {
                c.Font = new System.Drawing.Font(c.Font.FontFamily, c.Font.Size * scaleFactor, c.Font.Style);
                c.Width = (int)(c.Width * scaleFactor);
                c.Height = (int)(c.Height * scaleFactor);
                c.Left = (int)(c.Left * scaleFactor);
                c.Top = (int)(c.Top * scaleFactor) + topOffset;

                // Bruk en monospaced font for tabellen (lblTotal) for å holde kolonnene rette
                if (c.Name == "lblTotal")
                {
                    c.Font = new System.Drawing.Font("Consolas", 12f * scaleFactor, System.Drawing.FontStyle.Bold);
                    // Viktig: Venstrejuster tabellen så kolonnene ikke flytter seg
                    ((Label)c).TextAlign = System.Drawing.ContentAlignment.TopCenter;
                }

            }
        }

        private void StartActivePeriod(bool first = false)
        {
            StartActivePeriod(first, false);
        }
        
        private void StartActivePeriod(bool first, bool skip)
        {
            PlayCached(whistleSound);

            isActivePeriod = true;
            remainingSeconds = activeSeconds;
            lblStatus.Text = "Aktiv periode";

            if (!first && !skip)
            {
                LogMatchResult();
                UpdateStats(); // Lagre resultater fra forrige kamp
                currentMatchIndex = (currentMatchIndex + 1) % 3; // Roter til neste kamp
                UpdateMatchLabels();
            }
            else if (skip)
            {
                currentMatchIndex = (currentMatchIndex + 1) % 3; // Roter til neste kamp uten å lagre
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
            //TODO: Fjernes
            if (index == 0) return isHome ? red : yellow;
            
            return isHome ? yellow : red;
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
            UpdateTotal(); // Oppdaterer tabellen live ved hver scoring
        }
        


        private void TimerForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                TogglePause();
            }
            else if (e.KeyCode == Keys.S)
            {
                StartActivePeriod(first: false, skip: true);
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

        private void TimerForm_KeyUp(object sender, KeyEventArgs e)
        {
            //:TODO Forenkles
            if (e.KeyCode == Keys.Y)
            {
                yDown = false;
                yHoldTimer.Stop();
                if (!yHoldTriggered)
                {
                    homeScore++;

                    UpdateScores();
                }
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

        
        private void LoadStatsFromLog()
        {
            return;
        }
        
        private void LogMatchResult()
        {
            try
            {
                string homeName = lblHomeScoreDescription.Text.Split(' ')[0];
                string awayName = lblAwayScoreDescription.Text.Split(' ')[0];

                string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                Directory.CreateDirectory(dir);
                string file = Path.Combine(dir, DateTime.Now.ToString("yyyy-MM-dd") + ".txt");

                string line =
                    $"{DateTime.Now:HH:mm:ss} - Sluttresultat: {homeName} {homeScore} - {awayScore} {awayName}";

                File.AppendAllText(file, line + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Kunne ikke skrive til logg: " + ex.Message);
            }
        }

        private void TimerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isActivePeriod)
            {
                LogMatchResult();
            }

            tickTimer?.Stop();
            yHoldTimer?.Stop();
            rHoldTimer?.Stop();

            outputDevice?.Stop();
            outputDevice?.Dispose();
            // mixer is managed object only; let GC collect
        }
        
        private void UpdateTotal()
        {
            var teams = new System.Collections.Generic.List<TeamStats> { red, yellow };

            // Lag en midlertidig liste for å vise live-statistikk uten å lagre den permanent ennå
            var displayStats = teams.Select(t => new TeamStats
            {
                Name = t.Name,
                Played = t.Played,
                Won = t.Won,
                Draw = t.Draw,
                Lost = t.Lost,
                GoalsFor = t.GoalsFor,
                GoalsAgainst = t.GoalsAgainst
            }).ToList();

            // Legg til score fra pågående kamp i visningen

            // Finn lagene som spiller nå (fjerner "(gul knapp)" osv. fra sjekken)
            string homeName = lblHomeScoreDescription.Text.Split(' ')[0];
            string awayName = lblAwayScoreDescription.Text.Split(' ')[0];

            var homeLive = displayStats.FirstOrDefault(t => t.Name == homeName);
            var awayLive = displayStats.FirstOrDefault(t => t.Name == awayName);

            if (homeLive != null && awayLive != null)
            {
                homeLive.Played++;
                awayLive.Played++;
                homeLive.GoalsFor += homeScore;
                homeLive.GoalsAgainst += awayScore;
                awayLive.GoalsFor += awayScore;
                awayLive.GoalsAgainst += homeScore;

                if (homeScore > awayScore)
                {
                    homeLive.Won++;
                    awayLive.Lost++;
                }
                else if (awayScore > homeScore)
                {
                    awayLive.Won++;
                    homeLive.Lost++;
                }
                else
                {
                    homeLive.Draw++;
                    awayLive.Draw++;
                }
            }

            displayStats.Sort((a, b) =>
            {
                int res = b.Points.CompareTo(a.Points);
                if (res == 0) res = b.GoalDiff.CompareTo(a.GoalDiff);
                return res;
            });

            // Definer faste bredder for hver kolonne
            string header = "#  Lag      S   V   U   T   Mål    +/-  P";
            string format = "{0,-3}{1,-9}{2,3}{3,4}{4,4}{5,4}  {6,2}-{7,-2} {8,4} {9,3}";

            var lines = new System.Collections.Generic.List<string> { header };

            for (int i = 0; i < displayStats.Count; i++)
            {
                var team = displayStats[i];
                string goalDiffStr = (team.GoalDiff > 0 ? "+" : "") + team.GoalDiff;

                lines.Add(string.Format(format,
                    (i + 1) + ".",
                    team.Name,
                    team.Played,
                    team.Won,
                    team.Draw,
                    team.Lost,
                    team.GoalsFor,
                    team.GoalsAgainst,
                    goalDiffStr,
                    team.Points
                ));
            }

            lblTotal.Text = string.Join("\n", lines);
        }
    }
}
