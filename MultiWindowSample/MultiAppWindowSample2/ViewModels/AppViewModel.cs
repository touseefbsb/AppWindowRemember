using MultiAppWindowSample2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml.Controls;
using System;
using Windows.UI.Xaml.Hosting;
using Windows.Foundation;

namespace MultiAppWindowSample2.ViewModels
{
    public static class AppViewModel
    {
        public static List<AppWindowViewModel> AppWindowViewModels = new List<AppWindowViewModel>();
        internal static void UpdateAppWindowsPlacements()
        {
            foreach (var item in AppWindowViewModels)
            {
                ApplicationData.Current.LocalSettings.Values[$"AppWindow_SecondaryView_Show_{item.Key}"] = item.AppWindow != null;
                if (item.AppWindow != null)
                {
                    var placement = item.AppWindow.GetPlacement();
                    var regions = new List<DisplayRegion>();
                    foreach (var dr in item.AppWindow.WindowingEnvironment.GetDisplayRegions())
                    {
                        regions.Add(dr);
                    }
                    //Size is full screen size and can be bigger bcz it also includes taskbar etc.
                    //Display region excludes taskbar etc
                    var displayRegion = placement.DisplayRegion;
                    var displayRegionWidth = displayRegion.WorkAreaSize.Width;
                    var displayRegionHeight = displayRegion.WorkAreaSize.Height;

                    var sizeWidth = placement.Size.Width;
                    var sizeHeight = placement.Size.Height;

                    ApplicationData.Current.LocalSettings.Values[$"AppWindow_SecondaryView_Width_{item.Key}"] = sizeWidth > displayRegionWidth ? displayRegionWidth : sizeWidth;
                    ApplicationData.Current.LocalSettings.Values[$"AppWindow_SecondaryView_Height_{item.Key}"] = sizeHeight > displayRegionHeight ? displayRegionHeight : sizeHeight;

                    ApplicationData.Current.LocalSettings.Values[$"AppWindow_SecondaryView_X_{item.Key}"] = placement.Offset.X;
                    ApplicationData.Current.LocalSettings.Values[$"AppWindow_SecondaryView_Y_{item.Key}"] = placement.Offset.Y;
                }
            }
        }
        internal static async Task OpenSecondaryWindows(int total)
        {
            for (int i = 0; i < total; i++)
            {
                var appWindowViewModel = new AppWindowViewModel(i.ToString());
                AppWindowViewModels.Add(appWindowViewModel);
                var open = ApplicationData.Current.LocalSettings.Values[$"AppWindow_SecondaryView_Show_{i}"];
                if (open == null)
                {
                    ApplicationData.Current.LocalSettings.Values[$"AppWindow_SecondaryView_Show_{i}"] = true;
                    open = true;
                }
                if ((bool)open)
                {
                    await View(appWindowViewModel);
                }
            }
        }
        private static async Task View(AppWindowViewModel appWindowViewModel)
        {
            if (appWindowViewModel.AppWindow is null)
            {
                appWindowViewModel.AppWindow = await AppWindow.TryCreateAsync();
                var frame = new Frame();
                frame.Navigate(typeof(SecondaryPage), appWindowViewModel.Key);
                ElementCompositionPreview.SetAppWindowContent(appWindowViewModel.AppWindow, frame);

                appWindowViewModel.AppWindow.Closed += delegate
                {
                    frame.Content = null;
                    appWindowViewModel.AppWindow = null;
                };
            }

            var shown = await appWindowViewModel.AppWindow.TryShowAsync();

            var windowWidth = ApplicationData.Current.LocalSettings.Values[$"AppWindow_SecondaryView_Width_{appWindowViewModel.Key}"];
            var windowHeight = ApplicationData.Current.LocalSettings.Values[$"AppWindow_SecondaryView_Height_{appWindowViewModel.Key}"];
            if (windowWidth is double wWidth && windowHeight is double wHeight)
            {
                appWindowViewModel.AppWindow.RequestSize(new Size(wWidth, wHeight));
            }

            var xposition = ApplicationData.Current.LocalSettings.Values[$"AppWindow_SecondaryView_X_{appWindowViewModel.Key}"];
            var yposition = ApplicationData.Current.LocalSettings.Values[$"AppWindow_SecondaryView_Y_{appWindowViewModel.Key}"];
            if (xposition is double xpos && yposition is double ypos)
            {
                var placement = appWindowViewModel.AppWindow.GetPlacement();
                appWindowViewModel.AppWindow.RequestMoveRelativeToDisplayRegion(placement.DisplayRegion, new Point(xpos, ypos));
            }
            else
            {
                appWindowViewModel.AppWindow.RequestMoveAdjacentToCurrentView();
            }
        }
        internal static void CloseAllSecondaryWindows()
        {
            foreach (var item in AppWindowViewModels)
            {
                item.AppWindow?.CloseAsync();
            }
        }
    }
}
