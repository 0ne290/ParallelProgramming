namespace ParallelProgrammingLab1;

public class Resourse
{
    public Resourse(string name, int capacity)
    {
        _name = name;
        Resourses.Add(this);
        
        ResoursePlaces.Add(this, new Place(capacity));
    }
    
    public override string ToString() => $"{_name} = {string.Join(" ", _threadNames)};";

    public void Hold(string name) => _threadNames.Add(name);
    
    public void Realese(string name) => _threadNames.Remove(name);

    public static List<Resourse> Resourses { get; } = new();

    public static Dictionary<Resourse, Place> ResoursePlaces { get; } = new ();
    
    private readonly List<string> _threadNames = new();
    
    private string _name;
}