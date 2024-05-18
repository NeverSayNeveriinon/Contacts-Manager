using Core.Domain.Entities;


namespace Core.DTO.CountryDTO;

public class CountryResponse
{
    public Guid? ID { get; set; }
    public string? Name { get; set; }


    // Check equality of two 'CountryResponse' objects
    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj is not CountryResponse)
        {
            return false;
        }
        
        CountryResponse objCompare = (CountryResponse) obj;
        if (Name != objCompare.Name || ID != objCompare.ID)
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
}

public static class CountryExtensions
{ 
    public static CountryResponse ToCountryResponse(this Country country) 
    { 
        CountryResponse response = new CountryResponse()
        {
            ID = country.ID,
            Name = country.Name
        };

        return response;
    }
}