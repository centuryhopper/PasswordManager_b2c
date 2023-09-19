
namespace MVC_B2C_PasswordManager.Contexts.Models;

public partial class PasswordmanagerAccount
{
    public string Id { get; set; } = null!;

    public string Userid { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? CreatedAt { get; set; }
    public string? LastUpdatedAt { get; set; }

    public override string ToString()
    {
        return $"{nameof(Userid)}:{Userid}, {nameof(Title)}:{Title}, {nameof(Username)}:{Username}, {nameof(Password)}:{Password},";
    }
}




