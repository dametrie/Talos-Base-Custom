using System.Windows.Forms;
using Talos.Properties;

namespace Talos.Options
{

    public partial class LogoutTimer : UserControl, IOptionsPage
    {
        MainForm _mainForm;
        public LogoutTimer(MainForm mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
        }

        public void Save()
        {
            Settings.Default.Save();
        }

        private void setTimerBtn_Click(object sender, System.EventArgs e)
        {
            _mainForm.hours = int.Parse(txtHours.Text);
            _mainForm.minutes = int.Parse(txtMinutes.Text);
            _mainForm.seconds = int.Parse(txtSeconds.Text);
            _mainForm.killTimer.Enabled = true;
            _mainForm.killTimer.Start();
            lblSet.Text = "Timer set!";
            setTimerBtn.Enabled = false;
        }

        private void ClearBtn_Click(object sender, System.EventArgs e)
        {
            _mainForm.killTimer.Stop();
            _mainForm.killTimer.Enabled = false;
            _mainForm.hours = 0;
            _mainForm.minutes = 0;
            _mainForm.seconds = 0;
            lblSet.Text = "Timer cleared!";
            setTimerBtn.Enabled = true;
            _mainForm.Text = "Talos";
        }
    }
}
