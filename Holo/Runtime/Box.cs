using CaseConverter;
using System.Reflection;
using Holo.Native;

namespace Holo;

public class Box(object? Phantom = null) {
    public object? Phantom = Phantom;
    public Actor Actor = Actor.Main;

    private readonly Dictionary<string, Box> Properties = [];
    private readonly Dictionary<string, Method> Methods = [];

    public static Box CreateFrom(object? Phantom) {
        Phantom ??= new NativeNull();
        Box Box = new(Phantom);
        foreach (MethodInfo Method in Phantom.GetMethods()) {
            Box.Methods[Converters.ToSnakeCase(Method.Name)] = new Method(Method);
        }
        return Box;
    }

    public void SetProperty(string Name, Box Value) {
        lock (Actor.Lock) {
            Properties[Name] = Value;
            Console.WriteLine($"Set {Name} to {Value}");
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