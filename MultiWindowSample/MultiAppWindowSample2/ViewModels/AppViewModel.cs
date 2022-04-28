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
                    //var regions2 = new List<DisplayRegion>(); 
                    //foreach (var dr in item.AppWindow.GetDisplayRegions())
                    //{
                    //    regions2.Add(dr);// this list is just for testing, it gives me boh monitors/DisplayRegions, but no way to find out where this window resides.
                    //}
                    //Size is full screen size and can be bigger bcz it also includes taskbar etc.
                    //Display region excludes taskbar etc
                    var displayRegion = placement.DisplayRegion;
                    var displayRegionWidth = displayRegion.WorkAreaSize.Width;
                    var displayRegionHeight = displayRegion.WorkAreaSize.Height;

                    var sizeWidth = placement.Size.Width;
                    var sizeHeight = placement.Size.Height;

                    var displayRegionX = displayRegion.WorkAreaOffset.X;
                    var displayRegionY = displayRegion.WorkAreaOffset.Y;

                    var sizeX = placement.Offset.X;
                    var sizeY = placement.Offset.Y;

                    var windowAppWidth = sizeWidth > displayRegionWidth ? displayRegionWidth : sizeWidth;
                    var windowAppHeight = sizeHeight > displayRegionHeight ? displayRegionHeight : sizeHeight;
                    ApplicationData.Current.LocalSettings.Values[$"AppWindow_SecondaryView_Width_{item.Key}"] = windowAppWidth;
                    ApplicationData.Current.LocalSettings.Values[$"AppWindow_SecondaryView_Height_{item.Key}"] = windowAppHeight;

                    var windowX = sizeX < displayRegionX ? displayRegionX : sizeX;
                    var windowY = sizeY < displayRegionY ? displayRegionY : sizeY;
                    //var windowX = sizeX + 18;
                    //var windowY = sizeY + 18;
                    ApplicationData.Current.LocalSettings.Values[$"AppWindow_SecondaryView_X_{item.Key}"] = windowX;
                    ApplicationData.Current.LocalSettings.Values[$"AppWindow_SecondaryView_Y_{item.Key}"] = windowY;

                    //foreach (var region in item.AppWindow.WindowingEnvironment.GetDisplayRegions())
                    //{
                    //    if (region.WorkAreaOffset.X == windowX && region.WorkAreaOffset.Y == windowY)
                    //    {
                    //        ApplicationData.Current.LocalSettings.Values[$"AppWindow_SecondaryView_DisplayMonitorDeviceId_{item.Key}"] = region.DisplayMonitorDeviceId;
                    //    }
                    //}
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
                var displayRegion = appWindowViewModel.AppWindow.GetPlacement().DisplayRegion;
                //if (ApplicationData.Current.LocalSettings.Values[$"AppWindow_SecondaryView_DisplayMonitorDeviceId_{appWindowViewModel.Key}"] is string monitorid)
                //{
                //    foreach (var region in appWindowViewModel.AppWindow.WindowingEnvironment.GetDisplayRegions())
                //    {
                //        if (region.DisplayMonitorDeviceId == monitorid)
                //        {
                //            displayRegion = region;
                //            break;
                //        }
                //    }
                //}
                //appWindowViewModel.AppWindow.RequestMoveToDisplayRegion(displayRegion);
                appWindowViewModel.AppWindow.RequestMoveRelativeToDisplayRegion(displayRegion, new Point(xpos, ypos));
                //appWindowViewModel.AppWindow.RequestMoveRelativeToCurrentViewContent(new Point(xpos, ypos));
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
