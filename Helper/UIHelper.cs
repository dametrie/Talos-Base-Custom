using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Talos.Base;

namespace Talos.Helper
{
    internal static class UIHelper
    {
        private static Client _client;

        internal static void Initialize(Client client)
        {
            _client = client;
        }

        public static async Task<T> GetUIState<T>(Func<T> getStateFunc)
        {
            return await Task.Run(getStateFunc);
        }

        internal static void SetupComboBox(ComboBox comboBox, string[] spellNames, Control control1, Control control2 = null, Control control3 = null, Control control4 = null, bool partialMatch = false, string[] genericNames = null, Dictionary<string, string> abbreviations = null)
        {
            comboBox.Items.Clear();
            HashSet<string> addedItems = new HashSet<string>();

            foreach (var spellName in spellNames)
            {
                if (partialMatch)
                {
                    var matchingSpells = _client.Spellbook.Where(spell => spell.Name.IndexOf(spellName, StringComparison.OrdinalIgnoreCase) >= 0);

                    foreach (var spell in matchingSpells)
                    {
                        // Check if there's an abbreviation for this spell pattern
                        var abbreviation = abbreviations?.FirstOrDefault(a => spell.Name.IndexOf(a.Key, StringComparison.OrdinalIgnoreCase) >= 0).Value;
                        if (!string.IsNullOrEmpty(abbreviation))
                        {
                            // Add the abbreviation if not already added
                            if (!addedItems.Contains(abbreviation))
                            {
                                comboBox.Items.Add(abbreviation);
                                addedItems.Add(abbreviation);
                            }
                        }
                        else
                        {
                            comboBox.Items.Add(spell.Name);
                        }
                    }
                }
                else
                {
                    if (_client.Spellbook[spellName] != null)
                    {
                        comboBox.Items.Add(spellName);
                    }
                    if (_client.HasItem(spellName))
                    {
                        comboBox.Items.Add(spellName);
                    }
                }
            }

            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
            else
            {
                DisableControls(control1, control2, control3, control4, comboBox);
            }
        }

        internal static void SetupCheckbox(CheckBox checkBox, params string[] spellOrItemNames)
        {
            bool enabled = spellOrItemNames.Any(name => _client.Spellbook[name] != null || _client.HasItem(name));
            checkBox.Enabled = enabled;
        }
        private static void DisableControls(Control control1, Control control2, Control control3, Control control4, ComboBox comboBox)
        {
            control1.Enabled = false;
            comboBox.Enabled = false;
            comboBox.Text = String.Empty;
            if (control2 is not null)
            {
                control2.Enabled = false;
                if (control2 is NumericUpDown numericUpDown)
                    numericUpDown.Value = 0;
            }
            if (control3 is not null)
            {
                control3.Enabled = false;
                if (control3 is TextBox textBox)
                    textBox.Text = String.Empty;
            }
            if (control4 is not null)
                control4.Enabled = false;
        }

    }
}
