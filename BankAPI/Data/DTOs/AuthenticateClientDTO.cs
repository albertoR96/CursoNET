namespace BankAPI.Data.DTOs;

public class AuthenticateClientDTO
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Pwd { get; set; }
}