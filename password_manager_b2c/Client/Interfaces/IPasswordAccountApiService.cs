using password_manager_b2c.Shared;

namespace password_manager_b2c.Client.Interfaces;

public interface IPasswordAccountApiService<T>
{
    Task<IEnumerable<T>> GetPasswordAccounts(string UserId);
    Task<T> CreateAsync(T model);
    Task<T> DeleteAsync(T model);
    Task<T> UpdateAsync(T model);
}