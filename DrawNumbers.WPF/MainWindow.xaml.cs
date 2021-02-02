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
        private ProgressDialogController dialogController;

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
        private void Initialize()
        {           
            DrawInfo.Initialize();
            NumbersUsage = DrawInfo.UsageStage.ToString();
            CloseProgressDialog();
        }

        protected void NotifyPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region controls events
        private async void RunDraw_Button_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(AmountOfNumbers_TextBox.Text, out int amountOfNumbers);
            if (amountOfNumbers > 0 && !Draw.IsRunning)
            {
                var draw = new Draw(amountOfNumbers);
                draw.DrawnNewNumber += OnNewNumberDrawn;
                draw.DrawCompleted += OnDrawCompleted;
                draw.NotEnoughNumbers += OnNotEnoughNumbers;
                draw.DrawResultsSaved += OnDrawResultsSaved;

                await Task.Run(draw.Run);
            }
        }

        private void MetroWindow_ContentRendered(object sender, EventArgs e)
        {
            ShowProgressDialog("", "Initializing data. Please wait...");
            new Thread(Initialize).Start();

        }
        #endregion


        #region dialog methods
        private async void ShowDialog(string title, string message)
        {
            await this.ShowMessageAsync(title, message);
        }

        private async void ShowProgressDialog(string title, string message)
        {
            dialogController = await this.ShowProgressAsync(title, message);
        }

        private async void CloseProgressDialog()
        {
            if (dialogController != null && dialogController.IsOpen)
                await dialogController.CloseAsync();
        }
        #endregion


        #region draw events
        private void OnNewNumberDrawn(float progress)
        {
            Progress = (int)(progress * 100.0);
            ProgresLabel = (progress * 100.0).ToString();
        }

        private void OnDrawCompleted()
        {
            Dispatcher.Invoke(delegate () {
                ShowProgressDialog("Task completed successfully", "Saving results in database. Please wait...");
            });
        }

        private void OnDrawResultsSaved()
        {
            NumbersUsage = DrawInfo.UsageStage.ToString();
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
