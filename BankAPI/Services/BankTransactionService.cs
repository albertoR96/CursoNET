using Microsoft.EntityFrameworkCore;
using BankAPI.Data;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;

namespace BankAPI.Services;

public class BankTransactionService
{
    private readonly BankContext _context;

    public BankTransactionService(BankContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BankTransaction>> GetAll()
    {
        return await _context.BankTransactions.ToListAsync();
    }

    public async Task<BankTransaction?> GetById(int id)
    {
        return await _context.BankTransactions.FindAsync(id);
    }

    public async Task<BankTransaction> Create(BankTransaction newBankTransaction)
    {
        _context.BankTransactions.Add(newBankTransaction);
        await _context.SaveChangesAsync();
        return newBankTransaction;
    }

    public async Task Update(int id, BankTransaction bankTransaction)
    {
        var existingTransaction = await GetById(id);
        
        if (existingTransaction is not null)
        {
            existingTransaction.AccountId = bankTransaction.AccountId;
            existingTransaction.TransactionType = bankTransaction.TransactionType;
            existingTransaction.Amount = bankTransaction.Amount;
            existingTransaction.ExternalAccount = bankTransaction.ExternalAccount;

            await _context.SaveChangesAsync();
        }
    }

    public async Task Delete(int id)
    {
        var transactionToDelete = await GetById(id);
        
        if (transactionToDelete is not null)
        {
            _context.BankTransactions.Remove(transactionToDelete);
            await _context.SaveChangesAsync();
        }
    }
}