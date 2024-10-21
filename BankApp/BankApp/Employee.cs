namespace BankApp;

public class Employee : User
{
    public string Department { get; set; }
    public Employee(int id, string name, string email, decimal balance, string department) : base (id, name, email, balance)
    {
        this.Department = department;
    }

    public override void setBalance(decimal balance)
    {
        base.setBalance(balance);

        if (!string.IsNullOrEmpty(this.Department) && this.Department.Equals("IT"))
        {
            this.Balance += (balance * 0.05m);
        }
    }
}