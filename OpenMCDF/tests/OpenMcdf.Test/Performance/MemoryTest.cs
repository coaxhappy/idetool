using System;
using System.Diagnostics;
using System.IO;
using NBench;

namespace OpenMcdf.Test.Performance
{
    public class MemoryTest : PerformanceTestStuite<MemoryTest>
    {
        private Counter _testCounter;

        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            _testCounter = context.GetCounter("TestCounter");
        }

        [PerfBenchmark(NumberOfIterations = 1, RunMode = RunMode.Iterations, TestMode = TestMode.Test,
            SkipWarmups = false)]
        [CounterMeasurement("TestCounter")]
        [CounterTotalAssertion("TestCounter", MustBe.LessThanOrEqualTo, 7000.0d)] // max 7 sec
        [MemoryAssertion(MemoryMetric.TotalBytesAllocated, MustBe.LessThanOrEqualTo,
            450 * 1024 * 1024)] // max 450 Mb in RAM
        public void PerfMem_MultipleCodeFeatures()
        {
            const int N_FACTOR = 1000;

            byte[] bA = Helpers.GetBuffer(20 * 1024 * N_FACTOR, 0x0A);
            byte[] bB = Helpers.GetBuffer(5 * 1024, 0x0B);
            byte[] bC = Helpers.GetBuffer(5 * 1024, 0x0C);
            byte[] bD = Helpers.GetBuffer(5 * 1024, 0x0D);
            byte[] bE = Helpers.GetBuffer(8 * 1024 * N_FACTOR + 1, 0x1A);
            byte[] bF = Helpers.GetBuffer(16 * 1024 * N_FACTOR, 0x1B);
            byte[] bG = Helpers.GetBuffer(14 * 1024 * N_FACTOR, 0x1C);
            byte[] bH = Helpers.GetBuffer(12 * 1024 * N_FACTOR, 0x1D);
            byte[] bE2 = Helpers.GetBuffer(8 * 1024 * N_FACTOR, 0x2A);
            byte[] bMini = Helpers.GetBuffer(1027, 0xEE);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            var cf = new CompoundFile(CFSVersion.Ver_3, CFSConfiguration.SectorRecycle);
            cf.RootStorage.AddStream("A").SetData(bA);
            cf.Save("OneStream.cfs");

            cf.Close();

            cf = new CompoundFile("OneStream.cfs", CFSUpdateMode.ReadOnly, CFSConfiguration.SectorRecycle);

            cf.RootStorage.AddStream("B").SetData(bB);
            cf.RootStorage.AddStream("C").SetData(bC);
            cf.RootStorage.AddStream("D").SetData(bD);
            cf.RootStorage.AddStream("E").SetData(bE);
            cf.RootStorage.AddStream("F").SetData(bF);
            cf.RootStorage.AddStream("G").SetData(bG);
            cf.RootStorage.AddStream("H").SetData(bH);

            cf.Save("8_Streams.cfs");

            cf.Close();

            File.Copy("8_Streams.cfs", "6_Streams.cfs", true);

            cf = new CompoundFile("6_Streams.cfs", CFSUpdateMode.Update,
                CFSConfiguration.SectorRecycle | CFSConfiguration.EraseFreeSectors);
            cf.RootStorage.Delete("D");
            cf.RootStorage.Delete("G");
            cf.Commit();

            cf.Close();

            File.Copy("6_Streams.cfs", "6_Streams_Shrinked.cfs", true);

            cf = new CompoundFile("6_Streams_Shrinked.cfs", CFSUpdateMode.Update, CFSConfiguration.SectorRecycle);
            cf.RootStorage.AddStream("ZZZ").SetData(bF);
            cf.RootStorage.GetStream("E").Append(bE2);
            cf.Commit();
            cf.Close();

            cf = new CompoundFile("6_Streams_Shrinked.cfs", CFSUpdateMode.Update, CFSConfiguration.SectorRecycle);
            cf.RootStorage.CLSID = new Guid("EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
            cf.Commit();
            cf.Close();

            cf = new CompoundFile("6_Streams_Shrinked.cfs", CFSUpdateMode.Update, CFSConfiguration.SectorRecycle);
            cf.RootStorage.AddStorage("MyStorage").AddStream("ANS").Append(bE);
            cf.Commit();
            cf.Close();

            cf = new CompoundFile("6_Streams_Shrinked.cfs", CFSUpdateMode.Update, CFSConfiguration.SectorRecycle);
            cf.RootStorage.AddStorage("AnotherStorage").AddStream("ANS").Append(bE);
            cf.RootStorage.Delete("MyStorage");
            cf.Commit();
            cf.Close();

            CompoundFile.ShrinkCompoundFile("6_Streams_Shrinked.cfs");

            cf = new CompoundFile("6_Streams_Shrinked.cfs", CFSUpdateMode.Update, CFSConfiguration.SectorRecycle);
            cf.RootStorage.AddStorage("MiniStorage").AddStream("miniSt").Append(bMini);
            cf.RootStorage.GetStorage("MiniStorage").AddStream("miniSt2").Append(bMini);
            cf.Commit();
            cf.Close();

            cf = new CompoundFile("6_Streams_Shrinked.cfs", CFSUpdateMode.Update, CFSConfiguration.SectorRecycle);
            cf.RootStorage.GetStorage("MiniStorage").Delete("miniSt");


            cf.RootStorage.GetStorage("MiniStorage").GetStream("miniSt2").Append(bE);
            cf.Commit();
            cf.Close();

            cf = new CompoundFile("6_Streams_Shrinked.cfs", CFSUpdateMode.ReadOnly, CFSConfiguration.SectorRecycle);

            var myStream = cf.RootStorage.GetStream("C");
            var data = myStream.GetData();
            Console.WriteLine(data[0] + " : " + data[data.Length - 1]);

            myStream = cf.RootStorage.GetStream("B");
            data = myStream.GetData();
            Console.WriteLine(data[0] + " : " + data[data.Length - 1]);

            cf.Close();

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);

            for (int i = 0; i < sw.ElapsedMilliseconds; i++)
            {
                _testCounter.Increment();
            }
        }

