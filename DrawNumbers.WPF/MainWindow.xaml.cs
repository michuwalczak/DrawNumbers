using DrawNumbers.BLL;
using System;
using System.Collections.Generic;
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
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Create path to database
            var dataBlockPath = System.IO.Path.Combine(@"..\..\..", "DrawNumbers.DAL");
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Path.GetFullPath(dataBlockPath));

            InitializeComponent();

            var draw = new Draw(5);
            draw.Run();
        }
    }
}
