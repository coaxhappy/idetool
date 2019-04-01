using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JumpList.Custom
{
    public class CustomDestination
    {
        private readonly byte[] footerBytes = {0xAB, 0xFB, 0xBF, 0xBA};

        public CustomDestination(byte[] rawBytes, string sourceFile)
        {
            if (rawBytes.Length == 0)
            {
                throw new Exception("Empty file");
            }

            SourceFile = sourceFile;

            var appid = Path.GetFileName(sourceFile).Split('.').FirstOrDefault();
            if (appid != null)
            {
                var aid = new AppIdInfo(appid);
                AppId = aid;
            }
            else
            {
                AppId = new AppIdInfo("Unable to determine AppId");
            }

            if (rawBytes.Length <= 24)
            {
                throw new Exception("Empty custom destinations jump list");
            }

            var footerSig = BitConverter.ToInt32(footerBytes, 0);
            var fileSig = BitConverter.ToInt32(rawBytes, rawBytes.Length - 4);

            if (footerSig != fileSig)
            {
                throw new Exception("Invalid signature (footer missing)");
            }

            Entries = new List<Entry>();

            var index = 0;

            //first, check for footer offsets. some files have more than one

            var footerOffsets = new List<int>();

            while (index < rawBytes.Length)
            {
                var lo = ByteSearch(rawBytes, footerBytes, index);

                if (lo == -1)
                {
                    break;
                }

                footerOffsets.Add(lo);

                index = lo + footerBytes.Length; //add length so we do not hit on it again
            }

            var byteChunks = new List<byte[]>();

            var absOffsets = new List<int>();

            var chunkStart = 0;
            foreach (var footerOffset in footerOffsets)
            {
                var chunkSize = footerOffset - chunkStart + 4;
                var bytes = new byte[chunkSize];

                Buffer.BlockCopy(rawBytes, chunkStart, bytes, 0, bytes.Length);

                absOffsets.Add(chunkStart);

                byteChunks.Add(bytes);

                chunkStart += chunkSize;
            }

            var counter = 0;
            foreach (var byteChunk in byteChunks)
            {
                if (byteChunk.Length > 30)
                {
                    var e = new Entry(byteChunk, absOffsets[counter]);

                    Entries.Add(e);
                    counter += 1;
                }
            }
        }

        public string SourceFile { get; }

        public AppIdInfo AppId { get; }

        public List<Entry> Entries { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Source: {SourceFile}");
            sb.AppendLine($"    AppId: {AppId}");
            sb.AppendLine($"    Total entries: {Entries.Count}");

            var entryNum = 0;
            foreach (var entry in Entries)
            {
                sb.AppendLine($"    Entry #: {entryNum}, Rank: {entry.Rank}");

                if (entry.Name.Length > 0)
                {
                    sb.AppendLine($"   Name: {entry.Name}");
                }

                sb.AppendLine($"    Total lnk count: {entry.LnkFiles.Count}");

                foreach (var lnkFile in entry.LnkFiles)
                {
                    sb.AppendLine($"     lnk header flags: {lnkFile.Header.DataFlags}");
                    sb.AppendLine($"      Target created: {lnkFile.Header.TargetCreationDate}");
                    sb.AppendLine($"      Target modified: {lnkFile.Header.TargetModificationDate}");
                    sb.AppendLine($"      Target accessed: {lnkFile.Header.TargetLastAccessedDate}");
                }
                sb.AppendLine();
                entryNum += 1;
            }


            return sb.ToString();
        }


        public static int ByteSearch(byte[] searchIn, byte[] searchBytes, int start = 0)
        {
            var found = -1;
            var matched = false;
            //only look at this if we have a populated search array and search bytes with a sensible start
            if (searchIn.Length > 0 && searchBytes.Length > 0 && start <= searchIn.Length - searchBytes.Length &&
                searchIn.Length >= searchBytes.Length)
            {
                //iterate through the array to be searched
                for (var i = start; i <= searchIn.Length - searchBytes.Length; i++)
                {
                    //if the start bytes match we will start comparing all other bytes
                    if (searchIn[i] == searchBytes[0])
                    {
                        if (searchIn.Length > 1)
                        {
                            //multiple bytes to be searched we have to compare byte by byte
                            matched = true;
                            for (var y = 1; y <= searchBytes.Length - 1; y++)
                            {
                                if (searchIn[i + y] != searchBytes[y])
                                {
                                    matched = false;
                                    break;
                                }
                            }
                            //everything matched up
                            if (matched)
                            {
                                found = i;
                                break;
                            }
                        }
                        else
                        {
                            //search byte is only one bit nothing else to do
                            found = i;
                            break; //stop the loop
                        }
                    }
                }
            }
            return found;
        }
    }
}