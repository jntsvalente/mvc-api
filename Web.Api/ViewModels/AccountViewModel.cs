using System.ComponentModel.DataAnnotations;

namespace Web.Api.ViewModels;
public class AccountViewModel
{
    [Required(ErrorMessage = "E-mail is required")]
    [EmailAddress(ErrorMessage = "Invalid e-mail")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Password is required")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "This field must contain between 6 and 20 characters")]
    public string Password { get; set; } = "";
}