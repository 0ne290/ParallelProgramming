namespace ParallelProgramming;

public class Detail
{
    public Detail(Machine[] machines, string name, int quantity, int cpuBurst)
    {
        Machines = machines;
        Name = name;
        Quantity = quantity;
        CpuBurst = cpuBurst;
        State = ProcessingStates.Queued;
    }

    public async Task Process()
    {
        State = ProcessingStates.InProgress;
        
        await Task.Run(() => Machines[_machineIndex].Mill(CpuBurst, Name));
        
        _machineIndex++;
        State = ProcessingStates.Queued;
        if (_machineIndex == Machines.Length)
        {
            if (Quantity < 2)
                State = ProcessingStates.Completed;
            else
            {
                Quantity--;
                _machineIndex = 0;
            }
        }
    }

    public Machine[] Machines { get; }
    
    public string Name { get; }
    
    public int Quantity { get; private set; }
    
    public int CpuBurst { get; }

    public ProcessingStates State { get; private set; }

    private int _machineIndex;
}
