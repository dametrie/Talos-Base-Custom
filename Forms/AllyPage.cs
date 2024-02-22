using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Talos.Base;
using Talos.Forms.UI;

namespace Talos.Forms
{
    internal partial class AllyPage : UserControl
    {
        internal Client _client;
        private Ally _ally;
        public AllyPage(Ally ally, Client client)
        {
            base.Name = ally.Name;
            _ally = ally;
            _client = client;
            InitializeComponent();
            OnlyDisplaySpellsWeHave();
        }

        private void OnlyDisplaySpellsWeHave()
        {
            SetupComboBox(dbIocCombox, new[] { "nuadhaich", "ard ioc", "ard ioc comlha", "mor ioc", "mor ioc comlha", "ioc", "beag ioc" }, dbIocCbox, dbIocNumPct);
            SetupComboBox(dbFasCombox, new[] { "ard fas nadur", "mor fas nadur", "fas nadur", "beag fas nadur" }, dbFasCbox);
            SetupComboBox(dbAiteCombox, new[] { "ard naomh aite", "mor naomh aite", "naomh aite", "beag naomh aite" }, dbAiteCbox);

            SetControlEnabled(dispelSuainCbox, new[] { "ao suain", "Leafhopper Chirp" });
            SetControlEnabled(dispelCurseCbox, new[] { "ao beag cradh", "ao cradh", "ao mor cradh", "ao ard cradh" });
            SetControlEnabled(dispelPoisonCbox, "ao puinsein");
            SetControlEnabled(dbArmachdCbox, "armachd");
            SetControlEnabled(dbBCCbox, "beag cradh");
            SetControlEnabled(dbRegenCbox, new[] { "Regeneration", "Increased Regeneration" });
            SetControlEnabled(miscLyliacCbox, "Lyliac Plant", additionalControl: miscLyliacTbox);

            SetRadioButtonEnabled(allyMDCRbtn, "mor dion comlha");
            SetRadioButtonEnabled(allyMDCSpamRbtn, "mor dion comlha");
            SetRadioButtonEnabled(allyMICSpamRbtn, "mor ioc comlha");
        }

        private void SetupComboBox(ComboBox comboBox, string[] spells, Control disableControl, NumericUpDown numericUpDown = null)
        {
            comboBox.Items.Clear();
            foreach (var spell in spells)
            {
                if (_client.Spellbook[spell] != null)
                    comboBox.Items.Add(spell);
            }

            bool hasSpells = comboBox.Items.Count > 0;
            comboBox.Enabled = hasSpells;
            if (hasSpells)
                comboBox.SelectedIndex = 0;
            else
            {
                disableControl.Enabled = false;
                comboBox.Text = string.Empty;
                if (numericUpDown != null)
                {
                    numericUpDown.Enabled = false;
                    numericUpDown.Value = 0;
                }
            }
        }

        private void SetControlEnabled(Control control, string spellOrItemName, Control additionalControl = null)
        {
            bool isEnabled = _client.Spellbook[spellOrItemName] != null || _client.HasItem(spellOrItemName);
            control.Enabled = isEnabled;
            if (additionalControl != null)
                additionalControl.Enabled = isEnabled;
        }

        private void SetControlEnabled(Control control, string[] spellOrItemNames, Control additionalControl = null)
        {
            bool isEnabled = spellOrItemNames.Any(name => _client.Spellbook[name] != null || _client.HasItem(name));
            control.Enabled = isEnabled;
            if (additionalControl != null)
                additionalControl.Enabled = isEnabled;
        }

        private void SetRadioButtonEnabled(RadioButton radioButton, string spellName)
        {
            radioButton.Enabled = _client.Spellbook[spellName] != null;
        }

    }
}
