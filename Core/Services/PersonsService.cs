using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Core.DTO.PersonDTO;
using Core.Enums;
using Core.Exceptions;
using Core.Helpers;
using Core.ServiceContracts;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogTimings;

namespace Core.Services
{
    public class PersonsService : IPersonsService                           
    {
        private readonly IPersonsRepository _personsRepository;

        private readonly ILogger<PersonsService>? _logger;
        private readonly IDiagnosticContext? _diagnosticContext;

        
        public PersonsService(IPersonsRepository personsRepository, ILogger<PersonsService>? logger,
                              IDiagnosticContext? diagnosticContext, bool withMock = false)
        {
            _personsRepository = personsRepository;
            
            _logger = logger;
            _diagnosticContext = diagnosticContext;

            // Some Mock Data
            // if (withMock)
            // {
            //     _db.Persons.AddRange(new List<Person>
            //     {
            //         new Person ()
            //         {
            //             ID = Guid.Parse("59163933-774F-4ECB-BDBC-BD459EA56499"),
            //             Name = "Kevin",
            //             Email = "kevin.fake.mail@gmail.com",
            //             Address = "London, st 28",
            //             DateOfBirth = DateTime.Parse("1998/12/08"),
            //             Gender = GenderOptions.Male.ToString(),
            //             CountryID = Guid.Parse("B0A177B9-A3D3-40A1-A048-157BAB19B727"),
            //             RecieveNewsLetters = true
            //         },
            //         new Person ()
            //         {
            //             ID = Guid.Parse("59E8021F-5D37-4A98-83A7-01EA39914B02"),
            //             Name = "kevinJill",
            //             Email = "jill.fake.mail@gmail.com",
            //             Address = "Stockholm, st 58",
            //             DateOfBirth = DateTime.Parse("2004/11/11"),
            //             Gender = GenderOptions.Female.ToString(),
            //             CountryID = Guid.Parse("A3C51F30-6C58-42F4-89D2-7105BD971A58"),
            //             RecieveNewsLetters = false
            //         },
            //         new Person ()
            //         {
            //             ID = Guid.Parse("3DF46AD3-B1EF-4B7E-9A9B-174E25D3571C"),
            //             Name = "Nora",
            //             Email = "nora.fake.mail@gmail.com",
            //             Address = "London, st 49",
            //             DateOfBirth = DateTime.Parse("1996/02/01"),
            //             Gender = GenderOptions.Female.ToString(),
            //             CountryID = Guid.Parse("B0A177B9-A3D3-40A1-A048-157BAB19B727"),
            //             RecieveNewsLetters = true
            //         },
            //         new Person ()
            //         {
            //             ID = Guid.Parse("2917892D-5864-4356-AEEA-D1CE7C11155D"),
            //             Name = "Rachel",
            //             Email = "rachel.fake.mail@gmail.com",
            //             Address = "NewYork, st 89",
            //             DateOfBirth = DateTime.Parse("1995/04/04"),
            //             Gender = GenderOptions.Female.ToString(),
            //             CountryID = Guid.Parse("78F62C16-E164-46F8-B978-5D3B511B0A9D"),
            //             RecieveNewsLetters = true
            //         },
            //         new Person ()
            //         {
            //             ID = Guid.Parse("272D82B2-DFD8-4005-AAE5-84EC368A3D28"),
            //             Name = "Chandler",
            //             Email = "chandler.fake.mail@gmail.com",
            //             Address = "NewYork, st 88",
            //             DateOfBirth = DateTime.Parse("1992/09/09"),
            //             Gender = GenderOptions.Male.ToString(),
            //             CountryID = Guid.Parse("78F62C16-E164-46F8-B978-5D3B511B0A9D"),
            //             RecieveNewsLetters = false
            //         }
            //     });
            // }
        }


        // private Person UpdatePersonWithPersonUpdateRequest(Person person, PersonUpdateRequest personUpdateRequest)
        // {
        //     person.ID = personUpdateRequest.ID;
        //     person.Name = personUpdateRequest.Name;
        //     person.Email = personUpdateRequest.Email;
        //     person.DateOfBirth = personUpdateRequest.DateOfBirth;
        //     person.Gender = personUpdateRequest.Gender.ToString();
        //     person.CountryID = personUpdateRequest.CountryID;
        //     person.Address = personUpdateRequest.Address;
        //     person.RecieveNewsLetters = personUpdateRequest.RecieveNewsLetters;
        //
        //     return person;
        // }



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

            // Bad Way 1
            // // 'personAddRequest.Email' is Null //
            //if (string.IsNullOrEmpty(personAddRequest.Email))
            //{
            //    throw new ArgumentException("The 'Person Email' in 'PersonAddRequest' object can't be blank");
            //}

            // Good Way 2
            ValidationHelper.ModelValidation(personAddRequest);

             // 'personAddRequest.Name' is Duplicate //
            // Way 1
            if ( (await _personsRepository.GetFilteredPersons(person => person.Name == personAddRequest.Name))?.Count > 0)
            {
                throw new ArgumentException("The 'Person Name' is already exists");
            }


