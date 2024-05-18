using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Core.Domain.Entities;

public class Person
{
    [Key]
    public Guid? ID { get; set; }

    [Required]
    [StringLength(30)]
    public string? Name { get; set; }

    [Required]
    [StringLength(50)]
    public string? Email { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [Required]
    [StringLength(10)]
    public string? Gender { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    public bool ReceiveNewsLetters { get; set; }
    
    [StringLength(11)]
    [RegularExpression(@"^[0-9]{3}-[0-9]{2}-[0-9]{4}$")] // NNN-NN-NNNN
    public string? TIN { get; set; }



    // Relationships //

    //                   (Dependent)                 (Principal)
    // With "Country" ---> Person 'N'====......----'1' Country
    [ForeignKey("Country")]
    public Guid CountryID { get; set; }
    public Country Country { get; set; } = null!;
    
    
    
    // Methods
    public override string ToString()
    {
        string info = $"Person ID: {ID}, Person Name: {Name}, Email: {Email}, Date of Birth: {DateOfBirth?.ToString("yyyy/MM/dd")}, " +
                      $"Gender: {Gender}, Country ID: {CountryID}, Country: {Country?.Name}, Address: {Address}, Receive News Letters: {ReceiveNewsLetters}";

        return info;
    }
}