using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MultiAppWindowSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private AppWindow appWindow1;
        private AppWindow appWindow2;
        public MainPage() { InitializeComponent(); }
        private async void Page_Loaded(object sender, RoutedEventArgs e) => await OpenSecondaryWindows();

        private async Task OpenSecondaryWindows()
        {
            var open1 = ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView1"];
            if (open1 == null)
            {
                ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView1"] = true;
                open1 = true;
            }
            if ((bool)open1)
            {
                await View1();
            }

            var open2 = ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView2"];
            if (open2 == null)
            {
                ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView2"] = true;
                open2 = true;
            }
            if ((bool)open2)
            {
                await View2();
            }
        }
        private async void Button1Click(object sender, RoutedEventArgs e) => await View1();

        private async Task View1()
        {
            if (appWindow1 is null)
            {
                appWindow1 = await AppWindow.TryCreateAsync();
                var frame = new Frame();
                frame.Navigate(typeof(SecondaryPage), "1");
                ElementCompositionPreview.SetAppWindowContent(appWindow1, frame);

                appWindow1.CloseRequested += (s, e) =>
                {
                    ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView1"] = false;

                    var placement = appWindow1.GetPlacement();
                    //Size is full screen size and can be bigger bcz it also includes taskbar etc.
                    //Display region excludes taskbar etc
                    var displayRegion = placement.DisplayRegion;
                    var displayRegionWidth = displayRegion.WorkAreaSize.Width;
                    var displayRegionHeight = displayRegion.WorkAreaSize.Height;

                    var sizeWidth = placement.Size.Width;
                    var sizeHeight = placement.Size.Height;

                    ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView1_Width"] = sizeWidth > displayRegionWidth ? displayRegionWidth : sizeWidth;
                    ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView1_Height"] = sizeHeight > displayRegionHeight ? displayRegionHeight : sizeHeight;

                    ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView1_X"] = placement.Offset.X;
                    ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView1_Y"] = placement.Offset.Y;

                    Button1.IsEnabled = true;
                };

                appWindow1.Closed += delegate
                {
                    frame.Content = null;
                    appWindow1 = null;
                };
            }

            var shown = await appWindow1.TryShowAsync();
            Button1.IsEnabled = !shown;

            var windowWidth = ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView1_Width"];
            var windowHeight = ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView1_Height"];
            if (windowWidth is double wWidth && windowHeight is double wHeight)
            {
                appWindow1.RequestSize(new Size(wWidth, wHeight));
            }
            
            var xposition = ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView1_X"];
            var yposition = ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView1_Y"];
            if (xposition is double xpos && yposition is double ypos)
            {
                var placement = appWindow1.GetPlacement();
                appWindow1.RequestMoveRelativeToDisplayRegion(placement.DisplayRegion, new Point(xpos, ypos));
            }
        }

        private async void Button2Click(object sender, RoutedEventArgs e) => await View2();

        private async Task View2()
        {
            if (appWindow2 is null)
            {
                appWindow2 = await AppWindow.TryCreateAsync();
                var frame = new Frame();
                frame.Navigate(typeof(SecondaryPage), "2");
                ElementCompositionPreview.SetAppWindowContent(appWindow2, frame);

                appWindow2.CloseRequested += (s, e) =>
                {
                    ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView2"] = false;

                    var placement = appWindow2.GetPlacement();
                    //Size is full screen size and can be bigger bcz it also includes taskbar etc.
                    //Display region excludes taskbar etc
                    var displayRegion = placement.DisplayRegion;
                    var displayRegionWidth = displayRegion.WorkAreaSize.Width;
                    var displayRegionHeight = displayRegion.WorkAreaSize.Height;

                    var sizeWidth = placement.Size.Width;
                    var sizeHeight = placement.Size.Height;

                    ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView2_Width"] = sizeWidth > displayRegionWidth ? displayRegionWidth : sizeWidth;
                    ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView2_Height"] = sizeHeight > displayRegionHeight ? displayRegionHeight : sizeHeight;

                    ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView2_X"] = placement.Offset.X;
                    ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView2_Y"] = placement.Offset.Y;

                    Button2.IsEnabled = true;
                };

                appWindow2.Closed += delegate
                {
                    frame.Content = null;
                    appWindow2 = null;
                };
            }

            var shown = await appWindow2.TryShowAsync();
            Button2.IsEnabled = !shown;

            var windowWidth = ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView2_Width"];
            var windowHeight = ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView2_Height"];
            if (windowWidth is double wWidth && windowHeight is double wHeight)
            {
                appWindow2.RequestSize(new Size(wWidth, wHeight));
            }

            var xposition = ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView2_X"];
            var yposition = ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView2_Y"];
            if (xposition is double xpos && yposition is double ypos)
            {
                var placement = appWindow2.GetPlacement();
                appWindow2.RequestMoveRelativeToDisplayRegion(placement.DisplayRegion, new Point(xpos, ypos));
            }
        }
    }
}
