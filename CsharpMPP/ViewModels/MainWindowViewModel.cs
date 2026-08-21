using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices.JavaScript;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System.Linq;
using Server;
using Model;
using Protobuf;
using Services;
using ReactiveUI;

namespace CsharpMPP.ViewModels;

public partial class MainWindowViewModel : INotifyPropertyChanged
{
    public MainWindowViewModel(MainViewModel parent, IServices proxy, User user){
        _parent = parent;
        _service = proxy;
        User = user;
        var races = proxy.GetAllRaces();
        var participants = proxy.GetAllParticipants();
        Races = new ObservableCollection<Race>(races);
        RacesForAdd = new ObservableCollection<Race>(races); 
        RacesForUpdate = new ObservableCollection<Race>(races);
        Participants = new ObservableCollection<Participant>(participants);
    }
    private readonly IServices _service;
    private readonly MainViewModel _parent;
    public User User { get; set; }
    public ObservableCollection<Race> Races { get; set; } 
    public ObservableCollection<Race> RacesForAdd { get; set; }
    public ObservableCollection<Race> RacesForUpdate { get; set; }
    public ObservableCollection<Participant>? Participants { get; set; }
    public ObservableCollection<Participant>? ParticipantsForRace { get; set; } = new();

    private string _name =  string.Empty;

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    private string _age = string.Empty;
    public string Age
    {
        get => _age;
        set
        {
            _age = value;
            OnPropertyChanged(nameof(Age));
        }
    }

    private string _message = "";
    public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }
    
    private Race? _selectedRace;
    public Race SelectedRace
        {
            get => _selectedRace!;
            set
            {
                _selectedRace = value;
                //OnPropertyChanged(nameof(SelectedRace));
                UpdateParticipants();
            }
        }

    private Participant? _selectedParticipant = null;
    public Participant SelectedParticipant
    {
        get => _selectedParticipant!;
        set
        {
            _selectedParticipant = value;
            UpdateRaces();
        }
    }

    private void UpdateRaces()
    {
        ShowStatus("Participant selectat id " +  SelectedParticipant.Id);
        var filteredRaces = 
        _service.GetAllRaces().Select(race =>
        {
            if (SelectedParticipant.Races!.Contains(race.Id))
                race.IsSelected = true;
            return race;
        }).ToList(); 
        RacesForUpdate.Clear();
        filteredRaces.ForEach(race => RacesForUpdate.Add(race));
    } 
    
    private void UpdateParticipants()
    {
        ShowStatus("Cursa selectata id " + SelectedRace.Id);
        ParticipantsForRace!.Clear();
        var part = _service.GetAllParticipantsById(SelectedRace.Participants!);
        foreach (var participantDto in part)
        {
            ParticipantsForRace.Add(participantDto);
        } 
    }

    
    public void ExecuteAdd()
    {
        int age = 0;
        if(Name.Length == 0 ||  Age.Length == 0)
            ShowStatus("Name or Age shouldn't be empty");
        else if (!Int32.TryParse(Age, out age))
            ShowStatus("Age should be a number");
        else
        {
            var races = new List<Race>();
            foreach (var raceDto in RacesForAdd)
            {
                if(raceDto.IsSelected)
                    races.Add(raceDto);
            }
            if(races.Count == 0)
                ShowStatus("Select at least a race");
            else
            {
                var racesIds = races.Select(raceDto => raceDto.Id).ToList();
                var p = _service.SaveParticipant(new Participant(Name, age){Races = racesIds});
                ShowStatus($"Successfully added participant with identity {p.Id}");
                RefreshAddParticipant();
            }
        } 
    }

    private void RefreshAddParticipant()
    {
        Participants!.Clear();
        _service.GetAllParticipants().ToList().ForEach(participant => Participants.Add(participant));
        RacesForAdd!.Clear();
        _service.GetAllRaces().ToList().ForEach(race => RacesForAdd.Add(race));
        Name = "";
        Age = "";
    }

    public void ExecuteUpdate()
    {
        if (SelectedParticipant != null)
        {
            var selectedRaces = new List<long>();
            foreach (var raceDto in RacesForUpdate)
            {
                if (raceDto.IsSelected)
                    selectedRaces.Add(raceDto.Id);
            }

            _service.UpdateParticipant(new Participant(SelectedParticipant.Name, SelectedParticipant.Age) {Id = SelectedParticipant.Id, Races = selectedRaces});
            //refreshUpdateParticipant();
        }
        else
        {
            ShowStatus("Select a participant first");
        }
    }

    private void refreshUpdateParticipant()
    {
        ShowStatus("Updated participant with id " + SelectedParticipant.Id);
        RacesForUpdate.Clear();
        _service.GetAllRaces().ToList().ForEach(race => RacesForUpdate.Add(race));
        Participants!.Clear();
        _service.GetAllParticipants().ToList().ForEach(participant => Participants.Add(participant));
    }

    public void LogoutCommand()
    {
        _service.Logout(User, _parent);
        _parent.CurrentPage = new LoginWindowViewModel(_parent, _service);
    }
    
    public async void ShowStatus(string text, int delay = 1000)
        {
            Message = text;
            await Task.Delay(delay);
            Message = "";
        }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}