using Microsoft.EntityFrameworkCore;
using BankAPI.Data;
using BankAPI.Data.BankModels;

namespace BankAPI.Services;

public class AccountTypeService
{
    private readonly BankContext _context;

    public AccountTypeService(BankContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AccountType>> GetAll()
    {
        return await _context.AccountTypes.ToListAsync();
    }

    public async Task<AccountType?> GetById(int id)
    {
        return await _context.AccountTypes.FindAsync(id);
    }

    public async Task<AccountType> Create(AccountType newAccountType)
    {
        _context.AccountTypes.Add(newAccountType);
        await _context.SaveChangesAsync();
        return newAccountType;
    }

    public async Task Update(int id, AccountType accountType)
    {
        var existingAccountType = await GetById(id);

        if (existingAccountType is not null)
        {
            existingAccountType.Name = accountType.Name;

            await _context.SaveChangesAsync();
        }
    }

    public async Task Delete(int id)
    {
        var accountTypeToDelete = await GetById(id);
        
        if (accountTypeToDelete is not null)
        {
            _context.AccountTypes.Remove(accountTypeToDelete);
            await _context.SaveChangesAsync();
        }
    }
}