            // 'personAddRequest.Name' is valid and there is no problem //

            // Converting from 'PersonAddRequest' to 'Person'
            Person person = personAddRequest.ToPerson();
            person.ID = Guid.NewGuid();


            // Way 1: (Normal Usage)
            await _personsRepository.AddPerson(person);
            
            // Way 2: (Using Stored Procedure)
            //_Db.SP_AddPerson(person);


            // Converting from 'Person' to 'PersonResponse'
            PersonResponse personResponse = person.ToPersonResponse();

            return personResponse;
        }



        //
        public async Task<List<PersonResponse>> GetAllPersons()
        {
            _logger?.LogInformation("~~~ Started 'GetAllPersons' method of 'Persons' service ");

            
            // This will give us an error, because we are trying to use our own function in an LINQ to Entity expression and
            // that is not allowed. So the solution is we have to convert the 'DbSet' to 'List' first.
            // !!ERROR!! --> return _Db.Persons.Select(personItem => personItem.ToPersonResponse()).ToList();


            List<Person> persons = new List<Person>();
            
             // Way 1: (Normal Usage)
            // stat of "using block" of serilog timings
            using (Operation.Time("~~~ Time for GetAllPersons from Database"))
            {
                persons = await _personsRepository.GetAllPersons();
            } // end of "using block" of serilog timings

            // Way 2: (Using Stored Procedure)
            //List<Person> persons =  _Db.SP_GetAllPersons();

            
            _diagnosticContext?.Set("Returned Persons List", persons);
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

            // Way 1: (Normal Usage)
            Person? person = await _personsRepository.GetPersonByID(id.Value);

            // Way 2: (Using Stored Procedure)
            //Person? person = _Db.SP_GetPersonByID(id.Value);

            // if 'id' doesn't exist in 'persons list' 
            if (person == null)
            {
                return null;
            }

            // if there is no problem
            PersonResponse personResponse = person.ToPersonResponse();

            return personResponse;
        }



