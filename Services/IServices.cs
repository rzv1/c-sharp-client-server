using Model;

namespace Services;

public interface IServices
{
    void Login(User user, IObserver proxy);
    void Logout(User user, IObserver proxy);
    IEnumerable<Race> GetAllRaces();
    IEnumerable<Race> GetAllRacesById(List<long> id);
    IEnumerable<Participant> GetAllParticipants();
    IEnumerable<Participant> GetAllParticipantsById(List<long> id);
    Participant SaveParticipant(Participant participant);
    void UpdateParticipant(Participant participant);
}