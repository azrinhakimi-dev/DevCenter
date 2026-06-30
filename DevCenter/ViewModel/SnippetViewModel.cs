using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevCenter.Models;
using DevCenter.Repositories;
using DevCenter.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace DevCenter.ViewModel
{
    public partial class SnippetViewModel : ObservableObject
    {
        private readonly SnippetRepo _repo;
        private readonly INavigationService _navigation;

        public string Header => "Snippets";
        public string PageDescription => "Save reusable code with context so future-you remembers why it exists.";

        [ObservableProperty]
        private ObservableCollection<Snippet> _snippets = [];

        [ObservableProperty]
        private Snippet? _selectedSnippet;

        [ObservableProperty]
        private string _searchText = string.Empty;

        public SnippetViewModel(SnippetRepo repo, INavigationService navigation)
        {
            _repo = repo;
            _navigation = navigation;
            LoadSnippets();
        }

        private void LoadSnippets()
        {
            Snippets = new ObservableCollection<Snippet>(_repo.GetAll());

        }

        partial void OnSearchTextChanged(string value)
        {
            var filtered = _repo.GetAll()
                .Where(s => string.IsNullOrWhiteSpace(value) ||
                            s.Title.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                            s.Language.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                            (s.Tags ?? "").Contains(value, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Snippets = new ObservableCollection<Snippet>(filtered);
        }

        [RelayCommand]
        private void Select(Snippet snippet)
        {
            SelectedSnippet = snippet;
        }

        [RelayCommand]
        private void CopyCode(Snippet snippet)
        {
            if (string.IsNullOrWhiteSpace(snippet.Code)) return;
            Clipboard.SetText(snippet.Code);
        }

        [RelayCommand]
        private void Delete(Snippet snippet)
        {
            _repo.Delete(snippet.Id);
            Snippets.Remove(snippet);
            if (SelectedSnippet?.Id == snippet.Id)
                SelectedSnippet = Snippets.FirstOrDefault();
        }

        [RelayCommand]
        private void New()
        {
            _navigation.NavigateTo<SnippetFormViewModel>();
        }

        [RelayCommand]
        private void ClearSearch()
        {
            SearchText = string.Empty;
        }

        [ObservableProperty]
        private bool _isEditingCode = false;

        [ObservableProperty]
        private string _editableCode = string.Empty;

        [RelayCommand]
        private void StartEditCode()
        {
            if (SelectedSnippet is null) return;
            EditableCode = SelectedSnippet.Code;
            IsEditingCode = true;
        }

        [RelayCommand]
        private void SaveCode()
        {
            if (SelectedSnippet is null) return;

            SelectedSnippet.Code = EditableCode;
            _repo.Update(SelectedSnippet);
            IsEditingCode = false;

            // Force the binding to refresh since Snippet isn't observable
            var current = SelectedSnippet;
            SelectedSnippet = null;
            SelectedSnippet = current;
        }

        [RelayCommand]
        private void CancelEditCode()
        {
            IsEditingCode = false;
            EditableCode = string.Empty;
        }

        partial void OnSelectedSnippetChanged(Snippet? value)
        {
            IsEditingCode = false;
            EditableCode = string.Empty;
        }
    }
}
