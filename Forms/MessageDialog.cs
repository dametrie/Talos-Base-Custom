using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Talos
{
    public partial class MessageDialog : Form
    {

        internal static DialogResult Show(MainForm mainForm, string message, IWin32Window parent = null, bool cancel = true)
        {
            if (mainForm.InvokeRequired)
            {
                return (DialogResult)mainForm.Invoke((Func<DialogResult>)(() => Show(mainForm, message, parent)));
            }
            using (MessageDialog messageDialog = new MessageDialog(message))
            {
                messageDialog.cancelBtn.Visible = cancel;
                return messageDialog.ShowDialog(parent ?? mainForm);
            }
        }

        internal MessageDialog(string message)
        {
            InitializeComponent();
            messageLbl.Text = message;
        }
        public MessageDialog()
        {
            InitializeComponent();
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.OK;
        }


        private void cancelBtn_Click_1(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Cancel;
        }
    }
}
