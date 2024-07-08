using System.Reflection;

namespace Holo;

public class Method(MethodInfo MethodInfo) {
    public readonly MethodInfo MethodInfo = MethodInfo;

    public Box Call(Box Context, Box[] Arguments) {
        return (Box)MethodInfo.Invoke(Context, Arguments)!;
    }
}