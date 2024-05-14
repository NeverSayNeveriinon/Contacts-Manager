using Core.DTO.PersonDTO;
using Core.Enums;
using Core.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        private readonly ILogger<HomeController>? _logger;

        public HomeController(IPersonsService personsService, ICountriesService countriesService,
            ILogger<HomeController>? logger)
        {
            _personsService = personsService;
            _countriesService = countriesService;

            _logger = logger;
        }


        // 'Home' page //
        [Route("/")]
        public async Task<IActionResult> Index(string searchBy, string? searchString,
            string sortBy = nameof(PersonResponse.Name),
            SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            _logger?.LogInformation("~~~ Started 'Index' action method of 'Persons' controller ");
            _logger?.LogDebug($"~~~ searchBy: {searchBy}, searchString: {searchString}, sortBy: {sortBy}, sortOrder: {sortOrder}");

            List<PersonResponse> matchedPersons = await _personsService.GetSearchedPersons(searchBy, searchString);


            Dictionary<string, string> selectColumns = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.Name), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DateOfBirth), "Birthday Date" },
                { nameof(PersonResponse.Address), "Address" },
                { nameof(PersonResponse.RecieveNewsLetters), "Recieve News Letters" }
            };


            List<PersonResponse> sortedPersons =
                await _personsService.GetSortedPersons(matchedPersons, sortBy, sortOrder);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();

            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString ?? string.Empty;
            ViewBag.SelectColumns = selectColumns;


            return View(sortedPersons);
        }
    }
}