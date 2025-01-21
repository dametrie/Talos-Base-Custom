using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Talos.Options;

namespace Talos.Forms
{
    public partial class Options : Form
    {
        private MainForm _mainForm;

        private UserControl _currentPage;

        private List<UserControl> _pages = new List<UserControl>();


        public Options(MainForm mainForm)
        {
            _mainForm = mainForm;
            InitializeComponent();
            AddPage("General", new GeneralPage(mainForm));
            AddPage("Auto Ascend", new AutoAscend(mainForm));
            //AddPage("Character Closet", new CharacterCloset(mainForm));
            AddPage("HotKeys", new HotKeys(mainForm));
            AddPage("Logout Timer", new LogoutTimer(mainForm));
            AddPage("Effect Options", new SpriteOverride(mainForm));
            pageList.SelectedIndex = 0;
        }

        private void AddPage(string @string, UserControl userControl)
        {
            _pages.Add(userControl);
            pageList.Items.Add(@string);
        }
        private void pageList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_currentPage != null)
            {
                pagePanel.Controls.Remove(_currentPage);
            }
            _currentPage = _pages[pageList.SelectedIndex];
            pagePanel.Controls.Add(_currentPage);
        }

        private void acceptButton_Click(object sender, EventArgs e)
        {
            foreach (IOptionsPage page in _pages)
                page.Save();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            _mainForm.UnregisterAllHotkeys();
            _mainForm.GenerateDefaultHotKeys();
        }
    }
}

