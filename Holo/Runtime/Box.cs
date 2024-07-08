namespace Holo;

public class Box(object? Phantom = null) {
    public object? Phantom = Phantom;
    public Actor Actor = Actor.Main;

    private readonly Dictionary<string, Box> Properties = [];
    private readonly Dictionary<string, Method> Methods = [];

    public void SetProperty(string Name, Box Value) {
        lock (Actor.Lock) {
            Properties[Name] = Value;
        }
    }
    public void SetMethod(string Name, Method Value) {
        lock (Actor.Lock) {
            Methods[Name] = Value;
        }
    }
    public Box GetProperty(string Name) {
        return Properties[Name];
    }
    public Method GetMethod(string Name) {
        return Methods[Name];
    }
}