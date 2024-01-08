using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Talos.Cryptography;
using Talos.Enumerations;
using Talos.Structs;

namespace Talos.Networking
{
    internal abstract class Packet
    {

        protected byte _signature;

        protected byte _opcode;

        protected byte _sequence;

        protected byte[] _data;

        protected int _position;

        protected byte[] _original;

        protected DateTime _created;

        internal byte Signature => _signature;

        internal byte Opcode => _opcode;

        internal byte Sequence {get => _sequence; set => _sequence = value; }

        internal byte[] Data => _data;

        internal int Position {get => _position; set => _position = value; }

        internal byte[] Original => _original;

        internal DateTime Created => _created;

        internal bool ShouldEncrypt => EncryptMethod != EncryptMethod.None;

        internal abstract EncryptMethod EncryptMethod
        {
            get;
        }

        internal Packet(byte opcode)
        {
            _signature = 170;
            _opcode = opcode;
            _data = new byte[0];
        }

        internal Packet(byte[] buffer)
        {
            _original = buffer;
            _signature = buffer[0];
            _opcode = buffer[3];
            _sequence = buffer[4];
            _created = DateTime.UtcNow;
            int num = buffer.Length - (ShouldEncrypt ? 5 : 4);
            _data = new byte[num];
            Array.Copy(buffer, buffer.Length - num, _data, 0, num);
        }

        internal abstract void Encrypt(Crypto crypto);

        internal abstract void Decrypt(Crypto crypto);

        internal void Clear()
        {
            _position = 0;
            _data = new byte[0];
        }

        internal byte[] Read(int length)
        {
            if (_position + length > _data.Length)
            {
                throw new EndOfStreamException();
            }
            byte[] array = new byte[length];
            Array.Copy(_data, _position, array, 0, length);
            _position += length;
            return array;
        }

        internal byte ReadByte()
        {
            if (_position >= _data.Length)
            {
                throw new EndOfStreamException();
            }
            return _data[_position++];
        }

        internal sbyte ReadSByte()
        {
            if (_position >= _data.Length)
            {
                throw new EndOfStreamException();
            }
            return (sbyte)_data[_position++];
        }

        internal bool ReadBoolean()
        {
            if (_position >= _data.Length)
            {
                throw new EndOfStreamException();
            }
            return _data[_position++] != 0;
        }

        internal short ReadInt16()
        {
            byte[] array = Read(2);
            return (short)((array[0] << 8) | array[1]);
        }

        internal ushort ReadUInt16()
        {
            byte[] array = Read(2);
            return (ushort)((array[0] << 8) | array[1]);
        }

        internal int ReadInt32()
        {
            byte[] array = Read(4);
            return (array[0] << 24) | (array[1] << 16) | (array[2] << 8) | array[3];
        }

        internal uint ReadUInt32()
        {
            byte[] array = Read(4);
            return (uint)((array[0] << 24) | (array[1] << 16) | (array[2] << 8) | array[3]);
        }

        internal string ReadString()
        {
            int length = _data.Length;
            for (int i = 0; i < _data.Length; i++)
            {
                if (_data[i] == 0)
                {
                    length = i;
                    break;
                }
            }
            byte[] array = new byte[length - _position];
            Buffer.BlockCopy(_data, _position, array, 0, array.Length);
            _position = length + 1;
            if (_position > _data.Length)
            {
                _position = _data.Length;
            }
            return Encoding.GetEncoding(949).GetString(array);
        }

        internal string ReadString8()
        {
            if (_position >= _data.Length)
            {
                throw new EndOfStreamException();
            }
            int length = ReadByte();
            if (_position + length > _data.Length)
            {
                _position--;
                throw new EndOfStreamException();
            }
            byte[] bytes = Read(length);
            return Encoding.GetEncoding(949).GetString(bytes);
        }

        internal string ReadString16()
        {
            if (_position + 1 > _data.Length)
            {
                throw new EndOfStreamException();
            }
            int length = ReadUInt16();
            if (_position + length > _data.Length)
            {
                _position -= 2;
                throw new EndOfStreamException();
            }
            byte[] bytes = Read(length);
            return Encoding.GetEncoding(949).GetString(bytes);
        }

        internal Point ReadStruct()
        {
            if (_position + 4 > _data.Length)
            {
                throw new EndOfStreamException();
            }
            return new Point((short)ReadUInt16(), (short)ReadUInt16());
        }

        internal void Write(byte[] value)
        {
            int num = _position + value.Length;
            if (num > _data.Length)
            {
                Array.Resize(ref _data, num);
            }
            Array.Copy(value, 0, _data, _position, value.Length);
            _position += value.Length;
        }

