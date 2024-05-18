using Microsoft.Extensions.Logging;
using Serilog;
using SerilogTimings;

using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Core.DTO.PersonDTO;
using Core.Enums;
using Core.ServiceContracts;
using Core.Exceptions;
using Core.Helpers;


namespace Core.Services;

public class PersonsService : IPersonsService                           
{
    private readonly IPersonsRepository _personsRepository;

    private readonly ILogger<PersonsService>? _logger;
    private readonly IDiagnosticContext? _diagnosticContext;

        
    public PersonsService(IPersonsRepository personsRepository, ILogger<PersonsService>? logger,
                          IDiagnosticContext? diagnosticContext)
    {
        _personsRepository = personsRepository;
        
        _logger = logger;
        _diagnosticContext = diagnosticContext;
    }
    
    //
    public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
    {
         // 'personAddRequest' is Null //
        if (personAddRequest == null)
        {
            throw new ArgumentNullException("'PersonAddRequest' object is Null");
        }

         // 'personAddRequest.Name' is Null //
        if (string.IsNullOrEmpty(personAddRequest.Name))
        {
            throw new ArgumentNullException("The 'Person Name' in 'PersonAddRequest' object can't be blank");
        }
        
        // Validate other properties
        ValidationHelper.ModelValidation(personAddRequest);

         // 'personAddRequest.Name' is Duplicate //
        if ( (await _personsRepository.GetFilteredPersons(person => person.Name == personAddRequest.Name))?.Count > 0)
        {
            throw new ArgumentException("The 'Person Name' is already exists");
        }

        
        // Converting from 'PersonAddRequest' to 'Person'
        Person person = personAddRequest.ToPerson();
        person.ID = Guid.NewGuid();
        
        // adding the object to db
        await _personsRepository.AddPerson(person);

        // Converting from 'Person' to 'PersonResponse'
        PersonResponse personResponse = person.ToPersonResponse();
        return personResponse;
    }


    //
    public async Task<List<PersonResponse>> GetAllPersons()
    {
        _logger?.LogInformation("~~~ Started 'GetAllPersons' method of 'Persons' service ~~~"); // log
        
        
        List<Person> persons = new List<Person>();
        
        // start of "using block" of serilog timings
        using (Operation.Time("~~~ Time for GetAllPersons from Database ~~~~")) // log
        {
            persons = await _personsRepository.GetAllPersons();
        } // end of "using block" of serilog timings

        _diagnosticContext?.Set("Returned Persons List", persons); // log

        
        return persons.Select(personItem => personItem.ToPersonResponse()).ToList();
    }


    //
    public async Task<PersonResponse?> GetPersonByID(Guid? id)
    {
        // if 'id' is null
        if (id == null)
        {
            return null;
        }

        Person? person = await _personsRepository.GetPersonByID(id.Value);

        // if 'id' doesn't exist in 'persons list'
        if (person == null)
        {
            return null;
        }

        PersonResponse personResponse = person.ToPersonResponse();
        return personResponse;
    }


