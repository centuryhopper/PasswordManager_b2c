using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using MVC_B2C_PasswordManager.Server.Contexts;
using MVC_B2C_PasswordManager.Contexts.Models;
using MVC_B2C_PasswordManager.Models;
using MVC_B2C_PasswordManager.Interfaces;
using cloudscribe.Pagination.Models;

namespace MVC_B2C_PasswordManager.Server.Repositories;

public class PasswordManagerAccountRepository : IPasswordManagerAccountRepository<PasswordmanagerAccount>
{
    private readonly EncryptionContext encryptionContext;
    private readonly ILogger<PasswordManagerAccountRepository> logger;
    private readonly EfDbContext efDbContext;

    public PasswordManagerAccountRepository(EncryptionContext encryptionContext, ILogger<PasswordManagerAccountRepository> logger, EfDbContext efDbContext)
    {
        this.encryptionContext = encryptionContext;
        this.logger = logger;
        this.efDbContext = efDbContext;
    }

    public async Task<PasswordmanagerAccount?> CreateAsync(PasswordmanagerAccount model)
    {
        model.Id = Guid.NewGuid().ToString();
        model.Password = Convert.ToBase64String(encryptionContext.Encrypt(model.Password));
        model.CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd");
        await efDbContext.PasswordmanagerAccounts.AddAsync(model);
        await efDbContext.SaveChangesAsync();
        return model;
    }

    public async Task<PasswordmanagerAccount?> DeleteAsync(PasswordmanagerAccount model)
    {
        var queryModel = await efDbContext.PasswordmanagerAccounts.FindAsync(model.Id, model.Userid);
        efDbContext.PasswordmanagerAccounts.Remove(queryModel!);
        await efDbContext.SaveChangesAsync();
        return model;
    }

    public IEnumerable<PasswordmanagerAccount> GetAccountsAsync(string UserId, string searchTitle, int excludeRecords, int pageSize)
    {
        var queriedResults = efDbContext.PasswordmanagerAccounts.Where(a => a.Userid == UserId && a.Title.ToLower().Contains(searchTitle)).Skip(excludeRecords).Take(pageSize).AsEnumerable();

        if (!queriedResults.Any())
        {
            return Enumerable.Empty<PasswordmanagerAccount>();
        }

        var results = queriedResults.Select(m =>
        {
            return new PasswordmanagerAccount
            {
                Id = m.Id,
                Title = m.Title,
                Username = m.Username,
                Password = encryptionContext.Decrypt(Convert.FromBase64String(m.Password)),
                Userid = m.Userid,
                CreatedAt = m.CreatedAt,
                LastUpdatedAt = m.LastUpdatedAt
            };
        });

        return results;
    }

    public int AccountsCount(string UserId, string title)
    {
        var cnt = efDbContext.PasswordmanagerAccounts.Where(a => a.Userid == UserId && a.Title.ToLower().Contains(title)).Count();
        return cnt;
    }

    public async Task<PasswordmanagerAccount?> UpdateAsync(PasswordmanagerAccount model)
    {
        var dbModel = await efDbContext.PasswordmanagerAccounts.FindAsync(model.Id, model.Userid);
        dbModel!.LastUpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd");
        dbModel.Title = model.Title;
        dbModel.Username = model.Username;
        dbModel.Password = Convert.ToBase64String(encryptionContext.Encrypt(model.Password));
        await efDbContext.SaveChangesAsync();

        return model;
    }

    public async Task<UploadStatus> UploadCsvAsync(IFormFile file, string userid)
    {
        // set up csv helper and read file
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };

        using var streamReader = new StreamReader(file.OpenReadStream());
        using var csvReader = new CsvReader(streamReader, config);
        IAsyncEnumerable<PasswordmanagerAccount> records;

        try
        {
            csvReader.Context.RegisterClassMap<PasswordsMapper>();
            records = csvReader.GetRecordsAsync<PasswordmanagerAccount>();

            await foreach (var record in records)
            {
                await CreateAsync(new PasswordmanagerAccount
                {
                    Id = Guid.NewGuid().ToString(),
                    Userid = userid,
                    Title = record.Title,
                    Username = record.Username,
                    Password = record.Password,
                });
            }
        }
        catch (CsvHelperException ex)
        {
            return new UploadStatus
            {
                Message = "Failed to upload csv",
                UploadEnum = UploadEnum.FAIL
            };
        }

        return new UploadStatus
        {
            Message = "Upload csv success!",
            UploadEnum = UploadEnum.SUCCESS
        };
    }

}

