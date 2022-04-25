using Windows.UI.WindowManagement;

namespace MultiAppWindowSample2.Models
{
    public class AppWindowViewModel
    {
        public AppWindowViewModel(string key) => Key = key;

        public string Key { get; set; }
        public AppWindow AppWindow { get; set; }
    }
}
