using System;
using System.Security.Cryptography;
using Talos;
using Talos.Cryptography;
using Talos.Enumerations;

namespace Talos.Networking
{
    internal delegate bool ClientMessageHandler(Client client, ClientPacket packet);
    internal sealed class ClientPacket : Packet
    {
        internal bool IsDialog => m_opcode == 57 || m_opcode == 58;

        internal override EncryptMethod EncryptMethod
        {
            get
            {
                switch (m_opcode)
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
            m_position = m_data.Length;
            ushort num = (ushort)(Utility.Random(65277) + 256);
            byte b = (byte)(Utility.Random(155) + 100);
            byte[] key;
            switch (this.EncryptMethod)
            {
                default:
                    return;
                case EncryptMethod.MD5Key:
                    WriteByte(0);
                    WriteByte(m_opcode);
                    key = crypto.GenerateKey(num, b);
                    break;
                case EncryptMethod.Normal:
                    WriteByte(0);
                    key = crypto.Key;
                    break;
            }
            for (int i = 0; i < m_data.Length; i++)
            {
                int num2 = i / crypto.Key.Length % 256;
                m_data[i] ^= (byte)(crypto.Salt[num2] ^ key[i % key.Length]);
                if (num2 != m_sequence)
                {
                    m_data[i] ^= crypto.Salt[m_sequence];
                }
            }
            byte[] buffer = new byte[m_data.Length + 2];
            buffer[0] = m_opcode;
            buffer[1] = m_sequence;
            Buffer.BlockCopy(m_data, 0, buffer, 2, m_data.Length);
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
            int num = m_data.Length - 7;
            ushort ushort_ = (ushort)(((m_data[num + 6] << 8) | m_data[num + 4]) ^ 0x7470);
            byte byte_ = (byte)(m_data[num + 5] ^ 0x23);
            byte[] array;
            switch (EncryptMethod)
            {
                default:
                    return;
                case EncryptMethod.MD5Key:
                    num -= 2;
                    array = crypto.GenerateKey(ushort_, byte_);
                    break;
                case EncryptMethod.Normal:
                    num--;
                    array = crypto.Key;
                    break;
            }
            for (int i = 0; i < num; i++)
            {
                int num2 = i / crypto.Key.Length % 256;
                m_data[i] ^= (byte)(crypto.Salt[num2] ^ array[i % array.Length]);
                if (num2 != m_sequence)
                {
                    m_data[i] ^= crypto.Salt[m_sequence];
                }
            }
            Array.Resize(ref m_data, num);
        }

        internal void GenerateDialogHeader()
        {
            ushort num = CRC.Calculate(m_data, 6, m_data.Length - 6);
            m_data[0] = (byte)Utility.Random();
            m_data[1] = (byte)Utility.Random();
            m_data[2] = (byte)((m_data.Length - 4) / 256);
            m_data[3] = (byte)((m_data.Length - 4) % 256);
            m_data[4] = (byte)((int)num / 256);
            m_data[5] = (byte)((int)num % 256);
        }

        internal void EncryptDialog()
        {
            Array.Resize(ref m_data, m_data.Length + 6);
            Buffer.BlockCopy(m_data, 0, m_data, 6, m_data.Length - 6);
            GenerateDialogHeader();
            int num = (m_data[2] << 8) | m_data[3];
            byte num2 = (byte)(m_data[1] ^ (byte)(m_data[0] - 45));
            byte b = (byte)(num2 + 114);
            byte b2 = (byte)(num2 + 40);
            m_data[2] ^= b;
            m_data[3] ^= (byte)((b + 1) % 256);
            for (int i = 0; i < num; i++)
            {
                m_data[4 + i] ^= (byte)((b2 + i) % 256);
            }
        }

        internal void DecryptDialog()
        {
            byte num = (byte)(m_data[1] ^ (byte)(m_data[0] - 45));
            byte b = (byte)(num + 114);
            byte b2 = (byte)(num + 40);
            m_data[2] ^= b;
            m_data[3] ^= (byte)((b + 1) % 256);
            int num2 = (m_data[2] << 8) | m_data[3];
            for (int i = 0; i < num2; i++)
            {
                m_data[4 + i] ^= (byte)((b2 + i) % 256);
            }
            Buffer.BlockCopy(m_data, 6, m_data, 0, m_data.Length - 6);
            Array.Resize(ref m_data, m_data.Length - 6);
        }

        internal ClientPacket Copy()
        {
            ClientPacket clientPacket = new ClientPacket(m_opcode);
            clientPacket.Write(m_data);
            clientPacket.dateTime_0 = dateTime_0;
            return clientPacket;
        }

        public string ToString()
        {
            string text = GetHexString().Substring(0, 2);
            switch (CalculateFNV(text))
            {
                case 334175660u:
                    if (text == "18")
                    {
                        return "[RequestWorldList] Sent> " + GetHexString();
                    }
                    goto default;
                case 233362851u:
                    if (text == "68")
                    {
                        return "[HomePage] Sent> " + GetHexString();
                    }
                    goto default;
                case 418357945u:
                    if (text == "3A")
                    {
                        return "[DialogResponse] Sent> " + GetHexString();
                    }
                    goto default;
                case 368025088u:
                    if (text == "3B")
                    {
                        return "[Board] Sent> " + GetHexString();
                    }
                    goto default;
                case 350953279u:
                    if (text == "19")
                    {
                        return "[Whisper] Sent> " + GetHexString();
                    }
                    goto default;
                case 451766088u:
                    if (text == "07")
                    {
                        return "[Pickup] Sent> " + GetHexString();
                    }
                    goto default;
                case 435135564u:
                    if (text == "3F")
                    {
                        return "[ClickWorldMap] Sent> " + GetHexString();
                    }
                    goto default;
                case 434988469u:
                    if (text == "08")
                    {
                        return "[Drop] Sent> " + GetHexString();
                    }
                    goto default;
                case 485174231u:
                    if (text == "11")
                    {
                        return "[Turn] Sent> " + GetHexString();
                    }
                    goto default;
                case 468543707u:
                    if (text == "06")
                    {
                        return "[Walk] Sent> " + GetHexString();
                    }
                    goto default;
                case 468396612u:
                    if (text == "10")
                    {
                        return "[ClientJoin] Sent> " + GetHexString();
                    }
                    goto default;
                case 502098945u:
                    if (text == "04")
                    {
                        return "[CreateB] Sent> " + GetHexString();
                    }
                    goto default;
                case 485468421u:
                    if (text == "3E")
                    {
                        return "[UseSkill] Sent> " + GetHexString();
                    }
                    goto default;
                case 485321326u:
                    if (text == "05")
                    {
                        return "[RequestMapData] Sent> " + GetHexString();
                    }
                    goto default;
                case 535654183u:
                    if (text == "02")
                    {
                        return "[CreateA] Sent> " + GetHexString();
                    }
                    goto default;
                case 518876564u:
                    if (text == "03")
                    {
                        return "[Login] Sent> " + GetHexString();
                    }
                    goto default;
                case 518729469u:
                    if (text == "13")
                    {
                        return "[Spacebar] Sent> " + GetHexString();
                    }
                    goto default;
                case 972166467u:
                    if (text == "2D")
                    {
                        return "[ProfileRequest] Sent> " + GetHexString();
                    }
                    goto default;
                case 955388848u:
                    if (text == "2E")
                    {
                        return "[GroupRequest] Sent> " + GetHexString();
                    }
                    goto default;
                case 569209421u:
                    if (text == "00")
                    {
                        return "[Join] Sent> " + GetHexString();
                    }
                    goto default;
                case 1206611848u:
                    if (text == "1D")
                    {
                        return "[Emote] Sent> " + GetHexString();
                    }
                    goto default;
                case 1022499324u:
                    if (text == "2A")
                    {
                        return "[DropGoldOnCreature] Sent> " + GetHexString();
                    }
                    goto default;
                case 1005721705u:
                    if (text == "2F")
                    {
                        return "[ToggleGroup] Sent> " + GetHexString();
                    }
                    goto default;
                case 1474509299u:
                    if (text == "4B")
                    {
                        return "[RequestNotification] Sent> " + GetHexString();
                    }
                    goto default;
                case 1324055181u:
                    if (text == "1C")
                    {
                        return "[UseItem] Sent> " + GetHexString();
                    }
                    goto default;
                case 1307277562u:
                    if (text == "1B")
                    {
                        return "[UserOptions] Sent> " + GetHexString();
                    }
                    goto default;
                case 1558397394u:
                    if (text == "4E")
                    {
                        return "[Chant] Sent> " + GetHexString();
                    }
                    goto default;
                case 1541619775u:
                    if (text == "4F")
                    {
                        return "[PortraitText] Sent> " + GetHexString();
                    }
                    goto default;
                case 1491286918u:
                    if (text == "4A")
                    {
                        return "[Exchange] Sent> " + GetHexString();
                    }
                    goto default;
                case 1827530846u:
                    if (text == "0E")
                    {
                        return "[internalChat] Sent> " + GetHexString();
                    }
                    goto default;
                case 1810753227u:
                    if (text == "0F")
                    {
                        return "[UseSpell] Sent> " + GetHexString();
                    }
                    goto default;
                case 1575175013u:
                    if (text == "4D")
                    {
                        return "[BeginChant] Sent> " + GetHexString();
                    }
                    goto default;
                case 2263057392u:
                    if (text == "43")
                    {
                        return "[ClickObject] Sent> " + GetHexString();
                    }
                    goto default;
                case 2112205916u:
                    if (text == "7B")
                    {
                        return "[MetafileRequest] Sent> " + GetHexString();
                    }
                    goto default;
                case 1877863703u:
                    if (text == "0B")
                    {
                        return "[ClientExit] Sent> " + GetHexString();
                    }
                    goto default;
                case 2330020773u:
                    if (text == "57")
                    {
                        return "[ServerTable] Sent> " + GetHexString();
                    }
                    goto default;
                case 2314375987u:
                    if (text == "24")
                    {
                        return "[DropGold] Sent> " + GetHexString();
                    }
                    goto default;
                case 2280673654u:
                    if (text == "30")
                    {
                        return "[SwapSlot] Sent> " + GetHexString();
                    }
                    goto default;
                case 2363723106u:
                    if (text == "45")
                    {
                        return "[HeartBeat] Sent> " + GetHexString();
                    }
                    goto default;
                case 2347931225u:
                    if (text == "26")
                    {
                        return "[ChangePassword] Sent> " + GetHexString();
                    }
                    goto default;
                case 2330167868u:
                    if (text == "47")
                    {
                        return "[AdjustStat] Sent> " + GetHexString();
                    }
                    goto default;
                case 2414894606u:
                    if (text == "38")
                    {
                        return "[RefreshRequest] Sent> " + GetHexString();
                    }
                    goto default;
                case 2380500725u:
                    if (text == "44")
                    {
                        return "[RemoveEquipment] Sent> " + GetHexString();
                    }
                    goto default;
                case 2363870201u:
                    if (text == "75")
                    {
                        return "[HeartBeatTimer] Sent> " + GetHexString();
                    }
                    goto default;
                case 2565201629u:
                    if (text == "79")
                    {
                        return "[SocialStatus] Sent> " + GetHexString();
                    }
                    goto default;
                case 2498929796u:
                    if (text == "29")
                    {
                        return "[DropItemOnCreature] Sent> " + GetHexString();
                    }
                    goto default;
                case 2431672225u:
                    if (text == "39")
                    {
                        return "[Pursuit] Sent> " + GetHexString();
                    }
                    goto default;
                case 501951850u:
                    if (text == "12")
                    {
                        return "[DisplayEntityRequest] Sent> " + GetHexString();
                    }
                    goto default;
                default:
                    return "[**Unknown**] Sent> " + GetHexString();
            }
        }

        internal static uint CalculateFNV(string hash)
        {
            uint num = default(uint);
            if (hash != null)
            {
                num = 2166136261u;
                for (int i = 0; i < hash.Length; i++)
                {
                    num = (hash[i] ^ num) * 16777619;
                }
            }
            return num;
        }


    }
}

