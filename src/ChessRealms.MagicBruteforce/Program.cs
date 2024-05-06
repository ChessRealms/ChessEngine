using ChessRealms.MagicBruteforce;

if (args.Length < 2)
{
    Exit(1, "Invallid agruments amount. You must pass 'seed' and 'iterations'.");
    Console.Error.WriteLine("ChessRealms.MagicBruteforce.exe [seed] [iterations]");
    return;
}

if (!int.TryParse(args[0], out int seed))
{
    Exit(1, "Invalid 'seed' value.");
    return;
}

if (!int.TryParse(args[1], out int iterations) || iterations <= 0)
{
    Exit(1, "Invalid 'iterations' value.");
    return;
}

Console.WriteLine("Running 'magic' bruteforce with next settings:");
Console.WriteLine("Seed: {0}", seed);
Console.WriteLine("Iterations: {0}", iterations);

Random rnd = new(seed);

Console.WriteLine("Bishop:");
for (int square = 0; square < 64; ++square)
{
    var magic = Magic.FindMagic(square, iterations, isBishop: true, rnd);
    
    if (magic == 0)
    {
        Console.Error.WriteLine("Error. Cannot find magic.");
    }
    else
    {
        Console.WriteLine("0x{0:X16}", magic);
    }    
}

Console.WriteLine("Rook:");
for (int square = 0; square < 64; ++square)
{
    var magic = Magic.FindMagic(square, iterations, isBishop: false, rnd);
    
    if (magic == 0)
    {
        Console.Error.WriteLine("Error. Cannot find magic.");
    }
    else
    {
        Console.WriteLine("0x{0:X16}", magic);
    }
}

static void Exit(int code, string msg)
{
    Console.Error.WriteLine(msg);
    Environment.Exit(code);
}