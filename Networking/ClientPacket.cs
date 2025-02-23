using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Talos.Base;
using Talos.Cryptography;
using Talos.Definitions;
using Talos.Utility;

namespace Talos.Networking
{
    internal delegate bool ClientMessageHandler(Client client, ClientPacket packet);
    internal sealed class ClientPacket : Packet
    {
        internal bool IsDialog => _opcode == 57 || _opcode == 58;

        internal override EncryptMethod EncryptMethod
        {
            get
            {
                switch (_opcode)
                {
                    case 0:
                    case 16:
                    case 72:
                        return EncryptMethod.None;
                    case 2:
                    case 3:
                    case 4:
                    case 11:
                    case 38:
                    case 45:
                    case 58:
                    case 66:
                    case 67:
                    case 75:
                    case 87:
                    case 98:
                    case 104:
                    case 113:
                    case 115:
                    case 123:
                        return EncryptMethod.Normal;
                    default:
                        return EncryptMethod.MD5Key;
                }
            }
        }

        internal ClientPacket(byte opcode)
            : base(opcode)
        {
        }

        internal ClientPacket(byte[] buffer)
            : base(buffer)
        {
        }

        internal override void Encrypt(Crypto crypto)
        {
            _position = _data.Length;
            ushort num = (ushort)(RandomUtils.Random(65277) + 256);
            byte b = (byte)(RandomUtils.Random(155) + 100);
            byte[] key;
            switch (EncryptMethod)
            {
                default:
                    return;
                case EncryptMethod.MD5Key:
                    WriteByte(0);
                    WriteByte(_opcode);
                    key = crypto.GenerateKey(num, b);
                    break;
                case EncryptMethod.Normal:
                    WriteByte(0);
                    key = crypto.Key;
                    break;
            }
            for (int i = 0; i < _data.Length; i++)
            {
                int num2 = i / crypto.Key.Length % 256;
                _data[i] ^= (byte)(crypto.Salt[num2] ^ key[i % key.Length]);
                if (num2 != _sequence)
                {
                    _data[i] ^= crypto.Salt[_sequence];
                }
            }
            byte[] buffer = new byte[_data.Length + 2];
            buffer[0] = _opcode;
            buffer[1] = _sequence;
            Buffer.BlockCopy(_data, 0, buffer, 2, _data.Length);
            byte[] array3 = MD5.Create().ComputeHash(buffer);
            WriteByte(array3[13]); //Packet.bodyData
            WriteByte(array3[3]);
            WriteByte(array3[11]);
            WriteByte(array3[7]);
            WriteByte((byte)((num % 256) ^ 0x70));
            WriteByte((byte)(b ^ 0x23));
            WriteByte((byte)(((num >> 8) % 256) ^ 0x74));
        }

