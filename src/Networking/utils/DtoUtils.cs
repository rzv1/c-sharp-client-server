using Model;
using Protobuf;

namespace Networking;

public static class DtoUtils
{
    public static User FromDto(UserDto dto)
    {
        return new User(dto.Username, dto.Password);
    }

    public static UserDto ToDto(User user)
    {
        return new UserDto() {Username = user.Username, Password = user.Password};
    }

    public static Participant FromDto(ParticipantDto dto)
    {
        return new Participant(dto.Name, dto.Age) { Id = dto.Id, Races =  dto.Races.ToList() };
    }

    public static ParticipantDto ToDto(Participant participant)
    {
        var p = new ParticipantDto() {Id = participant.Id, Name = participant.Name, Age = participant.Age };
        p.Races.AddRange(participant.Races);
        return p;
    }

    public static Race FromDto(RaceDto dto)
    {
        return new Race(dto.Distance, dto.Style) { Id = dto.Id, Participants = dto.Participants.ToList() };
    }

    public static RaceDto ToDto(Race race)
    {
        var r = new RaceDto() { Id = race.Id, Distance = race.Distance, Style = race.Style };
        r.Participants.AddRange(race.Participants);
        return r;
    }

    public static IEnumerable<ParticipantDto> ToDto(IEnumerable<Participant> participants)
    {
        var dtoList =  new List<ParticipantDto>();
        foreach (var p in participants)
        {
            var dtoOfP = new ParticipantDto() { Id = p.Id, Name = p.Name, Age = p.Age };
            if (p.Races != null) 
                dtoOfP.Races.AddRange(p.Races);
            dtoList.Add(dtoOfP);
        }
        return dtoList;
    }

    public static IEnumerable<RaceDto> ToDto(IEnumerable<Race> races)
    {
        var dtoList = new List<RaceDto>();
        foreach (var r in races)
        {
            var dtoOfR = new RaceDto() { Id = r.Id, Style = r.Style, Distance = r.Distance };
            if (r.Participants != null)
                dtoOfR.Participants.AddRange(r.Participants);
            dtoList.Add(dtoOfR);
        }
        return dtoList;
    }

    public static IEnumerable<Race> FromDto(IEnumerable<RaceDto> dtoList)
    {
        var races = new List<Race>();
        foreach (var dto in dtoList)
        {
            races.Add(new Race(dto.Distance, dto.Style) {Id = dto.Id, Participants = dto.Participants.ToList()});
        }
        return races;
    }

    public static IEnumerable<Participant> FromDto(IEnumerable<ParticipantDto> dtoList)
    {
        var participants = new List<Participant>();
        foreach (var dto in dtoList)
        {
            participants.Add(new Participant(dto.Name, dto.Age) {Id = dto.Id, Races = dto.Races.ToList()});
        }
        return participants;
    }
}