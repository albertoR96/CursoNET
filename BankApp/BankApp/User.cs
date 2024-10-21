using Newtonsoft.Json;
namespace BankApp;

public class User : Person
{
    [JsonProperty]
    protected int ID { set; get; }
    [JsonProperty]
    protected string Name { get; set; }
    [JsonProperty]
    protected string Email { get; set; }
    protected DateTime Birth { get; set; }
    [JsonProperty]
    protected DateTime RegisterDate { get; set; }
    [JsonProperty]
    protected decimal Balance { get; set; }

    public User(int id, string name, string email, decimal balance)
    {
        this.ID = id;
        this.Name = name;
        this.Email = email;
        this.Balance = balance;
        this.RegisterDate = new DateTime();
    }

    public virtual void setBalance(decimal balance)
    {
        if (balance < 0)
        {
            balance = 0;
        }

        this.Balance += balance;
    }

    public int getID()
    {
        return this.ID;
    }

    public string getDataString()
    {
        return "";
        //return Newtonsoft.Json.JsonConvert.SerializeObject(this);;
    }

    public bool store()
    {
        return true;
    }

    public override string getName()
    {
        return this.Name;
    }
}