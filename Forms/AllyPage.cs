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
            //setup heals combo box
            dbIocCombox.Items.Clear();
            string[] heals = { "nuadhaich", "ard ioc", "ard ioc comlha", "mor ioc", "mor ioc comlha", "ioc", "beag ioc", };
            foreach (string spell in heals)
            {
                if (_client.Spellbook[spell] != null)
                    dbIocCombox.Items.Add(spell);
            }
            if (dbIocCombox.Items.Count > 0)
                dbIocCombox.SelectedIndex = 0;
            else
            {
                dbIocCbox.Enabled = false;
                dbIocCombox.Enabled = false;
                dbIocCombox.Text = String.Empty;
                dbIocNumPct.Enabled = false;
                dbIocNumPct.Value = 0;
            }

            //setup fas combo box  
            dbFasCombox.Items.Clear();
            string[] fases = { "ard fas nadur", "mor fas nadur", "fas nadur", "beag fas nadur" };
            foreach (string spell in fases)
            {
                if (_client.Spellbook[spell] != null)
                    dbFasCombox.Items.Add(spell);
            }
            if (dbFasCombox.Items.Count > 0)
                dbFasCombox.SelectedIndex = 0;
            else
            {
                dbFasCbox.Enabled = false;
                dbFasCombox.Enabled = false;
                dbFasCombox.Text = String.Empty;
            }

            //setup aite combo box
            dbAiteCombox.Items.Clear();
            string[] aites = { "ard naomh aite", "mor naomh aite", "naomh aite", "beag naomh aite" };
            foreach (string spell in aites)
            {
                if (_client.Spellbook[spell] != null)
                {
                    dbAiteCombox.Items.Add(spell);
                }

            }
            if (dbAiteCombox.Items.Count > 0)
                dbAiteCombox.SelectedIndex = 0;
            else
            {
                dbAiteCbox.Enabled = false;
                dbAiteCombox.Enabled = false;
                dbAiteCombox.Text = String.Empty;
            }

            //ao suain check box
            if (_client.Spellbook["ao suain"] != null || _client.Spellbook["Leafhopper Chirp"] != null)
                dispelSuainCbox.Enabled = true;
            else
                dispelSuainCbox.Enabled = false;

            //ao curse check box
            //Adam add grim scent or make new checkbox?
            if (_client.Spellbook["ao beag cradh"] != null || _client.Spellbook["ao cradh"] != null
                || _client.Spellbook["ao mor cradh"] != null || _client.Spellbook["ao ard cradh"] != null)
                dispelCurseCbox.Enabled = true;
            else
                dispelCurseCbox.Enabled = false;

            //ao puinsein check box
            if (_client.Spellbook["ao puinsein"] != null)
                dispelPoisonCbox.Enabled = true;
            else
                dispelPoisonCbox.Enabled = false;


            //armachd
            if (_client.Spellbook["armachd"] != null)
                dbArmachdCbox.Enabled = true;
            else
                dbArmachdCbox.Enabled = false;

            //beag cradh
            if (_client.Spellbook["beag cradh"] != null)
                dbBCCbox.Enabled = true;
            else
                dbBCCbox.Enabled = false;

            //regeneration
            if (_client.Spellbook["Regeneration"] != null || _client.Spellbook["Increased Regeneration"] != null)
                dbRegenCbox.Enabled = true;
            else
                dbRegenCbox.Enabled = false;

            //Lyliac
            if (_client.Spellbook["Lyliac Plant"] != null)
            {
                miscLyliacCbox.Enabled = true;
                miscLyliacTbox.Enabled = true;
            }
            else
            {
                miscLyliacCbox.Enabled = false;
                miscLyliacTbox.Enabled = false;
            }

            //MDC
            if (_client.Spellbook["mor dion comlha"] != null)
            {
                allyMDCRbtn.Enabled = true;
                allyMDCSpamRbtn.Enabled = true;
            }
            else
            {
                allyMDCRbtn.Enabled = false;
                allyMDCSpamRbtn.Enabled = false;
            }

            //MIC
            if (_client.Spellbook["mor ioc comlha"] != null)
                allyMICSpamRbtn.Enabled = true;
            else
                allyMICSpamRbtn.Enabled = false;
        }
    }
}
