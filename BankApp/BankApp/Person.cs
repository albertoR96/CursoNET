namespace BankApp;

public abstract class Person 
{
    public abstract string getName();

    public string getCountry()
    {
        return "country";
    }
}

public interface IPerson
{
    string getName();
    string getCountry();
}