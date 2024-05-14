using System.Linq.Expressions;
using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly PersonsDbContext _dbContext;

        private readonly ILogger<PersonsRepository>? _logger;

        
        public PersonsRepository(PersonsDbContext dbContext, ILogger<PersonsRepository>? logger)
        {
            _dbContext = dbContext;
            
            _logger = logger;
        }
        
        public async Task<Person> AddPerson(Person person)
        {
            _dbContext.Persons.Add(person);
            await _dbContext.SaveChangesAsync();

            return person;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            _logger?.LogInformation("~~~ Started 'GetAllPersons' method of 'Persons' repository ");

            
            List<Person> personsList = await _dbContext.Persons.Include(person => person.Country)
                                                               .ToListAsync();

            return personsList;
        }

        public async Task<Person?> GetPersonByID(Guid id)
        {
            Person? person = await _dbContext.Persons.Include(person => person.Country)
                                                    .FirstOrDefaultAsync(personItem => personItem.ID == id);

            return person;
        }

        public async Task<Person> UpdatePerson(Person person, Person updatedPerson)
        {
            person.ID = updatedPerson.ID;
            person.Name = updatedPerson.Name;
            person.Email = updatedPerson.Email;
            person.DateOfBirth = updatedPerson.DateOfBirth;
            person.Gender = updatedPerson.Gender.ToString();
            person.CountryID = updatedPerson.CountryID;
            person.Address = updatedPerson.Address;
            person.RecieveNewsLetters = updatedPerson.RecieveNewsLetters;

            await _dbContext.SaveChangesAsync();
            return person;
        }
        
        public async Task<bool> DeletePerson(Person person)
        {
            _dbContext.Persons.Remove(person);
            int rowsAffected = await _dbContext.SaveChangesAsync();
            
            bool result = rowsAffected > 0 ? true : false;
            return result;
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            _logger?.LogInformation("~~~ Started 'GetFilteredPersons' method of 'Persons' repository ");

            List<Person> personsList = await _dbContext.Persons.Include(person => person.Country)
                                                               .Where(predicate)
                                                               .ToListAsync();
            return personsList;
        }
    }
}
