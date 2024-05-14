using Core.DTO.PersonDTO;
using Core.Enums;

namespace Core.ServiceContracts
{
    public interface IPersonsService
    {
        /// <summary>
        /// Add a person object to persons list
        /// </summary>
        /// <param name="personAddRequest">Person object to be added</param>
        /// <return>Returns the new person object (with ID and other attributes) after adding it</return>
        public Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);


        /// <summary>
        /// Retrieve all Person objects from persons list
        /// </summary>
        /// <returns>Returns all existing Persons</returns>
        public Task<List<PersonResponse>> GetAllPersons();


        /// <summary>
        /// Retrieve a Person object from persons list based on given id
        /// </summary>
        /// <param name="id">the person id to be searched for</param>
        /// <returns></returns>
        public Task<PersonResponse?> GetPersonByID(Guid? id);


        /// <summary>
        /// Search for proper person objects in persons list
        /// </summary>
        /// <param name="searchBy">search field(property) to search</param>
        /// <param name="searchString">search string to search</param>
        /// <returns>Returns matching persons based on 'searchBy' and 'searchString' </returns>
        public Task<List<PersonResponse>> GetSearchedPersons(string searchBy, string? searchString);


        /// <summary>
        /// Sort all persons in order of 'sortBy' field and in sortOrder (ASC or DSC)
        /// </summary>
        /// <param name="allPersons"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortOrder"></param>
        /// <returns>Returns sorted persons in ASC or DSC (sortOrder) based on 'sortBy'</returns>
        public Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);

        
        /// <summary>
        /// Find the object in persons list and update it with new details, then returns the 'PersonResponse' object
        /// </summary>
        /// <param name="personUpdateRequest"></param>
        /// <returns>Returns the person with updated details</returns>
        public Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);


        /// <summary>
        /// Find and Delete the person object with given 'id' from the 'persons list'
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns 'True' if person is deleted and if it isn't 'False'</returns>
        public Task<bool> DeletePerson(Guid? id); 

    }
}
