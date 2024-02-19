namespace ParallelProgramming;

public class Detail
{
    public Detail(string name, int quantity, string[] machineNames)
    {
        Name = name;
        Quantity = quantity;
        MachineNames = machineNames;
    }
    
    public string Name { get; }
    
    public int Quantity { get; }
    
    public string[] MachineNames { get; }
}