        //
        public async Task<List<PersonResponse>> GetSearchedPersons(string searchBy, string? searchString)
        {
            _logger?.LogInformation("~~~ Started 'GetSearchedPersons' method of 'Persons' service ");

            
            // List<PersonResponse> allPersons = await GetAllPersons();
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
                    
                    // !!Compiler Error!!InvalidOperationException: The LINQ expression 'DbSet<Person>()
                    //     .Where(p => p.Name.Contains(
                    //         value: __searchString_0,
                    //         comparisonType: OrdinalIgnoreCase))' could not be translated
                    // matchingPersons = (await _personsRepository.GetFilteredPersons(person => person.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ) )
                    //                                                                                     .Select(person => person.ToPersonResponse())
                    //                                                                                     .ToList();
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

                // case nameof(PersonResponse.Gender):
                // {
                //     matchingPersons = (await _personsRepository.GetFilteredPersons(person => 
                //                                   person.Gender.Contains(searchString)))
                //                                                .ToList();
                //     break;
                // }

                case nameof(PersonResponse.Address):
                {
                    matchingPersons = (await _personsRepository.GetFilteredPersons(person =>
                                                 person.Address!.Contains(searchString)))
                                                               .ToList();
                    break;
                }

                // case nameof(PersonResponse.Age):
                // {
                //     matchingPersons = (await _personsRepository.GetFilteredPersons(person => 
                //                                          double.Equals(person.ToPersonResponse().Age, Convert.ToDouble(searchString))))
                //                                                .ToList();
                //     break;
                // } 

                // case nameof(PersonResponse.CountryName):
                // {
                //     matchingPersons = (await _personsRepository.GetFilteredPersons(person =>  
                //           person.ToPersonResponse().CountryName.Contains(searchString)))
                //                                                .ToList();
                //     break;
                // }
                
                case nameof(PersonResponse.RecieveNewsLetters):
                {
                    matchingPersons = (await _personsRepository.GetFilteredPersons(person => 
                                                         string.Equals(person.RecieveNewsLetters.ToString(), searchString)))
                                                               .ToList();
                    break;
                }

                default:
                    break;
            }

            
            _diagnosticContext?.Set("Returned Persons List", matchingPersons);
            return matchingPersons.Select(person => person.ToPersonResponse()).ToList();
        }



        //
        public Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        { 
            _logger?.LogInformation("~~~ Started 'GetSortedPersons' method of 'Persons' service ");

            
            List<PersonResponse> sortedPersons = allPersons;

            switch(sortBy, sortOrder)
            {
                // Name
                case (nameof(PersonResponse.Name), SortOrderOptions.ASC):
                {
                    sortedPersons = allPersons.OrderBy( person => person.Name, StringComparer.OrdinalIgnoreCase).ToList();
                    // OR (Different Syntax)
                    //sortedPersons = [.. allPersons.OrderBy( person => person.Name, StringComparer.OrdinalIgnoreCase)];
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


                // RecieveNewsLetters
                case (nameof(PersonResponse.RecieveNewsLetters), SortOrderOptions.ASC):
                {
                    sortedPersons = allPersons.OrderBy( person => person.RecieveNewsLetters).ToList();
                    break;
                } 
                
                case (nameof(PersonResponse.RecieveNewsLetters), SortOrderOptions.DESC):
                {
                    sortedPersons = allPersons.OrderByDescending( person => person.RecieveNewsLetters).ToList();
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

            // Validation
            ValidationHelper.ModelValidation(personUpdateRequest);

            // 1.
            Person? person = await _personsRepository.GetPersonByID(personUpdateRequest.ID);
            // if 'personUpdateRequest.ID' is invalid (doesn't exist)
            if (person == null)
            {
                throw new InvalidPersonIDException("'Person' doesn't exist");
            }
            
            Person updatedPerson = await _personsRepository.UpdatePerson(person, personUpdateRequest.ToPerson());

            //// 2.
            //// if 'personUpdateRequest.ID' is invalid (doesn't exist)
            //if (!_Db.Persons.Any(person => person.ID == personUpdateRequest.ID))
            //{
            //    throw new ArgumentException("'Person' doesn't exist");
            //}
            //else
            //{
            //    Person person1 = _Db.Persons.ToList().Find(person => person.ID == personUpdateRequest.ID)!;
            //    // if 'personUpdateRequest.ID' is invalid(doesn't exist)
            //    Person updatedPerson1 = UpdatePersonWithPersonUpdateRequest(person1, personUpdateRequest);
            //    _Db.SaveChanges();
            //}

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
            
            // int numofRowsAffected = await _Db.SaveChangesAsync();
            //bool result = numofRowsAffected == 0 ? false : true;
            
            // const bool result = true;
            return result;
        }
    }
}


// ## Tips ##
// 1) *Important*: In 'GetAllPersons' method the following line gives us an error "_Db.Persons.Select(personItem => personItem.ToPersonResponse()).ToList()"
//                 because we are trying to use our own function in an LINQ to Entity expression and that is not allowed.
//                 So the solution is we have to convert the 'DbSet' to 'List' first (right after '_Db.Persons').
//
// 2) This "PersonsService" is injected inside a controller, but inside itself "PersonsDbContext" is injected, here we have a ver important subject,
//    we should keep in mind that "A Service with less lifetime (Singleton > Scoped > Transient) can't be injected into a Service with more lifetime"
//    The 'AddDbContext' in 'program.cs' file consider the service lifetime as 'Scoped' by default and the 'singleton lifetime of our 'services' like 'PersonsService'
//    will be a problem. the 'DbContext' with 'Scoped' lifetime is recommended, so it's better to define our 'services' with 'Scoped' lifetime too.
//
// 3) This is a table to show the dependencies of different Service Lifetimes: ('#' as yes and '-' as No)
//
//    1. 'if u read from up to down, use 'can have' after the word like this -> 'Scoped can have only Scoped and Singleton' '
//    2.'if u read from left to right, use 'can be injected into' after the word like this -> 'Scoped can be injected into only Transient and Scoped' '
//
//     'can have'\'can inject'     Transient         Scoped        Singleton
//
//    Transient                        #               #               #
//    Scoped                           -               #               #
//    Singleton                        -               -               #
//
//
// 4) The CRUD operations on 'DbContext' and how it's done:
//
//   #EF - Insert(Create)#
//      'INSERT - SQL'
//            INSERT INTO TableName(Column1, Column2) VALUES (Value1, Value2)
//       1.'Add - C# Code'
//            _dbContext.DbSetName.Add(entityObject); // Adds the given model object (entity object) to the DbSet.
//       2.'SaveChanges() - C# Code'
//             _dbContext.SaveChanges(); // Generates the SQL INSERT statement based on the model object data and executes the same at database server.
//
//   #EF - Query(Read)#
//      'SELECT - SQL'
//            SELECT Column1, Column2 FROM TableName
//            WHERE Column = value
//            ORDER BY Column
//      1.'LINQ Query - C# Code'
//            _dbContext.DbSetName
//                      .Where(item => item.Property == value) // Specifies condition for where clause (Generates the SQL WHERE)
//                      .OrderBy(item => item.Property) // Specifies condition for 'order by' clause (Generates the SQL ORDER BY)
//                      .Select(item => item); // Expression to be executed for each row (Generates the SQL SELECT)
//
//   #EF - Update#
//      'UPDATE - SQL'
//            UPDATE TableName SET Column1 = Value1, Column2 = Value2 WHERE PrimaryKey = Value
//      1.'Update - C# Code'
//            entityObject.Property = value; // Updates the specified value in the specific property of the model object (entity object) to the DbSet.
//      2.'SaveChanges() - C# Code'
//            _dbContext.SaveChanges(); // Generates the SQL UPDATE statement based on the model object data and executes the same at database server.
//
//   #EF - Delete#
//      'DELETE - SQL'
//             DELETE FROM TableName WHERE Condition
//      1.'Remove - C# Code'
//            _dbContext.DbSetName.Remove(entityObject); // Removes the specified model object (entity object) to the DbSet.
//      2.'SaveChanges() - C# Code'
//             _dbContext.SaveChanges(); // Generates the SQL DELETE statement based on the model object data and executes the same at database server.
//
//