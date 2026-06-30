using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevCenter.Models;
using DevCenter.Repositories;
using DevCenter.Services;
using System.Collections.ObjectModel;

namespace DevCenter.ViewModel
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly DevCommandRepo _commandRepo;
        private readonly SnippetRepo _snippetRepo;
        private readonly INavigationService _navigation;

        public string Header => "Home";
        public string PageDescription => "A quick overview of your saved commands and snippets.";

        [ObservableProperty]
        private int _commandCount;

        [ObservableProperty]
        private int _snippetCount;

        [ObservableProperty]
        private int _languageCount;

        [ObservableProperty]
        private ObservableCollection<DevCommand> _recentCommands = [];

        [ObservableProperty]
        private ObservableCollection<Snippet> _recentSnippets = [];

        public HomeViewModel(DevCommandRepo commandRepo, SnippetRepo snippetRepo, INavigationService navigation)
        {
            _commandRepo = commandRepo;
            _snippetRepo = snippetRepo;
            _navigation = navigation;
            LoadDashboard();
        }

        private void LoadDashboard()
        {
            var commands = _commandRepo.GetAll();
            var snippets = _snippetRepo.GetAll();

            CommandCount = commands.Count;
            SnippetCount = snippets.Count;
            LanguageCount = snippets
                .Select(s => s.Language)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();

            RecentCommands = new ObservableCollection<DevCommand>(commands.Take(4));
            RecentSnippets = new ObservableCollection<Snippet>(snippets.Take(4));
        }

        [RelayCommand]
        private void NewCommandAction() => _navigation.NavigateTo<CommandFormViewModel>();

        [RelayCommand]
        private void NewSnippetAction() => _navigation.NavigateTo<SnippetFormViewModel>();

        [RelayCommand]
        private void GoToCommands() => _navigation.NavigateTo<CommandViewModel>();

        [RelayCommand]
        private void GoToSnippets() => _navigation.NavigateTo<SnippetViewModel>();

        [RelayCommand]
        private void OpenCommand(DevCommand command) => _navigation.NavigateTo<CommandViewModel>();

        [RelayCommand]
        private void OpenSnippet(Snippet snippet) => _navigation.NavigateTo<SnippetViewModel>();
    }
}
