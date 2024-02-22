using ParallelProgramming.Enums;

namespace ParallelProgramming;

public class Detail
{
    private Detail(Machine[] machines, int quantity, int cpuBurst, int timeSlice, string name)
    {
        _machines = machines;
        _quantity = quantity;
        _cpuBurst = cpuBurst;
        _timeSlice = timeSlice;
        
        Name = name;
        
        State = ProcessingStates.Queued;
    }

    public async Task Process()// Извиняюсь за такой свинокод - я побоялся использовать return'ы, т. к. не имею опыта с async-await
    {
        await Task.Run(() =>
        {
            State = ProcessingStates.Waiting;
            TargetMachineName = _machines[_machineIndex].Name;
            
            _machines[_machineIndex].Semaphore.WaitOne();
            
            State = ProcessingStates.InProgress;
            
            _machines[_machineIndex].Mill(_timeSlice, Name);
            _machines[_machineIndex].Semaphore.Release();
        });

        _cpuBurstCompleted++;
        State = ProcessingStates.Queued;
        TargetMachineName = string.Empty;
        
        if (GetRestOfCpuBurst() == 0)
        {
            _cpuBurstCompleted = 0;
            _machineIndex++;
            if (_machineIndex == _machines.Length)
            {
                if (_quantity < 2)
                    State = ProcessingStates.Completed;
                else
                {
                    _quantity--;
                    _machineIndex = 0;
                    lock (DetailBlocker)
                    {
                        DetailsInQueue.Enqueue(this);
                    }
                }
            }
            else
            {
                lock (DetailBlocker)
                {
                    DetailsInQueue.Enqueue(this);
                }
            }
        }
        else
        {
            lock (DetailBlocker)
            {
                DetailsInQueue.Enqueue(this);
            }
        }
    }

    public int GetRestOfCpuBurst() => _cpuBurst - _cpuBurstCompleted;

    public static void CreateDetail(string[] machineNames, int quantity, int cpuBurst, int timeSlice, string name)
    {
        var detail = new Detail(Machine.GetMachinesByName(machineNames), quantity, cpuBurst, timeSlice, name);
        ((List<Detail>)Details).Add(detail);
        DetailsInQueue.Enqueue(detail);
    }
    
    public string Name { get; }

    public string TargetMachineName { get; private set; } = string.Empty;
    
    public ProcessingStates State { get; private set; }

    public static Queue<Detail> DetailsInQueue { get; } = new();
    
    public static IEnumerable<Detail> Details { get; } = new List<Detail>();

    private readonly Machine[] _machines;

    private int _quantity;

    private readonly int _cpuBurst;

    private readonly int _timeSlice;

    private int _machineIndex;
    
    private int _cpuBurstCompleted;

    private static readonly object DetailBlocker = new();
}
