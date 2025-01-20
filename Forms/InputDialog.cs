using System.Windows.Forms;

namespace Talos.Forms
{
    public partial class InputDialog : Form
    {
        internal static DialogResult Show(
            IWin32Window owner,
            string message,
            out string input,
            string acceptMsg = "Save")
        {
            using (InputDialog inputDialog = new InputDialog(message))
            {
                inputDialog.saveBtn.Text = acceptMsg;
                DialogResult result = inputDialog.ShowDialog(owner);
                input = inputDialog.saveName.Text;
                return result;
            }
        }


        internal InputDialog(string message)
        {
            InitializeComponent();
            msgLbl.Text = message;
        }

    }
}
