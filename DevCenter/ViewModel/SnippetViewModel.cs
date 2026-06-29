using CommunityToolkit.Mvvm.ComponentModel;

namespace DevCenter.ViewModel
{
    public partial class SnippetViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _header = "Snippets";
    }
}
