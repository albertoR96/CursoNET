using System.Text.RegularExpressions;
using BankApp;

int[] withdrawals = new int[10];

void ShowMenu()
{
    Console.WriteLine("Porfavor ingrese el número de opción a realizar.");
    Console.WriteLine("1. Crear un nuevo usuario");
    Console.WriteLine("2. Eliminar usuarios");
    Console.WriteLine("3. Salir");
}

int ReadInteger()
{
    int c;
    bool firstAttempt = true;
    while (true)
    {
        if (!firstAttempt)
        {
            Console.WriteLine("Ingrese un número valido.");
        }
        firstAttempt = false;
        var x = Console.ReadLine();
        if (int.TryParse(x, out c))
        {
            return c;
        }
    }
}

decimal ReadDecimal()
{
    decimal c;
    bool firstAttempt = true;
    while (true)
    {
        if (!firstAttempt)
        {
            Console.WriteLine("Ingrese un número valido.");
        }
        firstAttempt = false;
        var x = Console.ReadLine();
        if (decimal.TryParse(x, out c) && c >= 0)
        {
            return c;
        }
    }
}

string ReadEmail()
{
    string email;
    bool firstAttempt = true;
    while (true)
    {
        if (!firstAttempt)
        {
            Console.WriteLine("Ingrese un correo válido.");
        }
        firstAttempt = false;
        email = Console.ReadLine().ToString();
        if (Regex.IsMatch(email, "^(?(\")(\".+?(?<!\\\\)\"@)|(([0-9a-z]((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`\\{\\}\\|~\\w])*)(?<=[0-9a-z])@))(?(\\[)(\\[(\\d{1,3}\\.){3}\\d{1,3}\\])|(([0-9a-z][-\\w]*[0-9a-z]*\\.)+[a-z0-9][\\-a-z0-9]{0,22}[a-z0-9]))$"))
        {
            return email;
        }
    }
}

char ReadUserType()
{
    string s;
    bool firstAttempt = true;
    while (true)
    {
        if (!firstAttempt)
        {
            Console.WriteLine("Los tipos validos son c y e");
        }
        firstAttempt = false;
        s = Console.ReadLine().ToString();
        if (s.Length == 1 && (s == "c" || s == "e"))
        {
            return s.ToCharArray()[0];
        }
    }
}

void StoreUser()
{
    User userToCreate = null;

    Console.WriteLine("Por favor ingrese el id del usuario");
    int userId = ReadInteger();

    Console.WriteLine("Por favor ingrese el nombre del usuario");
    string userName = Console.ReadLine().ToString();

    Console.WriteLine("Por favor ingrese el correo del usuario");
    string userEmail = ReadEmail();

    Console.WriteLine("Por favor ingrese el saldo del usuario");
    decimal userBalance = ReadDecimal();

    Console.WriteLine("Por favor ingrese el tipo del usuario. Cliente(c) o empleado(e).");
    char userType = ReadUserType();

    switch (userType)
    {
        case 'c':
            userToCreate = new Client(userId, userName, userEmail, userBalance, 'M');
            break;

        case 'e':
            userToCreate = new Employee(userId, userName, userEmail, userBalance, "IT");
            break;

        default:
            break;
    }

    if (userToCreate != null && Storage.StoreUser(userToCreate))
    {
        Console.WriteLine("El usuario se creó exitosamente.");
    }
    else
    {
        Console.WriteLine("Inserción del usuario falló.");
    }
}

void DeleteUser()
{
    Console.WriteLine("Ingrese el ID del usuario a eliminar.");
    int userIDToRemove = ReadInteger();
    if (Storage.RemoveUser(userIDToRemove))
    {
        Console.WriteLine("Usuario eliminado exitosamente.");
    }
    else
    {
        Console.WriteLine("Usuario no encontrado.");
    }
}

void App()
{
    int o;
    bool reprintOptions;
    bool operationDone;
    bool finishProgram = false;
    Console.WriteLine("Bienvenido");
    do
    {
        reprintOptions = false;
        operationDone = false;
        ShowMenu();
        var x = Console.ReadLine();
        if (int.TryParse(x, out o))
        {
            switch (o)
            {
                case 1:
                    StoreUser();
                    operationDone = true;
                    break;
                case 2:
                    DeleteUser();
                    operationDone = true;
                    break;
                case 3:
                    finishProgram = true;
                    break;
                default:
                    reprintOptions = true;
                    break;
            }
        }
        else
        {
            reprintOptions = true;
        }
        if (reprintOptions)
        {
            Console.WriteLine("Por favor ingrese una opción valida");
        }
        if (operationDone)
        {
            Console.WriteLine("¿Desea realizar otra operación? (ingrese si o no)");
            string s = Console.ReadLine().ToString();
            while (!Regex.IsMatch(s, "^(si|no)$", RegexOptions.IgnoreCase))
            {
                Console.WriteLine("Por favor ingrese si o no");
                s = Console.ReadLine().ToString();
            }
            finishProgram = Regex.IsMatch(s, "^no$", RegexOptions.IgnoreCase);
        }
        Console.Clear();
    } while (!finishProgram);
    Console.WriteLine("Adios! :)");
}

App();