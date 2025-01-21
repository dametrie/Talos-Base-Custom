using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Talos.PInvoke;
using Talos.Properties;
using Talos.Utility;

namespace Talos.Options
{
    public partial class HotKeys : UserControl, IOptionsPage
    {
        private MainForm _mainForm;
        private List<Keys> mods;
        public HotKeys(MainForm mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;

            mods = new List<Keys>
            {
                Keys.Alt,
                Keys.Menu,
                Keys.Control,
                Keys.ControlKey,
                Keys.Shift,
                Keys.ShiftKey,
                Keys.LWin,
                Keys.RWin
            };

            Name = "HotKeys";


            refreshAllCbox.Checked = Settings.Default.RefreshAll;
            toggleBot.Text = Settings.Default.BotHotKey;
            toggleCasting.Text = Settings.Default.CastHotKey;
            toggleWalking.Text = Settings.Default.WalkHotKey;
            toggleSound.Text = Settings.Default.SoundHotKey;
            combo1.Text = Settings.Default.Combo1HotKey;
            combo2.Text = Settings.Default.Combo2HotKey;
            combo3.Text = Settings.Default.Combo3HotKey;
            combo4.Text = Settings.Default.Combo4HotKey;
        }

        internal void HandleHotKeyKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (!(sender is TextBox textBox))
                    return;

                int hotKeyId = GetHotKeyIdByTextBoxName(textBox.Name);
                if (hotKeyId == -1)
                    return;

                // Unregister any existing hotkey
                NativeMethods.UnregisterHotKey(_mainForm.Handle, hotKeyId);

                // Generate the key combination string
                var keyConverter = new KeysConverter();
                string modifierKeys = keyConverter.ConvertToString(e.Modifiers).Replace("None", "");
                string keyCombination = mods.Contains(e.KeyCode)
                    ? modifierKeys.TrimEnd()
                    : $"{modifierKeys} {keyConverter.ConvertToString(e.KeyCode)}".Trim();

                // Update the text box
                textBox.Text = keyCombination.Replace("+", " ");
                e.Handled = true;
            }
            catch
            {
                MessageDialog.Show(_mainForm, "An error occurred while handling the hotkey. Please report this issue.", this, true);
            }
        }

        internal void HandleHotKeyKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (!(sender is TextBox textBox) || string.IsNullOrWhiteSpace(textBox.Text))
                    return;

                // Parse the key combination from the text box
                var keyConverter = new KeysConverter();
                var keys = Regex.Matches(textBox.Text, @"\b[a-zA-Z0-9]+\b")
                    .Cast<Match>()
                    .Select(match => (Keys)keyConverter.ConvertFromString(match.Value))
                    .ToList();

                // Ensure the last key is not a modifier
                if (keys.Count == 0 || mods.Contains(keys[keys.Count - 1]))
                {
                    textBox.Clear();
                    return;
                }

                // Determine the hotkey ID
                int hotKeyId = GetHotKeyIdByTextBoxName(textBox.Name);
                if (hotKeyId == -1)
                    return;

                // Register the hotkey based on the number of keys
                uint keyHash = (uint)keys[keys.Count - 1].GetHashCode();
                uint modifierMask = 0;
                for (int i = 0; i < keys.Count - 1; i++)
                {
                    modifierMask |= KeyboardUtility.GetKeyModifierMask(keys[i]);
                }

                NativeMethods.RegisterHotKey(_mainForm.Handle, hotKeyId, modifierMask, keyHash);
            }
            catch
            {
                MessageDialog.Show(_mainForm, "An error occurred while registering the hotkey. Please report this issue.", this, true);
            }
        }

        private int GetHotKeyIdByTextBoxName(string name)
        {
            return name switch
            {
                "combo1" => 9,
                "combo2" => 10,
                "combo3" => 11,
                "combo4" => 12,
                "toggleBot" => 2,
                "toggleWalking" => 4,
                "toggleCasting" => 3,
                "toggleSound" => 5,
                _ => -1
            };
        }

        private void refreshAllCbox_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                if (checkBox.Checked)
                {
                    NativeMethods.RegisterHotKey(_mainForm.Handle, 1, 0, (uint)Keys.F5.GetHashCode());
                }
                else
                {
                    NativeMethods.UnregisterHotKey(_mainForm.Handle, 1);
                }
            }
        }

        public void Save() 
        { 
            Settings.Default.RefreshAll = refreshAllCbox.Checked;
            Settings.Default.BotHotKey = toggleBot.Text;
            Settings.Default.CastHotKey = toggleCasting.Text;
            Settings.Default.WalkHotKey = toggleWalking.Text;
            Settings.Default.SoundHotKey = toggleSound.Text;
            Settings.Default.Combo1HotKey = combo1.Text;
            Settings.Default.Combo2HotKey = combo2.Text;
            Settings.Default.Combo3HotKey = combo3.Text;
            Settings.Default.Combo4HotKey = combo4.Text;
            Settings.Default.Save();
        }
    }
}
