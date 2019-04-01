using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ExtensionBlocks;

namespace JumpList.Automatic
{
    public class DestListEntry
    {
        public DestListEntry(byte[] rawBytes, int version, int mruPosition, short pathSize, int entrySize)
        {
            Version = version;
            PathSize = pathSize;
            EntrySize = entrySize;

            MRUPosition = mruPosition;

            Checksum = BitConverter.ToInt64(rawBytes, 0);

            var volDroidBytes = new byte[16];
            Buffer.BlockCopy(rawBytes, 8, volDroidBytes, 0, 16);

            VolumeDroid = new Guid(volDroidBytes);

            var fileDroidBytes = new byte[16];
            Buffer.BlockCopy(rawBytes, 24, fileDroidBytes, 0, 16);

            FileDroid = new Guid(fileDroidBytes);

            var volBirthDroidBytes = new byte[16];
            Buffer.BlockCopy(rawBytes, 40, volBirthDroidBytes, 0, 16);

            VolumeBirthDroid = new Guid(volBirthDroidBytes);

            var fileBirthDroidBytes = new byte[16];
            Buffer.BlockCopy(rawBytes, 56, fileBirthDroidBytes, 0, 16);

            FileBirthDroid = new Guid(fileBirthDroidBytes);

            Hostname = Encoding.GetEncoding(1252).GetString(rawBytes, 72, 16).Split('\0').First();

            EntryNumber = BitConverter.ToInt32(rawBytes, 88);
            Unknown0 = BitConverter.ToInt32(rawBytes, 92);
            AccessCount = BitConverter.ToSingle(rawBytes, 96);

            LastModified = DateTimeOffset.FromFileTime(BitConverter.ToInt64(rawBytes, 100)).ToUniversalTime();

            PinStatus = BitConverter.ToInt32(rawBytes, 108);

            if (version > 1)
            {
                Unknown1 = BitConverter.ToInt32(rawBytes, 112);
                Unknown2 = BitConverter.ToInt32(rawBytes, 116);
                Unknown3 = BitConverter.ToInt32(rawBytes, 120);
                Unknown4 = BitConverter.ToInt32(rawBytes, 124);

                var v3PathLen = BitConverter.ToInt16(rawBytes, 128) * 2;

                Path = Encoding.Unicode.GetString(rawBytes, 130, v3PathLen);
                RawPath = Path;
            }
            else
            {
                var v1PathLen = BitConverter.ToInt16(rawBytes, 112) * 2;

                Path = Encoding.Unicode.GetString(rawBytes, 114, v1PathLen);
                RawPath = Path;
            }

            if (Path.StartsWith("knownfolder"))
            {
                var kfId = Path.Split('{').Last();
                kfId = kfId.Substring(0, kfId.Length - 1);

                var fName = Utils.GetFolderNameFromGuid(kfId);

                Path = $"{Path} ==> {fName}";
            }

            if (Path.StartsWith("::"))
            {
                var pathSegs = Path.Split('\\');

                var newPathSegs = new List<string>();

                foreach (var pathSeg in pathSegs)
                {
                    try
                    {
                        var regexObj =
                            new Regex(
                                @"\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b|\(\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b\)|\{\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b\}",
                                RegexOptions.IgnoreCase);
                        var matchResults = regexObj.Match(pathSeg);


                        if (matchResults.Success)
                        {
                            var pguid = matchResults.Groups[0].Value;
                            pguid = pguid.Substring(1, pguid.Length - 2);
                            var pName = Utils.GetFolderNameFromGuid(pguid);
                            newPathSegs.Add(pName);
                        }
                        else
                        {
                            newPathSegs.Add(pathSeg);
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        // Syntax error in the regular expression
                    }
                }

                Path = $"{Path} ==> {string.Join("\\", newPathSegs)}";
            }

            var tempMac = FileDroid.ToString().Split('-').Last();

            MacAddress = Regex.Replace(tempMac, ".{2}", "$0:");
            MacAddress = MacAddress.Substring(0, MacAddress.Length - 1);

            CreationTime = GetDateTimeOffsetFromGuid(FileDroid);
        }

        public int Version { get; set; }

        public short PathSize { get; }

        public int EntrySize { get; }

        public long Checksum { get; }
        public int EntryNumber { get; }
        public int MRUPosition { get; }
        public Guid FileBirthDroid { get; }
        public Guid FileDroid { get; }
        public string Hostname { get; }
        public DateTimeOffset LastModified { get; }

        internal string RawPath { get; }
        public string Path { get; }
        public int PinStatus { get; }
        public int Unknown0 { get; }
        public float AccessCount { get; }

        public int Unknown1 { get; }
        public int Unknown2 { get; }
        public int Unknown3 { get; }
        public int Unknown4 { get; }
        public Guid VolumeBirthDroid { get; }
        public Guid VolumeDroid { get; }

        public DateTimeOffset CreationTime { get; }
        public string MacAddress { get; }

        private DateTimeOffset GetDateTimeOffsetFromGuid(Guid guid)
        {
            // offset to move from 1/1/0001, which is 0-time for .NET, to gregorian 0-time of 10/15/1582
            var gregorianCalendarStart = new DateTimeOffset(1582, 10, 15, 0, 0, 0, TimeSpan.Zero);
            const int versionByte = 7;
            const int versionByteMask = 0x0f;
            const int versionByteShift = 4;
            const byte timestampByte = 0;

            var bytes = guid.ToByteArray();

            // reverse the version
            bytes[versionByte] &= versionByteMask;
            bytes[versionByte] |= 0x01 >> versionByteShift;

            var timestampBytes = new byte[8];
            Array.Copy(bytes, timestampByte, timestampBytes, 0, 8);

            var timestamp = BitConverter.ToInt64(timestampBytes, 0);
            var ticks = timestamp + gregorianCalendarStart.Ticks;

            return new DateTimeOffset(ticks, TimeSpan.Zero);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Checksum: {Checksum}");
            sb.AppendLine($"VolumeDroid: {VolumeDroid}");
            sb.AppendLine($"VolumeBirthDroid: {VolumeBirthDroid}");
            sb.AppendLine($"FileDroid: {FileDroid}");
            sb.AppendLine($"FileBirthDroid: {FileBirthDroid}");
            sb.AppendLine($"Hostname: {Hostname}");
            sb.AppendLine($"EntryNumber: {EntryNumber}");
            sb.AppendLine($"MRUPosition: {MRUPosition}");
            sb.AppendLine($"LastMod: {LastModified}");
            sb.AppendLine($"PinStatus: {PinStatus}");
            sb.AppendLine($"Path: {Path}");
            sb.AppendLine($"MacAddress: {MacAddress}");
            sb.AppendLine($"CreationTime: {CreationTime}");
            sb.AppendLine($"Unknown0: {Unknown0}");
            sb.AppendLine($"AccessCount: {AccessCount}");
            sb.AppendLine($"Unknown2: {Unknown2}");
            sb.AppendLine($"Unknown3: {Unknown3}");
            sb.AppendLine($"Unknown4: {Unknown4}");

            return sb.ToString();
        }

        public byte[] ToBuffer()
        {
            List<byte> bufferList = new List<byte>();

            bufferList.AddRange(BitConverter.GetBytes(Checksum));
            bufferList.AddRange(VolumeDroid.ToByteArray());
            bufferList.AddRange(FileDroid.ToByteArray());
            bufferList.AddRange(VolumeBirthDroid.ToByteArray());
            bufferList.AddRange(FileBirthDroid.ToByteArray());

            byte[] tempBuffer = Encoding.GetEncoding(1252).GetBytes(Hostname);
            byte[] hostNameBuffer = new byte[16];
            Buffer.BlockCopy(tempBuffer, 0, hostNameBuffer, 0, tempBuffer.Length);
            bufferList.AddRange(hostNameBuffer);

            bufferList.AddRange(BitConverter.GetBytes(EntryNumber));
            bufferList.AddRange(BitConverter.GetBytes(Unknown0));
            bufferList.AddRange(BitConverter.GetBytes(AccessCount));
            bufferList.AddRange(BitConverter.GetBytes(LastModified.ToFileTime()));
            bufferList.AddRange(BitConverter.GetBytes(PinStatus));

            if (this.Version > 1)
            {
                bufferList.AddRange(BitConverter.GetBytes(Unknown1));
                bufferList.AddRange(BitConverter.GetBytes(Unknown2));
                bufferList.AddRange(BitConverter.GetBytes(Unknown3));
                bufferList.AddRange(BitConverter.GetBytes(Unknown4));
                bufferList.AddRange(BitConverter.GetBytes(PathSize));

                byte[] pathBuffer = new byte[PathSize * 2];
                byte[] tempPathBuffer = Encoding.Unicode.GetBytes(RawPath);
                Buffer.BlockCopy(tempPathBuffer, 0, pathBuffer, 0, tempPathBuffer.Length);
                bufferList.AddRange(pathBuffer);

                bufferList.AddRange(new byte[4] { 0, 0, 0, 0 });
            }
            else
            {
                byte[] pathBuffer = new byte[PathSize * 2];
                byte[] tempPathBuffer = Encoding.Unicode.GetBytes(RawPath);
                Buffer.BlockCopy(tempPathBuffer, 0, pathBuffer, 0, tempPathBuffer.Length);
                bufferList.AddRange(pathBuffer);
            }

            return bufferList.ToArray();
        }
    }
}