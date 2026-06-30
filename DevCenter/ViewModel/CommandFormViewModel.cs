using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevCenter.Models;
using DevCenter.Repositories;
using DevCenter.Services;

namespace DevCenter.ViewModel
{
    public partial class CommandFormViewModel : ObservableObject
    {
        private readonly DevCommandRepo _repo;
        private readonly DevCommand? _existing;

        [ObservableProperty]
        public string _header = "Add New Command";
        public string PageDescription => "Save and run your frequently used scripts and shell commands.";

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private string _script = string.Empty;

        [ObservableProperty]
        private string _tags = string.Empty;

        public INavigationService Navigation { get; }

        // Create mode
        public CommandFormViewModel(DevCommandRepo repo, INavigationService navigation)
        {
            _repo = repo;
            Navigation = navigation;
        }

        // Edit mode
        public CommandFormViewModel(DevCommandRepo repo, DevCommand existing, INavigationService navigation)
        {
            _repo = repo;
            _existing = existing;
            Name = existing.Name;
            Description = existing.Description;
            Script = existing.Script;
            Tags = existing.Tags ?? string.Empty;
            Navigation = navigation;
        }

        [RelayCommand]
        private void Save()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Script))
                return;

            if (_existing is null)
            {
                _repo.Add(new DevCommand
                {
                    Name = Name,
                    Description = Description,
                    Script = Script,
                    Tags = Tags,
                    Created = DateTime.UtcNow
                });
            }
            else
            {
                _existing.Name = Name;
                _existing.Description = Description;
                _existing.Script = Script;
                _existing.Tags = Tags;
                _repo.Update(_existing);
            }

            Navigation.NavigateTo<CommandViewModel>();
        }

        [RelayCommand]
        private void Cancel()
        {
            Navigation.NavigateTo<CommandViewModel>();
        }
    }
}