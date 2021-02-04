using DrawNumbers.BLL;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace DrawNumbers.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int progress;
        private string progressLabel;
        private string numbersUsage;
        private long drawTime, saveTime, totalTime;
        private ProgressDialogController dialogController;
        private Draw draw;
        private int lowerBound = 1000000;
        private int upperBound = 9999999;

        private object dialogLock = new object();
        private ProgressDialogController DialogController
        {
            get
            {
                lock (dialogLock)
                {
                    return dialogController;
                }
            }

            set
            {
                lock (dialogLock)
                {
                    dialogController = value;
                }
            }
        }

        /// <summary>
        /// String representation of draw progress
        /// </summary>
        public string ProgresLabel
        {
            get { return progressLabel; }
            set
            {
                progressLabel = $"Draw progress: {value}%";
                NotifyPropertyChange(nameof(ProgresLabel));
            }
        }

        /// <summary>
        /// Integer representation of draw progress
        /// </summary>
        public int Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                NotifyPropertyChange(nameof(Progress));
            }
        }

        /// <summary>
        /// Percentage value of used numbers
        /// </summary>
        public string NumbersUsage
        {
            get { return numbersUsage; }
            set
            {
                numbersUsage = $"{value}%";
                NotifyPropertyChange(nameof(NumbersUsage));
            }
        }

        /// <summary>
        /// Time ow draw operation
        /// </summary>
        public long DrawTime
        {
            get { return drawTime; }
            set
            {
                drawTime = value;
                NotifyPropertyChange(nameof(DrawTime));
            }
        }

        /// <summary>
        /// Time of saving data to data base
        /// </summary>
        public long SaveTime
        {
            get { return saveTime; }
            set
            {
                saveTime = value;
                NotifyPropertyChange(nameof(SaveTime));
            }
        }

        /// <summary>
        /// Total time of operation
        /// </summary>
        public long TotalTime
        {
            get { return totalTime; }
            set
            {
                totalTime = value;
                NotifyPropertyChange(nameof(TotalTime));
            }
        }

        /// <summary>
        /// Lower bound of numbers range
        /// </summary>
        public int LowerBound
        {
            get { return lowerBound; }
            set 
            { 
                lowerBound = value;
                Task.Run(Update);
                NotifyPropertyChange(nameof(LowerBound));
            }
        }

        /// <summary>
        /// Upper bound of numbers range
        /// </summary>
        public int UpperBound
        {
            get { return upperBound; }
            set
            {
                upperBound = value;
                Task.Run(Update);
                NotifyPropertyChange(nameof(UpperBound));
            }
        }

        public MainWindow()
        {
            // Create path to database
            var dataBlockPath = System.IO.Path.Combine(@"..\..\..", "DrawNumbers.DAL");
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Path.GetFullPath(dataBlockPath));

            InitializeComponent();
            DataContext = this;
            ProgresLabel = "0";

        }

        /// <summary>
        /// Connect with database and initialize data
        /// </summary>
        private void InitializeAsync()
        {
            while (DialogController == null) { }

            this.draw = new Draw(lowerBound, upperBound);
            this.draw.ProgressUpdate += OnNewNumberDrawn;
            this.draw.DrawCompleted += OnDrawCompleted;
            this.draw.NotEnoughNumbers += OnNotEnoughNumbers;
            this.draw.DrawResultsSaved += OnDrawResultsSaved;
            NumbersUsage = this.draw.UsageStage.ToString();

            CloseProgressDialog();
        }

        /// <summary>
        /// Update visible data
        /// </summary>
        private void Update()
        {
            this.draw.UpdateRange(lowerBound, upperBound);
            NumbersUsage = this.draw.UsageStage.ToString();
        }

        protected void NotifyPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region controls events
        private async void RunDraw_Button_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(AmountOfNumbers_TextBox.Text, out int amountOfNumbers);
            if ((amountOfNumbers > 0 && !Draw.IsRunning) &&
                    (lowerBound < upperBound) && (lowerBound > 0))
                await Task.Run(() => draw.Run(amountOfNumbers));
        }

        private void MetroWindow_ContentRendered(object sender, EventArgs e)
        {
            ShowProgressDialog("", "Initializing data. Please wait...");
            new Thread(InitializeAsync).Start();
        }
        #endregion


        #region dialog methods
        private async void ShowDialog(string title, string message)
        {
            await this.ShowMessageAsync(title, message);
        }

        private async void ShowProgressDialog(string title, string message)
        {
            DialogController = await this.ShowProgressAsync(title, message);
        }

        private async void CloseProgressDialog()
        {
            if (DialogController != null)
            {
                while (!DialogController.IsOpen) { }
                await DialogController.CloseAsync();
            }
        }
        #endregion


        #region draw events
        private void OnNewNumberDrawn(float progress)
        {
            Progress = (int)(progress * 100.0);
            ProgresLabel = string.Format("{0:0.00}", (progress * 100.0));
        }

        private void OnDrawCompleted()
        {
            Dispatcher.Invoke(delegate () {
                ShowProgressDialog("Task completed successfully", "Saving results in database. Please wait...");
            });
        }

        private void OnDrawResultsSaved(long drawTime, long saveTime, long totalTime)
        {
            DrawTime = drawTime;
            SaveTime = saveTime;
            TotalTime = totalTime;

            NumbersUsage = draw.UsageStage.ToString();
            CloseProgressDialog();
        }

        private void OnNotEnoughNumbers(int maxAvailableCount)
        {
            Dispatcher.Invoke(delegate () {
                ShowDialog("Information", $"Not enough available numbers in database. Maximum number is {maxAvailableCount}");
            });
        }
        #endregion
    }
}
