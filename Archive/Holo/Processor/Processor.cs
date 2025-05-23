using Holo.CoreLibrary;

namespace Holo;

public static class Processor {
    public static void Process(HoloObject Target, IEnumerable<Instruction> Instructions) {
        Stack<HoloObject> Stack = [];

        foreach (Instruction Instruction in Instructions) {
            // Self
            if (Instruction is SelfInstruction SelfInstruction) {
                Stack.Push(Target);
                continue;
            }
            // Integer
            if (Instruction is IntegerInstruction IntegerInstruction) {
                Stack.Push(HoloObject.Convert(IntegerInstruction.Integer));
                continue;
            }
            // String
            if (Instruction is StringInstruction StringInstruction) {
                Stack.Push(HoloObject.Convert(StringInstruction.String));
                continue;
            }
            // Call
            if (Instruction is CallInstruction CallInstruction) {
                // Pop arguments from stack
                /*HoloObject[] Arguments = new HoloObject[CallInstruction.ArgumentCount];
                for (int Index = 0; Index < Arguments.Length; Index++) {
                    Arguments[Index] = Stack.Pop();
                }*/
                HoloTable<HoloObject, HoloObject> Arguments = new();
                for (int Index = 0; Index < CallInstruction.ArgumentCount; Index++) {
                    Arguments.Set(Index, Stack.Pop());
                }

                // Pop target from stack
                HoloObject CallTarget = Stack.Pop();

                // Get method by name
                Method Method = CallTarget.GetMethodOrNull(CallInstruction.MethodName)!;

                // Call method
                Method.Call(CallTarget, Arguments);
                continue;
            }
            // Assign
            if (Instruction is AssignInstruction AssignInstruction) {
                // Pop value from stack
                HoloObject Value = Stack.Pop();

                // Pop context from stack
                HoloObject AssignContext = Stack.Pop();

                // Assign to variable
                AssignContext.SetVariable(AssignInstruction.VariableName, Value);
                continue;
            }
            // Not implemented
            throw new NotImplementedException($"instruction not processed: '{Instruction.GetType().Name}'");
        }
    }
}