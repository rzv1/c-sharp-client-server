using Model;
using Services;

namespace Server;

public class ServiceImpl(UserService userService, ParticipantService participantService, RaceService raceService) : IServices
{
    private Dictionary<string, IObserver> _proxies = [];

    public void Login(User user, IObserver proxy)
    {
        if(_proxies.ContainsKey(user.Username))
            throw new AppException("User already logged in");
        if (userService.VerifyLogin(user.Username, user.Password) != null)
            _proxies.Add(user.Username, proxy);
        else
            throw new AppException("Username or password is incorrect");
    }

    public void Logout(User user, IObserver proxy)
    {
        if (!_proxies.Remove(user.Username)) 
            throw new AppException("User is not logged in");
    }

    public IEnumerable<Race> GetAllRaces()
    {
        return raceService.GetAll();
    }

    public IEnumerable<Race> GetAllRacesById(List<long> id)
    {
        return raceService.GetAllById(id);
    }

    public IEnumerable<Participant> GetAllParticipants()
    {
        return participantService.GetAll();
    }

    public IEnumerable<Participant> GetAllParticipantsById(List<long> id)
    {
        return participantService.GetAllById(id);
    }

    public Participant SaveParticipant(Participant participant)
    {
        var part = participantService.Save(participant.Name, participant.Age, participant.Races);
        NotifyProxies();
        return part;
    }

    public void UpdateParticipant(Participant participant)
    {
        participantService.Update(participant.Id, participant.Races); 
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