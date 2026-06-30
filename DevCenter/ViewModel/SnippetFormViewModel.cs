using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevCenter.Models;
using DevCenter.Repositories;
using DevCenter.Services;

namespace DevCenter.ViewModel
{
    public partial class SnippetFormViewModel : ObservableObject
    {
        private readonly SnippetRepo _repo;
        private readonly Snippet? _existing;
        private readonly INavigationService _navigation;

        public string Header => _existing is null ? "Add New Snippet" : "Edit Snippet";
        public string PageDescription => "Save reusable code with context so future-you remembers why it exists.";

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private string _language = string.Empty;

        [ObservableProperty]
        private string _tags = string.Empty;

        [ObservableProperty]
        private string _code = string.Empty;

        // Create mode
        public SnippetFormViewModel(SnippetRepo repo, INavigationService navigation)
        {
            _repo = repo;
            _navigation = navigation;
        }

        // Edit mode
        public SnippetFormViewModel(SnippetRepo repo, INavigationService navigation, Snippet existing)
        {
            _repo = repo;
            _navigation = navigation;
            _existing = existing;
            Title = existing.Title;
            Description = existing.Description;
            Language = existing.Language;
            Tags = existing.Tags;
            Code = existing.Code;
        }

        [RelayCommand]
        private void Save()
        {
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Code))
                return;

            if (_existing is null)
            {
                _repo.Add(new Snippet
                {
                    Title = Title,
                    Description = Description,
                    Language = Language,
                    Tags = Tags,
                    Code = Code,
                    Created = DateTime.UtcNow
                });
            }
            else
            {
                _existing.Title = Title;
                _existing.Description = Description;
                _existing.Language = Language;
                _existing.Tags = Tags;
                _existing.Code = Code;
                _repo.Update(_existing);
            }

            _navigation.NavigateTo<SnippetViewModel>();
        }

        [RelayCommand]
        private void Cancel()
        {
            _navigation.NavigateTo<SnippetViewModel>();
        }
    }
}
