

using CsvHelper.Configuration;
using MVC_B2C_PasswordManager.Shared;

namespace MVC_B2C_PasswordManager.Server.Models;

public class PasswordsMapper : ClassMap<PasswordmanagerAccount>
{
    public PasswordsMapper()
    {
        Map(m => m.Title).Name("Title");
        Map(m => m.Username).Name("Username");
        Map(m => m.Password).Name("Password");
    }
}