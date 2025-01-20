using System;
using System.Collections.Generic;
using Talos.Base;
using Talos.Cryptography;
using Talos.Enumerations;
using Talos.Utility;

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
            ushort a = (ushort)(RandomUtils.Random(65277) + 256);
            byte b = (byte)(RandomUtils.Random(155) + 100);

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
            WriteByte((byte)((a % 256) ^ 0x74));
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
            string opcodeHex = GetHexString().Substring(0, 2);
            byte opcode = Convert.ToByte(opcodeHex, 16); // Convert the hex string to a byte

            Dictionary<byte, string> messageMap = new Dictionary<byte, string>
            {
                { 0x00, "[ConnectionInfo] Recv> " },
                { 0x02, "[LoginMessage] Recv> " },
                { 0x03, "[Redirect] Recv> " },
                { 0x04, "[Location] Recv> " },
                { 0x05, "[UserID] Recv> " },
                { 0x07, "[DisplayVisibleEntities] Recv> " },
                { 0x08, "[Attributes] Recv> " },
                { 0x0A, "[ServerMessage] Recv> " },
                { 0x0B, "[ConfirmClientWalk] Recv> " },
                { 0x0C, "[CreatureWalk] Recv> " },
                { 0x0D, "[PublicMessage] Recv> " },
                { 0x0E, "[RemoveObject] Recv> " },
                { 0x0F, "[AddItemToPane] Recv> " },
                { 0x10, "[RemoveItemFromPane] Recv> " },
                { 0x11, "[CreatureTurn] Recv> " },
                { 0x13, "[HealthBar] Recv> " },
                { 0x15, "[MapInfo] Recv> " },
                { 0x17, "[AddSpellToPane] Recv> " },
                { 0x18, "[RemoveSpellFromPane] Recv> " },
                { 0x19, "[Sound] Recv> " },
                { 0x1A, "[BodyAnimation] Recv> " },
                { 0x1B, "[Notepad] Recv> " },
                { 0x1F, "[MapChangeComplete] Recv> " },
                { 0x20, "[LightLevel] Recv> " },
                { 0x22, "[RefreshResponse] Recv> " },
                { 0x29, "[Animation] Recv> " },
                { 0x2C, "[AddSkillToPane] Recv> " },
                { 0x2D, "[RemoveSkillFromPane] Recv> " },
                { 0x2E, "[WorldMap] Recv> " },
                { 0x2F, "[DisplayMenu] Recv> " },
                { 0x30, "[Dialog] Recv> " },
                { 0x31, "[Board] Recv> " },
                { 0x32, "[Door] Recv> " },
                { 0x33, "[DisplayAisling] Recv> " },
                { 0x34, "[Profile] Recv> " },
                { 0x36, "[WorldList] Recv> " },
                { 0x37, "[AddEquipment] Recv> " },
                { 0x38, "[Unequip] Recv> " },
                { 0x39, "[SelfProfile] Recv> " },
                { 0x3A, "[Effect] Recv> " },
                { 0x3B, "[HeartbeatResponse] Recv> " },
                { 0x3C, "[MapData] Recv> " },
                { 0x3F, "[Cooldown] Recv> " },
                { 0x42, "[Exchange] Recv> " },
                { 0x48, "[CancelCasting] Recv> " },
                { 0x49, "[ProfileRequest] Recv> " },
                { 0x4B, "[ForceClientPacket] Recv> " },
                { 0x4C, "[ConfirmExit] Recv> " },
                { 0x56, "[ServerTable] Recv> " },
                { 0x58, "[MapLoadComplete] Recv> " },
                { 0x60, "[LoginNotice] Recv> " },
                { 0x63, "[GroupRequest] Recv> " },
                { 0x66, "[LoginControls] Recv> " },
                { 0x67, "[MapChangePending] Recv> " },
                { 0x68, "[SynchronizeTicks] Recv> " },
                { 0x6F, "[MetaData] Recv> " },
                { 0x7E, "[AcceptConnection] Recv> " }
            };

            // Use the dictionary to get the corresponding string or a default value
            if (messageMap.TryGetValue(opcode, out string result))
            {
                return result + GetHexString();
            }

            // Default case for unknown opcodes
            return "[**Unknown**] Recv> " + GetHexString();
        }

    }

}
