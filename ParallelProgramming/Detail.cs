namespace ParallelProgramming;

public class Detail
{
    public Detail(string name, int quantity, string[] machineNames, int cpuBurst)
    {
        Name = name;
        Quantity = quantity;
        MachineNames = machineNames;
        CpuBurst = cpuBurst;
    }
    
    public string Name { get; }
    
    public int Quantity { get; }
    
    public string[] MachineNames { get; }
    
    public int CpuBurst { get; }
}