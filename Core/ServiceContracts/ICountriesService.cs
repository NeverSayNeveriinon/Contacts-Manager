using Core.DTO.CountryDTO;


namespace Core.ServiceContracts;

public interface ICountriesService
{
    /// <summary>
    /// Add a country object to countries list
    /// </summary>
    /// <param name="countryAddRequest">Country object to be added</param>
    /// <return>Returns the new country object (with ID) after adding it</return>
    public Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

    /// <summary>
    /// Retrieve all Country objects from countries list
    /// </summary>
    /// <returns>Returns all existing Countries</returns>
    public Task<List<CountryResponse>> GetAllCountries();

    /// <summary>
    /// Retrieve a Country object from countries list based on given id
    /// </summary>
    /// <param name="id">the country id to be searched for</param>
    /// <returns></returns>
    public Task<CountryResponse?> GetCountryByID(Guid? id);

}