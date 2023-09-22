
using cloudscribe.Pagination.Models;
using MVC_B2C_PasswordManager.Models;

namespace MVC_B2C_PasswordManager.Interfaces;

public interface IPasswordManagerAccountRepository<T> where T : class
{
    int AccountsCount(string UserId, string title);
    IEnumerable<T> GetAccountsAsync(string UserId, string searchTitle, int recordsToExclude, int pageSize);
    Task<T?> CreateAsync(T model);
    Task<T?> UpdateAsync(T model);
    Task<T?> DeleteAsync(T model);
    Task<UploadStatus> UploadCsvAsync(IFormFile file, string userid);
}