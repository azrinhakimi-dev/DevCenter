using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace DevCenter.Services;

public interface INavigationService
{

    void NavigateTo<TViewModel>() where TViewModel : ObservableObject;
}

public partial class NavigationService : ObservableObject, INavigationService
{
    private readonly IServiceProvider _services;

    public NavigationService(IServiceProvider service)
    {
        _services = service;
    }
    [ObservableProperty]
    private object? _currentViewModel;

    public string CurrentTitle =>
        (CurrentViewModel as dynamic)?.Header ?? string.Empty;

    public string CurrentPageDescription =>
        (CurrentViewModel as dynamic)?.PageDescription ?? string.Empty;


    public void NavigateTo<TViewModel>() where TViewModel : ObservableObject
    {
        CurrentViewModel = _services.GetRequiredService<TViewModel>();
        OnPropertyChanged(nameof(CurrentTitle));
        OnPropertyChanged(nameof(CurrentPageDescription));
    }
}

