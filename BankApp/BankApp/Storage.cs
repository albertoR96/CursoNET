using Newtonsoft.Json;
namespace BankApp;

public static class Storage
{
    static string FilePath = AppDomain.CurrentDomain.BaseDirectory + @"\users.json";

    private static bool UserIdExists(List<User> usersList, int userId)
    {
        foreach (User user in usersList)
        {
            if (user.getID() == userId) {
                return true;
            }
        }
        return false;
    }
    public static bool StoreUser(User user)
    {
        string jsonToStore;
        string usersInJson = "";
        if (File.Exists(FilePath))
        {
            usersInJson = File.ReadAllText(FilePath);
        }
        
        var usersList = ReadUsers();

        if (UserIdExists(usersList, user.getID()))
        {
            Console.WriteLine($"El id {user.getID()} no esta disponible.");
            return false;
        }

        usersList.Add(user);
        
        jsonToStore = JsonConvert.SerializeObject(usersList);
        File.WriteAllText(FilePath, jsonToStore);

        return true;
    }

    public static bool RemoveUser(int userID)
    {
        List<User> users = ReadUsers();
        List<User> newUsers = new List<User>();
        string jsonToStore;
        bool userFound = false;
        foreach (User user in users)
        {
            if (user.getID() != userID)
            {
                newUsers.Add(user);
            }
            else
            {
                userFound = true;
            }
        }
        jsonToStore = JsonConvert.SerializeObject(newUsers);
        File.WriteAllText(FilePath, jsonToStore);
        return userFound;
    }

    public static List<User> ReadUsers()
    {
        string usersInJson = "";
        if (File.Exists(FilePath))
        {
            usersInJson = File.ReadAllText(FilePath);
        }
        
        var usersList = JsonConvert.DeserializeObject<List<User>>(usersInJson);

        if (usersList == null)
        {
            usersList = new List<User>();
        }

        return usersList;
    }
}