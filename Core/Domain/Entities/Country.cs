using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Entities
{
    public class Country
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        [StringLength(20)]
        public string? Name { get; set; }



        // Relationships //

        //                   (Dependent)                (Principal)
        // With "Person" ---> Person 'N'====......----'1' Country
        public ICollection<Person>? Persons { get; } = new List<Person>();
    }
}
