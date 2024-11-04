namespace BankAPI.Data.DTOs;

public class WithdrawalDTO
{
    public int AccountId { get; set; }
    public int ExternalAccount { get; set; }
    public decimal Ammount { get; set; }

}