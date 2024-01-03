using System;
using System.Collections.Generic;
using Talos;
using Talos.Cryptography;
using Talos.Enumerations;

namespace Talos.Networking
{
    internal delegate bool ServerMessageHandler(Client client, ServerPacket packet);
    internal sealed class ServerPacket : Packet
    {
        internal override EncryptMethod EncryptMethod
        {
            get
            {
                switch (_opcode)
                {
                    case 0:
                    case 3:
                    case 64:
                    case 126:
                        return EncryptMethod.None;
                    case 1:
                    case 2:
                    case 10:
                    case 86:
                    case 96:
                    case 98:
                    case 102:
                    case 111:
                        return EncryptMethod.Normal;
                    default:
                        return EncryptMethod.MD5Key;
                }
            }
        }

        internal ServerPacket(byte opcode)
            : base(opcode)
        {
        }

        internal ServerPacket(byte[] buffer)
            : base(buffer)
        {
        }

        internal override void Encrypt(Crypto crypto)
        {
            _position = _data.Length;

            // Generate random values
            ushort a = (ushort)(Utility.Random(65277) + 256);
            byte b = (byte)(Utility.Random(155) + 100);

            // Select encryption key based on EncryptMethod
            byte[] key = EncryptMethod switch
            {
                EncryptMethod.MD5Key => crypto.GenerateKey(a, b),
                EncryptMethod.Normal => crypto.Key,
                _ => null  // Handle other cases as needed
            };

            if (key == null)
            {
                return; // No encryption required
            }

            // Perform encryption
            for (int i = 0; i < _data.Length; i++)
            {
                int num2 = i / crypto.Key.Length % 256;
                _data[i] ^= (byte)(crypto.Salt[num2] ^ key[i % key.Length]);

                if (num2 != _sequence)
                {
                    _data[i] ^= crypto.Salt[_sequence];
                }
            }

            // Write encryption bytes
            WriteByte((byte)(((int)a % 256) ^ 0x74));
            WriteByte((byte)(b ^ 0x24));
            WriteByte((byte)(((a >> 8) % 256) ^ 0x64));
        }

        internal override void Decrypt(Crypto crypto)
        {
            int num = _data.Length - 3;

            // Extract values from the last three bytes
            ushort ushort_ = (ushort)(((_data[num + 2] << 8) | _data[num]) ^ 0x6474);
            byte byte_ = (byte)(_data[num + 1] ^ 0x24);

            // Select decryption key based on EncryptMethod
            byte[] key = EncryptMethod switch
            {
                EncryptMethod.MD5Key => crypto.GenerateKey(ushort_, byte_),
                EncryptMethod.Normal => crypto.Key,
                _ => null  // Handle other cases as needed
            };

            if (key == null)
            {
                return; // No decryption required
            }

            // Perform decryption
            for (int i = 0; i < num; i++)
            {
                int num2 = i / crypto.Key.Length % 256;
                _data[i] ^= (byte)(crypto.Salt[num2] ^ key[i % key.Length]);

                if (num2 != _sequence)
                {
                    _data[i] ^= crypto.Salt[_sequence];
                }
            }

            // Resize the array to remove the last three bytes
            Array.Resize(ref _data, num);
        }

        internal ServerPacket Copy()
        {
            ServerPacket serverPacket = new ServerPacket(_opcode);
            serverPacket.Write(_data);
            serverPacket._created = _created;
            return serverPacket;
        }

        public override string ToString()
        {
            string text = GetHexString().Substring(0, 2);

            // Define a dictionary to map hash values to strings
            Dictionary<uint, string> messageMap = new Dictionary<uint, string>
        {
            { 350953279u, "[Sound] Recv> " },
            { 334175660u, "[RemSpell] Recv> " },
            { 233362851u, "[HeartBeatB] Recv> " },
            { 384802707u, "[MapData] Recv> " },
            { 368025088u, "[HeartBeatA] Recv> " },
            { 367583803u, "[LobbyNotification] Recv> " },
            { 434988469u, "[Attributes] Recv> " },
            { 418357945u, "[EffectsBar] Recv> " },
            { 418063755u, "[MapInfo] Recv> " },
            { 451471898u, "[MapChangePending] Recv> " },
            { 435135564u, "[Cooldown] Recv> " },
            { 451766088u, "[DisplayItemMonster] Recv> " },
            { 451618993u, "[AddSpell] Recv> " },
            { 485174231u, "[CreatureTurn] Recv> " },
            { 468396612u, "[RemoveItem] Recv> " },
            { 468249517u, "[LobbyControls] Recv> " },
            { 518729469u, "[HealthBar] Recv> " },
            { 502098945u, "[Location] Recv> " },
            { 485321326u, "[UserID] Recv> " },
            { 569209421u, "[ConnectionInfo] Recv> " },
            { 535654183u, "[LobbyMessage] Recv> " },
            { 518876564u, "[Redirect] Recv> " },
            { 955388848u, "[WorldMap] Recv> " },
            { 736691421u, "[Metafile] Recv> " },
            { 1005721705u, "[MerchantMenu] Recv> " },
            { 972166467u, "[RemoveSkill] Recv> " },
            { 1290499943u, "[CreatureAnimation] Recv> " },
            { 1240167086u, "[MapChangeComplete] Recv> " },
            { 1056054562u, "[AddSkill] Recv> " },
            { 1844308465u, "[internalChat] Recv> " },
            { 1827530846u, "[RemoveObject] Recv> " },
            { 1810753227u, "[AddItem] Recv> " },
            { 1894641322u, "[SysMsg] Recv> " },
            { 1877863703u, "[ClientWalk] Recv> " },
            { 1861086084u, "[CreatureWalk] Recv> " },
            { 2247118416u, "[Door] Recv> " },
            { 2095428297u, "[AcceptConnection] Recv> " },
            { 2279835011u, "[Exchange] Recv> " },
            { 2263896035u, "[DisplayUser] Recv> " },
            { 2313243154u, "[ServerTable] Recv> " },
            { 2297451273u, "[Board] Recv> " },
            { 2280673654u, "[Dialog] Recv> " },
            { 2346798392u, "[MapLoadComplete] Recv> " },
            { 2331006511u, "[AddEquipment] Recv> " },
            { 2314228892u, "[WorldList] Recv> " },
            { 2414894606u, "[RemoveEquipment] Recv> " },
            { 2381486463u, "[LightLevel] Send> " },
            { 2347784130u, "[Profile] Recv> " },
            { 2431672225u, "[ProfileSelf] Recv> " },
            { 2415041701u, "[RefreshResponse] Recv> " },
            { 2498929796u, "[Animation] Recv> " },
            { 2447611201u, "[CancelCast] Recv> " },
        };

            // Use the dictionary to get the corresponding string or a default value
            if (messageMap.TryGetValue(Utility.CalculateFNV(text), out string result))
            {
                return result + GetHexString();
            }

            // Default case for unknown hash values
            return "[**Unknown**] Recv> " + GetHexString();
        }

    }

}
