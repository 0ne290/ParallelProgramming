namespace ParallelProgrammingLab1.PetriNet;

public class Semaphore : IDisposable
{
    public Semaphore(string name, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            name = $"R{_semaphoreCounter}";
            _semaphoreCounter++;
        }
        
        if (Semaphores.Any(r => r._name == name))
            throw new Exception($"Семафор с именем {name} уже существует.");
        
        _name = name;
        _semaphorePlace = new Place(capacity);
        Semaphores.Add(this);
    }
    
    public override string ToString() => $"{_name} = {string.Join(" ", _namesOfHoldingThreads)};";

    public void Hold(MyThread thread)
    {
        lock (_locker1)
        {
            while (!_holdingTransitions[thread].IsAvailable()) { }
        }
        
        lock (_locker)
        {
            Console.WriteLine("YYYYYYYYYYYY");
            
            _namesOfHoldingThreads.Add(thread.Name);
            
            _holdingTransitions[thread].Execute();
        }
    }

    public void Release(MyThread thread)
    {
        lock (_locker)
        {
            Console.WriteLine("XXXXXXXXXXXX");
            
            _namesOfHoldingThreads.Remove(thread.Name);
            
            _releasingTransitions[thread].Execute();
        }
    }
    
    public bool TryAddUser(MyThread thread)
    {
        lock (_locker)
        {
            if (_holdingTransitions.ContainsKey(thread))
                return false;

            var holdingPlace = new Place(1);
            var releasingPlace = new Place();

            _holdingTransitions.Add(thread,
                new Transition(new[] { holdingPlace, _semaphorePlace }, new[] { releasingPlace }));
            _releasingTransitions.Add(thread,
                new Transition(new[] { releasingPlace }, new[] { holdingPlace, _semaphorePlace }));

            return true;
        }
    }

    public static Semaphore GetByName(string name) => Semaphores.Find(s => s._name == name) ?? throw new Exception($"Ресурса с именем \"{name}\" нет.");
    
    public void Dispose() => _synchronizer.Dispose();

    private readonly string _name;

    private readonly Place _semaphorePlace;
    
    private readonly Dictionary<MyThread, Transition> _holdingTransitions = new();
    
    private readonly Dictionary<MyThread, Transition> _releasingTransitions = new();
    
    private readonly object _locker = new();
    
    private readonly object _locker1 = new();
    
    private readonly AutoResetEvent _synchronizer = new(false);
    
    private readonly List<string> _namesOfHoldingThreads = new();
    
    private static int _semaphoreCounter = 1;
    
    public static List<Semaphore> Semaphores { get; } = new ();
}