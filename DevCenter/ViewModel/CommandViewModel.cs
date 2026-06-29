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
                            (c.Description ?? "").Contains(value, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Commands = new ObservableCollection<DevCommand>(filtered);
        }

        [RelayCommand]
        private void New()
        {
            _navigation.NavigateTo<CommandFormViewModel>();
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
            ActiveCommand = command;

            var lines = command.Script
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToList();

            ActiveCommandLines = new ObservableCollection<string>(lines);

            try
            {
                foreach (var line in lines)
                {
                    // Print the command line first
                    Application.Current.Dispatcher.Invoke(() =>
                        Output += $"$ {line}\n");

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
                            Output += e.Data + "\n");
                    };

                    process.ErrorDataReceived += (s, e) =>
                    {
                        if (e.Data is null) return;
                        Application.Current.Dispatcher.Invoke(() =>
                            Output += $"[Error] {e.Data}\n");
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    await process.WaitForExitAsync();

                    if (!process.HasExited)
                        process.WaitForExit();

                    Application.Current.Dispatcher.Invoke(() =>
                        Output += "\n");

                    process.Dispose();

                    // Stop remaining lines if user hit Stop
                    if (_runningProcess is null) break;
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                    Output += $"\nFailed: {ex.Message}");
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
                Output += "\n[Stopped by user]";
            }
            catch (Exception ex)
            {
                Output += $"\n[Stop failed: {ex.Message}]";
            }
            finally
            {
                IsRunning = false;
                _runningProcess = null;
                // ActiveCommand intentionally kept — shows last ran command
            }
        }

        [RelayCommand]
        private void ClearSearch()
        {
            SearchText = string.Empty;
        }
    }
}