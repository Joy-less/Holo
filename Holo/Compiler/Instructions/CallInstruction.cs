namespace Holo;

public class CallInstruction(string MethodName, int ArgumentCount) : Instruction {
    public string MethodName = MethodName;
    public int ArgumentCount = ArgumentCount;

    public override string ToString() {
        return $"call({MethodName}, {ArgumentCount});";
    }
}