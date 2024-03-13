using System.Timers;

namespace ParallelProgrammingLab1.PetriNetSemaphore;

public class Resource
{
    private Resource(string name, HoldingTransition holdingTransition, ReleasingTransition releasingTransition)
    {
        _name = name;
        _holdingTransition = holdingTransition;
        _releasingTransition = releasingTransition;
        
        Resources.Add(this);
    }

    public static Resource CreateResource(string name, int capacity)
    {
        var innerPlace = new Place();
        var semaphorePlace = new Place { Tokens = capacity };

        return new Resource(name, new HoldingTransition(semaphorePlace, innerPlace),
            new ReleasingTransition(innerPlace, semaphorePlace));
    }

    public static void Execute()
    {
        var timer = new System.Timers.Timer(Timeslice);
        timer.Elapsed += OnTimedEvent1;
        timer.Elapsed += OnTimedEvent;
        timer.Start();

        var cond = true;
        while (cond)
        {
            foreach (var thread in MyThread.Threads)
            {
                if (thread.State != ThreadStates.Completed)
                    break;
                cond = false;
            }
        }
        
        timer.Stop();

        while (_sync > 0) { }// Можно было бы заменить это на AutoResetEvent или добавить к спин-локу код, отдающий квант времени этого потока другому потоку (Thread.Curent.Join(100) или Thread.Sleep(0))
        
        timer.Elapsed -= OnTimedEvent1;
        timer.Elapsed -= OnTimedEvent;
        timer.Close();
        timer.Dispose();
    }
    
    private static void OnTimedEvent(object? source, ElapsedEventArgs e)
    {
        Interlocked.Increment(ref _sync);

        var resources = Resources.Aggregate("", (current, resourse) => current + resourse);

        var threads = MyThread.Threads.Aggregate("", (current, thread) => current + thread);

        Console.WriteLine($"{_quantumNumber} | {resources} | {threads}");

        _quantumNumber++;

        Interlocked.Decrement(ref _sync);
    }
    
    private static void OnTimedEvent1(object? source, ElapsedEventArgs e)
    {
        Interlocked.Increment(ref _sync);

        foreach (var resource in Resources)
            resource.Distribute();

        Interlocked.Decrement(ref _sync);
    }
    
    public override string ToString() => $"{_name} = {string.Join(" ", _namesOfHoldingThreads)};";

    public void Hold(MyThread thread)
    {
        lock (_locker)
        {
            _waitingThreads.Add(thread);
        }
    }

    public void Distribute()
    {
        lock (_locker)
        {
            _waitingThreads.Sort((x, y) =>
            {
                var ret = y.Priority.CompareTo(x.Priority);
                return ret != 0 ? ret : x.GetRestOfCpuBurst().CompareTo(y.GetRestOfCpuBurst());
            });
            
            var capacity = GetCapacity();
            var i = 0;
            while (_waitingThreads.Count > 0 && i < capacity)
            {
                _holdingTransition.Execute();
                _namesOfHoldingThreads.Add(_waitingThreads[0].Name);
                _waitingThreads[0].Locker = false;
                _waitingThreads.Remove(_waitingThreads[0]);
                i++;
            }
        }
    }
    
    public void Release(string threadName)
    {
        lock (_locker)
        {
            _releasingTransition.Execute();
            _namesOfHoldingThreads.Remove(threadName);
        }
    }

    public int GetCapacity() => _holdingTransition.GetCapacity();
    
    public static List<Resource> Resources { get; } = new();
    
    public static int Timeslice { get; set; }

    private readonly string _name;
    
    private readonly HoldingTransition _holdingTransition;
    
    private readonly ReleasingTransition _releasingTransition;

    private readonly List<string> _namesOfHoldingThreads = new();

    private readonly List<MyThread> _waitingThreads = new();

    private readonly object _locker = new();
    
    private static int _sync;
    
    private static int _quantumNumber;
}