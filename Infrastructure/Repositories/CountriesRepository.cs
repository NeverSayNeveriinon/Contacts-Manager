using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CountriesRepository : ICountriesRepository
    {
        private readonly PersonsDbContext _dbContext;
        public CountriesRepository(PersonsDbContext dbContext) 
        { 
            _dbContext = dbContext;
        }

        public async Task<Country> AddCountry(Country country)
        {
            _dbContext.Countries.Add(country);
            await _dbContext.SaveChangesAsync();

            return country;
        }

        public async Task<List<Country>> GetAllCountries()
        {
            var countriesList = await _dbContext.Countries.ToListAsync();

            return countriesList;
        }

        public async Task<Country?> GetCountryByID(Guid id)
        {
            var finalCountry = await _dbContext.Countries.FirstOrDefaultAsync(country => country.ID == id);

            return finalCountry;
        }

        public async Task<Country?> GetCountryByName(string name)
        {
            var finalCountry = await _dbContext.Countries.FirstOrDefaultAsync(country => country.Name == name);
            
            return finalCountry;
        }
    }
}
