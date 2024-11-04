using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using BankAPI.Services;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BankAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly AccountService _service;
    private readonly AccountTypeService _accountTypeService;
    private readonly ClientService _clientService;

    public AccountController(AccountService service, AccountTypeService accountTypeService, ClientService clientService)
    {
        _service = service;
        _accountTypeService = accountTypeService;
        _clientService = clientService;
    }

    /*[HttpGet]
    public async Task<IEnumerable<AccountDtoOut>> Get()
    {
        return  await _service.GetAll();
    }*/

    [HttpGet("{id}")]
    public async Task<ActionResult<AccountDtoOut>> GetById(int id)
    {
        var accountDTO = await _service.GetDTOById(id);
        var account = await _service.GetById(id);
        if (accountDTO is null) {
            return AccountNotFound(id);
        }
        var clientId = LoginController.GetClientID(HttpContext);
        if (clientId == 0)
        {
            return BadRequest(new { message = "Usuario no es cliente." });
        }
        else if (account.ClientId != clientId)
        {
            return Unauthorized(new { message = "No puedes acceder a esta cuenta." });
        }
        return accountDTO;
    }

    [Authorize(Policy = "SuperAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create(AccountDtoIn account)
    {
        /*
            Next block generates a JsonException: A possible object cycle was detected which is not supported. This can either be due to a cycle or if the object depth is larger than the maximum allowed depth of 32.
        */
        /*string validationResult = await ValidateAccount(account);
        if (!validationResult.Equals("Valid"))
        {
            return BadRequest(new { message = validationResult });
        }*/
        var newAccount = await _service.Create(account);

        return CreatedAtAction(nameof(GetById), new { id = newAccount.Id }, newAccount);
    }

    [Authorize(Policy = "SuperAdmin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, AccountDtoIn account)
    {
        if (id != account.Id) {
            return BadRequest(new { message = $"El ID({id}) de la URL no coincide con el ID({account.Id}) del cuerpo de la solicitud." });
        }

        var accountToUpdate = await _service.GetById(id);
        if (accountToUpdate is null) {
            return AccountNotFound(id);
        }

        string validationResult = await ValidateAccount(account);
        if (!validationResult.Equals("Valid"))
        {
            return BadRequest(new { message = validationResult });
        }

        await _service.Update(id, account);
        return NoContent();
    }

    [Authorize(Policy = "SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var accountToDelete = await _service.GetById(id);
        if (accountToDelete is null) {
            return AccountNotFound(id);
        }

        await _service.Delete(id);

        return Ok();
    }

    [NonAction]
    public NotFoundObjectResult AccountNotFound(int id)
    {
        return NotFound(new { message = $"La cuenta con ID = {id} no existe." });
    }

    [NonAction]
    public async Task<string> ValidateAccount(AccountDtoIn account)
    {
        string result = "Valid";
        var accountType = await _accountTypeService.GetById(account.AccountType);
        if (accountType is null)
        {
            result = $"El tipo de cuenta {account.AccountType} no existe.";
        }

        var clientId = account.ClientId;
        var client = await _clientService.GetById(clientId);
        if (client is null)
        {
            result = $"El cliente {clientId} no existe.";
        }
        return result;
    }
}
