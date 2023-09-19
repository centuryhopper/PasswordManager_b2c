
using MVC_B2C_PasswordManager.Server.Models;

public interface IPasswordManagerAccountRepository<T>
{
    Task<IEnumerable<T>> GetAccountsAsync(string UserId);
    Task<T?> CreateAsync(T model);
    Task<T?> UpdateAsync(T model);
    Task<T?> DeleteAsync(T model);
    Task<UploadStatus> UploadCsvAsync(IFormFile file, string userid);
}