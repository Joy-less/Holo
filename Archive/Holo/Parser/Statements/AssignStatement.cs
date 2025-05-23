namespace Holo;

public class AssignStatement(Expression Context, string VariableName, Expression Value) : Statement {
    public Expression Context = Context;
    public string VariableName = VariableName;
    public Expression Value = Value;

    public override string ToString() {
        return $"(assignment, {Context}, {Value});";
    }
}