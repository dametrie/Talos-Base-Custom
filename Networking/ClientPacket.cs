using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Talos;
using Talos.Cryptography;
using Talos.Enumerations;

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
            ushort num = (ushort)(Utility.Random(65277) + 256);
            byte b = (byte)(Utility.Random(155) + 100);
            byte[] key;
            switch (this.EncryptMethod)
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
            WriteByte((byte)(((int)num % 256) ^ 0x70));
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
            _data[0] = (byte)Utility.Random();
            _data[1] = (byte)Utility.Random();
            _data[2] = (byte)((_data.Length - 4) / 256);
            _data[3] = (byte)((_data.Length - 4) % 256);
            _data[4] = (byte)((int)num / 256);
            _data[5] = (byte)((int)num % 256);
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

        public string ToString()
        {
            string text = GetHexString().Substring(0, 2);

            //define a dictionary to map hash values to strings
            Dictionary<uint, string> messageMap = new Dictionary<uint, string>
            {
                { 334175660u, "[RequestWorldList] Sent> " },
                { 233362851u, "[HomePage] Sent> "},
                { 418357945u, "[DialogResponse] Sent> "},
                { 368025088u, "[Board] Sent> "},
                { 350953279u, "[Whisper] Sent> "},
                { 451766088u, "[Pickup] Sent> "},
                { 435135564u, "[ClickWorldMap] Sent> "},
                { 434988469u, "[Drop] Sent> "},
                { 485174231u, "[Turn] Sent> "},
                { 468543707u, "[Walk] Sent> "},
                { 468396612u, "[ClientJoin] Sent> "},
                { 502098945u, "[CreateB] Sent> "},
                { 485468421u, "[UseSkill] Sent> "},
                { 485321326u, "[RequestMapData] Sent> "},
                { 535654183u, "[CreateA] Sent> "},
                { 518876564u, "[Login] Sent> "},
                { 518729469u, "[Spacebar] Sent> "},
                { 972166467u, "[ProfileRequest] Sent> "},
                { 955388848u, "[GroupRequest] Sent> "},
                { 569209421u, "[Join] Sent> "},
                { 1206611848u, "[Emote] Sent> " },
                { 1022499324u, "[DropGoldOnCreature] Sent> "},
                { 1005721705u, "[ToggleGroup] Sent> "},
                { 1474509299u, "[RequestNotification] Sent> "},
                { 1324055181u, "[UseItem] Sent> "},
                { 1307277562u, "[UserOptions] Sent> "},
                { 1558397394u, "[Chant] Sent> "},
                { 1541619775u, "[PortraitText] Sent> "},
                { 1491286918u, "[Exchange] Sent> "},
                { 1827530846u, "[PublicChat] Sent>"},
                { 1810753227u, "[UseSpell] Sent> "},
                { 1575175013u, "[BeginChant] Sent> "},
                { 2263057392u, "[ClickObject] Sent> " },
                { 2112205916u, "[MetafileRequest] Sent> "},
                { 1877863703u, "[ClientExit] Sent> "},
                { 2330020773u, "[ServerTable] Sent> "},
                { 2314375987u, "[DropGold] Sent> "},
                { 2280673654u, "[SwapSlot] Sent> "},
                { 2363723106u, "[HeartBeat] Sent> "},
                { 2347931225u, "[ChangePassword] Sent> "},
                { 2330167868u, "[AdjustStat] Sent> "},
                { 2414894606u, "[RefreshRequest] Sent> "},
                { 2380500725u, "[RemoveEquipment] Sent> "},
                { 2363870201u, "[HeartBeatTimer] Sent> "},
                { 2565201629u, "[SocialStatus] Sent> "},
                { 2498929796u, "[DropItemOnCreature] Sent> "},
                { 2431672225u, "[Pursuit] Sent> "},
                { 501951850u,  "[DisplayEntityRequest] Sent> "},
            };

            // Use the dictionary to get the corresponding string or a default value
            if (messageMap.TryGetValue(Utility.CalculateFNV(text), out string result))
            {
                return result + GetHexString();
            }

            // Default case for unknown hash values
            return "[**Unknown**] Sent> " + GetHexString();
        }


    }
}

