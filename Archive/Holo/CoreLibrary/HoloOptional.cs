namespace Holo.CoreLibrary;

public class HoloOptional<T>(T? Value = null) where T : HoloObject {
    public T? Value = Value;

    public bool IsValid {
        get => Value is not null;
    }
}