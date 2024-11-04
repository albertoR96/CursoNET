using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using BankAPI.Services;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace BankAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClientController : ControllerBase
{
    private readonly ClientService _service;
    public ClientController(ClientService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IEnumerable<Client>> Get()
    {
        return await _service.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Client>> GetById(int id)
    {
        var client = await _service.GetById(id);
        if (client is null) {
            return ClientNotFound(id);
        }
        return client;
    }

    [Authorize(Policy = "SuperAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create(ClientDTOIn client)
    {
        Client newClient = new Client();
        newClient.Name = client.Name;
        newClient.PhoneNumber = client.PhoneNumber;
        newClient.Email = client.Email;

        byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
        newClient.SaltString = Convert.ToBase64String(salt);

        newClient.HashString = Convert.ToBase64String(LoginController.HashPassword(salt, client.Pwd));
        _service.Create(newClient);

        return CreatedAtAction(nameof(GetById), new { id = newClient.Id }, newClient);
    }

    [Authorize(Policy = "SuperAdmin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Client client)
    {
        if (id != client.Id) {
            return BadRequest(new { message = $"El ID({id}) de la URL no coincide con el ID({client.Id}) del cuerpo de la solicitud." });
        }
        var clientToUpdate = await _service.GetById(id);
        if (clientToUpdate is null) {
            return ClientNotFound(id);
        }

        await _service.Update(id, client);
        return NoContent();
    }

    [Authorize(Policy = "SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var clientToDelete = await _service.GetById(id);
        if (clientToDelete is null) {
            return ClientNotFound(id);
        }

        await _service.Delete(id);

        return Ok();
    }

    //[ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public NotFoundObjectResult ClientNotFound(int id)
    {
        return NotFound(new { message = $"El cliente con ID = {id} no existe." });
    }
}
