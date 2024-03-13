namespace ParallelProgrammingLab1.PetriNetSemaphore;

public class Resource
{
    public Resource(string name, HoldingTransition holdingTransition, ReleasingTransition releasingTransition)
    {
        _name = name;
        _holdingTransition = holdingTransition;
        _releasingTransition = releasingTransition;
        
        Resources.Add(this);
    }
    
    public override string ToString() => $"{_name} = {string.Join(" ", _namesOfHoldingThreads)};";
    
    public void Hold(MyThread thread)
    {
        thread.State = ThreadStates.InQueue;

        while (!_holdingTransition.IsAvailable()) { }
        lock (_locker)
        {
            _holdingTransition.Execute();
            _namesOfHoldingThreads.Add(thread.Name);
        }
        
        thread.State = ThreadStates.Running;
    }
    
    public void Release(MyThread thread)
    {
        lock (_locker)
        {
            _releasingTransition.Execute();
            _namesOfHoldingThreads.Remove(thread.Name);
        }
        
        thread.State = ThreadStates.InQueue;
    }
    
    public static List<Resource> Resources { get; } = new();

    private readonly string _name;
    
    private readonly HoldingTransition _holdingTransition;
    
    private readonly ReleasingTransition _releasingTransition;

    private readonly List<string> _namesOfHoldingThreads = new();

    private readonly object _locker = new();
}