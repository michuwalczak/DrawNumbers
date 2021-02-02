using DrawNumbers.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DrawNumbers.BLL
{
    /// <summary>
    /// Draw defined amount of numbers
    /// </summary>
    public class Draw
    {
        public delegate void DrawCompleted_Handler();
        public delegate void DrawnNewNumber_Handler(float progress);
        public delegate void NotEnoughNumbers_Handler(int maxAvailableCount);
        public delegate void DrawResultsSaved_Handler();
        public event DrawCompleted_Handler DrawCompleted;
        public event DrawnNewNumber_Handler DrawnNewNumber;
        public event NotEnoughNumbers_Handler NotEnoughNumbers;
        public event DrawResultsSaved_Handler DrawResultsSaved;

        private readonly DataBaseHelper dataBase;
        private readonly int amountOfNumbers;
        private readonly object drawnNumberLock = new object();
        private readonly List<int> drawnNumbers;

        /// <summary>
        /// Chacks if any draw started
        /// </summary>
        public static bool IsRunning { get; private set; }

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


        public Draw(int amountOfNumbers)
        {
            this.dataBase = new DataBaseHelper();
            this.drawnNumbers = new List<int>();
            this.amountOfNumbers = amountOfNumbers;

            this.DrawCompleted += OnCompletedTask;
        }

        /// <summary>
        /// Run draw operation
        /// </summary>
        public void Run()
        {
            var availableNumbers = DrawInfo.AvailableNumbers;

            if (availableNumbers.Count() >= amountOfNumbers)
            {
                IsRunning = true;

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
            var number = RandomizeEnumerable((IEnumerable<int>)part).First();
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
        /// Save results in database and update information about draw
        /// </summary>
        private void SaveResults()
        {
            dataBase.Add(DrawnNumbers.Select(num => new DrawnNumber() { Value = num }).ToList());
            dataBase.Save();
            DrawInfo.Update();
            IsRunning = false;
            DrawResultsSaved?.Invoke();
        }

        /// <summary>
        /// Insert drawn numbers to data base on operation end
        /// </summary>
        private void OnCompletedTask()
        {
            new Thread(SaveResults).Start();
        }
    }
}
