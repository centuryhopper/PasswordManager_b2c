using System.Runtime.Intrinsics.X86;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using password_manager_b2c.Server.Contexts;
using password_manager_b2c.Server.Models;
using password_manager_b2c.Shared;

namespace password_manager_b2c.Server.Repositories;

public class PasswordManagerAccountRepository : IPasswordManagerAccountRepository<PasswordmanagerAccount>
{
    private readonly DapperContext dapperContext;
    private readonly EncryptionContext encryptionContext;
    private readonly ILogger<PasswordManagerAccountRepository> logger;
    private readonly EfDbContext efDbContext;

    public PasswordManagerAccountRepository(DapperContext dapperContext, EncryptionContext encryptionContext, ILogger<PasswordManagerAccountRepository> logger, EfDbContext efDbContext)
    {
        this.dapperContext = dapperContext;
        this.encryptionContext = encryptionContext;
        this.logger = logger;
        this.efDbContext = efDbContext;
    }

    public async Task<PasswordmanagerAccount?> CreateAsync(PasswordmanagerAccount model)
    {
        model.Password = Convert.ToBase64String(encryptionContext.Encrypt(model.Password));
        model.CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd");
        await efDbContext.PasswordmanagerAccounts.AddAsync(model);
        await efDbContext.SaveChangesAsync();
        return model;
    }

    public async Task<PasswordmanagerAccount?> DeleteAsync(PasswordmanagerAccount model)
    {
        efDbContext.PasswordmanagerAccounts.Remove(model);
        await efDbContext.SaveChangesAsync();
        return model;
    }

    public async Task<IEnumerable<PasswordmanagerAccount>> GetAccountsAsync(string UserId)
    {
        var results = await efDbContext.PasswordmanagerAccounts.Where(a => a.Userid == UserId).ToListAsync();

        if (!results.Any())
        {
            return Enumerable.Empty<PasswordmanagerAccount>();
        }

        return results.Select(m =>
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
    }

    public async Task<PasswordmanagerAccount?> UpdateAsync(PasswordmanagerAccount model)
    {
        var dbModel = await efDbContext.PasswordmanagerAccounts.FindAsync(model.Id, model.Userid);
        dbModel.LastUpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd");
        dbModel!.Title = model.Title;
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
            return new UploadStatus {
                Message = "Failed to upload csv",
                UploadEnum = UploadEnum.FAIL
            };
        }

        return new UploadStatus {
            Message = "Upload csv success!",
            UploadEnum = UploadEnum.SUCCESS
        };
    }
}
