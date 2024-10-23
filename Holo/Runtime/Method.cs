using System.Reflection;
using CaseConverter;
using Holo.CoreLibrary;

namespace Holo;

public class Method {
    public readonly MethodInfo ClrMethod;
    public Actor Actor = Actor.Default;

    //private readonly List<Parameter> Parameters = [];

    public Method(MethodInfo ClrMethod) {
        this.ClrMethod = ClrMethod;

        /*foreach (ParameterInfo ClrParameter in ClrMethod.GetParameters()) {
            // if (ClrParameter.ParameterType)

            string ParameterName = ClrParameter.Name.ToSnakeCase();
            int ParameterPosition = ClrParameter.Position;

            // Parameters[ParameterName] = new Parameter(ParameterName, ParameterPosition);
            Parameter Parameter = new(ParameterName, ParameterPosition);
            Parameters.Add(Parameter);
        }*/
    }
    public HoloObject Call(object? Target, HoloTable<HoloObject, HoloObject> Arguments) {
        // 
        object?[] FinalArguments = new object?[ClrMethod.GetParameters().Length];
        // 
        foreach (ParameterInfo ClrParameter in ClrMethod.GetParameters()) {
            if (ClrParameter.Position < Arguments.Count) {
                if (Arguments.GetOrNull(ClrParameter.Position) is HoloObject Argument) {
                    FinalArguments[ClrParameter.Position] = Argument;
                }
            }
        }
        // 
        return HoloObject.Convert(ClrMethod.Invoke(Target, FinalArguments));

        //return HoloObject.Convert(ClrMethod.Invoke(Target.Ghost, Arguments.Select(Argument => Argument.Ghost).ToArray()));
    }

    private readonly struct Parameter(string Name, int Position) {
        public readonly string Name = Name;
        public readonly int Position = Position;
    }
}