using System.Collections.Concurrent;
using Model;
using Services;

namespace Server;

public class ServiceImpl(UserService userService, ParticipantService participantService, RaceService raceService) : IServices
{
    private readonly ConcurrentDictionary<string, IObserver> _proxies = new();
    private readonly object _dbLock = new();

    public void Login(User user, IObserver proxy)
    {
        lock (_dbLock)
        {
            if (_proxies.ContainsKey(user.Username))
                throw new AppException("User already logged in");
            if (userService.VerifyLogin(user.Username, user.Password) != null)
                _proxies[user.Username] = proxy;
            else
                throw new AppException("Username or password is incorrect");
        }
    }

    public void Logout(User user, IObserver proxy)
    {
        if (!_proxies.TryRemove(user.Username, out _)) 
            throw new AppException("User is not logged in");
    }

    public IEnumerable<Race> GetAllRaces()
    {
        lock (_dbLock)
        {
            return raceService.GetAll();
        }
    }

    public IEnumerable<Race> GetAllRacesById(List<long> id)
    {
        lock (_dbLock)
        {
            return raceService.GetAllById(id);
        }
    }

    public IEnumerable<Participant> GetAllParticipants()
    {
        lock (_dbLock)
        {
            return participantService.GetAll();
        }
    }

    public IEnumerable<Participant> GetAllParticipantsById(List<long> id)
    {
        lock (_dbLock)
        {
            return participantService.GetAllById(id);
        }
    }

    public Participant SaveParticipant(Participant participant)
    {
        Participant part;
        lock (_dbLock)
        {
            part = participantService.Save(participant.Name, participant.Age, participant.Races ?? []);
        }
        NotifyProxies();
        return part;
    }

    public void UpdateParticipant(Participant participant)
    {
        lock (_dbLock)
        {
            participantService.Update(participant.Id, participant.Races ?? []);
        }
        NotifyProxies();
    }

    private void NotifyProxies()
    {
        foreach (var proxy in _proxies.Values)
        {
            proxy.Update();
        }
    }
}