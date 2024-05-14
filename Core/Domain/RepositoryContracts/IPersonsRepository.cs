using System.Linq.Expressions;
using Core.Domain.Entities;

namespace Core.Domain.RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing Person entity
    /// </summary>
    public interface IPersonsRepository
    {
        /// <summary>
        /// Adds a person object to the data store
        /// </summary>
        /// <param name="person">Person object to add</param>
        /// <returns>Returns the person object after adding it to the table</returns>
        Task<Person> AddPerson(Person person);


        /// <summary>
        /// Returns all persons in the data store
        /// </summary>
        /// <returns>List of person objects from table</returns>
        Task<List<Person>> GetAllPersons();


        /// <summary>
        /// Returns a person object based on the given person id
        /// </summary>
        /// <param name="id">PersonID (guid) to search</param>
        /// <returns>A person object or null</returns>
        Task<Person?> GetPersonByID(Guid id);


        /// <summary>
        /// Returns all person objects based on the given expression
        /// </summary>
        /// <param name="predicate">LINQ expression to check</param>
        /// <returns>All matching persons with given condition</returns>
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);


        /// <summary>
        /// Deletes a person object based on the given person object
        /// </summary>
        /// <param name="person">Person object to delete</param>
        /// <returns>Returns true, if the deletion is successful; otherwise false</returns>
        Task<bool> DeletePerson(Person person);


        /// <summary>
        /// Updates a person object (person name and other details) based on the given person object (updatedPerson)
        /// </summary>
        /// <param name="person">Person object to be updated</param>
        /// <param name="updatedPerson">The updated Person object to apply to actual person object</param>
        /// <returns>Returns the updated person object</returns>
        Task<Person> UpdatePerson(Person person, Person updatedPerson);
    }
}
