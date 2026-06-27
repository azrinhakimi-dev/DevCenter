using CommunityToolkit.Mvvm.ComponentModel;

namespace DevCenter.ViewModel
{
    public partial class HomeViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title = "Home";
    }
}
