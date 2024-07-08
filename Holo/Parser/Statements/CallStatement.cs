namespace Holo;

public class CallStatement(Expression Context, string MethodName, Expression[] Arguments) : Statement {
    public Expression Context = Context;
    public string MethodName = MethodName;
    public Expression[] Arguments = Arguments;

    public override string ToString() {
        return $"(call, {Context}, {string.Join(", ", (IEnumerable<Expression>)Arguments)});";
    }
}