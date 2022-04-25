using MultiAppWindowSample2.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MultiAppWindowSample2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage() => InitializeComponent();
        private async void Button1Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(TotalWindowsBox.Text))
            {
                await AppViewModel.OpenSecondaryWindows(Convert.ToInt32(TotalWindowsBox.Text));
            }
        }
    }
}
