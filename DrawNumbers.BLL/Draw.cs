using DrawNumbers.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DrawNumbers.BLL
{
    public class Draw
    {
        private const int rangeLowerBound = 1;
        private const int rangeUpperBound = 22;
        private static readonly List<int> poolOfNumbers = Enumerable.Range(rangeLowerBound, rangeUpperBound - rangeLowerBound + 1).ToList();

        public delegate void DrawCompleted_Handler();
        public delegate void DrawnNewNumber_Handler(float progress);
        public delegate void NotEnoughNumbers_Handler(int maxAvailableCount);
        public event DrawCompleted_Handler DrawCompleted;
        public event DrawnNewNumber_Handler DrawnNewNumber;
        public event NotEnoughNumbers_Handler NotEnoughNumbers;

        private readonly DataBaseHelper dataBase;
        private int amountOfNumbers;
        private readonly object drawnNumberLock = new object();
        private List<int> drawnNumbers;

        public int AmountOfNumbers
        {
            get { return amountOfNumbers; }
            set { amountOfNumbers = value; }
        }

        /// <summary>
        /// Drawn numbers
        /// </summary>
        public List<int> DrawnNumbers
        {
            get
            {
                lock (drawnNumberLock)
                {
                    return drawnNumbers;
                };
            }

            private set
            {
                lock (drawnNumberLock)
                {
                    drawnNumbers.AddRange(value);
                    DrawnNewNumber?.Invoke((float)drawnNumbers.Count / amountOfNumbers);

                    if (drawnNumbers.Count == amountOfNumbers)
                        DrawCompleted?.Invoke();
                }
            }
        }

        /// <summary>
        /// Not drawn numbers
        /// </summary>
        private List<int> AvailableNumbers
        {
            get
            {
                var drawnNumbers = dataBase.DrawnNumbers.Select(num => num.Value);
                return poolOfNumbers.Except(drawnNumbers).ToList();
            }
        }

        public Draw(int amountOfNumbers)
        {
            dataBase = new DataBaseHelper();
            drawnNumbers = new List<int>();
            this.amountOfNumbers = amountOfNumbers;

            this.DrawCompleted += OnCompletedTask;
        }

        /// <summary>
        /// Run draw operation
        /// </summary>
        public void Run()
        {
            var availableNumbers = RandomizeEnumerable(AvailableNumbers);

            if (availableNumbers.Count() >= amountOfNumbers)
            {
                // Divide available numbers on parts and run tasks
                var parts = SplitOnParts(availableNumbers);
                foreach (var part in parts)
                    RunDrawnTask(part);
            }
            else
            {
                // Inform observers about not enough available numbers
                NotEnoughNumbers?.Invoke(availableNumbers.Count());
            }
        }

        /// <summary>
        /// Randomize enumerable collection
        /// </summary>
        /// <param name="source">Enumerable to randomize</param>
        /// <returns></returns>
        private IEnumerable<int> RandomizeEnumerable(IEnumerable<int> source)
        {
            var rnd = new Random();
            return source.OrderBy(x => rnd.Next());
        }

        /// <summary>
        /// Run new drawn thread
        /// </summary>
        /// <param name="source">Enumerable collection of numbers</param>
        private void RunDrawnTask(IEnumerable<int> source)
        {
            var thread = new Thread(new ParameterizedThreadStart(DrawNumber));
            thread.Start(source);
        }

        /// <summary>
        /// Draw number based on the selected part of numbers
        /// </summary>
        /// <param name="part">Part of selected numbers</param>
        private void DrawNumber(object part)
        {
            var numbers = (IEnumerable<int>)part;
            var rnd = new Random();
            var number = numbers.ToArray()[rnd.Next(numbers.Count())];
            DrawnNumbers = new List<int>() { number };
        }

        /// <summary>
        /// Split enumerable collection on parts
        /// </summary>
        /// <param name="source">Enumerable to split</param>
        /// <returns></returns>
        private IEnumerable<IEnumerable<int>> SplitOnParts(IEnumerable<int> source)
        {
            int i = 0;
            var splits = from item in source
                         group item by i++ % amountOfNumbers into part
                         select part.AsEnumerable();

            return splits;
        }

        /// <summary>
        /// Insert drawn numbers to data base on operation end
        /// </summary>
        private void OnCompletedTask()
        {
            DrawnNumbers.ForEach(num => dataBase.Add(new DrawnNumber() { Value = num }));
            dataBase.Save();
        }
    }
}
