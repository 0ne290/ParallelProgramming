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
            
            _semaphores[_currentSemaphoreIndex].Hold(this);

            State = ThreadState.Running;
            _stopwatch.Restart();
            while (_stopwatch.ElapsedMilliseconds < timeslice * timesliceNumber) { }

            _semaphores[_currentSemaphoreIndex].Release(this);

            _cpuBurstCompleted += timesliceNumber;

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

    public override string ToString() => $"{Name} state = {State};";
    
    public void Execute(int timeslice, int timesliceNumber)
    {
        State = ThreadState.Waiting;
        var thread = new Thread(_threadAction);
        thread.Start(new[] { timeslice, timesliceNumber });
    }

    public int GetRestOfCpuBurst() => CpuBurst - _cpuBurstCompleted;

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
