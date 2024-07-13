using System.Numerics;

namespace Holo;

public static class Compiler {
    public static List<Instruction> Compile(IEnumerable<Statement> Statements) {
        List<Instruction> Instructions = [];

        foreach (Statement Statement in Statements) {
            // Call
            if (Statement is CallStatement CallStatement) {
                Instructions.AddRange(CompileExpression(CallStatement.Context));
                foreach (Expression Argument in CallStatement.Arguments) {
                    Instructions.AddRange(CompileExpression(Argument));
                }
                Instructions.Add(new CallInstruction(CallStatement.MethodName, CallStatement.Arguments.Length));
                continue;
            }
            // Assignment
            if (Statement is AssignStatement AssignmentStatement) {
                Instructions.AddRange(CompileExpression(AssignmentStatement.Context));
                Instructions.AddRange(CompileExpression(AssignmentStatement.Value));
                Instructions.Add(new AssignInstruction(AssignmentStatement.VariableName));
                continue;
            }
            // Not implemented
            throw new NotImplementedException($"statement not compiled: '{Statement.GetType().Name}'");
        }

        return Instructions;
    }

    private static List<Instruction> CompileExpression(Expression Expression) {
        // Self
        if (Expression is SelfExpression) {
            return [new SelfInstruction()];
        }
        // Integer
        if (Expression is IntegerExpression IntegerExpression) {
            return [new IntegerInstruction(BigInteger.Parse(IntegerExpression.Integer))];
        }
        // Decimal
        if (Expression is DecimalExpression DecimalExpression) {

        }
        // String
        if (Expression is StringExpression StringExpression) {
            return [new StringInstruction(StringExpression.String, StringExpression.ProcessEscapeSequences)];
        }
        // Method
        if (Expression is MethodExpression MethodExpression) {

        }
        // Invalid
        throw new NotImplementedException($"expression not compiled: {Expression.GetType().Name}");
    }
}