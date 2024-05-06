using ChessRealms.ChessEngine.Core;
using ChessRealms.ChessEngine.Core.Attacks;
using ChessRealms.ChessEngine.Core.Types;
using System.Numerics;

int seed = 1618033988;
int iterations = 1000000000;

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
        Console.WriteLine("0x{0:X16}", Magic.FindMagic(square, iterations, isBishop: true, rnd));
    }    
}
Console.WriteLine();

Console.WriteLine("Rook:");
for (int square = 0; square < 64; ++square)
{
    var magic = Magic.FindMagic(square, iterations, isBishop: true, rnd);
    
    if (magic == 0)
    {
        Console.Error.WriteLine("Error. Cannot find magic.");
    }
    else
    {
        Console.WriteLine("0x{0:X16}", Magic.FindMagic(square, iterations, isBishop: false, rnd));
    }
}


static class Magic
{
    private static ulong NextMagic(Random rnd)
    {
        return (ulong)rnd.NextInt64() & (ulong)rnd.NextInt64() & (ulong)rnd.NextInt64();
    }

    public static ulong FindMagic(SquareIndex square, int iterations, bool isBishop, Random rnd)
    {
        ulong[] occupancies = new ulong[4096];
        ulong[] attacks = new ulong[4096];
        ulong[] usedAttacks = new ulong[4096];
        
        ulong attackMask = BishopLookups.AttackMasks[square];
        int relevantBits = BishopLookups.RelevantBits[square];

        int occupancyIndicies = 1 << relevantBits;

        for (int i = 0; i < occupancyIndicies; ++i)
        {
            occupancies[i] = Occupancy.CreateAtIndex(i, relevantBits, attackMask);

            if (isBishop)
            {
                attacks[i] = BishopLookups.MaskBishopSliderAttackOnTheFly(square, occupancies[i]);
            }
            else
            {
                attacks[i] = RookLookups.MaskRookSliderAttackOnTheFly(square, occupancies[i]);
            }
        }

        for (int i = 0; i < iterations; ++i)
        {
            ulong magicNum = NextMagic(rnd);

            if (BitOperations.PopCount(attackMask * magicNum & 0xFF00000000000000) < 6)
            {
                continue;
            }

            Array.Clear(usedAttacks);

            int index;
            bool fail;

            for (index = 0, fail = false; !fail && index < occupancyIndicies; ++index)
            {
                uint magicIndex = (uint)((occupancies[index] * magicNum) >> (64 - relevantBits));

                if (usedAttacks[magicIndex] == 0)
                {
                    usedAttacks[magicIndex] = attacks[index];
                }
                else if (usedAttacks[magicIndex] != attacks[index])
                {
                    fail = true;
                }
            }

            if (!fail)
            {
                return magicNum;
            }
        }

        return 0;
    }
}