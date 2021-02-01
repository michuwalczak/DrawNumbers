using DrawNumbers.BLL;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DrawNumbers.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int progress;

        public int Progress
        {
            get { return progress; }
            set 
            { 
                progress = value;
                NotifyPropertyChange(nameof(Progress));
            }
        }

        private string numbersUsage;

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

            NumbersUsage = DrawInfo.UsageStage.ToString();
        }

        
        protected void NotifyPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnNewNumberDrawn(float progress)
        {
            Progress = (int)(progress * 100.0);  
        }

        private void OnDrawCompleted()
        {
            NumbersUsage = DrawInfo.UsageStage.ToString();
        }

        private async void RunDraw_Button_Click(object sender, RoutedEventArgs e)
        {
            var amountOfNumbers = int.Parse(AmountOfNumbers_TextBox.Text);
            var draw = new Draw(amountOfNumbers);
            draw.DrawnNewNumber += OnNewNumberDrawn;
            draw.DrawCompleted += OnDrawCompleted;
            await Task.Run(draw.Run);
        }
    }
}
