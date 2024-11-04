using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using BankAPI.Services;
using BankAPI.Data.BankModels;

namespace BankAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountTypeController : ControllerBase
{
    private readonly AccountTypeService _service;
    public AccountTypeController(AccountTypeService context)
    {
        _service = context;
    }

    [HttpGet]
    public async Task<IEnumerable<AccountType>> Get()
    {
        return await _service.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AccountType>> GetById(int id)
    {
        var accountType = await _service.GetById(id);
        if (accountType is null) {
            return AccountTypeNotFound(id);
        }
        return accountType;
    }

    [HttpPost]
    public async Task<IActionResult> Create(AccountType accountType)
    {
        var newAccountType = await _service.Create(accountType);

        return CreatedAtAction(nameof(GetById), new { id = newAccountType.Id }, newAccountType);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, AccountType accountType)
    {
        if (id != accountType.Id) {
            return BadRequest(new { message = $"El ID({id}) de la URL no coincide con el ID({accountType.Id}) del cuerpo de la solicitud." });
        }
        var accountTypeToUpdate = await _service.GetById(id);
        if (accountTypeToUpdate is null) {
            return AccountTypeNotFound(id);
        }

        await _service.Update(id, accountTypeToUpdate);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var accountTypeToDelete = await _service.GetById(id);
        if (accountTypeToDelete is null) {
            return AccountTypeNotFound(id);
        }

        await _service.Delete(id);

        return Ok();
    }

    [NonAction]
    public NotFoundObjectResult AccountTypeNotFound(int id)
    {
        return NotFound(new { message = $"El tipo de cuenta con ID = {id} no existe." });
    }
}
