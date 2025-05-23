namespace Holo;

public class StringInstruction(string String, bool ProcessEscapeSequences) : Instruction {
    public string String = String;
    public bool ProcessEscapeSequences = ProcessEscapeSequences;

    public override string ToString() {
        return $"(string, {String}, {ProcessEscapeSequences})";
    }
}