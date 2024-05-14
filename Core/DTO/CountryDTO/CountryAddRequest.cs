using Core.Domain.Entities;

namespace Core.DTO.CountryDTO
{
    public class CountryAddRequest
    {
        public string? Name { get; set; }

        public Country ToCountry()
        {
            Country country = new Country()
            {
                //ID = Guid.NewGuid(),
                Name = this.Name
            };

            return country;
        }
    }

  
}
