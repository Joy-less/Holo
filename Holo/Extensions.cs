namespace Holo;

internal static class Extensions {
    public static bool IsValidIdentifier(this char Char) {
        return char.IsLetterOrDigit(Char) || Char == '_';
    }
    public static bool IsValidIdentifier(this string String) {
        return String.All(IsValidIdentifier);
    }
}