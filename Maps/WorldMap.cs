using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talos.Cryptography;
using Talos.Structs;

namespace Talos.Maps
{
    internal class WorldMap
    {
        internal string Field { get; }
        internal List<WorldMapNode> Nodes { get; }
        internal WorldMap(string field, params WorldMapNode[] nodes)
        {
            Field = field;
            Nodes = new List<WorldMapNode>(nodes);
        }

        internal uint GetCRC32()
        {
            MemoryStream output = new MemoryStream();
            BinaryWriter binaryWriter1 = new BinaryWriter(output);
            binaryWriter1.Write((byte)Nodes.Count);
            foreach (WorldMapNode node in Nodes)
            {
                BinaryWriter binaryWriter2 = binaryWriter1;
                Point position = node.Position;
                int x = position.X;
                binaryWriter2.Write((short)x);
                BinaryWriter binaryWriter3 = binaryWriter1;
                position = node.Position;
                int y = position.Y;
                binaryWriter3.Write((short)y);
                binaryWriter1.Write(node.Name);
                binaryWriter1.Write(node.MapID);
            }
            binaryWriter1.Flush();
            byte[] array = output.ToArray();
            binaryWriter1.Close();
            return CRC32.Calculate(array);
        }
    }
}
