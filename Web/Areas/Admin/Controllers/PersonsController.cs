using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Core.DTO.CountryDTO;
using Core.DTO.PersonDTO;
using Core.Enums;
using Core.ServiceContracts;


namespace Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class PersonsController : Controller
{
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;

    private readonly ILogger<PersonsController>? _logger;
        
    public PersonsController (IPersonsService personsService, ICountriesService countriesService,
                              ILogger<PersonsController>? logger)
    {
        _personsService = personsService;
        _countriesService = countriesService;

        _logger = logger;
    }


    // 'Persons' page //
    [Route("/persons/index")]
    public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.Name), 
                                            SortOrderOptions sortOrder = SortOrderOptions.ASC)
    {
        _logger?.LogInformation("~~~ Started 'Index' action method of 'Persons' controller ~~~");
        _logger?.LogDebug($"~~~ searchBy: {searchBy}, searchString: {searchString}, sortBy: {sortBy}, sortOrder: {sortOrder} ~~~");
            
        
        List<PersonResponse> matchedPersons = await _personsService.GetSearchedPersons(searchBy, searchString);
        List<PersonResponse> sortedPersons = await _personsService.GetSortedPersons(matchedPersons, sortBy, sortOrder);
        
        Dictionary<string, string> selectColumns = new Dictionary<string, string>()
        {
            { nameof(PersonResponse.Name), "Person Name" },
            { nameof(PersonResponse.Email), "Email" },
            { nameof(PersonResponse.DateOfBirth), "Birthday Date" },
            { nameof(PersonResponse.Address), "Address" },
            { nameof(PersonResponse.ReceiveNewsLetters), "Recieve News Letters" }
        };

        ViewBag.CurrentSortBy = sortBy;
        ViewBag.CurrentSortOrder = sortOrder.ToString();
        
        ViewBag.CurrentSearchBy = searchBy;
        ViewBag.CurrentSearchString = searchString ?? string.Empty;
        ViewBag.SelectColumns = selectColumns;


        return View(sortedPersons);
    }



    // 'Create' page //
    // Read
    [HttpGet("/persons/create")]
    public async Task<IActionResult> Create()
    {
        //// Send Country names to view
        List<CountryResponse> allCountries = await _countriesService.GetAllCountries();
        ViewBag.AllCountries = allCountries.Select(item =>
            new SelectListItem() 
            {
                Text = item.Name,
                Value = item.ID.ToString()
            }
        );
        //// Done

        return View();
    }

    // Write
    [HttpPost("/persons/create")]
    public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
    {
        if (!ModelState.IsValid)
        {
            //// Send Country names to view
            List<CountryResponse> allCountries = await _countriesService.GetAllCountries();

            ViewBag.AllCountries = allCountries.Select(item =>
                new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.ID.ToString()
                }
            );
            //// Done

            ViewBag.ErrorsList = ModelState.Values.SelectMany(el => el.Errors).Select(error => error.ErrorMessage).ToList();
            return View();
        }

        await _personsService.AddPerson(personAddRequest);

        return RedirectToAction("Index", "Persons");
    }



    // 'Edit' page //
    // Read
    [HttpGet("/persons/edit/{ID}")]
    public async Task<IActionResult> Edit(Guid ID)
    {
        PersonResponse? person = await _personsService.GetPersonByID(ID);

        if (person == null)
        {
            return RedirectToAction("Index", "Persons");
        }

        //// Send Country names to view
        List<CountryResponse> allCountries = await _countriesService.GetAllCountries();
        ViewBag.AllCountries = allCountries.Select(item =>
            new SelectListItem()
            {
                Text = item.Name,
                Value = item.ID.ToString()
            }
        );
        //// Done

        return View(person);
    }

    // Write
    [HttpPost("/persons/edit")]
    public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
    {
        PersonResponse? person = await _personsService.GetPersonByID(personUpdateRequest.ID);

        if (person == null)
        {
            return RedirectToAction("Index", "Persons");
        }

        if (!ModelState.IsValid)
        {
            List<CountryResponse> allCountries = await _countriesService.GetAllCountries();
            ViewBag.AllCountries = allCountries.Select(item =>
                new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.ID.ToString()
                }
            ); 

            ViewBag.ErrorsList = ModelState.Values.SelectMany(el => el.Errors).Select(error => error.ErrorMessage).ToList();
            return View(person);
        }

        await _personsService.UpdatePerson(personUpdateRequest);

        return RedirectToAction("Index", "Persons");
    }


        
    // 'Delete' page //
    // Read
    [HttpGet("/persons/delete/{ID}")]
    public async Task<IActionResult> Delete(Guid ID)
    {
        PersonResponse? person = await _personsService.GetPersonByID(ID);

        if (person == null)
        {
            return RedirectToAction("Index", "Persons");
        }

        return View(person);
    }

    // Write
    [HttpPost("/persons/delete")]
    // more correct version -> public IActionResult Delete(Guid ID) -> compilation error due to have same signature
    public async Task<IActionResult> Delete(PersonUpdateRequest personDeleteRequest)
    {
        PersonResponse? person = await _personsService.GetPersonByID(personDeleteRequest.ID);

        if (person == null)
        {
            return RedirectToAction("Index", "Persons");
        }

        await _personsService.DeletePerson(personDeleteRequest.ID);

        return RedirectToAction("Index", "Persons");
    }
}