        internal override void Decrypt(Crypto crypto)
        {
            int num = _data.Length - 7;
            ushort a = (ushort)(((_data[num + 6] << 8) | _data[num + 4]) ^ 0x7470);
            byte b = (byte)(_data[num + 5] ^ 0x23);
            byte[] key;
            // Select encryption key based on EncryptMethod
            switch (EncryptMethod)
            {
                default:
                    return;
                case EncryptMethod.MD5Key:
                    num -= 2;
                    key = crypto.GenerateKey(a, b);
                    break;
                case EncryptMethod.Normal:
                    num--;
                    key = crypto.Key;
                    break;
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
            Array.Resize(ref _data, num);
        }

        internal void GenerateDialogHeader()
        {
            ushort num = CRC.Calculate(_data, 6, _data.Length - 6);
            _data[0] = (byte)RandomUtils.Random();
            _data[1] = (byte)RandomUtils.Random();
            _data[2] = (byte)((_data.Length - 4) / 256);
            _data[3] = (byte)((_data.Length - 4) % 256);
            _data[4] = (byte)(num / 256);
            _data[5] = (byte)(num % 256);
        }

        internal void EncryptDialog()
        {
            Array.Resize(ref _data, _data.Length + 6);
            Buffer.BlockCopy(_data, 0, _data, 6, _data.Length - 6);
            GenerateDialogHeader();
            int num = (_data[2] << 8) | _data[3];
            byte num2 = (byte)(_data[1] ^ (byte)(_data[0] - 45));
            byte b = (byte)(num2 + 114);
            byte b2 = (byte)(num2 + 40);
            _data[2] ^= b;
            _data[3] ^= (byte)((b + 1) % 256);
            for (int i = 0; i < num; i++)
            {
                _data[4 + i] ^= (byte)((b2 + i) % 256);
            }
        }

        internal void DecryptDialog()
        {
            byte num = (byte)(_data[1] ^ (byte)(_data[0] - 45));
            byte b = (byte)(num + 114);
            byte b2 = (byte)(num + 40);
            _data[2] ^= b;
            _data[3] ^= (byte)((b + 1) % 256);
            int num2 = (_data[2] << 8) | _data[3];
            for (int i = 0; i < num2; i++)
            {
                _data[4 + i] ^= (byte)((b2 + i) % 256);
            }
            Buffer.BlockCopy(_data, 6, _data, 0, _data.Length - 6);
            Array.Resize(ref _data, _data.Length - 6);
        }

        internal ClientPacket Copy()
        {
            ClientPacket clientPacket = new ClientPacket(_opcode);
            clientPacket.Write(_data);
            clientPacket._created = _created;
            return clientPacket;
        }

        public override string ToString()
        {
            string opcodeHex = GetHexString().Substring(0, 2);
            byte opcode = Convert.ToByte(opcodeHex, 16); // Convert the hex string to a byte

            // Define a dictionary to map opcodes to strings
            Dictionary<byte, string> messageMap = new Dictionary<byte, string>
            {
                { 0x00, "[Version] Sent> " },
                { 0x02, "[CreateCharRequest] Sent> " },
                { 0x03, "[Login] Sent> " },
                { 0x04, "[CreateCharFinalize] Sent> " },
                { 0x05, "[MapDataRequest] Sent> " },
                { 0x06, "[ClientWalk] Sent> " },
                { 0x07, "[Pickup] Sent> " },
                { 0x08, "[ItemDrop] Sent> " },
                { 0x0B, "[ExitRequest] Sent> " },
                { 0x0C, "[DisplayEntityRequest] Sent> " },
                { 0x0D, "[Ignore] Sent> " },
                { 0x0E, "[PublicMessage] Sent> " },
                { 0x0F, "[UseSpell] Sent> " },
                { 0x10, "[ClientJoin] Sent> " },
                { 0x11, "[Turn] Sent> " },
                { 0x13, "[SpaceBar] Sent> " },
                { 0x18, "[WorldListRequest] Sent> " },
                { 0x19, "[Whisper] Sent> " },
                { 0x1B, "[UserOptionToggle] Sent> " },
                { 0x1C, "[UseItem] Sent> " },
                { 0x1D, "[Emote] Sent> " },
                { 0x23, "[SetNotepad] Sent> " },
                { 0x24, "[GoldDrop] Sent> " },
                { 0x26, "[PasswordChange] Sent> " },
                { 0x29, "[ItemDroppedOnCreature] Sent> " },
                { 0x2A, "[GoldDroppedOnCreature] Sent> " },
                { 0x2D, "[ProfileRequest] Sent> " },
                { 0x2E, "[GroupRequest] Sent> " },
                { 0x2F, "[ToggleGroup] Sent> " },
                { 0x30, "[SwapSlot] Sent> " },
                { 0x38, "[RefreshRequest] Sent> " },
                { 0x39, "[MenuIteraction] Sent> " },
                { 0x3A, "[DialogResponse] Sent> " },
                { 0x3B, "[BoardRequest] Sent> " },
                { 0x3E, "[UseSkill] Sent> " },
                { 0x3F, "[WorldMapClick] Sent> " },
                { 0x43, "[ClickObject] Sent> " },
                { 0x44, "[Unequip] Sent> " },
                { 0x45, "[HeartBeat] Sent> " },
                { 0x47, "[RaiseStat] Sent> " },
                { 0x4A, "[Exchange] Sent> " },
                { 0x4B, "[NoticeRequest] Sent> " },
                { 0x4D, "[BeginChant] Sent> " },
                { 0x4E, "[DisplayChant] Sent> " },
                { 0x4F, "[Profile] Sent> " },
                { 0x57, "[ServerTableRequest] Sent> " },
                { 0x62, "[SequenceChange] Sent> " },
                { 0x68, "[HomePageRequest] Sent> " },
                { 0x75, "[SynchronizeTicks] Sent> " },
                { 0x79, "[SocialStatus] Sent> " },
                { 0x7B, "[MetaDataRequest] Sent> " }
            };


            // Use the dictionary to get the corresponding string or a default value
            if (messageMap.TryGetValue(opcode, out string result))
            {
                return result + GetHexString();
            }

            // Default case for unknown opcodes
            return "[**Unknown**] Sent> " + GetHexString();
        }


    }
}

