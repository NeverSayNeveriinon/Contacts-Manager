using Core.Domain.Entities;
using Core.DTO.CountryDTO;
using Core.DTO.PersonDTO;
using Core.Enums;
using Core.ServiceContracts;
using Entities;
using Web.Areas.Admin.Controllers;

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;

namespace xUnit_Tests.UnitTest.Controllers
{
    public class PersonsControllerTest
    {
        private readonly PersonsController _personsController;
        
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        
        private readonly Mock<IPersonsService> _personsServiceMock;
        private readonly Mock<ICountriesService> _countriesServiceMock;
        
        public PersonsControllerTest()
        {
            //
            _personsServiceMock = new Mock<IPersonsService>();
            _personsService = _personsServiceMock.Object;
            
            //
            _countriesServiceMock = new Mock<ICountriesService>();
            _countriesService = _countriesServiceMock.Object;
            
            // 
            var loggerMock = new Mock<ILogger<PersonsController>>();
            
            //
            _personsController = new PersonsController(_personsService, _countriesService, loggerMock.Object);
        }
        
        #region Index

        // When 'searchBy', 'searchString', 'sortBy', 'sortOrder' are valid, it should returns proper 'View' with proper 'persons list'
        [Fact]
        public async Task Index_Valid()
        {
            // Arrange
            string searchBy = "Name";
            string? searchString = "Kevin";
            string sortBy = "Name";
            SortOrderOptions sortOrder = SortOrderOptions.ASC;
            
            Person person1 = new Person()
            {
                ID = Guid.NewGuid(),
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
                ID = Guid.NewGuid(),
                Name = "bKevinJill",
                Email = "jill.fake.mail@gmail.com",
                Address = "Liverpool, st 14",
                DateOfBirth = DateTime.Parse("2005/08/16"),
                Gender = GenderOptions.Female.ToString(),
                CountryID = Guid.NewGuid(),
                RecieveNewsLetters = false
            };

            List<Person> personsList = [person1, person2];
            List<PersonResponse> personResponseList = personsList.Select(person => person.ToPersonResponse()).ToList();
            
             // Mocking the Required Methods
            _personsServiceMock.Setup(entity => entity.GetSearchedPersons(It.IsAny<string>(),It.IsAny<string>()))
                               .ReturnsAsync(personResponseList.Where(person => person.Name!.Contains(searchString)).ToList());  // search the 'Kevin' by 'name'  
            
            _personsServiceMock.Setup(entity => entity.GetSortedPersons(It.IsAny<List<PersonResponse>>(),It.IsAny<string>()
                                                                        ,It.IsAny<SortOrderOptions>()))
                               .ReturnsAsync(personResponseList.OrderBy(person => person.Name).ToList()); // order by 'name' and with 'ASC'  
            
            // Act
            IActionResult result_fromController = await _personsController.Index(searchBy,searchString,sortBy,sortOrder);
            
            // Assert
             // Way 1
            var result = result_fromController.Should().BeOfType<ViewResult>();
            var viewResult = result.Subject; // or "result.Which"
            viewResult.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
            // Or "viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();"
            viewResult.Model.Should().BeEquivalentTo(personResponseList);

             // Way 2
            // var viewResult1 = Assert.IsType<ViewResult>(result_fromController);
            // Assert.IsAssignableFrom<IEnumerable<PersonResponse>>(viewResult1.Model);
            // Assert.Equal(viewResult1.Model,personResponseList);
        }

        #endregion 
        
        #region Create - Post

        // When there is some model errors, it should return a 'ViewResult' with two 'ViewData' objects ("AllCountries" and "ErrorsList")
        [Fact]
        public async Task CreatePost_WithModelErrors()
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
            
            List<Country> countriesList = [country1, country2];
            List<CountryResponse> countryResponseList = countriesList.Select(person => person.ToCountryResponse()).ToList();
            IEnumerable<SelectListItem> countrySelectListItems = countryResponseList.Select(item => new SelectListItem(item.Name, item.ID.ToString()));
            
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                Name = "aKevin",
                Email = "kevin.fake.mail@gmail.com",
                Address = "London, st 28",
                DateOfBirth = DateTime.Parse("1998/12/08"),
                Gender = GenderOptions.Male,
                CountryID = Guid.NewGuid(),
                RecieveNewsLetters = true
            };
            
             // Mocking the Required Methods
            _countriesServiceMock.Setup(entity => entity.GetAllCountries())
                                 .ReturnsAsync(countryResponseList);
            
            // Act
            _personsController.ModelState.AddModelError("Name","'Person Name' can't be blank!!");
            List<string> errorsList = _personsController.ModelState.Values.SelectMany(el => el.Errors).Select(error => error.ErrorMessage).ToList();
            
            IActionResult result_fromController = await _personsController.Create(personAddRequest);
            
            // Assert
             // return type
            var result = result_fromController.Should().BeOfType<ViewResult>();
            var viewResult = result.Subject; // or "result.Which"
            
             // 'ViewData["AllCountries2"]' object
            viewResult.ViewData["AllCountries2"].Should().BeAssignableTo<IEnumerable<SelectListItem>>();
            viewResult.ViewData["AllCountries2"].Should().BeEquivalentTo(countrySelectListItems);
            
            // 'ViewData["ErrorsList"]' object
            viewResult.ViewData["ErrorsList"].Should().BeAssignableTo<IEnumerable<string>>();
            viewResult.ViewData["ErrorsList"].Should().BeEquivalentTo(errorsList);
        }

        // When there is no model errors, it should return a 'RedirectToActionResult' to 'Index' action method
        [Fact]
        public async Task CreatePost_WithoutModelErrors()
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
            PersonResponse personResponse = person.ToPersonResponse();
            
             // Mocking the Required Methods
            _personsServiceMock.Setup(entity => entity.AddPerson(It.IsAny<PersonAddRequest>()))
                               .ReturnsAsync(personResponse);
            
            // Act
            IActionResult result_fromController = await _personsController.Create(personAddRequest);
            
            // Assert
             // return type
            var result = result_fromController.Should().BeOfType<RedirectToActionResult>();
            var redirectToActionResult = result.Subject;
            
             // the action name to be redirected
            redirectToActionResult.ActionName.Should().Be("Index");
        }
        
        #endregion
    }
}
