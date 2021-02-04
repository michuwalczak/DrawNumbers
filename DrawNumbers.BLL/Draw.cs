using DrawNumbers.DAL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public delegate void ProgressUpdate_Handler(float progress);
        public delegate void NotEnoughNumbers_Handler(int maxAvailableCount);
        public delegate void DrawResultsSaved_Handler(long drawTime, long saveTime, long totalTime);
        public event DrawCompleted_Handler DrawCompleted;
        public event ProgressUpdate_Handler ProgressUpdate;
        public event NotEnoughNumbers_Handler NotEnoughNumbers;
        public event DrawResultsSaved_Handler DrawResultsSaved;

        private readonly DataBaseHelper dataBase;
        private int amountOfNumbers;
        private List<int> dataBasNumbers;
        private List<int> rangeOfNumbers;
        private List<int> availableNumbers;
        private List<DrawnNumber> drawnNumbers;
        private readonly Stopwatch drawTimer, saveTimer, totalTimer;
        private float usageStage;
        private long drawTime = 0, saveTime = 0, totalTime = 0;

        public int lowerBound, upperBound;


        /// <summary>
        /// Chacks if any draw started
        /// </summary>
        public static bool IsRunning { get; private set; }

        /// <summary>
        /// Drawn numbers
        /// </summary>
        public List<DrawnNumber> DrawnNumbers { get => drawnNumbers; set => drawnNumbers = value; }

        /// <summary>
        /// Number of used numbers from range
        /// </summary>
        public float UsageStage { get => this.usageStage; }


        public Draw(int lowerBound, int upperBound)
        {
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
            this.dataBase = new DataBaseHelper();
            this.dataBasNumbers = dataBase.DrawnNumbers.Select(num => num.Value).ToList();
            this.drawnNumbers = new List<DrawnNumber>();
            this.drawTimer = new Stopwatch();
            this.saveTimer = new Stopwatch();
            this.totalTimer = new Stopwatch();
            this.DrawCompleted += OnCompletedTask;

            UpdateRange(this.lowerBound, this.upperBound);
        }

        /// <summary>
        /// Run draw operation
        /// </summary>
        public void Run(int amountOfNumbers)
        {
            this.amountOfNumbers = amountOfNumbers;

            if (availableNumbers.Count() >= amountOfNumbers)
                RunDrawnTask(availableNumbers);
            else
                NotEnoughNumbers?.Invoke(availableNumbers.Count());
        }

        /// <summary>
        /// Update range of numbers
        /// </summary>
        /// <param name="lowerBound">Lower bound of range</param>
        /// <param name="upperBound">Upper bound of range</param>
        public void UpdateRange(int lowerBound, int upperBound)
        {
            if((lowerBound < upperBound) && lowerBound > 0)
            {
                this.lowerBound = lowerBound;
                this.upperBound = upperBound;

                this.usageStage = (float)dataBasNumbers.Where(x => x>=lowerBound && x<=upperBound).Count() / (this.upperBound - this.lowerBound + 1) * 100f;

                rangeOfNumbers = Enumerable.Range(lowerBound, upperBound - lowerBound + 1).ToList();
                availableNumbers = rangeOfNumbers.Except(this.dataBasNumbers).ToList();
            }
        }

        /// <summary>
        /// Run new drawn thread
        /// </summary>
        /// <param name="source">Enumerable collection of numbers</param>
        private void RunDrawnTask(List<int> source)
        {
            IsRunning = true;
            totalTimer.Start();
            drawTimer.Start();

            var thread = new Thread(new ParameterizedThreadStart(DrawNumber));
            thread.Start(source);
        }

        /// <summary>
        /// Draw number based on the selected part of numbers
        /// </summary>
        /// <param name="availableNumbers">All available numbers</param>
        private void DrawNumber(object availableNumbers)
        {
            var source = (List<int>)availableNumbers;
            ShuffleNumbers(source);
            ProgressUpdate?.Invoke(1);

            DrawnNumbers = source.Take(amountOfNumbers)
                .Select(num => new DrawnNumber() { Value = num })
                .ToList();

            drawTimer.Stop();
            drawTime = drawTimer.ElapsedMilliseconds;
            drawTimer.Reset();

            DrawCompleted?.Invoke();
        }

        /// <summary>
        /// Shuffle numbers in list
        /// </summary>
        /// <param name="source">Source list</param>
        private void ShuffleNumbers(List<int> source)
        {
            var random = new Random();
            var numbersCount = source.Count();
            var max = source.Count;
            var index = max - 1;
            int updateProgressTrigger = (int)(numbersCount * 0.01) + 1;

            for (int i = 0; i < max; i++)
            {
                var rnd = random.Next(0, index);
                var temp = source[index];
                source[index--] = source[rnd];
                source[rnd] = temp;

                if (i % updateProgressTrigger == 0)
                    ProgressUpdate?.Invoke((float)(i + 1) / numbersCount);
            }
        }

        /// <summary>
        /// Save results in database and update information about draw
        /// </summary>
        private void SaveResults()
        {
            saveTimer.Start();
            dataBase.DrawnNumbers = DrawnNumbers;
            this.dataBasNumbers.AddRange(DrawnNumbers.Select(num => num.Value));

            saveTimer.Stop();
            saveTime = saveTimer.ElapsedMilliseconds;
            saveTimer.Reset();

            UpdateRange(this.lowerBound, this.upperBound);

            totalTimer.Stop();
            totalTime = totalTimer.ElapsedMilliseconds;
            totalTimer.Reset();

            IsRunning = false;
            DrawResultsSaved?.Invoke(drawTime, saveTime, totalTime);
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