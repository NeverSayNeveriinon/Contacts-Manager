using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Core.DTO.CountryDTO;
using Core.ServiceContracts;
using Core.Services;
using Entities;
using FluentAssertions;
using Moq;

namespace xUnit_Tests.UnitTest.Services
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly ICountriesRepository _countriesRepository;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMoq;

        
        public CountriesServiceTest()
        {
            _countriesRepositoryMoq = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMoq.Object;

            _countriesService = new CountriesService(_countriesRepository,false);
        }

        #region AddCountry 

        // When 'CountryAddRequest' is Null, it should throws 'ArgumentNullException'
        [Fact]
        public async Task AddCountryNull()
        {
            
            // Arrange
            CountryAddRequest? countryAddRequest = null;
            
             // Way 1 
            // Act
            Func<Task> action = async () => await _countriesService.AddCountry(countryAddRequest);

            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();

             // OR Way 2
            //// Assert
            //await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            //{
            //    // Act
            //    await _countriesService.AddCountry(countryAddRequest);
            //});
        }

        // When 'CountryAddRequest.Name' is Null, it should throws 'ArgumentNullException'
        [Fact]
        public async Task AddCountry_NameNull()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                Name = null
            };
            
            // Way 1
            // Act
            Func<Task> action = async () => await _countriesService.AddCountry(countryAddRequest);

            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();

            // OR Way 2
            //// Assert
            //await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            //{
            //    // Act
            //    await _countriesService.AddCountry(countryAddRequest);
            //});
        }

        // When 'CountryAddRequest.Name' is Duplicate, it should throws 'ArgumentException'
        [Fact]
        public async Task AddCountry_NameDuplicate()
        {
            // Arrange
            CountryAddRequest countryAddRequest1 = new CountryAddRequest()
            {
                Name = "Norway"
            };

            CountryAddRequest countryAddRequest2 = new CountryAddRequest()
            {
                Name = "Norway"
            };
                
            Country? country1 = countryAddRequest1.ToCountry();
            Country? country2 = countryAddRequest2.ToCountry();

            // Mocking the Required Methods
            _countriesRepositoryMoq.SetupSequence(entity => entity.GetCountryByName("Norway"))
                                   .ReturnsAsync(null as Country)  // because first time there is no country with same name
                                   .ReturnsAsync(country2);   
            
            _countriesRepositoryMoq.SetupSequence(entity => entity.AddCountry(It.IsAny<Country>()))
                                   .ReturnsAsync(country1)   
                                   .ReturnsAsync(country2);   
            
             // Way 1 
            // Act
            Func<Task> action = async () =>
            {
                await _countriesService.AddCountry(countryAddRequest1);
                await _countriesService.AddCountry(countryAddRequest2);
            };

            // Assert
            await action.Should().ThrowAsync<ArgumentException>();


            // OR Way 2
            //// Assert
            //await Assert.ThrowsAsync<ArgumentException>(async () =>
            //{
            //    // Act
            //    await _countriesService.AddCountry(countryAddRequest1);
            //    await _countriesService.AddCountry(countryAddRequest2);
            //});
        }

        // When 'CountryAddRequest.Name' is valid then there is no problem, it should add the country object to countries list
        [Fact]
        public async Task AddCountry_Valid()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                Name = "Norway"
            };
            
            Country? country = countryAddRequest.ToCountry();
            CountryResponse? countryResponse_fromTest = country.ToCountryResponse();

             // Mocking the Required Methods
            _countriesRepositoryMoq.Setup(entity => entity.AddCountry(It.IsAny<Country>()))
                                   .ReturnsAsync(country);   
            
            // Act
            CountryResponse countryResponse_fromService = await _countriesService.AddCountry(countryAddRequest);
            countryResponse_fromTest.ID = countryResponse_fromService.ID;

            // Assert
             // Way 1
             countryResponse_fromService.ID.Should().NotBe(Guid.Empty);
             countryResponse_fromService.Should().BeEquivalentTo(countryResponse_fromTest);
             
             // Way 2
             //Assert.True(countryResponse.ID != Guid.Empty);
        }

        #endregion

        #region GetAllCountries

        // When no country is added in list(db), then the list should be empty
        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            // Arrange
            var emptyList = new List<Country>();
            
             // Mocking the Required Methods
            _countriesRepositoryMoq.Setup(entity => entity.GetAllCountries())
                                   .ReturnsAsync(emptyList);   
            
            // Act
            List<CountryResponse> countryResponseList = await _countriesService.GetAllCountries();

            // Assert
             // Way 1
            countryResponseList.Should().BeEmpty();

             // Way 2
            //Assert.Empty(countryResponseList);
        }

        // When there is no problem
        [Fact]
        public async Task GetAllCountries_Valid()
        {
            // Arrange
            Country country1 = new Country()
            {
                ID = Guid.NewGuid(),
                Name = "Italy"
            };
            Country country2 = new Country()
            {
                ID = Guid.NewGuid(),
                Name = "Finland"
            };
            List<Country> countryList = new List<Country>() { country1, country2 };
            List<CountryResponse> countryResponsesList_fromTest = countryList.Select(country => country.ToCountryResponse()).ToList();
            
              // Mocking the Required Methods
             _countriesRepositoryMoq.Setup(entity => entity.GetAllCountries())
                                    .ReturnsAsync(countryList);
            
            // Act
            List<CountryResponse> countryResponsesList_fromService = await _countriesService.GetAllCountries();

            // Assert
             // Way 1
            countryResponsesList_fromService.Should().Equal(countryResponsesList_fromTest);

             // Way 2
            // // check the number of items in two lists
            // // , this can prevents from having extra objects in service 'countries list' because that can't be found out in below 'foreach'
            //Assert.True(countryResponsesList_fromTest.Count == countryResponseList_fromService.Count);

            //// check every 'country object' in test exist in service 'country list' 
            //foreach (var countryItem in countryResponsesList_fromTest)
            //{
            //    Assert.Contains(countryItem, countryResponseList_fromService);
            //}
        }

        #endregion

        #region GetCountryByID

        // When 'country id' is null, it should returns null
        [Fact]
        public async Task GetCountryByID_NullID()
        {
            // Arrange
            Guid? id = null;
            
            // Act
            CountryResponse? countryResponse = await _countriesService.GetCountryByID(id);
            
            // Assert
             // Way 1
            countryResponse.Should().BeNull();

             // Way 2
            //Assert.Null(countryResponse);
        }

        // When 'country id' doesn't exist in 'countries list', it should return null
        [Fact]
        public async Task GetCountryByID_NotFoundID()
        {
            // Arrange
            Guid? id = Guid.NewGuid(); // a random 'guid' that didn't exist 
            
             // Mocking the Required Methods
            _countriesRepositoryMoq.Setup(entity => entity.GetCountryByID(It.IsAny<Guid>()))
                                   .ReturnsAsync(null as Country);
            
            // Act
            CountryResponse? countryResponse = await _countriesService.GetCountryByID(id);
            //List<CountryResponse> countryResponsesList = _countriesService.GetAllCountries();

            // Assert
             // Way 1
            countryResponse.Should().BeNull();

             // Way 2
            //Assert.Null(countryResponse);
            ////Assert.DoesNotContain(countryResponse, countryResponsesList);
        }
        
        // When there is no problem
        [Fact]
        public async Task GetCountryByID_Valid()
        {
            // Arrange
            Country country = new Country()
            {
                ID = Guid.NewGuid(),
                Name = "Austria"
            };
            CountryResponse countryResponse_fromTest = country.ToCountryResponse();

             // Mocking the Required Methods
            _countriesRepositoryMoq.Setup(entity => entity.GetCountryByID(It.IsAny<Guid>()))
                                   .ReturnsAsync(country);
            
            // Act
            CountryResponse? countryResponse_fromService = await _countriesService.GetCountryByID(countryResponse_fromTest.ID);
            //List<CountryResponse> countryResponsesList = _countriesService.GetAllCountries();

            // Assert
             // Way 1
            countryResponse_fromService.Should().BeEquivalentTo(countryResponse_fromTest);

             // Way 2
            // Assert.Equal(countryResponse_fromTest, countryResponse_fromService);
        }


        #endregion
    }
}

// ##Tips## //
// 1) The Steps to " 'Mock' the 'DbContext' layer for Testing purpose ":
//     1. Install Packages 'Moq' and 'EntityFrameworkCoreMock.Moq' and if needed 'Microsoft.EntityFrameworkCore'
//     2. Mock the 'DbContext' and required 'DbSet' in constructor of your test class
//     3. 
//
// 2) One thing that is recommended in 'Unit Test' testing, is to use 'Fluent Assertions' instead of built in 'Assert' and it needs to
//    Install package "FluentAssertions".
// 3) Difference between 'BeEquivalentTo' and 'Equal' between two collections, is that first one dosen't care about 'Order' of elements
//    but 'Equal' does, means [1,2,3] and [1,2,3] are 'Equal' and 'Equivalent, [1,2,3] and [3,1,2] are 'Equivalent' but not 'Equal'