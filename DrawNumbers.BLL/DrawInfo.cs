using DrawNumbers.DAL;
using System.Collections.Generic;
using System.Linq;

namespace DrawNumbers.BLL
{
    /// <summary>
    /// Information about draw operation
    /// </summary>
    public static class DrawInfo
    {
        private const int rangeLowerBound = 1000000;
        private const int rangeUpperBound = 9999999;
        public static List<int> poolOfNumbers;
        private static List<int> availableNumbers;
        private static float usageStage;

        /// <summary>
        /// Percentage value of used numbers
        /// </summary>
        public static float UsageStage { get => usageStage; }

        /// <summary>
        /// Not drawn numbers
        /// </summary>
        public static List<int> AvailableNumbers { get => availableNumbers; }

        /// <summary>
        /// Connect with database and initialize information about numbers
        /// </summary>
        public static void Initialize()
        {
            poolOfNumbers = Enumerable.Range(rangeLowerBound, rangeUpperBound - rangeLowerBound + 1).ToList();
            Update();
        }

        /// <summary>
        /// Update information
        /// </summary>
        public static void Update()
        {
            var drawnNumbers = new DataBaseHelper().DrawnNumbers.Select(num => num.Value);
            availableNumbers = poolOfNumbers.Except(drawnNumbers).ToList();

            usageStage = ((float)drawnNumbers.Count() / poolOfNumbers.Count) * 100f;
        }
    }
}
