namespace BankAPI.Data.DTOs;

public class BankTransactionDTO
{
    public int AccountId { get; set; }
    public int? ExternalAccount { get; set; } = null!;
    public decimal Ammount { get; set; }

}