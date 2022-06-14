using System;
using System.Windows;
using FundamentalStructures;
using MDCourseProject.AppWindows;

namespace MDCourseProject
{
    public partial class MainWindow
    {
        private void DebugButtonClick(object sender, RoutedEventArgs e)
        {
            var window = new DebugWindow();
            window.Owner = this;
            window.ShowDialog();
        }
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}