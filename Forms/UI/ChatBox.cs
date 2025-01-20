using System.Windows.Forms;

namespace Talos.Forms.UI
{
    internal class ChatBox : TextBox
    {
        protected override bool IsInputKey(Keys keyData)
        {
            bool flag = true;
            switch (keyData)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Right:
                case Keys.Down:
                    return flag;
                default:
                    flag = base.IsInputKey(keyData);
                    goto case Keys.Left;
            }
        }
    }
}
