using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Talos.Cryptography;
using Talos.Networking;

namespace Talos.Forms
{
    public partial class ClientTab : UserControl
    {
        internal Client _client;
        internal ClientTab(Client client)
        {
            _client = client;
            _client.ClientTab = this;
            InitializeComponent();
        }

        internal void RemoveClient()
        {
            _client = null;
        }

        private void packetList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        internal void LogPackets(Packet  p)
        {
            if (base.InvokeRequired) { Invoke((Action)delegate { LogPackets(p); }); }
            else
                packetList.Items.Add(p);
        }
    }
}
