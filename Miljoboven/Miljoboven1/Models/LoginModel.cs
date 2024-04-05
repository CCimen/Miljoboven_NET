using System.ComponentModel.DataAnnotations;

namespace Miljoboven2.Models;

public class LoginModel
{
    [Required(ErrorMessage = "Du måste fylla i ett användarnamn")]
    [Display(Name = "Användarnamn")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Du måste fylla i ett lösenord")]
    [Display(Name = "Lösenord")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public string ReturnUrl { get; set; }
}