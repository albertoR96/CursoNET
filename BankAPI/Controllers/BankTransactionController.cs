using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using BankAPI.Services;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace BankAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BankTransactionController : ControllerBase
{
    private readonly BankTransactionService _service;
    private readonly AccountService _accountService;
    private readonly ClientService _clientService;
    public BankTransactionController(BankTransactionService service, AccountService accountService, ClientService clientService)
    {
        _service = service;
        _accountService = accountService;
        _clientService = clientService;
    }

    [Authorize(Policy = "SuperAdmin")]
    [HttpGet]
    public async Task<IEnumerable<BankTransaction>> GetTransactions()
    {
        return await _service.GetAll();
    }

    [HttpGet("accounts")]
    public async Task<IActionResult> GetAccounts()
    {
        var clientId = LoginController.GetClientID(HttpContext);
        if (clientId == 0)
        {
            return BadRequest(new { message = "Usuario no es cliente." });
        }
        else
        {
            try
            {
                return Ok(await _accountService.GetByClientId(clientId));
            }
            catch (ClientNotFoundException e)
            {
                BadRequest();
                throw;
            }

        }

    }

    [HttpPost("deposit")]
    public async Task<ActionResult> Deposit(BankTransactionDTO deposit)
    {
        var clientId = LoginController.GetClientID(HttpContext);
        if (clientId == 0)
        {
            return BadRequest(new { message = "Usuario no es cliente." });
        }

        var account = await _accountService.GetById(deposit.AccountId);
        if (account is null)
        {
            return BadRequest(new { message = "La cuenta no existe." });
        }
        else if ((account.ClientId != 0) && (account.ClientId != clientId))
        {
            return BadRequest(new { message = "No puedes realizar depositos a cuentas ajenas." });
        }

        await _accountService.UpdateAccountBalance(deposit.AccountId, deposit.Ammount);

        var newTransaction = new BankTransaction();
        newTransaction.AccountId = deposit.AccountId;
        newTransaction.TransactionType = 1;
        newTransaction.Amount = deposit.Ammount;
        await _service.Create(newTransaction);


        return Ok();
    }

    [HttpPost("withdrawal")]
    public async Task<ActionResult> Withdrawal(WithdrawalDTO withdrawal)
    {
        var clientId = LoginController.GetClientID(HttpContext);
        if (clientId == 0)
        {
            return BadRequest(new { message = "Usuario no es cliente." });
        }

        var account = await _accountService.GetById(withdrawal.AccountId);
        Account externalAccount = null;
        if ((withdrawal.ExternalAccount != 0) && (await _accountService.AccountExists(withdrawal.ExternalAccount)))
        {
            externalAccount = await _accountService.GetById(withdrawal.ExternalAccount);
        }

        if (account is null)
        {
            return BadRequest(new { message = "La cuenta no existe." });
        }

        if ((withdrawal.ExternalAccount != 0) && !(await _accountService.AccountExists(withdrawal.ExternalAccount)))
        {
            return BadRequest(new { message = "La cuenta externa no existe." });
        }

        if (account.ClientId != clientId)
        {
            return BadRequest(new { message = "No puedes realizar retiros de cuentas ajenas." });
        }

        _accountService.UpdateAccountBalance(withdrawal.AccountId, 0 - withdrawal.Ammount);
        if (externalAccount is not null)
        {
            await _accountService.UpdateAccountBalance(externalAccount.Id, withdrawal.Ammount);
        }

        var newTransaction = new BankTransaction();
        newTransaction.AccountId = withdrawal.AccountId;
        newTransaction.TransactionType = 2;
        newTransaction.Amount = withdrawal.Ammount;
        newTransaction.ExternalAccount = withdrawal.ExternalAccount;
        await _service.Create(newTransaction);


        return Ok();
    }

    [HttpDelete("account/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var accountToDelete = await _accountService.GetById(id);

        if (accountToDelete is null) {
            return AccountNotFound(id);
        }

        if (accountToDelete.Balance == 0)
        {
            await _service.Delete(id);
        }
        else
        {
            return BadRequest(new { message = "Las cuenta debe estar vacía para ser eliminada" });
        }


        return Ok();
    }

    public NotFoundObjectResult AccountNotFound(int id)
    {
        return NotFound(new { message = $"La cuenta con ID = {id} no existe." });
    }

}
