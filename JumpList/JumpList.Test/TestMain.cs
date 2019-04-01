using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FluentAssertions;
using JumpList.Automatic;
using JumpList.Custom;
using NUnit.Framework;

namespace JumpList.Test
{
    [TestFixture]
    public class TestMain
    {
        public static string BasePath = @"..\..\TestFiles";
        public static string Win10Path = Path.Combine(BasePath, "Win10");
        public static string Win2K3Path = Path.Combine(BasePath, "Win2k3");
        public static string Win7Path = Path.Combine(BasePath, "Win7");
        public static string Win80Path = Path.Combine(BasePath, "Win80");
        public static string Win81Path = Path.Combine(BasePath, "Win81");
        public static string Win2012Path = Path.Combine(BasePath, "Win2012");
        public static string Win2012R2Path = Path.Combine(BasePath, "Win2012R2");

        // A bunch of good jump lists that I don't want to share =)
        public static string LocalPath = @"C:\Users\e\AppData\Roaming\Microsoft\Windows\Recent";

        public static string LocalPath2 = @"D:\OneDrive\Jump lists and lnks\ITA_JumpLists";
        public static string LocalPath3 = @"D:\OneDrive\Jump lists and lnks\MTF";
        public static string LocalPath4 = @"D:\OneDrive\Jump lists and lnks\Tom";

        private readonly List<string> _allPaths = new List<string>
        {
            //MiscPath,
            //WinXpPath,
            Win10Path,
            //Win2K3Path,
            Win7Path,
            Win80Path,
            Win81Path,
            //LocalPath,
            LocalPath2,
            LocalPath3
            //Win2012Path,
            //Win2012R2Path,
        };

        [Test]
        public void AutoTests()
        {
            //  var r2 = File.ReadAllBytes(@"C:\Users\e\Desktop\ITA_JumpLists\PC2_Win10\AutomaticDestinations\5f7b5f1e01b83767.automaticDestinations-ms");
            // var aa = new AutomaticDestination(r2, @"C:\Users\e\Desktop\ITA_JumpLists\PC2_Win10\AutomaticDestinations\5f7b5f1e01b83767.automaticDestinations-ms");

//            var f = @"C:\Users\eric\Desktop\NewToAdd.txt";
//
//            var f1 = JumpList.AppIdList.LoadAppListFromFile(f);

            foreach (var allPath in _allPaths)
            {
                foreach (
                    var fname in Directory.GetFiles(allPath, "*.automaticDestinations-ms", SearchOption.AllDirectories))
                {
                    Debug.WriteLine(fname);
                    var raw = File.ReadAllBytes(fname);

                    var a = new AutomaticDestination(raw, fname);

                    var foo = JumpList.AppIdList.GetDescriptionFromId(a.AppId.AppId);

                    if (foo.Contains("Unknown AppId") == false)
                    {
                        Debug.WriteLine(foo);
                    }

                    a.DestListCount.Should().Be(a.DestListEntries.Count);
                    a.DestListCount.Should().Be(a.Directory.Count - 2);

                    //  Debug.WriteLine(a);


                    Debug.WriteLine("-----------------------------------------------------------------");
                }
            }
        }


        [Test]
        public void CustomTests()
        {
            foreach (var allPath in _allPaths)
            {
                foreach (
                    var fname in Directory.GetFiles(allPath, "*.customDestinations-ms", SearchOption.AllDirectories))
                {
                    var raw = File.ReadAllBytes(fname);

                    try
                    {
                        var c = new CustomDestination(raw, fname);

                        Debug.WriteLine(c);
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("Empty custom") == false)
                        {
                            throw;
                        }
                    }
                }
            }
        }

        [Test]
        public void OneOff()
        {
            var f = @"D:\Temp\a52b0784bd667468.automaticDestinations-ms";


            var raw = File.ReadAllBytes(f);

            var aaaa = new AutomaticDestination(raw, f);

            Debug.WriteLine(aaaa);
        }
    }
}