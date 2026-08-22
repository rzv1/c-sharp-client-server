using Avalonia;
using Avalonia.Threading;
using Server;
using Services;

namespace AvaloniaClient.ViewModels;

public class MainViewModel : ViewModelBase, IObserver
{
    private object _currentPage;
    private IServices _services;
    public object CurrentPage
    {
        get => _currentPage;
        set
        {
            _currentPage = value;
            OnPropertyChanged(nameof(CurrentPage));
        }
    }

    public MainViewModel(IServices proxy)
    {
        _services = proxy;
        CurrentPage = new LoginWindowViewModel(this, proxy);
    }

    public void Update()
    {
        if (CurrentPage is MainWindowViewModel view)
            Dispatcher.UIThread.Post(() => CurrentPage = new MainWindowViewModel(this, _services, view.User)); 
            //Application.Invoke(() => CurrentPage = new MainWindowViewModel(this, _services, view.User)); 
    }
}