
using MVC_B2C_PasswordManager.Models;

namespace MVC_B2C_PasswordManager.Interfaces;

public interface IPasswordManagerAccountRepository<T>
{
    Task<IEnumerable<T>> GetAccountsAsync(string UserId);
    Task<T?> CreateAsync(T model);
    Task<T?> UpdateAsync(T model);
    Task<T?> DeleteAsync(T model);
    Task<UploadStatus> UploadCsvAsync(IFormFile file, string userid);
}