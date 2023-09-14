

using CsvHelper.Configuration;
using password_manager_b2c.Shared;

namespace password_manager_b2c.Server.Models;

public class PasswordsMapper : ClassMap<PasswordmanagerAccount>
{
    public PasswordsMapper()
    {
        Map(m=>m.Title).Name("Title");
        Map(m=>m.Username).Name("Username");
        Map(m=>m.Password).Name("Password");
    }
}