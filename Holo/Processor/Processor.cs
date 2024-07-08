namespace Holo;

public static class Processor {
    public static void Process(Box Context, IEnumerable<Instruction> Instructions) {
        Stack<Box> Stack = [];

        foreach (Instruction Instruction in Instructions) {
            // Call
            if (Instruction is CallInstruction CallInstruction) {
                // Pop arguments from stack
                Box[] Arguments = new Box[CallInstruction.ArgumentCount];
                for (int Index = 0; Index < Arguments.Length; Index++) {
                    Arguments[Index] = Stack.Pop();
                }

                // Get method
                Method Method = Context.GetMethod(CallInstruction.MethodName);

                // Call method
                Method.Call(Context, Arguments);
                continue;
            }
            // String
            if (Instruction is StringInstruction StringInstruction) {
                Stack.Push(new Box(StringInstruction.String));
                continue;
            }
            // Not implemented
            throw new NotImplementedException($"instruction not processed: '{Instruction.GetType().Name}'");
        }
    }
}