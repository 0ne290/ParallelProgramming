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

    public void Preprocess()
    {
        _machines[_machineIndex].Hold(Name);
        State = ProcessingStates.InProgress;
    }

    public async Task<Detail> Process()
    {
        await _machines[_machineIndex].Mill(_timeSlice);

        return this;
    }
    
    public void Postprocess()
    {
        _cpuBurstCompleted++;
        State = ProcessingStates.Queued;
        if (GetRestOfCpuBurst() == 0)
        {
            _cpuBurstCompleted = 0;
            _machineIndex++;
            if (_machineIndex == _machines.Length)
            {
                if (_quantity < 2)
                {
                    _machineIndex--;
                    State = ProcessingStates.Completed;
                }
                else
                {
                    _quantity--;
                    _machineIndex = 0;
                }
            }
        }
        
        _machines[_machineIndex].Release(Name);
    }

    public bool IsAvailabe() => _machines[_machineIndex].IsAvailable();

    public int GetRestOfCpuBurst() => _cpuBurst - _cpuBurstCompleted;

    public static void CreateDetail(string[] machineNames, int quantity, int cpuBurst, int timeSlice, string name)
    {
        var detail = new Detail(Machine.GetMachinesByName(machineNames), quantity, cpuBurst, timeSlice, name);
        ((List<Detail>)Details).Add(detail);
    }
    
    public string Name { get; }

    public string TargetMachineName => _machines[_machineIndex].Name;

    public ProcessingStates State { get; private set; }
    
    public static IEnumerable<Detail> Details { get; } = new List<Detail>();

    private readonly Machine[] _machines;

    private int _quantity;

    private readonly int _cpuBurst;

    private readonly int _timeSlice;

    private int _machineIndex;
    
    private int _cpuBurstCompleted;
}
