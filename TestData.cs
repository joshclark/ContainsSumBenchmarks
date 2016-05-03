using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainsSumBenchmarks
{
    class TestData
    {
        public TestData(IEnumerable<int> numbers, int sum, bool expectedResult)
        {
            Numbers = new List<int>(numbers);
            Sum = sum;
            ExpectedResult = expectedResult;
        }

        public List<int> Numbers { get; set; }
        public int Sum { get; set; }
        public bool ExpectedResult { get; set; }

        public static TestData MakeLargeList(int numberOfNumbers)
        {
            var random = new Random();
            var maxValue = numberOfNumbers*2;
            var numbers = Enumerable.Range(0, numberOfNumbers).Select(x => random.Next(maxValue)).ToList();

            return new TestData(numbers, maxValue*2+1, false);
        }
    }
}
