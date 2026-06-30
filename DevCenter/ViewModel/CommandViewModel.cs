using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevCenter.Models;
using DevCenter.Repositories;
using DevCenter.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace DevCenter.ViewModel
{
    public partial class CommandViewModel : ObservableObject
    {
        private readonly DevCommandRepo _repo;
        private readonly INavigationService _navigation;
        private Process? _runningProcess;

        public string Header => "Commands";
        public string PageDescription => "Save and run your frequently used scripts and shell commands.";

        [ObservableProperty]
        private ObservableCollection<DevCommand> _commands = [];

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private string _output = string.Empty;

        [ObservableProperty]
        private bool _isRunning = false;

        [ObservableProperty]
        private DevCommand? _activeCommand;

        [ObservableProperty]
        private ObservableCollection<string> _activeCommandLines = [];

        public record OutputBlock(string Kind, string Text); // Kind: "cmd" | "output" | "error" | "separator"

        [ObservableProperty]
        private ObservableCollection<OutputBlock> _outputBlocks = [];

        public CommandViewModel(DevCommandRepo repo, INavigationService navigation)
        {
            _repo = repo;
            _navigation = navigation;
            LoadCommands();
        }

        private void LoadCommands()
        {
            Commands = new ObservableCollection<DevCommand>(_repo.GetAll());
        }

        partial void OnSearchTextChanged(string value)
        {
            var filtered = _repo.GetAll()
                .Where(c => string.IsNullOrWhiteSpace(value) ||
                            c.Name.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                            (c.Description ?? "").Contains(value, StringComparison.OrdinalIgnoreCase) ||
                            c.TagList.Any(t => t.Contains(value, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            Commands = new ObservableCollection<DevCommand>(filtered);
        }

        [RelayCommand]
        private void FilterByTag(string tag)
        {
            SearchText = tag;
        }

        [RelayCommand]
        private void Delete(DevCommand command)
        {
            _repo.Delete(command.Id);
            Commands.Remove(command);
        }

        [RelayCommand]
        private async Task Run(DevCommand command)
        {
            if (IsRunning) return;

            IsRunning = true;
            Output = string.Empty;
            OutputBlocks = [];
            ActiveCommand = command;

            var lines = command.Script
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToList();

            ActiveCommandLines = new ObservableCollection<string>(lines);

            try
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    var line = lines[i];

                    // Separator between commands
                    if (i > 0)
                        Application.Current.Dispatcher.Invoke(() =>
                            OutputBlocks.Add(new OutputBlock("separator", string.Empty)));

                    // Command line
                    Application.Current.Dispatcher.Invoke(() =>
                        OutputBlocks.Add(new OutputBlock("cmd", line)));

                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = $"/c {line}",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        },
                        EnableRaisingEvents = true
                    };

                    _runningProcess = process;

                    process.OutputDataReceived += (s, e) =>
                    {
                        if (e.Data is null) return;
                        Application.Current.Dispatcher.Invoke(() =>
                            OutputBlocks.Add(new OutputBlock("output", e.Data)));
                    };

                    process.ErrorDataReceived += (s, e) =>
                    {
                        if (e.Data is null) return;
                        Application.Current.Dispatcher.Invoke(() =>
                            OutputBlocks.Add(new OutputBlock("error", e.Data)));
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    await process.WaitForExitAsync();

                    if (!process.HasExited)
                        process.WaitForExit();

                    process.Dispose();

                    if (_runningProcess is null) break;
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                    OutputBlocks.Add(new OutputBlock("error", $"Failed: {ex.Message}")));
            }
            finally
            {
                IsRunning = false;
                _runningProcess = null;
            }
        }

        [RelayCommand]
        private void Stop()
        {
            if (_runningProcess is null || _runningProcess.HasExited) return;

            try
            {
                _runningProcess.Kill(entireProcessTree: true);
                Application.Current.Dispatcher.Invoke(() =>
                    OutputBlocks.Add(new OutputBlock("error", "[Stopped by user]")));
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                    OutputBlocks.Add(new OutputBlock("error", $"[Stop failed: {ex.Message}]")));
            }
            finally
            {
                IsRunning = false;
                _runningProcess = null;
            }
        }

        [RelayCommand]
        private void ClearSearch()
        {
            SearchText = string.Empty;
        }

        [RelayCommand]
        private void New()
        {
            _navigation.NavigateTo<CommandFormViewModel>();
        }

        [RelayCommand]
        private void Edit(DevCommand command)
        {
            if (command is null) return;
            _navigation.NavigateTo<CommandFormViewModel>(command);
        }
    }
}