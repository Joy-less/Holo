namespace Holo;

public class StringExpression(string String, bool ProcessEscapeSequences) : Expression {
    public string String = String;
    public bool ProcessEscapeSequences = ProcessEscapeSequences;

    public override string ToString() {
        return $"(string, {String}, {ProcessEscapeSequences}";
    }
}