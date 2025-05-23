using System.Reflection;
using CaseConverter;

namespace Holo.CoreLibrary;

public class HoloObject {
    public static readonly HoloObject Main = new();

    public Actor Actor = Actor.Default;
    public object? Ghost = null;

    private readonly Dictionary<string, HoloObject> Variables = [];
    private readonly Dictionary<string, Method> Methods = [];
    private readonly List<HoloObject> Components = [];

    public static HoloObject Create(object Ghost) {
        HoloObject Object = new() {
            Ghost = Ghost,
        };
        Type Type = Ghost is Type GhostType ? GhostType : Ghost.GetType();
        if (Type.BaseType is Type BaseType) {
            Object.Components.Add(Convert(BaseType));
        }
        Object.Components.Add(Main);
        foreach (MethodInfo Method in Ghost.GetMethods()) {
            Object.SetMethod(Method.Name.ToSnakeCase(), new Method(Method));
        }
        return Object;
    }
    public static HoloObject Convert(object? Ghost) {
        return Ghost switch {
            null => HoloNull.Main,
            Array Array => new HoloTable<HoloObject, HoloObject>(),
            _ => Create(Ghost),
        };
    }

    public HoloObject() {
    }
    public HoloObject(IEnumerable<HoloObject> Components) {
        this.Components.AddRange(Components);
    }

    public void SetVariable(string Name, HoloObject Value) {
        lock (Actor.Lock) {
            Variables[Name] = Value;
            Console.WriteLine($"Set {Name} to {Value}");
        }
    }
    public void SetMethod(string Name, Method Value) {
        lock (Actor.Lock) {
            Methods[Name] = Value;
        }
    }
    public HoloObject? GetVariableOrNull(string Name) {
        lock (Actor.Lock) {
            if (Variables.GetValueOrDefault(Name) is HoloObject Value) {
                return Value;
            }
            foreach (HoloObject Component in Components) {
                if (Component.GetVariableOrNull(Name) is HoloObject Value2) {
                    return Value2;
                }
            }
            return null;
        }
    }
    public Method? GetMethodOrNull(string Name) {
        lock (Actor.Lock) {
            if (Methods.GetValueOrDefault(Name) is Method Value) {
                return Value;
            }
            foreach (HoloObject Component in Components) {
                if (Component.GetMethodOrNull(Name) is Method Value2) {
                    return Value2;
                }
            }
            return null;
        }
    }

    public static implicit operator HoloObject(bool Value) => Convert(Value);
    public static implicit operator HoloObject(string Value) => Convert(Value);
    public static implicit operator HoloObject(sbyte Value) => Convert(Value);
    public static implicit operator HoloObject(byte Value) => Convert(Value);
    public static implicit operator HoloObject(int Value) => Convert(Value);
    public static implicit operator HoloObject(uint Value) => Convert(Value);
    public static implicit operator HoloObject(long Value) => Convert(Value);
    public static implicit operator HoloObject(ulong Value) => Convert(Value);
    public static implicit operator HoloObject(Int128 Value) => Convert(Value);
    public static implicit operator HoloObject(UInt128 Value) => Convert(Value);
    public static implicit operator HoloObject(Half Value) => Convert(Value);
    public static implicit operator HoloObject(float Value) => Convert(Value);
    public static implicit operator HoloObject(double Value) => Convert(Value);
    public static implicit operator HoloObject(decimal Value) => Convert(Value);
    public static implicit operator HoloObject(Array Value) => Convert(Value);
}