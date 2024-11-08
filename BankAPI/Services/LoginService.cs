using Microsoft.EntityFrameworkCore;
using BankAPI.Data;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;

namespace BankAPI.Services;

public class LoginService
{
    private BankContext _context;
    public LoginService(BankContext context)
    {
        _context = context;
    }

    public async Task<Administrator?> GetAdmin(AdminDto admin)
    {
        return await _context.Administrators.SingleOrDefaultAsync(x => x.Email == admin.Email);
    }

    public async Task<Administrator> Create(Administrator newAdmin)
    {
        _context.Administrators.Add(newAdmin);
        await _context.SaveChangesAsync();
        return newAdmin;
    }
}