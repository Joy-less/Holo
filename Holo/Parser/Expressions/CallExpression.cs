namespace Holo;

public class CallExpression(Expression Context, string MethodName, Expression[] Arguments) : Expression {
    public Expression Context = Context;
    public string MethodName = MethodName;
    public Expression[] Arguments = Arguments;

    public override string ToString() {
        return $"(call, {Context}, {string.Join(", ", (IEnumerable<Expression>)Arguments)})";
    }
}