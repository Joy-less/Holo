namespace Holo;

public class MethodExpression(string Name) : Expression {
    public string Name = Name;

    public override string ToString() {
        return $"method:{Name}";
    }
}