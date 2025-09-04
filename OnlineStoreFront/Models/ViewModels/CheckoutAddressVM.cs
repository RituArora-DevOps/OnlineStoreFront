using System.ComponentModel.DataAnnotations;

namespace OnlineStoreFront.Models.ViewModels;

public class CheckoutAddressVM
{
    [Required] public string FullName { get; set; } = "";
    [Required] public string Address1 { get; set; } = "";
    public string? Address2 { get; set; }
    [Required] public string City { get; set; } = "";
    [Required] public string Province { get; set; } = "";
    [Required] public string PostalCode { get; set; } = "";
    [Required, Phone] public string Phone { get; set; } = "";
}
