using System.ComponentModel.DataAnnotations;
namespace password_manager_b2c.Shared;

public class LoginModel
{
    [Required, EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }

    public override string ToString()
    {
        return $"{nameof(Email)}:{Email}, {nameof(Password)}:{Password}, ";
    }
}
