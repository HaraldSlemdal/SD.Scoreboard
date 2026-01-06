namespace SD.Scoreboard;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
        nudActiveMinutes.Value = 3;
        nudPauseMinutes.Value = 2;
        
        //For testing purposes
        //nudActiveSeconds.Value = 10;
        //nudPauseSeconds.Value = 5;
        
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

        Form displayForm;
        if (chkThreeTeams.Checked)
        {
            displayForm = new ResultForm(activeSeconds, pauseSeconds);
        }
        else
        {
            displayForm = new TimerForm(activeSeconds, pauseSeconds);
        }

        displayForm.Show();
        this.Hide();
        
        displayForm.FormClosed += (s, ev) => this.Show();
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        
    }
}