using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;

namespace xUnit_Tests.IntegrationTest
{
    public class PersonsControllerIntegrationTest
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        
        public PersonsControllerIntegrationTest()
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();
        }
        
        #region Index

        // When we request to "~/persons/index", it should return '2xx' status code and proper 'table html tag'    
        [Fact]
        public async Task Index_Valid_ReturnIndexView()
        {
            // Arrange
            
            // Act
            HttpResponseMessage response = await _client.GetAsync("/Persons/Index");
            
            // Assert
             // any '2xx' status code
            response.Should().BeSuccessful(); 

             // check the 'response important html tag' to be not empty (here we'll check table)
            string responseBody = await response.Content.ReadAsStringAsync();
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(responseBody);
            var document = html.DocumentNode;

            document.QuerySelectorAll("table").Should().NotBeNullOrEmpty();
        }

        #endregion 
        
        // #region Create - Post
        //
        // // When there is some model errors, it should return a 'ViewResult' with two 'ViewData' objects ("AllCountries" and "ErrorsList")
        // [Fact]
        // public async Task CreatePost_WithModelErrors()
        // {
        //     // Arrange
        //     Country country1 = new Country()
        //     {
        //         ID = Guid.NewGuid(),
        //         Name = "Italy"
        //     };
        //     Country country2 = new Country()
        //     {
        //         ID = Guid.NewGuid(),
        //         Name = "Finland"
        //     };
        //     
        //     List<Country> countriesList = [country1, country2];
        //     List<CountryResponse> countryResponseList = countriesList.Select(person => person.ToCountryResponse()).ToList();
        //     IEnumerable<SelectListItem> countrySelectListItems = countryResponseList.Select(item => new SelectListItem(item.Name, item.ID.ToString()));
        //     
        //     PersonAddRequest personAddRequest = new PersonAddRequest()
        //     {
        //         Name = "aKevin",
        //         Email = "kevin.fake.mail@gmail.com",
        //         Address = "London, st 28",
        //         DateOfBirth = DateTime.Parse("1998/12/08"),
        //         Gender = GenderOptions.Male,
        //         CountryID = Guid.NewGuid(),
        //         RecieveNewsLetters = true
        //     };
        //     
        //      // Mocking the Required Methods
        //     _countriesServiceMock.Setup(entity => entity.GetAllCountries())
        //                          .ReturnsAsync(countryResponseList);
        //     
        //     // Act
        //     _personsController.ModelState.AddModelError("Name","'Person Name' can't be blank!!");
        //     List<string> errorsList = _personsController.ModelState.Values.SelectMany(el => el.Errors).Select(error => error.ErrorMessage).ToList();
        //     
        //     IActionResult result_fromController = await _personsController.Create(personAddRequest);
        //     
        //     // Assert
        //      // return type
        //     var result = result_fromController.Should().BeOfType<ViewResult>();
        //     var viewResult = result.Subject; // or "result.Which"
        //     
        //      // 'ViewData["AllCountries2"]' object
        //     viewResult.ViewData["AllCountries2"].Should().BeAssignableTo<IEnumerable<SelectListItem>>();
        //     viewResult.ViewData["AllCountries2"].Should().BeEquivalentTo(countrySelectListItems);
        //     
        //     // 'ViewData["ErrorsList"]' object
        //     viewResult.ViewData["ErrorsList"].Should().BeAssignableTo<IEnumerable<string>>();
        //     viewResult.ViewData["ErrorsList"].Should().BeEquivalentTo(errorsList);
        // }
        //
        // // When there is no model errors, it should return a 'RedirectToActionResult' to 'Index' action method
        // [Fact]
        // public async Task CreatePost_WithoutModelErrors()
        // {
        //     // Arrange
        //     PersonAddRequest personAddRequest = new PersonAddRequest()
        //     {
        //         Name = "Kevin",
        //         Email = "kevin.fake.mail@gmail.com",
        //         Address = "London, st 28",
        //         DateOfBirth = DateTime.Parse("1998/12/08"),
        //         Gender = GenderOptions.Male,
        //         CountryID = Guid.NewGuid(),
        //         RecieveNewsLetters = true
        //     };
        //     
        //     Person person = personAddRequest.ToPerson();
        //     PersonResponse personResponse = person.ToPersonResponse();
        //     
        //      // Mocking the Required Methods
        //     _personsServiceMock.Setup(entity => entity.AddPerson(It.IsAny<PersonAddRequest>()))
        //                        .ReturnsAsync(personResponse);
        //     
        //     // Act
        //     IActionResult result_fromController = await _personsController.Create(personAddRequest);
        //     
        //     // Assert
        //      // return type
        //     var result = result_fromController.Should().BeOfType<RedirectToActionResult>();
        //     var redirectToActionResult = result.Subject;
        //     
        //      // the action name to be redirected
        //     redirectToActionResult.ActionName.Should().Be("Index");
        // }
        //
        // #endregion
    }
}
