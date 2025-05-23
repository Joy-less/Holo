namespace Holo.CoreLibrary;

public abstract class HoloTable : HoloSequence {
    private readonly Dictionary<HoloObject, HoloObject> Dictionary = [];

    public override int Count {
        get => Dictionary.Count;
    }
    public HoloObject Get(HoloObject Key) {
        return Dictionary[Key];
    }
    public HoloObject? GetOrNull(HoloObject Key) {
        return Dictionary.GetValueOrDefault(Key);
    }
    public void Set(HoloObject Key, HoloObject Value) {
        Dictionary[Key] = Value;
    }
    public bool ContainsKey(HoloObject Key) {
        return Dictionary.ContainsKey(Key);
    }
    public void Add(HoloObject Value) {
        // int MaxKey = Dictionary.Max(Entry => Entry.Key);
        int MaxKey = Count;
        Set(MaxKey + 1, Value);
    }
}
public class HoloTable<TKey, TValue> : HoloTable where TKey : HoloObject where TValue : HoloObject {
}