using Core.Domain.Entities;


namespace Core.DTO.PersonDTO;

public class PersonResponse
{
    public Guid? ID { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public Guid CountryID { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }

    // Additional
    public string? CountryName { get; set; }
    public double? Age { get; set; }


    // Check equality of two 'PersonResponse' objects
    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj.GetType() != typeof(PersonResponse))
        {
            return false;
        }

        PersonResponse objCompare = (PersonResponse)obj;
        if (Name != objCompare.Name || ID != objCompare.ID || Email != objCompare.Email || DateOfBirth != objCompare.DateOfBirth ||
            Gender != objCompare.Gender || CountryID != objCompare.CountryID || Address != objCompare.Address ||
            ReceiveNewsLetters != objCompare.ReceiveNewsLetters || CountryName != objCompare.CountryName || Age != objCompare.Age)
        {
            return false;
        }

        return true;
    }

    // Just For ignoring compiler 'Warning'
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        string message = $"ID:{ID}, Name:{Name}, Email:{Email}, DateOfBirth:{DateOfBirth.ToString()}, Gender: {Gender}" +
                         $", CountryID: {CountryID.ToString()}, Address: {Address}, ReceiveNewsLetters: {ReceiveNewsLetters}" ;

        return message;
    }
}


public static class PersonExtensions
{
    public static PersonResponse ToPersonResponse (this Person person)
    {
        PersonResponse response = new PersonResponse()
        {
            ID = person.ID,
            Name = person.Name,
            Email = person.Email,
            DateOfBirth = person.DateOfBirth,
            Gender = person.Gender,
            CountryID = person.CountryID,
            Address = person.Address,
            ReceiveNewsLetters = person.ReceiveNewsLetters,
            CountryName = person.Country?.Name,
            Age = (DateTime.Now.Year - person.DateOfBirth?.Year)
        };
        return response;
    }
}