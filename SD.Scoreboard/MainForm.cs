namespace SD.Scoreboard;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
    }
    
    private void btnStart_Click(object sender, EventArgs e)
    {
        int activeSeconds = (int)nudActiveMinutes.Value * 60 + (int)nudActiveSeconds.Value;
        int pauseSeconds = (int)nudPauseMinutes.Value * 60 + (int)nudPauseSeconds.Value;
        if (activeSeconds <= 0 || pauseSeconds <= 0)
        {
            MessageBox.Show("Sett gyldige tidsintervaller (større enn 0).", "Feil", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var timerForm = new TimerForm(activeSeconds, pauseSeconds);
        timerForm.Show();
        this.Hide();
        timerForm.FormClosed += (s, ev) => this.Show();
    }
}