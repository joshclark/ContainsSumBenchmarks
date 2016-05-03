using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnostics;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Newtonsoft.Json;

namespace ContainsSumBenchmarks
{
    [Config(typeof(Config))]
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Contains("test"))
            {
                TestImplementation(nameof(ContainsSumDoubleLoop), ContainsSumDoubleLoop);
                TestImplementation(nameof(ContainsSumDoubleLoopOptimized), ContainsSumDoubleLoopOptimized);
                TestImplementation(nameof(ContainsSumBinarySearch), ContainsSumBinarySearch);
                TestImplementation(nameof(ContainsSumLinearHashSet), ContainsSumLinearHashSet);
                TestImplementation(nameof(ContainsSumLinearDictionary), ContainsSumLinearDictionary);
            }
            else
            {
                var summary = BenchmarkRunner.Run<Program>();
            }


            if (Debugger.IsAttached)
            {
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
        }

        private class Config : ManualConfig
        {
            public Config()
            {
                Add(Job.Clr.WithLaunchCount(1));
                Add(new GCDiagnoser());
            }
        }

        private static readonly TestData[] _testdata =
        {
            new TestData(new[] {1, 2, 3, 4, 5}, 7, true),
            new TestData(new[] {1, 2, 3, 4, 5}, 2, false),
            new TestData(new[] {8, 2, 6, 4, 1}, 2, false),
            new TestData(new[] {8, 2, 6, 4, 1}, 5, true),
            new TestData(new[] {9, -2, 6, 4, 1}, 6, false),
            TestData.MakeLargeList(5000)
        };

        [Params(0,1,2,3,4,5)]
        public int TestDataIndex { get; set; }



        [Benchmark(Baseline = true)]
        public bool ContainsSumDoubleLoop()
        {
            var test = _testdata[TestDataIndex];
            return ContainsSumDoubleLoop(test.Numbers, test.Sum);
        }

        [Benchmark]
        public bool ContainsSumDoubleLoopOptimized()
        {
            var test = _testdata[TestDataIndex];
            return ContainsSumDoubleLoopOptimized(test.Numbers, test.Sum);
        }

        [Benchmark]
        public bool ContainsSumBinarySearch()
        {
            var test = _testdata[TestDataIndex];
            return ContainsSumBinarySearch(test.Numbers, test.Sum);
        }

        [Benchmark]
        public bool ContainsSumLinearHashSet()
        {
            var test = _testdata[TestDataIndex];
            return ContainsSumLinearHashSet(test.Numbers, test.Sum);
        }

        [Benchmark]
        public bool ContainsSumLinearDictionary()
        {
            var test = _testdata[TestDataIndex];
            return ContainsSumLinearDictionary(test.Numbers, test.Sum);
        }



        public static bool ContainsSumDoubleLoop(List<int> numbers, int sum)
        {
            for (int i = 0; i < numbers.Count; ++i)
            {
                for (int j = 0; j < numbers.Count; ++j)
                {
                    if (i == j)
                        continue;
                    
                    if (numbers[i] + numbers[j] == sum)
                        return true;
                }
            }

            return false;
        }

        public static bool ContainsSumDoubleLoopOptimized(List<int> numbers, int sum)
        {
            for (int i = 0; i < numbers.Count - 1; ++i)
            {
                for (int j = i + 1; j < numbers.Count; ++j)
                {
                    if (numbers[i] + numbers[j] == sum)
                        return true;
                }
            }

            return false;
        }

        public static bool ContainsSumBinarySearch(List<int> numbers, int sum)
        {
            var sorted = new List<int>(numbers);
            sorted.Sort();

            for (int i = 0; i < sorted.Count - 1; ++i)
            {
                int index = sorted.BinarySearch(i + 1, sorted.Count - i - 1, sum - sorted[i], null);
                if (index >= 1)
                    return true;
            }

            return false;
        }


        public static bool ContainsSumLinearHashSet(List<int> numbers, int sum)
        {
            var seen = new HashSet<int>();

            foreach (int number in numbers)
            {
                if (seen.Contains(sum - number))
                    return true;

                seen.Add(number);
            }

            return false;
        }
        public static bool ContainsSumLinearDictionary(List<int> numbers, int sum)
        {
            var seen = new Dictionary<int, bool>();

            foreach (int number in numbers)
            {
                if (seen.ContainsKey(sum - number))
                    return true;

                seen.Add(number, false);
            }

            return false;
        }



        public static void TestImplementation(string name, Func<List<int>, int, bool> impl)
        {

            foreach (var test in _testdata)
            {
                bool actual = impl(test.Numbers, test.Sum);
                if (actual != test.ExpectedResult)
                {
                    var json = JsonConvert.SerializeObject(test, Formatting.Indented);
                    Console.WriteLine($"Implementation '{name}' failed for test data: {json}");
                }
                else
                {
                    Console.WriteLine($"Implementation '{name}' passed.");
                }
            }
        }

    }
}
