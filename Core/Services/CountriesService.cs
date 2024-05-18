using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Core.DTO.CountryDTO;
using Core.ServiceContracts;


namespace Core.Services;

public class CountriesService : ICountriesService
{
    private readonly ICountriesRepository _countriesRepository;

    public CountriesService(ICountriesRepository countriesRepository)
    {
        _countriesRepository = countriesRepository;
    }


    //
    public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
    {
         // 'countryAddRequest' is Null //
        if (countryAddRequest == null)
        {
            throw new ArgumentNullException("'CountryAddRequest' object is Null");
        }

         // 'countryAddRequest.Name' is Null //
        if (countryAddRequest.Name == null)
        {
            throw new ArgumentNullException("The 'Country Name' in 'CountryAddRequest' object is Null");
        }

         // 'countryAddRequest.Name' is Duplicate //
        if (await _countriesRepository.GetCountryByName(countryAddRequest.Name) != null)
        {
            throw new ArgumentException("The 'Country Name' is already exists");
        }


        // Converting from 'CountryAddRequest' to 'Country'
        Country country = countryAddRequest.ToCountry();
        country.ID = Guid.NewGuid();

        await _countriesRepository.AddCountry(country);

        // Converting from 'Country' to 'CountryResponse'
        CountryResponse countryResponse = country.ToCountryResponse();
        return countryResponse;
    }


    //
    public async Task<List<CountryResponse>> GetAllCountries()
    {
        return (await _countriesRepository.GetAllCountries()).Select(country => country.ToCountryResponse()).ToList();
    }


    //
    public async Task<CountryResponse?> GetCountryByID(Guid? id)
    {
        // if 'id' is null
        if (id == null)
        {
            return null;
        }

        Country? country = await _countriesRepository.GetCountryByID(id.Value);

        // if 'id' doesn't exist in 'countries list'
        if (country == null)
        {
            return null;
        }

        // if there is no problem
        CountryResponse countryResponse = country.ToCountryResponse();
        return countryResponse;
    }
}