        [PerfBenchmark(NumberOfIterations = 1, RunMode = RunMode.Iterations, TestMode = TestMode.Test,
            SkipWarmups = false)]
        [CounterMeasurement("TestCounter")]
        [CounterTotalAssertion("TestCounter", MustBe.LessThanOrEqualTo, 5500.0d)] // max 5.5 sec
        [MemoryAssertion(MemoryMetric.TotalBytesAllocated, MustBe.LessThanOrEqualTo,
            8 * 1024 * 1024)] // max 8 Mb in RAM
        public void PerfMem_MultipleStreamCommit()
        {
            File.Copy("report.xls", "reportOverwriteMultiple.xls", true);

            Stopwatch sw = new Stopwatch();

            using (var cf = new CompoundFile("reportOverwriteMultiple.xls", CFSUpdateMode.Update, CFSConfiguration.SectorRecycle))
            {
                sw.Start();

                Random r = new Random();

                for (int i = 0; i < 1000; i++)
                {
                    byte[] buffer = Helpers.GetBuffer(r.Next(100, 3500), 0x0A);

                    if (i > 0)
                    {
                        if (r.Next(0, 100) > 50)
                        {
                            cf.RootStorage.Delete("MyNewStream" + (i - 1).ToString());
                        }
                    }

                    CFStream addedStream = cf.RootStorage.AddStream("MyNewStream" + i.ToString());

                    addedStream.SetData(buffer);

                    // Random commit, not on single addition
                    if (r.Next(0, 100) > 50)
                        cf.Commit();
                }

                cf.Close();
                sw.Stop();
            }

            Console.WriteLine(sw.ElapsedMilliseconds);

            for (int i = 0; i < sw.ElapsedMilliseconds; i++)
            {
                _testCounter.Increment();
            }
        }
    }
}