namespace GameLogic;

public class BattleLog
{
    private readonly List<string> _entries;
    
    public BattleLog()
    {
        _entries = new List<string>();
    }
    
    public void AddEntry(string entry)
    {
        _entries.Add(entry);
    }
    
    public string[] GetEntries()
    {
        return _entries.ToArray();
    }
}
