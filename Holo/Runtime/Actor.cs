namespace Holo;

public class Actor {
    public object Lock = new();

    public static readonly Actor Main = new();
}