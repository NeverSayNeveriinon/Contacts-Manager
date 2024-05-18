﻿using Entities;
using System.Linq.Expressions;
using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Core.DTO.PersonDTO;
using Core.Enums;
using Core.ServiceContracts;
using Core.Services;
using Xunit.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;

namespace xUnit_Tests.UnitTest.Services
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;
        
        private readonly IPersonsRepository _personsRepository;
        private readonly Mock<IPersonsRepository> _personsRepositoryMock;
        
        private readonly ITestOutputHelper _testOutputHelper;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            //
            _personsRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personsRepositoryMock.Object;
            
            //
            var diagnosticContextMock = new Mock<IDiagnosticContext>();
            var loggerMock = new Mock<ILogger<PersonsService>>();
            
            _personsService = new PersonsService(_personsRepository, loggerMock.Object, diagnosticContextMock.Object, false);

            _testOutputHelper = testOutputHelper;
        }

        private void PrintPersonCompareTest(PersonResponse? personResponse_fromTest, PersonResponse? personResponse_fromService)
        {
            _testOutputHelper.WriteLine("Expected:");
            _testOutputHelper.WriteLine(personResponse_fromTest?.ToString());

            _testOutputHelper.WriteLine("Actual:");
            _testOutputHelper.WriteLine(personResponse_fromService?.ToString());
        }

        private void PrintListPersonCompareTest(List<PersonResponse> personResponsesList_fromTest, List<PersonResponse>? personResponseList_fromService)
        {
            _testOutputHelper.WriteLine("Expected:");
            foreach (var item in personResponsesList_fromTest)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            _testOutputHelper.WriteLine("Actual:");
            if (personResponseList_fromService != null)
            {
                foreach (var item in personResponseList_fromService)
                {
                    _testOutputHelper.WriteLine(item.ToString());
                }
            }
        }


        #region AddPerson

        // When 'PersonAddRequest' is Null, it should throws 'ArgumentNullException'
        [Fact]
        public async Task AddPerson_Null()
        {
            // Arrange
            PersonAddRequest? personAddRequest = null;
            
             // Way 1
            // Act
            Func<Task> action = async () =>  await _personsService.AddPerson(personAddRequest);
            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();

             // Way 2
            //// Assert
            //await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            //{
            //    // Act
            //    await _personsService.AddPerson(personAddRequest);
            //});
        }


        // When 'PersonAddRequest.Name' is Null means invalid, email not entered and etc.(here just name), it should throws 'ArgumentNullException'
        [Fact]
        public async Task AddPerson_NameNull()
        {
            // Arrange
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                Name = null,
                Email = "fake.mail@gmail.com",
                Address = "Japan, st 90",
                DateOfBirth = DateTime.Parse("2001/01/01"),
                Gender = GenderOptions.Female,
                CountryID = Guid.NewGuid(),
                RecieveNewsLetters = true
            };

            // Way 1
            // Act
            Func<Task> action = async () => await _personsService.AddPerson(personAddRequest);
            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();

            // Way 2
            //// Assert
            //await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            //{
            //    // Act
            //    await _personsService.AddPerson(personAddRequest);
            //});
        }


        // When 'PersonAddRequest.Name' is Duplicate, it should throws 'ArgumentException'
        [Fact]
        public async Task AddPerson_NameDuplicate()
        {
            // Arrange
            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                Name = "Nora",
                Email = "nora.fake.mail@gmail.com",
                Address = "London, st 20",
                DateOfBirth = DateTime.Parse("2002/02/02"),
                Gender = GenderOptions.Female,
                CountryID = Guid.NewGuid(),
                RecieveNewsLetters = true
            };

            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                Name = "Nora",
                Email = "fake.nora.fake.mail@gmail.com",
                Address = "Manchester, st 32",
                DateOfBirth = DateTime.Parse("2001/11/1"),
                Gender = GenderOptions.Female,
                CountryID = Guid.NewGuid(),
                RecieveNewsLetters = false
            };

            Person person1 = personAddRequest1.ToPerson();
            Person person2 = personAddRequest2.ToPerson();
            
             // Mocking the Required Methods
            _personsRepositoryMock.SetupSequence(entity => entity.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                                  .ReturnsAsync(new List<Person>())  // because first time there is no person with same name
                                  .ReturnsAsync( [person2] );   

            _personsRepositoryMock.SetupSequence(entity => entity.AddPerson(It.IsAny<Person>()))
                                  .ReturnsAsync(person1)   
                                  .ReturnsAsync(person2);   
            
            // Way 1
            // Act
            Func<Task> action = async () =>
            {
                await _personsService.AddPerson(personAddRequest1);
                await _personsService.AddPerson(personAddRequest2);
            };
            // Assert
            await action.Should().ThrowAsync<ArgumentException>();

             // Way 2
            //// Assert
            //await Assert.ThrowsAsync<ArgumentException>(async () =>
            //{
            //    // Act
            //    await _personsService.AddPerson(personAddRequest1);
            //    await _personsService.AddPerson(personAddRequest2);
            //});
        }


        // When 'PersonAddRequest.Name' is valid then there is no problem, it should add the person object to persons list
        [Fact]
        public async Task AddPerson_Valid()
        {
            // Arrange
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                Name = "Kevin",
                Email = "kevin.fake.mail@gmail.com",
                Address = "London, st 28",
                DateOfBirth = DateTime.Parse("1998/12/08"),
                Gender = GenderOptions.Male,
                CountryID = Guid.NewGuid(),
                RecieveNewsLetters = true
            };
            
            Person person = personAddRequest.ToPerson();
            PersonResponse personResponse_fromTest = person.ToPersonResponse();
            
             // Mocking the Required Methods
            _personsRepositoryMock.Setup(entity => entity.AddPerson(It.IsAny<Person>()))
                                  .ReturnsAsync(person);   
            
            // Act
            PersonResponse personResponse_fromService = await _personsService.AddPerson(personAddRequest);
            personResponse_fromTest.ID = personResponse_fromService.ID;
            
            // Assert
             // Way 1
            personResponse_fromService.ID.Should().NotBe(Guid.Empty);
            personResponse_fromService.Should().Be(personResponse_fromTest);

             // Way2
            //Assert.True(personResponse.ID != Guid.Empty);
            //Assert.Contains(personResponse, personResponsesList);

        }

        #endregion

        #region GetAllPersons

        // When no person is added in list(db), then the list should be empty
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            // Arrange
            var emptyList = new List<Person>();
            
             // Mocking the Required Methods
            _personsRepositoryMock.Setup(entity => entity.GetAllPersons())
                                  .ReturnsAsync(emptyList);
            
            // Act
            List<PersonResponse> personResponseList = await _personsService.GetAllPersons();

            // Assert
             // Way 1
            personResponseList.Should().BeEmpty();

             // Way 2
            //Assert.Empty(personResponseList);

        }

        // When there is no problem
        [Fact]
        public async Task GetAllPersons_Valid()
        {
            // Arrange
            Person person1 = new Person()
            {
                Name = "Kevin",
                Email = "kevin.fake.mail@gmail.com",
                Address = "London, st 28",
                DateOfBirth = DateTime.Parse("1998/12/08"),
                Gender = GenderOptions.Male.ToString(),
                CountryID = Guid.NewGuid(),
                RecieveNewsLetters = true
            };
            Person person2 = new Person()
            {
                Name = "Jill",
                Email = "jill.fake.mail@gmail.com",
                Address = "Liverpool, st 14",
                DateOfBirth = DateTime.Parse("2005/08/16"),
                Gender = GenderOptions.Female.ToString(),
                CountryID = Guid.NewGuid(),
                RecieveNewsLetters = false
            };
            List<Person> personRequestsList = new List<Person>() { person1, person2 };
            
            List<Person> personList = new List<Person>() { person1, person2 };
            List<PersonResponse> personResponsesList_fromTest = personList.Select(person => person.ToPersonResponse()).ToList();
            
             // Mocking the Required Methods
            _personsRepositoryMock.Setup(entity => entity.GetAllPersons())
                                  .ReturnsAsync(personList);
            
            // Act
            List<PersonResponse> personResponseList_fromService = await _personsService.GetAllPersons();


            // Assert
             // Way 1
            personResponseList_fromService.Should().BeEquivalentTo(personResponsesList_fromTest);

             // Way 2
            //// check the number of items in two lists
            //// , this can prevents from having extra objects in service (personResponseList_fromService) because that can't be found out in below 'foreach'
            //Assert.True(personResponsesList_fromTest.Count == personResponseList_fromService.Count);

            //// check every 'person object' in test exist in service 'person list' 
            //foreach (var personItem in personResponsesList_fromTest)
            //{
            //    Assert.Contains(personItem, personResponseList_fromService);
            //}


            // Print Test
            PrintListPersonCompareTest(personResponsesList_fromTest, personResponseList_fromService);
        }

        #endregion

        #region GetPersonByID

        // When 'person id' is null, it should returns null
        [Fact]
        public async Task GetPersonByID_NullID()
        {
            // Arrange
            Guid? id = null;

            // Act
            PersonResponse? personResponse = await _personsService.GetPersonByID(id);

            // Assert
             // Way 1
            personResponse.Should().BeNull();

             // Way 2
            //Assert.Null(personResponse);

        }

        // When 'person id' doesn't exist in 'persons list', it should return null
        [Fact]
        public async Task GetPersonByID_NotFoundID()
        {
            // Arrange
            Guid? id = Guid.NewGuid(); // a random 'guid' that didn't exist 
            
             // Mocking the Required Methods
            _personsRepositoryMock.Setup(entity => entity.GetPersonByID(It.IsAny<Guid>()))
                                                         .ReturnsAsync(null as Person);
            // Act
            PersonResponse? personResponse = await _personsService.GetPersonByID(id);
            //List<PersonResponse> personResponsesList = _personsService.GetAllPersons();

            // Assert
             // Way 1
            personResponse.Should().BeNull();

             // Way 2
            //Assert.Null(personResponse);
            //Assert.DoesNotContain(personResponse, personResponsesList);
        }

        // When there is no problem
        [Fact]
        public async Task GetPersonByID_Valid()
        {
            // Arrange
            Person person = new Person()
            {
                ID = Guid.NewGuid(),
                Name = "Jill",
                Email = "jill.fake.mail@gmail.com",
                Address = "North London, st 26",
                DateOfBirth = DateTime.Parse("2006/01/17"),
                Gender = GenderOptions.Female.ToString(),
                CountryID = Guid.NewGuid(),
                RecieveNewsLetters = false
            };
            PersonResponse personResponse_fromTest = person.ToPersonResponse();

            _personsRepositoryMock.Setup(entity => entity.GetPersonByID(It.IsAny<Guid>()))
                                  .ReturnsAsync(person);
            
            // Act
            PersonResponse? personResponse_fromService = await _personsService.GetPersonByID(personResponse_fromTest.ID);

            // Assert
             // Way 1
            personResponse_fromService.Should().BeEquivalentTo(personResponse_fromTest);

             // Way 2
            //Assert.Equal(personResponse_fromTest, personResponse_fromService);

            // Print Test
            PrintPersonCompareTest(personResponse_fromTest, personResponse_fromService);
        }


        #endregion

        #region GetSearchedPersons

        // When 'SearchString' is Empty and 'searchBy' is any person field (here person 'Name'), it should returns full 'persons list'
        [Fact]
        public async Task GetSearchedPersons_SearchStringEmpty()
        {
            // Arrange
            Person person1 = new Person()
            {
                ID = Guid.NewGuid(),
                Name = "Kevin",
                Email = "kevin.fake.mail@gmail.com",
                Address = "London, st 28",
                DateOfBirth = DateTime.Parse("1998/12/08"),
                Gender = GenderOptions.Male.ToString(),
                CountryID = Guid.NewGuid(),
                RecieveNewsLetters = true
            };
            Person person2 = new Person()
            {
                ID = Guid.NewGuid(),
                Name = "Jill",
                Email = "jill.fake.mail@gmail.com",
                Address = "Liverpool, st 14",
                DateOfBirth = DateTime.Parse("2005/08/16"),
                Gender = GenderOptions.Female.ToString(),
                CountryID = Guid.NewGuid(),
                RecieveNewsLetters = false
            };
            
            List<Person> personsList = new List<Person>() { person1, person2 };
            List<PersonResponse> personResponsesList_fromTest = personsList.Select(person => person.ToPersonResponse()).ToList();
            
             // Mocking the Required Methods
            _personsRepositoryMock.Setup(entity => entity.GetAllPersons())
                                  .ReturnsAsync(personsList);
            
            // Act

            // this should returns all persons existing in 'persons list'
            List<PersonResponse> personResponseList_fromService = await _personsService.GetSearchedPersons(nameof(Person.Name), "");


            // Assert
             // Way 1
            personResponseList_fromService.Should().BeEquivalentTo(personResponsesList_fromTest);

             // Way 2
            //// check the number of items in two lists
            //// , this can prevents from having extra objects in service (personResponseList_fromService) because that can't be found out in below 'foreach'
            //Assert.True(personResponsesList_fromTest.Count == personResponseList_fromService.Count);

            //// check every 'person object' in test exist in service 'person list' 
            //foreach (var personItem in personResponsesList_fromTest)
            //{
            //    Assert.Contains(personItem, personResponseList_fromService);
            //}


            // Print Test
            PrintListPersonCompareTest(personResponsesList_fromTest, personResponseList_fromService);
        }

        // When there is no problem, it should returns the matched person objects based on searchBy and searchString 
        [Fact]
        public async Task GetSearchedPersons_Valid()
        {
            // Arrange
            Person person1 = new Person()
            {
                ID = Guid.NewGuid(),
                Name = "Kevin",
                Email = "kevin.fake.mail@gmail.com",
                Address = "London, st 28",
                DateOfBirth = DateTime.Parse("1998/12/08"),
                Gender = GenderOptions.Male.ToString(),
                CountryID = Guid.NewGuid(),
                RecieveNewsLetters = true
            };
            Person person2 = new Person()
            {
                ID = Guid.NewGuid(),
                Name = "Jill",
                Email = "jill.fake.mail@gmail.com",
                Address = "Liverpool, st 14",
                DateOfBirth = DateTime.Parse("2005/08/16"),
                Gender = GenderOptions.Female.ToString(),
                CountryID = Guid.NewGuid(),
                RecieveNewsLetters = false
            };
            
            List<Person> personsList = new List<Person>() { person1, person2 };
            List<PersonResponse> personResponsesList_fromTest = personsList.Select(person => person.ToPersonResponse()).ToList();
            
             // Mocking the Required Methods
            _personsRepositoryMock.Setup(entity => entity.GetFilteredPersons(It.IsAny<Expression<Func<Person,bool>>>()))
                                                         .ReturnsAsync(personsList);
            
            // Act

            // this should returns all matching persons in 'persons list'
            List<PersonResponse> personResponseList_fromService = await _personsService.GetSearchedPersons(nameof(Person.Email), "fake");


            // Assert
             // Way 1
            personResponseList_fromService.Should().OnlyContain(personItem => personItem.Email!.Contains("fake", StringComparison.OrdinalIgnoreCase));
            personResponseList_fromService.Should().BeEquivalentTo(personResponsesList_fromTest); // because the both persons in 'personsList' have 'fake' in email 
            
            // !!Compiler ERROR!! 'OnlyContain' has parameter of 'Expression<Func<T, bool>> predicate' and not 'Func<T, bool> predicate',
            // and this can't be done because 'Expression' doesn't accept a lambda with body expression.
            //personResponseList_fromService.Should().OnlyContain(personItem => 
            //{
            //    if (personItem != null && personItem.Email != null)
            //        return personItem.Email.Contains("fake", StringComparison.OrdinalIgnoreCase);
            //    return false;
            //});

            
             // Way 2
            //// check the number of items in two lists
            //// , this can prevents from having extra objects in service (personResponseList_fromService) because that can't be found out in below 'foreach'
            //Assert.True(personResponsesList_fromTest.Count == personResponseList_fromService.Count);

            //// check every 'person object' in test exist in service 'person list' 
            //foreach (var personItem in personResponseList_fromService)
            //{
            //    if (personItem != null && personItem.Email != null)
            //    {
            //        if (personItem.Email.Contains("fake", StringComparison.OrdinalIgnoreCase))
            //        {
            //            Assert.Contains(personItem, personResponseList_fromService);
            //        }
            //    }
            //}
            
            
            // Print Test
            PrintListPersonCompareTest(personResponsesList_fromTest, personResponseList_fromService);
        }

        #endregion  

        #region GetSortedPersons

        // When there is no problem,in scenario where 'sortBy' is 'Person.Name' and 'sortOrder' is 'DSC' it should returns
        // the list of sorted person objects based on 'Person.Name' and in 'DSC'
        [Fact]
        public async Task GetSortedPersons_Valid()
        {
            // Arrange
            Person person1 = new Person()
            {
                Name = "aKevin",
                Email = "kevin.fake.mail@gmail.com",
                Address = "London, st 28",
                DateOfBirth = DateTime.Parse("1998/12/08"),
                Gender = GenderOptions.Male.ToString(),
                CountryID = Guid.NewGuid(),
                RecieveNewsLetters = true
            };
            Person person2 = new Person()
            {
                Name = "cJill",
                Email = "jill.fake.mail@gmail.com",
                Address = "Liverpool, st 14",
                DateOfBirth = DateTime.Parse("2005/08/16"),
                Gender = GenderOptions.Female.ToString(),
                CountryID = Guid.NewGuid(),
                RecieveNewsLetters = false
            };
            Person person3 = new Person()
            {
                Name = "bNora",
                Email = "nora.fake.mail@gmail.com",
                Address = "Newcastle, st 44",
                DateOfBirth = DateTime.Parse("1996/03/26"),
                Gender = GenderOptions.Female.ToString(),
                CountryID = Guid.NewGuid(),
                RecieveNewsLetters = false
            };
            
            List<Person> personsList = new List<Person>() { person1, person2, person3 };
            List<PersonResponse> personResponsesList_fromTest = personsList.Select(person => person.ToPersonResponse()).ToList();
            
            // Act
            List<PersonResponse> personResponsesList_fromTestOrdered = personResponsesList_fromTest.OrderByDescending(perosn => perosn.Name).ToList();
            
            // this should return all persons but in sorted version
            List<PersonResponse> personResponseList_fromService = await _personsService.GetSortedPersons(personResponsesList_fromTest, nameof(Person.Name), SortOrderOptions.DESC);

            // Assert
             // Way 1
            personResponseList_fromService.Should().Equal(personResponsesList_fromTestOrdered); // 'Equal' because the order of elements are important
            
             // Way 2
            //// check the number of items in two lists
            //Assert.True(personResponsesList_fromTest.Count == personResponseList_fromService.Count);

            //// check every 'person object' in test exist in service 'person list' in right order (that's because we'll use 'for')
            //for (int i = 0; i < personResponsesList_fromTest.Count; i++)
            //{
            //    Assert.Equal(personResponsesList_fromTest[i], personResponseList_fromService[i]);
            //}
            
            // Print Test
            
            PrintListPersonCompareTest(personResponsesList_fromTestOrdered, personResponseList_fromService);
        }

        #endregion

        #region UpdatePerson

        // When 'PersonUpdateRequest' object is Null, it should throws 'ArgumentNullException'
        [Fact]
        public async Task UpdatePerson_Null()
        {
            // Arrange
            PersonUpdateRequest? personUpdateRequest = null;

             // Way 1
            // Act
            Func<Task> action = async () => await _personsService.UpdatePerson(personUpdateRequest);
            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();

             // Way 2
            //// Assert
            //await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            //{
            //    // Act
            //    await _personsService.UpdatePerson(personUpdateRequest);
            //});
        }


        // When 'PersonUpdateRequest.Name' is Null means invalid, email not entered and etc.(here just name), it should throws 'ArgumentNullException'
        [Fact]
        public async Task UpdatePerson_NameNull()
        {
            // Arrange
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                Name = "Nora",
                Email = "nora.fake.mail@gmail.com",
                Address = "Japan, st 90",
                DateOfBirth = DateTime.Parse("2001/01/01"),
                Gender = GenderOptions.Female,
                CountryID = Guid.NewGuid(),
                RecieveNewsLetters = true
            };
            PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest()
            {
                ID = personResponse.ID!.Value,
                Name = null, // the updated value
                Email = "nora.fake.mail@gmail.com",
                Address = "Japan, st 90",
                DateOfBirth = DateTime.Parse("2001/01/01"),
                Gender = GenderOptions.Female,
                CountryID = personResponse.CountryID,
                RecieveNewsLetters = true
            };

             // Way 1
            // Act
            Func<Task> action = async () => await _personsService.UpdatePerson(personUpdateRequest);
            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();

             // Way 2
            //// Assert
            //await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            //{
            //    // Act
            //    await _personsService.UpdatePerson(personUpdateRequest);
            //});
        }


        // When 'PersonUpdateRequest.ID' is invalid (doesn't exist), it should throws 'ArgumentException'
        [Fact]
        public async Task UpdatePerson_IdInvalid()
        {
            // Arrange
            Person person = new Person()
            {
                ID = Guid.NewGuid(),
                Name = "Nora",
                Email = "fake.mail@gmail.com",
                Address = "USA, st 90",
                DateOfBirth = DateTime.Parse("2001/01/01"),
                Gender = GenderOptions.Female.ToString(),
                CountryID = Guid.NewGuid(),
                RecieveNewsLetters = true
            };

            PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest()
            {
                ID = Guid.NewGuid(), // this generated ID definitely doesn't exist in persons list
                Name = "Nora not any more", // the updated value
                Email = "fake.mail@gmail.com",
                Address = "USA, st 90",
                DateOfBirth = DateTime.Parse("2001/01/01"),
                Gender = GenderOptions.Female,
                CountryID = person.CountryID,
                RecieveNewsLetters = true
            };
            
            Person updatedPerson = personUpdateRequest.ToPerson();
            
             // Mocking the Required Methods
            _personsRepositoryMock.Setup(entity => entity.GetPersonByID(It.IsAny<Guid>()))
                                  .ReturnsAsync(null as Person);

             // Way 1
            // Act
            Func<Task> action = async () => await _personsService.UpdatePerson(personUpdateRequest);
            // Assert
            await action.Should().ThrowAsync<ArgumentException>();

             // Way 2
            //// Assert
            //await Assert.ThrowsAsync<ArgumentException>(async () =>
            //{
            //    // Act
            //    await _personsService.UpdatePerson(personUpdateRequest);
            //});
        }


        // When 'PersonUpdateRequest.Name' is valid then there is no problem, it should add the person object to persons list
        [Fact]
        public async Task UpdatePerson_Valid()
        {
            // Arrange
            Person person = new Person()
            {
                ID = Guid.NewGuid(),
                Name = "Kevin",
                Email = "kevin.fake.mail@gmail.com",
                Address = "London, st 28",
                DateOfBirth = DateTime.Parse("1998/12/08"),
                Gender = GenderOptions.Male.ToString(),
                CountryID = Guid.NewGuid(),
                RecieveNewsLetters = true
            };

            PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest()
            {
                ID = person.ID.Value,
                Name = "Kevin not anymore", // the updated value
                Email = "kevin.fake.mail@gmail.com",
                Address = "London, st 28",
                DateOfBirth = DateTime.Parse("1998/12/08"),
                Gender = GenderOptions.Male,
                CountryID = person.CountryID,
                RecieveNewsLetters = true 
            };

            Person updatedPerson =  personUpdateRequest.ToPerson();
            
             // Mocking the Required Methods
            _personsRepositoryMock.Setup(entity => entity.GetPersonByID(It.IsAny<Guid>())) 
                                  .ReturnsAsync(person);    

            _personsRepositoryMock.Setup(entity => entity.UpdatePerson(It.IsAny<Person>(),It.IsAny<Person>()))
                                  .ReturnsAsync(updatedPerson);                                               
            
            // Act
            PersonResponse? personResponse_fromTest = updatedPerson.ToPersonResponse();
            PersonResponse personResponse_fromService = await _personsService.UpdatePerson(personUpdateRequest);
            
            // Assert
             // Way 1
            personResponse_fromService.Should().BeEquivalentTo(personResponse_fromTest);

             // Way 2
            //Assert.Equal(personResponse_fromTest, personResponse_fromService);
        }

        #endregion

        #region DeletePerson

        // When 'id' of person is null, it should throw 'ArgumentNullException'
        [Fact]
        public async Task DeletePerson_IdNull()
        {
            // Arrange
            Guid? id = null;

             // Way 1
            // Act
            Func<Task> action = async () => await _personsService.DeletePerson(id);
            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();

             // Way 2
            //// Assert
            //await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            //{
            //    // Act
            //    await _personsService.DeletePerson(id);
            //});
        }


        // When person object with specified 'id' doesn't exist in 'persons list', it should returns 'false' 
        [Fact]
        public async Task DeletePerson_IdInavlid()
        {
            // Arrange
            Person person1 = new Person()
            {
                ID = Guid.NewGuid(),
                Name = "Kevin",
                Email = "kevin.fake.mail@gmail.com",
                Address = "London, st 28",
                DateOfBirth = DateTime.Parse("1998/12/08"),
                Gender = GenderOptions.Male.ToString(),
                CountryID = Guid.NewGuid(),
                RecieveNewsLetters = true
            };

             // Mocking the Required Methods
            _personsRepositoryMock.Setup(entity => entity.GetPersonByID(It.IsAny<Guid>()))
                                  .ReturnsAsync(null as Person);
            
            // Act
            Guid? fakeId = Guid.NewGuid(); // this new 'guid' definitely doesn't exist and is invalid
            bool isDeleted_fromService = await _personsService.DeletePerson(fakeId);

            // Assert
             // Way 1
            isDeleted_fromService.Should().BeFalse();

            // Way 2
            //Assert.True(isDeleted_fromService == false);
        }


        // When person object with specified 'id' is valid, it should returns 'true' 
        [Fact]
        public async Task DeletePerson_Valid()
        {
            // Arrange
            Person person = new Person()
            {
                ID = Guid.NewGuid(),
                Name = "Kevin",
                Email = "kevin.fake.mail@gmail.com",
                Address = "London, st 28",
                DateOfBirth = DateTime.Parse("1998/12/08"),
                Gender = GenderOptions.Male.ToString(),
                CountryID = Guid.NewGuid(),
                RecieveNewsLetters = true
            };

             // Mocking the Required Methods
            _personsRepositoryMock.Setup(entity => entity.GetPersonByID(It.IsAny<Guid>()))
                                  .ReturnsAsync(person);
            
            _personsRepositoryMock.Setup(entity => entity.DeletePerson(It.IsAny<Person>()))
                                  .ReturnsAsync(true);
            
             // Another Way
            // _personsRepositoryMock.Setup(entity => entity.GetPersonByID(person.ID.Value))
            //     .ReturnsAsync(person);
            //
            // _personsRepositoryMock.Setup(entity => entity.DeletePerson(person))
            //     .ReturnsAsync(true);
            
            // Act
            
            bool isDeleted_fromService = await _personsService.DeletePerson(person.ID);

            // Assert
             // Way 1
            isDeleted_fromService.Should().BeTrue();

             // Way 2
            //Assert.True(isDeleted_fromService == true);
        }

        #endregion
    }
}
