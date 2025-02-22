using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Talos.PInvoke;
using Talos.Properties;

namespace Talos.Options
{
    public partial class GeneralPage : UserControl, IOptionsPage
    {
        private MainForm _mainForm;
        public GeneralPage(MainForm mainForm)
        {
            _mainForm = mainForm;
            InitializeComponent();
            darkAgesPath.Text = Settings.Default.DarkAgesPath;
            dataPath.Text = Settings.Default.DataPath;
            smallWindowOpt.Checked = Settings.Default.SmallWindowOpt;
            largeWindowOpt.Checked = Settings.Default.LargeWindowOpt;
            fullWindowOpt.Checked = Settings.Default.FullWindowOpt;
            whisperFlash.Checked = Settings.Default.whisperFlash;
            whisperSound.Checked = Settings.Default.whisperSound;
            enableKom.Checked = Settings.Default.enableKom;
            useDawnd.Checked = Settings.Default.useDawnd;
            removeSpamCbox.Checked = Settings.Default.removeSpam;
            botOpacitySldr.Value = Settings.Default.BotOpacity;
            daOpacitySldr.Value = Settings.Default.DAOpacity;
            botTransVal.Text = Settings.Default.BotOpacity.ToString();
            daTransVal.Text = Settings.Default.DAOpacity.ToString();
            enableOverlay.Checked = Settings.Default.EnableOverlay;
            logOnStartup.Checked = Settings.Default.LogOnStartup;
            paranoiaCbox.Checked = Settings.Default.paranoiaMode;
            chkNoWalls.Checked = Settings.Default.NoWalls;

        }

        public void Save()
        {
            Settings.Default.DarkAgesPath = darkAgesPath.Text;
            Settings.Default.DataPath = dataPath.Text;
            Settings.Default.SmallWindowOpt = smallWindowOpt.Checked;
            Settings.Default.LargeWindowOpt = largeWindowOpt.Checked;
            Settings.Default.FullWindowOpt = fullWindowOpt.Checked;
            Settings.Default.whisperFlash = whisperFlash.Checked;
            Settings.Default.whisperSound = whisperSound.Checked;
            Settings.Default.enableKom = enableKom.Checked;
            Settings.Default.useDawnd = useDawnd.Checked;
            Settings.Default.removeSpam = removeSpamCbox.Checked;
            Settings.Default.BotOpacity = botOpacitySldr.Value;
            Settings.Default.DAOpacity = daOpacitySldr.Value;
            Settings.Default.EnableOverlay = enableOverlay.Checked;
            Settings.Default.LogOnStartup = logOnStartup.Checked;
            Settings.Default.paranoiaMode = paranoiaCbox.Checked;
            Settings.Default.NoWalls = chkNoWalls.Checked;
            Settings.Default.Save();
        }

        private void botOpacitySldr_Scroll(object sender, EventArgs e)
        {
            _mainForm.Opacity = (sender as TrackBar).Value / 100.0;
            botTransVal.Text = (sender as TrackBar).Value.ToString();
        }

        private void daOpacitySldr_Scroll(object sender, EventArgs e)
        {
            if (sender is TrackBar trackBar)
            {
                byte opacity = (byte)Math.Truncate(255.0 / (100.0 / trackBar.Value));
                foreach (var client in _mainForm.Server.ClientList)
                {
                    if (client?.processId > 0)
                    {
                        IntPtr windowHandle = Process.GetProcessById(client.processId).MainWindowHandle;
                        NativeMethods.SetLayeredWindowAttributes(windowHandle, 0, opacity, 2);
                    }
                }
                daTransVal.Text = trackBar.Value.ToString();
            }
        }

        private void darkAgesPathButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog
            {
                Filter = "Executable File (*.exe)|*.exe",
                InitialDirectory = Path.GetDirectoryName(Settings.Default.DarkAgesPath)
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    darkAgesPath.Text = dialog.FileName;
                }
            }
        }

        private void dataPathButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog
            {
                SelectedPath = Settings.Default.DataPath
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    dataPath.Text = dialog.SelectedPath;
                }
            }
        }

        private void paranoiaCbox_CheckedChanged(object sender, EventArgs e)
        {
            _mainForm.Text = (sender as CheckBox).Checked ? "Talos(Paranoid)" : "Talos";
        }

        private void useDawnd_CheckedChanged(object sender, EventArgs e)
        {
            // If the user is enabling Dawnd, we want to enable the radio buttons for display sise,
            // otherwise we want to disable them
            if (useDawnd.Checked)
            {
                smallWindowOpt.Enabled = true;
                largeWindowOpt.Enabled = true;
                fullWindowOpt.Enabled = true;
            }
            else
            {
                smallWindowOpt.Enabled = false;
                largeWindowOpt.Enabled = false;
                fullWindowOpt.Enabled = false;
            }
        }
    }
}
