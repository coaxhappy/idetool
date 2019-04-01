using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JumpList.Automatic
{
    public class DestListHeader
    {
        public DestListHeader(byte[] rawBytes)
        {
            Version = BitConverter.ToInt32(rawBytes, 0);
            NumberOfEntries = BitConverter.ToInt32(rawBytes, 4);
            NumberOfPinnedEntries = BitConverter.ToInt32(rawBytes, 8);
            UnknownCounter = BitConverter.ToSingle(rawBytes, 12);
            LastEntryNumber = BitConverter.ToInt32(rawBytes, 16);
            Unknown1 = BitConverter.ToInt32(rawBytes, 20);
            LastRevisionNumber = BitConverter.ToInt32(rawBytes, 24);
            Unknown2 = BitConverter.ToInt32(rawBytes, 28);
        }

        public int Version { get; }
        public int NumberOfEntries { get; private set; }
        public int NumberOfPinnedEntries { get; private set; }
        public float UnknownCounter { get; }
        public int LastEntryNumber { get; }
        public int Unknown1 { get; }
        public int LastRevisionNumber { get; }
        public int Unknown2 { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Version: {Version}");
            sb.AppendLine($"NumberOfEntries: {NumberOfEntries}");
            sb.AppendLine($"NumberOfPinnedEntries: {NumberOfPinnedEntries}");
            sb.AppendLine($"LastEntryNumber: {LastEntryNumber}");
            sb.AppendLine($"LastRevisionNumber: {LastRevisionNumber}");
            sb.AppendLine($"Unknown0: {UnknownCounter}");
            sb.AppendLine($"AccessCount: {Unknown1}");
            sb.AppendLine($"Unknown2: {Unknown2}");

            return sb.ToString();
        }

        public void RefreshHeader(int entryNumber, int entryPinnedNumber)
        {
            NumberOfEntries = entryNumber;
            NumberOfPinnedEntries = entryPinnedNumber;
        }
        
        public byte[] ToBuffer()
        {
            List<byte> byteList = new List<byte>();

            byteList.AddRange(BitConverter.GetBytes(Version).ToList());
            byteList.AddRange(BitConverter.GetBytes(NumberOfEntries));
            byteList.AddRange(BitConverter.GetBytes(NumberOfPinnedEntries));
            byteList.AddRange(BitConverter.GetBytes(UnknownCounter));
            byteList.AddRange(BitConverter.GetBytes(LastEntryNumber));
            byteList.AddRange(BitConverter.GetBytes(Unknown1));
            byteList.AddRange(BitConverter.GetBytes(LastRevisionNumber));
            byteList.AddRange(BitConverter.GetBytes(Unknown2));

            return byteList.ToArray();
        }
    }
}