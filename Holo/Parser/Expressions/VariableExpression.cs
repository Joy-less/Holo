namespace Holo;

public class VariableExpression(string Name) : Expression {
    public string Name = Name;

    public override string ToString() {
        return $"variable:{Name}";
    }
}