        internal void WriteByte(byte value)
        {
            Write(new byte[1]
            {
            value
            });
        }

        internal void WriteSByte(sbyte value)
        {
            Write(new byte[1]
            {
            (byte)value
            });
        }

        internal void WriteBoolean(bool value)
        {
            WriteByte((byte)(value ? 1 : 0));
        }

        internal void WriteInt16(short value)
        {
            byte[] byte_ = new byte[2]
            {
            (byte)(value >> 8),
            (byte)value
            };
            Write(byte_);
        }

        internal void WriteUInt16(ushort value)
        {
            Write(new byte[2]
            {
            (byte)((uint)value >> 8),
            (byte)value
            });
        }

        internal void WriteInt32(int value)
        {
            byte[] byte_ = new byte[4]
            {
            (byte)(value >> 24),
            (byte)(value >> 16),
            (byte)(value >> 8),
            (byte)value
            };
            Write(byte_);
        }

        internal void WriteUInt32(uint value)
        {
            byte[] byte_ = new byte[4]
            {
            (byte)(value >> 24),
            (byte)(value >> 16),
            (byte)(value >> 8),
            (byte)value
            };
            Write(byte_);
        }

        internal void WriteString(string value, bool terminate = false)
        {
            byte[] bytes = Encoding.GetEncoding(949).GetBytes(value);
            Write(bytes);
            if (terminate)
            {
                WriteByte(0);
            }
        }

        internal void WriteString8(string value)
        {
            byte[] bytes = Encoding.GetEncoding(949).GetBytes(value);
            if (bytes.Length > 255)
            {
                throw new ArgumentOutOfRangeException("value", value, "Length of string must not exceed 255 characters");
            }
            WriteByte((byte)bytes.Length);
            Write(bytes);
        }

        internal void WriteString16(string value)
        {
            byte[] bytes = Encoding.GetEncoding(949).GetBytes(value);
            if (bytes.Length > 65535)
            {
                throw new ArgumentOutOfRangeException("value", value, "Length of string must not exceed 65535 characters");
            }
            WriteUInt16((ushort)bytes.Length);
            Write(bytes);
        }

        internal void WriteStruct(Point value)
        {
        	WriteInt16(value.X);
        	WriteInt16(value.Y);
        }

        internal void WriteArray(Array value)
        {
            foreach (object item in value)
            {
                if (item is char)
                {
                    WriteByte((byte)item);
                }
                if (item is byte)
                {
                    WriteByte((byte)item);
                }
                if (item is sbyte)
                {
                    WriteSByte((sbyte)item);
                }
                if (item is bool)
                {
                    WriteBoolean((bool)item);
                }
                if (item is short)
                {
                    WriteInt16((short)item);
                }
                if (item is ushort)
                {
                    WriteUInt16((ushort)item);
                }
                if (item is int)
                {
                    WriteInt32((int)item);
                }
                if (item is uint)
                {
                    WriteUInt32((uint)item);
                }
                if (item is string)
                {
                    WriteString8((string)item);
                }
                if (item is Point)
                {
                	WriteStruct((Point)item);
                }
                if (item is Array)
                {
                    WriteArray((Array)item);
                }
            }
        }

        internal byte[] CreatePacket()
        {
            int num = _data.Length + (ShouldEncrypt ? 5 : 4) - 3;
            byte[] array = new byte[num + 3];
            array[0] = _signature;
            array[1] = (byte)(num / 256);
            array[2] = (byte)(num % 256);
            array[3] = _opcode;
            array[4] = _sequence;
            Array.Copy(_data, 0, array, array.Length - _data.Length, _data.Length);
            return array;
        }

        internal string GetHexString()
        {
            byte[] array = new byte[_data.Length + 1];
            array[0] = _opcode;
            Array.Copy(_data, 0, array, 1, _data.Length);
            return BitConverter.ToString(array).Replace('-', ' ');
        }

        internal string GetAsciiString(bool bool_0 = true)
        {
            char[] array = new char[_data.Length + 1];
            byte[] array2 = new byte[_data.Length + 1];
            array2[0] = _opcode;
            Array.Copy(_data, 0, array2, 1, _data.Length);
            for (int i = 0; i < array2.Length; i++)
            {
                byte b = array2[i];
                array[i] = (char)(((b == 10 || b == 13) && !bool_0) ? b : ((b < 32 || b > 126) ? 46 : b));
            }
            return new string(array);
        }


    }

}
