namespace Holo;

public class AssignInstruction(string VariableName) : Instruction {
    public string VariableName = VariableName;

    public override string ToString() {
        return $"assign({VariableName});";
    }
}