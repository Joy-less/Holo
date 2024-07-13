using System.Numerics;

namespace Holo;

public class IntegerInstruction(BigInteger Integer) : Instruction {
    public BigInteger Integer = Integer;

    public override string ToString() {
        return $"(integer, {Integer})";
    }
}