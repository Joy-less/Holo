namespace Holo;

public static class Compiler {
    public static List<Instruction> Compile(IEnumerable<Statement> Statements) {
        List<Instruction> Instructions = [];

        foreach (Statement Statement in Statements) {
            // Call
            if (Statement is CallStatement CallStatement) {
                foreach (Expression Argument in CallStatement.Arguments) {
                    Instructions.AddRange(CompileExpression(Argument));
                }
                Instructions.Add(new CallInstruction(CallStatement.MethodName, CallStatement.Arguments.Length));
                continue;
            }
            // Not implemented
            throw new NotImplementedException($"statement not compiled: '{Statement.GetType().Name}'");
        }

        return Instructions;
    }

    private static List<Instruction> CompileExpression(Expression Expression) {
        // Decimal
        if (Expression is DecimalExpression DecimalExpression) {

        }
        // Integer
        if (Expression is IntegerExpression IntegerExpression) {

        }
        // String
        if (Expression is StringExpression StringExpression) {
            return [new StringInstruction(StringExpression.String, StringExpression.ProcessEscapeSequences)];
        }
        // Method
        if (Expression is MethodExpression MethodExpression) {

        }
        // Self
        if (Expression is SelfExpression SelfExpression) {

        }
        // Invalid
        throw new NotImplementedException($"expression not compiled: {Expression.GetType().Name}");
    }
}