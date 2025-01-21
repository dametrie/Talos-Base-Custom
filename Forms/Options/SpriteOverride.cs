using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Talos.Properties;

namespace Talos.Options
{
    public partial class SpriteOverride : UserControl, IOptionsPage
    {
        private MainForm _mainForm;
        public SpriteOverride(MainForm mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
            enableOverrideRBtn.Checked = Settings.Default.OverrideSprites;
            disableAllRBtn.Checked = Settings.Default.disableSprites;
            normalSpritesRBtn.Checked = Settings.Default.normalSprites;

            foreach (KeyValuePair<int, int> kvp in _mainForm.SpriteOverrides)
            {
                effectLbox.Items.Add($"{kvp.Key} is now {kvp.Value}");
            }
        }

        public void Save()
        {
            string dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            var contents = effectLbox.Items
                .OfType<string>()
                .Select(item =>
                {
                    var parts = item.Split(new[] { " is now " }, StringSplitOptions.None);
                    return $"{parts[0]} {parts[1]}";
                })
                .ToList();

            Directory.CreateDirectory(dataPath); // Ensure the directory exists
            File.WriteAllLines(Path.Combine(dataPath, "override"), contents);

            Settings.Default.OverrideSprites = enableOverrideRBtn.Checked;
            Settings.Default.disableSprites = disableAllRBtn.Checked;
            Settings.Default.normalSprites = normalSpritesRBtn.Checked;
            Settings.Default.Save();
        }

        private void removeOverrideBtn_Click(object sender, EventArgs e)
        {
            var selectedItems = effectLbox.SelectedItems.OfType<string>().ToList();

            foreach (var item in selectedItems)
            {
                var parts = item.Split(new[] { " is " }, StringSplitOptions.None);
                if (int.TryParse(parts[0], out int key))
                {
                    _mainForm.SpriteOverrides.Remove(key);
                    effectLbox.Items.Remove(item);
                }
            }
        }

        private void spriteOverBtn_Click(object sender, EventArgs e)
        {
            int oldSprite = (int)spriteOldNum.Value;
            int newSprite = (int)spriteNewNum.Value;

            if (!_mainForm.SpriteOverrides.ContainsKey(oldSprite))
            {
                _mainForm.SpriteOverrides.Add(oldSprite, newSprite);
                effectLbox.Items.Add($"{oldSprite} is now {newSprite}");
            }
        }

    }
}
