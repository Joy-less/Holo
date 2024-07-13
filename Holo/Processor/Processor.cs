namespace Holo;

public static class Processor {
    public static void Process(Box Context, IEnumerable<Instruction> Instructions) {
        Stack<Box> Stack = [];

        foreach (Instruction Instruction in Instructions) {
            // Self
            if (Instruction is SelfInstruction SelfInstruction) {
                Stack.Push(Context);
                continue;
            }
            // Integer
            if (Instruction is IntegerInstruction IntegerInstruction) {
                Stack.Push(new Box(IntegerInstruction.Integer));
                continue;
            }
            // String
            if (Instruction is StringInstruction StringInstruction) {
                Stack.Push(new Box(StringInstruction.String));
                continue;
            }
            // Call
            if (Instruction is CallInstruction CallInstruction) {
                // Pop arguments from stack
                Box[] Arguments = new Box[CallInstruction.ArgumentCount];
                for (int Index = 0; Index < Arguments.Length; Index++) {
                    Arguments[Index] = Stack.Pop();
                }

                // Pop context from stack
                Box CallContext = Stack.Pop();

                // Get method by name
                Method Method = CallContext.GetMethod(CallInstruction.MethodName);

                // Call method
                Method.Call(CallContext, Arguments);
                continue;
            }
            // Assign
            if (Instruction is AssignInstruction AssignInstruction) {
                // Pop value from stack
                Box Value = Stack.Pop();

                // Pop context from stack
                Box AssignContext = Stack.Pop();

                // Assign to variable
                AssignContext.SetProperty(AssignInstruction.VariableName, Value);
                continue;
            }
            // Not implemented
            throw new NotImplementedException($"instruction not processed: '{Instruction.GetType().Name}'");
        }
    }
}