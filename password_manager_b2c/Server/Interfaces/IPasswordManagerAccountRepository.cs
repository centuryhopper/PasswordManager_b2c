using password_manager_b2c.Shared;

public interface IPasswordManagerAccountRepository<T>
{
    Task<IEnumerable<T>> GetAccountsAsync(string UserId);
    Task<T?> CreateAsync(T model);
    Task<T?> UpdateAsync(T model);
    Task<T?> DeleteAsync(T model);
}