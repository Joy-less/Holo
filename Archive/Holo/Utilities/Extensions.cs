using Holo.CoreLibrary;
using System.Reflection;

namespace Holo;

internal static class Extensions {
    public static IEnumerable<MethodInfo> GetMethods(this object Object) {
        const BindingFlags Bindings = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy;

        Type Type = Object.GetType();

        foreach (MethodInfo Method in Type.GetMethods(Bindings)) {
            yield return Method;
        }
        foreach (FieldInfo Field in Type.GetFields(Bindings)) {
            yield return ((Func<object?>)(() => Field.GetValue(Object))).Method;
        }
        foreach (PropertyInfo Property in Type.GetProperties(Bindings)) {
            if (Property.GetMethod is not null) {
                yield return Property.GetMethod;
            }
        }
    }
    public static HoloTable<HoloObject, HoloObject> ToTable(this IEnumerable Enumerable) {
        HoloTable<HoloObject, HoloObject> Table = new();
        foreach (object? Object in Enumerable) {
            Table.Add(HoloObject.Convert(Object));
        }
        return Table;
    }
}