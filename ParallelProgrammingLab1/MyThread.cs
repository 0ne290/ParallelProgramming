using System.Diagnostics;
using Semaphore = ParallelProgrammingLab1.PetriNet.Semaphore;

namespace ParallelProgrammingLab1;

public class MyThread
{
    public MyThread(string name, int priority, int cpuBurst, int quantity, IEnumerable<Semaphore> semaphores)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            name = $"T{_threadCounter}";
            _threadCounter++;
        }

        if (Threads.Any(r => r.Name == name))
            throw new Exception($"Поток с именем {name} уже существует.");
        
        Name = name;
        Priority = priority;
        CpuBurst = cpuBurst;
        _quantity = quantity;
        _semaphores = semaphores.ToArray();

        foreach (var semaphore in _semaphores)
            semaphore.TryAddUser(this);
        
        _threadAction = obj =>
        {
            var timeslice = ((int[])obj!)[0];
            var timesliceNumber = ((int[])obj)[1];

            State = ThreadState.Running;
            if (timesliceNumber == 1)
                Reseter.WaitOne();
            else
            {
                _stopwatch.Restart();
                while (_stopwatch.ElapsedMilliseconds < timeslice * timesliceNumber) { }
            }
            
            _cpuBurstCompleted += timesliceNumber;
            
            _semaphores[_currentSemaphoreIndex].Release(this);

            if (_cpuBurstCompleted < CpuBurst)
            {
                State = ThreadState.InQueue;

                return;
            }

            _cpuBurstCompleted = 0;

            _currentSemaphoreIndex++;

            if (_currentSemaphoreIndex < _semaphores.Length)
            {
                State = ThreadState.InQueue;

                return;
            }

            _currentSemaphoreIndex = 0;

            _quantity--;

            State = _quantity < 1 ? ThreadState.Completed : ThreadState.InQueue;
        };
        
        Threads.Add(this);
    }

    public AutoResetEvent Reseter { get; } = new(false);

    public override string ToString() => $"{State} {_semaphores[_currentSemaphoreIndex].Name};";
    
    public void Execute(int timeslice, int timesliceNumber)
    {
        var thread = new Thread(_threadAction);
        _semaphores[_currentSemaphoreIndex].Hold(this);
        thread.Start(new[] { timeslice, timesliceNumber });
    }

    public bool IsAvailable() => _semaphores[_currentSemaphoreIndex].IsAvailable(this);

    public int GetRestOfCpuBurst() => CpuBurst - _cpuBurstCompleted;

    public IEnumerable<string> GetSemaphoreNames() => _semaphores.Select(s => s.Name);

    public ThreadState State { get; private set; } = ThreadState.InQueue;

    public string Name { get; }

    public int Priority { get; }

    public int CpuBurst { get; }

    public static List<MyThread> Threads { get; } = new();

    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly Semaphore[] _semaphores;
    
    private readonly ParameterizedThreadStart _threadAction;

    private readonly Stopwatch _stopwatch = new();

    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private int _quantity;

    private int _cpuBurstCompleted;

    private int _currentSemaphoreIndex;

    private static int _threadCounter = 1;
}
