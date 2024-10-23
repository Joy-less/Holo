using Holo;
using Holo.CoreLibrary;

const string Code = @"
log 'Hi!'
log('Hi!')
log  ( 'Hi!' )

counter = 0
";

Console.WriteLine("Statements:");
foreach (Statement Statement in Parser.Parse(Code)) {
    Console.WriteLine(Statement);
}

Console.WriteLine("\nInstructions:");
foreach (Instruction Instruction in Compiler.Compile(Parser.Parse(Code))) {
    Console.WriteLine(Instruction);
}

Console.WriteLine("\nProcess:");
Processor.Process(new HoloObject(), Compiler.Compile(Parser.Parse(Code)));