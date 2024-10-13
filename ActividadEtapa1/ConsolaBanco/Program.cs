using System.Text.RegularExpressions;

int withdrawCounter = 0;
int[] withdrawals = new int[10];

void printOptions()
{
    Console.WriteLine("Porfavor ingrese el número de opción a realizar.");
    Console.WriteLine("1. Ingresar retiros por usuarios");
    Console.WriteLine("2. Revisar cantidad de billetes y monedas entregadas");
    Console.WriteLine("3. Salir");
}

int readInteger()
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

void storeWithdrawals()
{
    if (withdrawCounter == 10)
    {
        Console.WriteLine("Ya ingresó la mayor cantidad de retiros");
        return;
    }
    Console.WriteLine("Ingrese la cantidad de retiros");
    int c = readInteger();
    while (c == 0 || (c + withdrawCounter) > 10)
    {
        if (c + withdrawCounter > 10)
        {
            Console.WriteLine($"Usted cuenta con {withdrawCounter} retiros ingresados. Solo puede ingresar 10 retiros.");
        }
        c = readInteger();
    }
    for (int i = 0; i < c; i++)
    {
        Console.WriteLine("================================");
        Console.WriteLine($"Ingrese el monto del retiro #{withdrawCounter + 1}");
        int withdrawMount = readInteger();
        while (withdrawMount > 50000)
        {
            Console.WriteLine("Los retiros no deben ser mayor a 50,000");
            withdrawMount = readInteger();
        }
        withdrawals[withdrawCounter] = withdrawMount;
        withdrawCounter++;
    }
}

void printPaperNumber(int numberOfPaper, int denomination)
{
    if (numberOfPaper > 0)
    {
        Console.WriteLine($"{numberOfPaper} billetes de {denomination}");
    }
}

void printCoinNumber(int numberOfCoins, int denomination)
{
    if (numberOfCoins > 0)
    {
        Console.WriteLine($"{numberOfCoins} monedas de {denomination}");
    }
}

void getNumberOfPapers(int withdrawal)
{
    int[] denominations = [500, 200, 100, 50, 20];
    int paperNumber = 0;
    for (int i = 0; i < denominations.Length; i++)
    {
        paperNumber += withdrawal / denominations[i];
        printPaperNumber(withdrawal / denominations[i], denominations[i]);
        if (withdrawal % denominations[i] > 0)
        {
            withdrawal = withdrawal % denominations[i];
        }
    }
    Console.WriteLine($"Total de billetes: {paperNumber}");
}

void getNumberOfCoins(int withdrawal)
{
    withdrawal = (withdrawal % 500 % 200 % 100 % 50 % 20);
    int[] denominations = [10, 5, 1];
    int coinNumber = 0;
    for (int i = 0; i < denominations.Length; i++)
    {
        coinNumber += withdrawal / denominations[i];
        printCoinNumber(withdrawal / denominations[i], denominations[i]);
        if (withdrawal % denominations[i] > 0)
        {
            withdrawal = withdrawal % denominations[i];
        }
    }
    Console.WriteLine($"Total de monedas: {coinNumber}");
}

void resumeWithdrawals()
{
    for (int i = 1; i <= withdrawCounter; i++)
    {
        Console.WriteLine($"======== Retiro #{i}: ${withdrawals[i - 1]} ========");
        getNumberOfPapers(withdrawals[i - 1]);
        getNumberOfCoins(withdrawals[i - 1]);
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
        printOptions();
        var x = Console.ReadLine();
        if (int.TryParse(x, out o))
        {
            switch (o)
            {
                case 1:
                    storeWithdrawals();
                    operationDone = true;
                    break;
                case 2:
                    resumeWithdrawals();
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
            Console.WriteLine("¿Desea realizar otra operación?");
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