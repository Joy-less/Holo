namespace Holo.CoreLibrary;

public class HoloGlobals : HoloObject {
    public new static readonly HoloGlobals Main = new();

    public void Log(object? Message) {
        Console.WriteLine(Message);
    }
}