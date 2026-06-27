using CommunityToolkit.Mvvm.ComponentModel;

namespace DevCenter.ViewModel
{
    public partial class CommandViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title = "Commands";
    }
}
