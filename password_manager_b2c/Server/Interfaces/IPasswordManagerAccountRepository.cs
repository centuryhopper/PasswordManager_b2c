using Microsoft.AspNetCore.Mvc;
using password_manager_b2c.Server.Models;
using password_manager_b2c.Shared;

public interface IPasswordManagerAccountRepository<T>
{
    Task<IEnumerable<T>> GetAccountsAsync(string UserId);
    Task<T?> CreateAsync(T model);
    Task<T?> UpdateAsync(T model);
    Task<T?> DeleteAsync(T model);
    Task<UploadStatus> UploadCsvAsync(IFormFile file, string userid);
}