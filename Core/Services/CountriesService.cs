using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Core.DTO.CountryDTO;
using Core.ServiceContracts;

namespace Core.Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ICountriesRepository _countriesRepository;
        
        public CountriesService(ICountriesRepository countriesRepository, bool withMock = false)
        {
            _countriesRepository = countriesRepository;
            
            // Some Mock Data
            // if (withMock)
            // {
            //     _db.Countries..AddRange(new List<Country> 
            //     {
            //         new Country
            //         {
            //             ID = Guid.Parse("A3C51F30-6C58-42F4-89D2-7105BD971A58"),
            //             Name = "Sweden"
            //         },
            //         new Country
            //         {
            //             ID = Guid.Parse("B0A177B9-A3D3-40A1-A048-157BAB19B727"),
            //             Name = "England"
            //         },
            //         new Country
            //         {
            //             ID = Guid.Parse("78F62C16-E164-46F8-B978-5D3B511B0A9D"),
            //             Name = "USA"
            //         },
            //     });
            // }
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
            // Way 1
            if (await _countriesRepository.GetCountryByName(countryAddRequest.Name) != null)
            {
                throw new ArgumentException("The 'Country Name' is already exists");
            }

            // OR Way 2
            //foreach (var countryItem in await _Db.Countries.ToListAsync())
            //{
            //    if (countryItem.Name == countryAddRequest.Name)
            //    {
            //        throw new ArgumentException("The 'Country Name' is already exists");
            //    }
            //}

            // OR Way 3
            //if (_Db.Countries.Where(country => country.Name == countryAddRequest.Name).Count() > 0)
            //{
            //    throw new ArgumentException("The 'Country Name' is already exists");
            //}

            // OR Way 4
            //if (_Db.Countries.Any(country => country.Name == countryAddRequest.Name))
            //{
            //    throw new ArgumentException("The 'Country Name' is already exists");
            //}



            // 'countryAddRequest.Name' is valid and there is no problem //

            // Converting from 'CountryAddRequest' to 'Country'
            Country country = countryAddRequest.ToCountry();
            country.ID = Guid.NewGuid();


            // Way 1: (Normal Usage)
            await _countriesRepository.AddCountry(country);

            // Way 2: (Using Stored Procedure)
            //_Db.SP_GetAllCountries();

            // Converting from 'Country' to 'CountryResponse'
            CountryResponse countryResponse = country.ToCountryResponse();

            return countryResponse;
        }


        //
        public async Task<List<CountryResponse>> GetAllCountries()
        {
            // Way1
            //List<CountryResponse> countryResponsesList = new List<CountryResponse>();

            //foreach (var countryItem in _Db.Countries)
            //{
            //    countryResponsesList.Add(countryItem.ToCountryResponse());
            //}

            //return countryResponsesList;

            // Way 2
            return (await _countriesRepository.GetAllCountries()).Select(country => country.ToCountryResponse()).ToList();

            //var countries = await _Db.SP_GetAllCountries();
            //return countries.Select(country => country.ToCountryResponse()).ToList();
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
}
