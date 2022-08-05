using System;
using System.Collections.Generic;
using System.IO;
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
using EraCsvManager.VM;

namespace EraCsvManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
        }
        
        public MainWindowVM main = new MainWindowVM();

        private void SelectErbExeDir(object sender, RoutedEventArgs e)
        {
            txtRootDir.Text = main.SelectExeDir("root");
        }
        private void SelectOutDir(object sender, RoutedEventArgs e)
        {
            txtOutDir.Text = main.SelectExeDir("out");
        }
    }
}
