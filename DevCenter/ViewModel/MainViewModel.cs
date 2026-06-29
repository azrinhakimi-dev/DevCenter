using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevCenter.Services;

namespace DevCenter.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        public INavigationService Navigation { get; }

        public MainViewModel(INavigationService navigation)
        {
            Navigation = navigation;
            Navigation.NavigateTo<HomeViewModel>(); // set default page
        }

        [RelayCommand]
        private void GoToHome() => Navigation.NavigateTo<HomeViewModel>();

        [RelayCommand]
        private void GoToSnippet() => Navigation.NavigateTo<SnippetViewModel>();

        [RelayCommand]
        private void GoToCommands() => Navigation.NavigateTo<CommandViewModel>();

        [RelayCommand]
        private void GoToCommandForm() => Navigation.NavigateTo<CommandFormViewModel>();
    }
}
