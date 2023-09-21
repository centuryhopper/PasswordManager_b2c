
using cloudscribe.Pagination.Models;
using MVC_B2C_PasswordManager.Models;

namespace MVC_B2C_PasswordManager.Interfaces;

public interface IPasswordManagerAccountRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAccountsAsync(string UserId);
    Task<IEnumerable<T>> GetAccountsAsync(string UserId, int recordsToExclude, int pageSize);
    Task<T?> CreateAsync(T model);
    Task<T?> UpdateAsync(T model);
    Task<T?> DeleteAsync(T model);
    Task<UploadStatus> UploadCsvAsync(IFormFile file, string userid);
}