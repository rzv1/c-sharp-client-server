using System.ComponentModel;
using AvaloniaClient.Models;
using AvaloniaClient.Views;
using Microsoft.Data.SqlClient;
using Model;
using Server;
using Services;

namespace AvaloniaClient.ViewModels;

public class LoginWindowViewModel(MainViewModel parent, IServices proxy) 
{
    private string _username = String.Empty;
    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged(nameof(Username));
        }
    }

    private string _password = String.Empty;
    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged(nameof(Password));
        }
    }

    public void Login()
    {
        try
        {
            var u = new User(Username, Password);
            proxy.Login(u, parent);
            var page = new MainWindowViewModel(parent, proxy, u);
            parent.CurrentPage = page;
            Username = "";
            Password = "";
        }
        catch (AppException exception)
        {
            Utils.ShowAlert(exception.Message);
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}