    //
    public async Task<List<PersonResponse>> GetSearchedPersons(string searchBy, string? searchString)
    {
        List<Person> matchingPersons = new List<Person>();

        if (string.IsNullOrEmpty(searchString) || string.IsNullOrEmpty(searchBy))
        {
            return (await GetAllPersons());
        }

        switch(searchBy)
        {
            case nameof(PersonResponse.Name):
            {
                matchingPersons = (await _personsRepository.GetFilteredPersons(person => person.Name!
                                                           .Contains(searchString)))
                                                           .ToList();
                
                matchingPersons = matchingPersons.Where(person => person.Name!.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                                                                              .ToList();
                
                break;
            }
            
            // It is more correct to add the 'ordinalIgnoreCase' code like above for rest of the properties
            case nameof(PersonResponse.Email):
            {
                matchingPersons = (await _personsRepository.GetFilteredPersons( person => string.IsNullOrEmpty(person.Email) ? false 
                                             : person.Email.Contains(searchString) ) )
                                                           .ToList();
                break;
            }
                
            case nameof(PersonResponse.DateOfBirth):
            {
                matchingPersons = (await _personsRepository.GetFilteredPersons( person =>  !person.DateOfBirth.HasValue ? false 
                      : person.DateOfBirth.Value.ToString().Contains(searchString) ) )
                                                           .ToList();
                break;
            }

            case nameof(PersonResponse.Address):
            {
                matchingPersons = (await _personsRepository.GetFilteredPersons(person =>
                                             person.Address!.Contains(searchString)))
                                                           .ToList();
                break;
            }

            case nameof(PersonResponse.ReceiveNewsLetters):
            {
                matchingPersons = (await _personsRepository.GetFilteredPersons(person => 
                                                     string.Equals(person.ReceiveNewsLetters.ToString(), searchString)))
                                                           .ToList();
                break;
            }

            default:
                break;
        }

        
        return matchingPersons.Select(person => person.ToPersonResponse()).ToList();
    }


    //
    public Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
    { 
        List<PersonResponse> sortedPersons = allPersons;

        switch(sortBy, sortOrder)
        {
            // Name
            case (nameof(PersonResponse.Name), SortOrderOptions.ASC):
            {
                sortedPersons = allPersons.OrderBy( person => person.Name, StringComparer.OrdinalIgnoreCase).ToList();
                break;
            } 
            
            case (nameof(PersonResponse.Name), SortOrderOptions.DESC):
            {
                sortedPersons = allPersons.OrderByDescending( person => person.Name, StringComparer.OrdinalIgnoreCase).ToList();
                break;
            }
            
            
            // Email
            case (nameof(PersonResponse.Email), SortOrderOptions.ASC):
            {
                sortedPersons = allPersons.OrderBy( person => person.Email, StringComparer.OrdinalIgnoreCase).ToList();
                break;
            } 
            
            case (nameof(PersonResponse.Email), SortOrderOptions.DESC):
            {
                sortedPersons = allPersons.OrderByDescending(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList();
                break;
            }
                 
            
            // DateOfBirth
            case (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC):
            {
                sortedPersons = allPersons.OrderBy( person => person.DateOfBirth).ToList();
                break;
            } 
            
            case (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC):
            {
                sortedPersons = allPersons.OrderByDescending( person => person.DateOfBirth).ToList();
                break;
            }


            // Gender
            case (nameof(PersonResponse.Gender), SortOrderOptions.ASC):
            {
                sortedPersons = allPersons.OrderBy( person => person.Gender, StringComparer.OrdinalIgnoreCase).ToList();
                break;
            } 
            
            case (nameof(PersonResponse.Gender), SortOrderOptions.DESC):
            {
                sortedPersons = allPersons.OrderByDescending( person => person.Gender, StringComparer.OrdinalIgnoreCase).ToList();
                break;
            }

            
            // Address
            case (nameof(PersonResponse.Address), SortOrderOptions.ASC):
            {
                sortedPersons = allPersons.OrderBy( person => person.Address, StringComparer.OrdinalIgnoreCase).ToList();
                break;
            } 
            
            case (nameof(PersonResponse.Address), SortOrderOptions.DESC):
            {
                sortedPersons = allPersons.OrderByDescending( person => person.Address, StringComparer.OrdinalIgnoreCase).ToList();
                break;
            }


            // ReceiveNewsLetters
            case (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC):
            {
                sortedPersons = allPersons.OrderBy( person => person.ReceiveNewsLetters).ToList();
                break;
            } 
            
            case (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC):
            {
                sortedPersons = allPersons.OrderByDescending( person => person.ReceiveNewsLetters).ToList();
                break;
            }


            // CountryName
            case (nameof(PersonResponse.CountryName), SortOrderOptions.ASC):
            {
                sortedPersons = allPersons.OrderBy( person => person.CountryName, StringComparer.OrdinalIgnoreCase).ToList();
                break;
            } 
            
            case (nameof(PersonResponse.CountryName), SortOrderOptions.DESC):
            {
                sortedPersons = allPersons.OrderByDescending( person => person.CountryName, StringComparer.OrdinalIgnoreCase).ToList();
                break;
            }
            
                
            // Age
            case (nameof(PersonResponse.Age), SortOrderOptions.ASC):
            {
                sortedPersons = allPersons.OrderBy( person => person.Age).ToList();
                break;
            } 
            
            case (nameof(PersonResponse.Age), SortOrderOptions.DESC):
            {
                sortedPersons = allPersons.OrderByDescending( person => person.Age).ToList();
                break;
            }
            
            default:
                break;
        }

        return Task.FromResult(sortedPersons);
    }


    //
    public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
        // if 'personUpdateRequest' is null
        if (personUpdateRequest == null)
        {
            throw new ArgumentNullException();
        }

        // if 'personUpdateRequest.Name' is null (invalid)
        if (string.IsNullOrEmpty(personUpdateRequest.Name))
        {
            throw new ArgumentNullException();
        }

        // Validate other properties
        ValidationHelper.ModelValidation(personUpdateRequest);

        
        Person? person = await _personsRepository.GetPersonByID(personUpdateRequest.ID);
            
        // if 'personUpdateRequest.ID' is invalid (doesn't exist)
        if (person == null)
        {
            throw new InvalidPersonIDException("'Person' doesn't exist");
        }
        
        Person updatedPerson = await _personsRepository.UpdatePerson(person, personUpdateRequest.ToPerson());
        
        return updatedPerson.ToPersonResponse();
    }


    //
    public async Task<bool> DeletePerson(Guid? id)
    {
        // if 'id' is null
        if (id == null)
        {
            throw new ArgumentNullException();
        }

        Person? person = await _personsRepository.GetPersonByID(id.Value);
        
        // if 'id' doesn't exist in persons list, means it's invalid
        if (person == null) 
        {
            return false;
        }

        bool result = await _personsRepository.DeletePerson(person);
        
        return result;
    }
}