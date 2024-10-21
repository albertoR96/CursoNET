namespace BankApp;

public class Client : User
{
    public char TaxRegime { get; set; }
    public Client(int id, string name, string email, decimal balance, char taxRegime) : base (id, name, email, balance)
    {
        this.ID = id;
        this.Name = name;
        this.Email = email;
        this.Balance = balance;
        this.RegisterDate = new DateTime();
        this.TaxRegime = taxRegime;
    }

    public override void setBalance(decimal balance)
    {
        base.setBalance(balance);
        if (TaxRegime.Equals('M'))
        {
            this.Balance += (balance * 0.02m);
        }
    }
}