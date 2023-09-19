using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MVC_B2C_PasswordManager.Utils;

public static class Helpers
{
    public static string GetErrors<T>(this ModelStateDictionary ModelState, ILogger<T>? logger = null)
    {
        // Retrieve the list of errors
        var errors = ModelState.Values.SelectMany(v => v.Errors);
        if (logger is not null)
        {
            errors.ToList().ForEach(e => logger.LogWarning(e.ErrorMessage));
        }

        return string.Join("|||", errors.Select(e => e.ErrorMessage));
    }

    public static async Task SendEmail(string subject, string body, string sender, IEnumerable<string> receivers)
    {
    }
}
