using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MultiWindowSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly CoreDispatcher coreDispatcher;
        public MainPage() { InitializeComponent(); coreDispatcher = Dispatcher; }
        protected async override void OnNavigatedTo(NavigationEventArgs e) => await OpenSecondaryWindows();

        //private async Task OpenSecondaryWindows()
        //{
        //    var open1 = ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView1"];
        //    if (open1 == null)
        //    {
        //        ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView1"] = true;
        //        open1 = true;
        //    }
        //    if ((bool)open1)
        //    {
        //        await View1();
        //    }

        //    var open2 = ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView2"];
        //    if (open2 == null)
        //    {
        //        ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView2"] = true;
        //        open2 = true;
        //    }
        //    if ((bool)open2)
        //    {
        //        await View2();
        //    }
        //}
        private async Task OpenSecondaryWindows()
        {
            await View1();
        }
        //private async void Button1Click(object sender, RoutedEventArgs e) => await View1();
        private async void Button1Click(object sender, RoutedEventArgs e) { }

        private async Task View1()
        {
            var oldCurrentView = ApplicationView.GetForCurrentView();
            var newView = CoreApplication.CreateNewView();
            var newViewId = 0;
            ApplicationView newCurrentView = null;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var frame = new Frame();
                frame.Navigate(typeof(SecondaryPage), "1");
                Window.Current.Content = frame;
                // You have to activate the window in order to show it later.
                Window.Current.Activate();
                newCurrentView = ApplicationView.GetForCurrentView();
                newViewId = ApplicationView.GetForCurrentView().Id;

                ApplicationView.GetForCurrentView().Consolidated += async (s, e) =>
                {
                    ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView1"] = false;
                    ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView1_Width"] = frame.ActualWidth;
                    ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView1_Height"] = frame.ActualHeight;
                    await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Button1.IsEnabled = true);
                };
            });
            var shown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newCurrentView.Id, ViewSizePreference.Default, oldCurrentView.Id, ViewSizePreference.Default);
            var windowWidth = ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView1_Width"];
            var windowHeight = ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView1_Height"];
            if (windowWidth is double wWidth && windowHeight is double wHeight)
            {
                newCurrentView.TryResizeView(new Windows.Foundation.Size(wWidth, wHeight));
                //newCurrentView.TryResizeView(new Windows.Foundation.Size(3055,1250));
                //newCurrentView.TryResizeView(new Windows.Foundation.Size(3072,1656));
            }
            ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView1"] = shown;
            Button1.IsEnabled = !shown;
        }

        //private async void Button2Click(object sender, RoutedEventArgs e) => await View2();
        private async void Button2Click(object sender, RoutedEventArgs e) { }

        //private async Task View2()
        //{
        //    var newView = CoreApplication.CreateNewView();
        //    var newViewId = 1;
        //    await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        //    {
        //        var frame = new Frame();
        //        frame.Navigate(typeof(SecondaryPage), "2");
        //        Window.Current.Content = frame;

        //        var windowWidth = ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView2_Width"];
        //        var windowHeight = ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView2_Height"];
        //        if (windowWidth is double wWidth && windowHeight is double wHeight)
        //        {
        //            ApplicationView.PreferredLaunchViewSize = new Windows.Foundation.Size(wWidth, wHeight);
        //            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        //        }
        //        // You have to activate the window in order to show it later.
        //        Window.Current.Activate();

        //        newViewId = ApplicationView.GetForCurrentView().Id;

        //        ApplicationView.GetForCurrentView().Consolidated += async (s, e) =>
        //        {
        //            ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView2"] = false;
        //            ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView2_Width"] = frame.ActualWidth;
        //            ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView2_Height"] = frame.ActualHeight;
        //            await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Button2.IsEnabled = true);
        //        };
        //    });
        //    var shown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId, ViewSizePreference.Custom);
        //    ApplicationData.Current.LocalSettings.Values["AppWindow_SecondaryView2"] = shown;
        //    Button2.IsEnabled = !shown;
        //}
    }
}
