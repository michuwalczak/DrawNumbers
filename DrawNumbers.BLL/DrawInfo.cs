using DrawNumbers.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawNumbers.BLL
{
    public static class DrawInfo
    {
        private const int rangeLowerBound = 1000000;
        private const int rangeUpperBound = 9999999;
        public static readonly List<int> poolOfNumbers = Enumerable.Range(rangeLowerBound, rangeUpperBound - rangeLowerBound + 1).ToList();

        public static float UsageStage { get => ((float) new DataBaseHelper().DrawnNumbers.Count / poolOfNumbers.Count) * 100f; }
    }
}
