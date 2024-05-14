using System.ComponentModel.DataAnnotations;
using Core.Domain.Entities;
using Core.Enums;

namespace Core.DTO.PersonDTO
{
    public class PersonUpdateRequest
    {
        [Required(ErrorMessage = "Person 'ID' can't be empty")]
        public Guid ID { get; set; }
        
        [Required(ErrorMessage = "Person 'Name' can't be empty")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Person 'Email' can't be empty")]
        [EmailAddress(ErrorMessage = "'Email address' should be a valid address")]
        // [DataType(DataType.EmailAddress)]  // -> useful for 'type' attribute in html tags
        public string? Email { get; set; }

        [Required(ErrorMessage = "Person 'Date Of Birth' can't be empty")]
        // [DataType(DataType.Date)]   // -> useful for 'type' attribute in html tags
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Person 'Gender' must be selected")]
        public GenderOptions? Gender { get; set; }

        [Required(ErrorMessage = "Person 'Country' must be selected")]
        public Guid CountryID { get; set; }

        [Required(ErrorMessage = "Person 'Address' can't be empty")]
        public string? Address { get; set; }

        public bool RecieveNewsLetters { get; set; }

        public Person ToPerson() 
        {
            Person person = new Person()
            {
                ID = ID,
                Name = Name,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender.ToString(),
                CountryID = CountryID,
                Address = Address,
                RecieveNewsLetters = RecieveNewsLetters
            };

            return person;
        }
    }

}
